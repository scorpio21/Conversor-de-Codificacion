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

        // Cargar preferencia de estilo de barra
        var estilo = Preferencias.CargarEstiloBarra();
        switch (estilo)
        {
            case "Blocks": pbProgreso.Style = ProgressBarStyle.Blocks; break;
            case "Marquee": pbProgreso.Style = ProgressBarStyle.Marquee; pbProgreso.MarqueeAnimationSpeed = 30; break;
            default: pbProgreso.Style = ProgressBarStyle.Continuous; break;
        }

        // Color inicial de barra según preferencias
        var color = Preferencias.CargarColorBarra();
        AplicarColorBarra(color);
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
                    }
                    catch (Exception ex)
                    {
                        ((IProgress<(string, string, bool)>)progress).Report((archivo, ex.Message, false));
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

    // Menú: Estilos de barra
    private void barraBloquesToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        pbProgreso.Style = ProgressBarStyle.Blocks;
        Preferencias.GuardarEstiloBarra("Blocks");
    }

    private void barraContinuaToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        pbProgreso.Style = ProgressBarStyle.Continuous;
        Preferencias.GuardarEstiloBarra("Continuous");
    }

    private void barraIndeterminadaToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        // Mostrar estilo indeterminado (marquee) mientras se realiza una tarea no cuantificable.
        pbProgreso.Style = ProgressBarStyle.Marquee;
        pbProgreso.MarqueeAnimationSpeed = 30; // ms
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
