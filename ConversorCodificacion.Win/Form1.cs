using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConversorCodificacion.Core.Servicios;
using ConversorCodificacion.Win.Estilos;

namespace ConversorCodificacion.Win;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        // Valores por defecto
        txtExtensiones.Text = "bas, cls, frm, vbp";
        chkRecursivo.Checked = true;
        chkBackup.Checked = true;
        rbAUtf8.Checked = true;
        txtCarpeta.Text = ObtenerCarpetaProyecto();
        // Cargar última ruta usada si existe
        var ultimaRuta = Preferencias.CargarUltimaRuta();
        if (!string.IsNullOrWhiteSpace(ultimaRuta) && Directory.Exists(ultimaRuta))
            txtCarpeta.Text = ultimaRuta;

        // Tema oscuro estilo web
        TemaOscuro.Aplicar(this);
        // Botón principal destacado
        TemaOscuro.EstilizarBotonPrimario(btnConvertir);
        // Botones secundarios
        TemaOscuro.EstilizarBotonSecundario(btnProyecto);
        TemaOscuro.EstilizarBotonSecundario(btnElegir);

        // Alinear 'Ayuda' a la derecha del MenuStrip
        try { ayudaToolStripMenuItem.Alignment = ToolStripItemAlignment.Right; } catch { }

        // Iconos desde carpeta 'img' (junto al .exe). Nombres fijos a 16x16. Si no existen, fallback a SystemIcons
        try
        {
            // Escalado uniforme
            var sz = new System.Drawing.Size(16, 16);
            menuStrip1.ImageScalingSize = sz;

            System.Drawing.Image? LoadFixed(string filename)
            {
                var baseDir = AppContext.BaseDirectory;
                string[] candidates = new[]
                {
                    System.IO.Path.Combine(baseDir, "img", filename),
                    System.IO.Path.Combine(baseDir, "img", "ico", filename),
                };

                foreach (var path in candidates)
                {
                    if (!System.IO.File.Exists(path)) continue;
                    var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
                    if (ext == ".ico")
                    {
                        using var ic = new System.Drawing.Icon(path, sz);
                        return ic.ToBitmap();
                    }
                    else
                    {
                        using var raw = System.Drawing.Image.FromFile(path);
                        return new System.Drawing.Bitmap(raw, sz);
                    }
                }
                return null;
            }

            System.Drawing.Image? LoadAny(string baseName)
            {
                // Prioridad: -16.ico, .ico, -16.png, .png
                return LoadFixed(baseName + "-16.ico")
                       ?? LoadFixed(baseName + ".ico")
                       ?? LoadFixed(baseName + "-16.png")
                       ?? LoadFixed(baseName + ".png");
            }

            System.Drawing.Bitmap S(System.Drawing.Icon ic) => new System.Drawing.Bitmap(ic.ToBitmap(), sz);

            abrirLogToolStripMenuItem.Image = LoadAny("log") ?? S(System.Drawing.SystemIcons.Information);
            abrirConfigToolStripMenuItem.Image = LoadAny("config") ?? S(System.Drawing.SystemIcons.Application);
            salirToolStripMenuItem.Image = LoadAny("exit") ?? S(System.Drawing.SystemIcons.Error);
            ayudaToolStripMenuItem.Image = LoadAny("help") ?? S(System.Drawing.SystemIcons.Question);
            // Botones con icono a la izquierda
            btnElegir.Image = LoadAny("folder") ?? S(System.Drawing.SystemIcons.Application);
            btnElegir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnElegir.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnElegir.Padding = new Padding(6, 0, 6, 0);

            btnConvertir.Image = LoadAny("convert") ?? S(System.Drawing.SystemIcons.Shield);
            btnConvertir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnConvertir.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnConvertir.Padding = new Padding(6, 0, 6, 0);

            // Submenús de Ayuda
            ayudaInstruccionesToolStripMenuItem.Image = LoadAny("guide")
                ?? LoadAny("info")
                ?? S(System.Drawing.SystemIcons.Information);
            ayudaAgradecimientosToolStripMenuItem.Image = LoadAny("thanks")
                ?? LoadAny("heart")
                ?? S(System.Drawing.SystemIcons.Asterisk);
        }
        catch { }

        // Cargar preferencia de estilo de barra
        var estilo = Preferencias.CargarEstiloBarra();
        // ProgressBarEx no usa estilos nativos; guardamos la preferencia pero no cambiamos propiedades aquí

        // Color inicial de barra según preferencias
        var color = Preferencias.CargarColorBarra();
        AplicarColorBarra(color);

        // El porcentaje debe mostrarse encima de la barra
        try
        {
            lblPct.Parent = pbProgreso; // renderiza encima del ProgressBar
            lblPct.BackColor = System.Drawing.Color.Transparent;
            lblPct.Dock = DockStyle.Fill;
            lblPct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblPct.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblPct.BringToFront();
        }
        catch { /* en caso de que el diseñador cambie el orden, se ignorará sin romper */ }
    }

    private static string ObtenerCarpetaProyecto()
    {
        try
        {
            // Carpeta raíz de la solución como aproximación
            return Directory.GetParent(AppContext.BaseDirectory)?.FullName ?? string.Empty;
        }
        catch { return string.Empty; }
    }

    private void btnProyecto_Click(object? sender, EventArgs e)
    {
        txtCarpeta.Text = ObtenerCarpetaProyecto();
    }

    private void btnElegir_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Selecciona la carpeta base a procesar",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false,
            SelectedPath = Directory.Exists(txtCarpeta.Text) ? txtCarpeta.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txtCarpeta.Text = dlg.SelectedPath;
            Preferencias.GuardarUltimaRuta(dlg.SelectedPath);
        }
    }

    private async void btnConvertir_Click(object? sender, EventArgs e)
    {
        lvResultados.BeginUpdate();
        lvResultados.Items.Clear();
        int total = 0, convertidos = 0, errores = 0;

        try
        {
            var basePath = (txtCarpeta.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
            {
                MessageBox.Show(this, "La ruta indicada no existe o no es accesible.", "Carpeta base", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var porDefecto = new[] { "bas", "cls", "frm", "vbp" };
            var exts = ConversorDeCodificacion.ParsearExtensiones(txtExtensiones.Text ?? string.Empty, porDefecto);
            if (exts.Count == 0) exts = porDefecto.ToList();

            var archivos = ConversorDeCodificacion.ListarArchivos(basePath, exts, chkRecursivo.Checked).ToList();
            total = archivos.Count;

            // Configurar progreso
            pbProgreso.Minimum = 0;
            pbProgreso.Maximum = total == 0 ? 1 : total;
            pbProgreso.Value = 0;
            lblProgreso.Text = $"0/{total}";
            lblPct.Text = "0%";
            // Mostrar barra y contador
            pnlProgresoHost.Visible = true;
            lblProgreso.Visible = true;
            // Garantizar superposición del porcentaje
            lblPct.BringToFront();

            // Deshabilitar UI crítica durante el proceso
            btnConvertir.Enabled = false; btnElegir.Enabled = false;

            int avanzados = 0;
            var progress = new Progress<(string archivo, string mensaje, bool ok)>();
            progress.ProgressChanged += (_, info) =>
            {
                // Actualiza UI desde el hilo de UI
                AgregarResultado(info.archivo, info.mensaje);
                if (info.ok) convertidos++; else errores++;
                avanzados++;
                if (pbProgreso.Value < pbProgreso.Maximum) pbProgreso.Value = avanzados;
                lblProgreso.Text = $"{avanzados}/{total}";
                if (total > 0)
                {
                    var pct = (int)Math.Round(100.0 * avanzados / Math.Max(1, total));
                    lblPct.Text = $"{pct}%";
                    lblPct.BringToFront();
                }
            };

            await Task.Run(() =>
            {
                foreach (var archivo in archivos)
                {
                    try
                    {
                        if (chkBackup.Checked)
                            ConversorDeCodificacion.AsegurarBackupOpcional(archivo);

                        (bool ok, string mensaje) r = rbAUtf8.Checked
                            ? ConversorDeCodificacion.ConvertirAUtf8(archivo)
                            : rbAWin1252.Checked
                                ? ConversorDeCodificacion.ConvertirAWin1252(archivo)
                                : rbRestoreBak.Checked
                                    ? ConversorDeCodificacion.RestaurarDesdeBak(archivo)
                                    : ConversorDeCodificacion.RepararMojibake(archivo);

                        ((IProgress<(string, string, bool)>)progress).Report((archivo, r.mensaje, r.ok));
                        // Pequeña espera para que el avance sea visible
                        Thread.Sleep(15);
                    }
                    catch (Exception ex)
                    {
                        ((IProgress<(string, string, bool)>)progress).Report((archivo, ex.Message, false));
                        Thread.Sleep(15);
                    }
                }
            });
        }
        finally
        {
            lvResultados.EndUpdate();
            lblResumen.Text = $"Procesados: {total} | Conv.: {convertidos} | Err.: {errores}";
            if (errores > 0)
            {
                MessageBox.Show(this,
                    "Se produjeron errores durante la conversión. Revisa el menú Archivo → Abrir log para ver los detalles.",
                    "Conversión con errores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            // Completar la barra si quedó a medias
            if (pbProgreso.Value < pbProgreso.Maximum)
            {
                pbProgreso.Value = pbProgreso.Maximum;
            }
            lblProgreso.Text = $"{pbProgreso.Value}/{total}";
            lblPct.Text = "100%";
            pbProgreso.Refresh();
            btnConvertir.Enabled = true; btnElegir.Enabled = true;
        }
    }

    private void AgregarResultado(string archivo, string mensaje)
    {
        var item = new ListViewItem(new[] { archivo, mensaje });
        lvResultados.Items.Add(item);
    }

    private void abrirLogToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        try
        {
            var ruta = Registro.ObtenerRutaLogs();
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            Process.Start(new ProcessStartInfo
            {
                FileName = ruta,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"No se pudo abrir la carpeta de logs. Detalle: {ex.Message}", "Abrir log", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void salirToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        try { Close(); }
        catch { Application.Exit(); }
    }

    private void abrirConfigToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        try
        {
            var ruta = Preferencias.ObtenerRutaConfig();
            Process.Start(new ProcessStartInfo { FileName = ruta, UseShellExecute = true, Verb = "open" });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"No se pudo abrir la carpeta Config. Detalle: {ex.Message}", "Abrir Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Menú: Ayuda → Instrucciones de uso
    private void ayudaInstruccionesToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var frm = new Form
        {
            Text = "Instrucciones de uso",
            StartPosition = FormStartPosition.CenterParent,
            Size = new System.Drawing.Size(780, 560),
            MinimumSize = new System.Drawing.Size(680, 460),
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            Padding = new Padding(12)
        };

        // Tema oscuro básico para el diálogo
        frm.BackColor = Estilos.TemaOscuro.ColorPanel;
        frm.ForeColor = System.Drawing.Color.FromArgb(229, 231, 235);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0),
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));

        var txt = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new System.Drawing.Font("Segoe UI", 10F),
            BackColor = Estilos.TemaOscuro.ColorPanel,
            ForeColor = System.Drawing.Color.FromArgb(229, 231, 235),
            Text =
            "1) Carpeta base: selecciona la carpeta raíz del proyecto.\r\n" +
            "2) Dirección de conversión: elige la operación (CP1252↔UTF-8, Arreglar mojibake, Restaurar .bak).\r\n" +
            "3) Extensiones a procesar: por defecto bas, cls, frm, vbp.\r\n" +
            "4) Opciones: Incluir subcarpetas y crear backup .bak (recomendado).\r\n" +
            "5) Convertir: se mostrará el resultado por archivo.\r\n\r\n" +
            "Notas VB6:\r\n" +
            "- .frm y .vbp no se convierten a UTF-8 por seguridad (se mantienen en Windows-1252).\r\n" +
            "- Para archivos VB6 se normaliza CRLF al guardar.\r\n\r\n" +
            "Registros:\r\n" +
            "- Los errores se guardan en Logs/app_YYYY-MM-DD.log.\r\n" +
            "- Menú Archivo → Abrir log para abrir la carpeta.\r\n\r\n" +
            "Preferencias:\r\n" +
            "- Config/appsettings.json guarda preferencias.\r\n" +
            "- Config/ultima-ruta.json guarda la última carpeta usada.\r\n"
        };

        var panelBotones = new Panel { Dock = DockStyle.Fill };
        var btnCerrar = new Button
        {
            Text = "Cerrar",
            DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            Width = 120,
            Height = 36,
            Margin = new Padding(0, 8, 0, 0)
        };
        // Estilo de botón secundario del tema
        Estilos.TemaOscuro.EstilizarBotonSecundario(btnCerrar);
        btnCerrar.Location = new System.Drawing.Point(panelBotones.Width - btnCerrar.Width - 8, panelBotones.Height - btnCerrar.Height - 8);
        panelBotones.Resize += (_, __) => btnCerrar.Location = new System.Drawing.Point(panelBotones.Width - btnCerrar.Width - 8, panelBotones.Height - btnCerrar.Height - 8);
        panelBotones.Controls.Add(btnCerrar);

        layout.Controls.Add(txt, 0, 0);
        layout.Controls.Add(panelBotones, 0, 1);
        frm.Controls.Add(layout);
        frm.AcceptButton = btnCerrar;
        frm.ShowDialog(this);
    }

    // Menú: Ayuda → Agradecimientos
    private void ayudaAgradecimientosToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var frm = new Form
        {
            Text = "Agradecimientos",
            StartPosition = FormStartPosition.CenterParent,
            Size = new System.Drawing.Size(680, 420),
            MinimumSize = new System.Drawing.Size(580, 360),
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            Padding = new Padding(12)
        };

        frm.BackColor = Estilos.TemaOscuro.ColorPanel;
        frm.ForeColor = System.Drawing.Color.FromArgb(229, 231, 235);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));

        var lbl = new Label
        {
            Dock = DockStyle.Fill,
            Font = new System.Drawing.Font("Segoe UI", 10F),
            TextAlign = System.Drawing.ContentAlignment.TopLeft,
            Padding = new Padding(4),
            Text =
            "Este proyecto reconoce y agradece la inspiración y herramientas de:\r\n\r\n" +
            "- jonathanhecl / vb6-ia-tools (https://github.com/jonathanhecl/vb6-ia-tools)\r\n\r\n" +
            "Así como a todas las personas que han aportado ideas y pruebas."
        };

        var panelBotones = new Panel { Dock = DockStyle.Fill };
        var btnCerrar = new Button
        {
            Text = "Cerrar",
            DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            Width = 120,
            Height = 36
        };
        Estilos.TemaOscuro.EstilizarBotonSecundario(btnCerrar);
        btnCerrar.Location = new System.Drawing.Point(panelBotones.Width - btnCerrar.Width - 8, panelBotones.Height - btnCerrar.Height - 8);
        panelBotones.Resize += (_, __) => btnCerrar.Location = new System.Drawing.Point(panelBotones.Width - btnCerrar.Width - 8, panelBotones.Height - btnCerrar.Height - 8);
        panelBotones.Controls.Add(btnCerrar);

        layout.Controls.Add(lbl, 0, 0);
        layout.Controls.Add(panelBotones, 0, 1);
        frm.Controls.Add(layout);
        frm.AcceptButton = btnCerrar;
        frm.ShowDialog(this);
    }

    // Menú: Estilos de barra
    private void barraBloquesToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        // ProgressBarEx no soporta estilos nativos; solo persistimos la preferencia
        Preferencias.GuardarEstiloBarra("Blocks");
    }

    private void barraContinuaToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        Preferencias.GuardarEstiloBarra("Continuous");
    }

    private void barraIndeterminadaToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        // No-op para ProgressBarEx; solo guardamos preferencia
        Preferencias.GuardarEstiloBarra("Marquee");
    }

    // Menú: Color de barra
    private void colorVerdeToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        AplicarColorBarra("Verde");
        Preferencias.GuardarColorBarra("Verde");
    }

    private void colorAmarilloToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        AplicarColorBarra("Amarillo");
        Preferencias.GuardarColorBarra("Amarillo");
    }

    private void colorRojoToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        AplicarColorBarra("Rojo");
        Preferencias.GuardarColorBarra("Rojo");
    }

    private void colorAzulToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        AplicarColorBarra("Azul");
        Preferencias.GuardarColorBarra("Azul");
    }

    private void colorCianToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        AplicarColorBarra("Cian");
        Preferencias.GuardarColorBarra("Cian");
    }

    private void colorElegirToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var dlg = new ColorDialog();
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            pbProgreso.ForeColor = dlg.Color;
            lblProgreso.ForeColor = System.Drawing.Color.FromArgb(229, 231, 235);
            Preferencias.GuardarColorBarra($"#{dlg.Color.R:X2}{dlg.Color.G:X2}{dlg.Color.B:X2}");
        }
    }

    private void AplicarColorBarra(string nombre)
    {
        // Nota: ProgressBar estándar no soporta color directo en estilos modernos.
        // Como solución simple, ajustamos ForeColor que en muchos temas afecta al relleno.
        // Si no fuese suficiente, podríamos reemplazar por un control custom.
        if (nombre?.StartsWith("#") == true && nombre.Length == 7)
        {
            pbProgreso.ForeColor = System.Drawing.ColorTranslator.FromHtml(nombre);
        }
        else
        {
            switch (nombre)
            {
                case "Amarillo": pbProgreso.ForeColor = System.Drawing.Color.Gold; break;
                case "Rojo": pbProgreso.ForeColor = System.Drawing.Color.IndianRed; break;
                case "Azul": pbProgreso.ForeColor = System.Drawing.Color.SteelBlue; break;
                case "Cian": pbProgreso.ForeColor = System.Drawing.Color.MediumTurquoise; break;
                default: pbProgreso.ForeColor = System.Drawing.Color.MediumSeaGreen; break; // Verde
            }
        }
        lblProgreso.ForeColor = System.Drawing.Color.FromArgb(229, 231, 235); // texto claro
    }
}
