using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ConversorCodificacion.Core.Servicios;

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
        }
    }

    private void btnConvertir_Click(object? sender, EventArgs e)
    {
        lvResultados.BeginUpdate();
        lvResultados.Items.Clear();
        int procesados = 0, convertidos = 0, errores = 0;

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
            procesados = archivos.Count;

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

                    if (r.ok) convertidos++; else errores++;
                    AgregarResultado(archivo, r.mensaje);
                }
                catch (Exception ex)
                {
                    errores++;
                    AgregarResultado(archivo, ex.Message);
                }
            }
        }
        finally
        {
            lvResultados.EndUpdate();
            lblResumen.Text = $"Procesados: {procesados} | Conv.: {convertidos} | Err.: {errores}";
        }
    }

    private void AgregarResultado(string archivo, string mensaje)
    {
        var item = new ListViewItem(new[] { archivo, mensaje });
        lvResultados.Items.Add(item);
    }
}
