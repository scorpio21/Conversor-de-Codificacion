using System;
using System.Threading;
using System.Windows.Forms;
using ConversorCodificacion.Core.Servicios;

namespace ConversorCodificacion.Win;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => ManejarExcepcion(e.Exception, "ThreadException");
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ManejarExcepcion(e.ExceptionObject as Exception, "UnhandledException");

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
        catch (Exception ex)
        {
            ManejarExcepcion(ex, "Main try/catch");
        }
    }

    private static void ManejarExcepcion(Exception? ex, string origen)
    {
        try { if (ex != null) Registro.LogError($"Excepción no controlada ({origen})", ex); }
        catch { }
        MessageBox.Show($"Ocurrió un error inesperado. Revisa Archivo → Abrir log para más detalles.\n\n{ex}",
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}