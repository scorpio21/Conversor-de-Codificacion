using System;
using System.IO;
using System.Text;

namespace ConversorCodificacion.Core.Servicios
{
    /// <summary>
    /// Registro simple de logs en carpeta "Logs" junto al ejecutable.
    /// Crea un archivo por día con formato app_yyyy-MM-dd.log.
    /// </summary>
    public static class Registro
    {
        private static readonly object _lock = new object();

        private static string BaseLogs()
        {
            try
            {
                var baseDir = AppContext.BaseDirectory;
                var logs = Path.Combine(baseDir, "Logs");
                Directory.CreateDirectory(logs);
                return logs;
            }
            catch
            {
                // Fallback a carpeta temporal si no se puede crear junto al exe
                var tmp = Path.Combine(Path.GetTempPath(), "ConversorCodificacionLogs");
                Directory.CreateDirectory(tmp);
                return tmp;
            }
        }

        private static string ArchivoDelDia()
        {
            var nombre = $"app_{DateTime.Now:yyyy-MM-dd}.log";
            return Path.Combine(BaseLogs(), nombre);
        }

        /// <summary>
        /// Devuelve la ruta absoluta de la carpeta donde se guardan los logs.
        /// </summary>
        public static string ObtenerRutaLogs()
        {
            return BaseLogs();
        }

        public static void LogInfo(string mensaje)
        {
            Escribir($"[INFO]  {mensaje}");
        }

        public static void LogError(string mensaje, Exception ex)
        {
            var texto = new StringBuilder();
            texto.AppendLine($"[ERROR] {mensaje}");
            texto.AppendLine(ex.ToString());
            Escribir(texto.ToString());
        }

        private static void Escribir(string linea)
        {
            try
            {
                var path = ArchivoDelDia();
                var registro = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {linea}{Environment.NewLine}";
                lock (_lock)
                {
                    File.AppendAllText(path, registro, Encoding.UTF8);
                }
            }
            catch
            {
                // Evitar que un fallo en logging afecte a la app
            }
        }
    }
}
