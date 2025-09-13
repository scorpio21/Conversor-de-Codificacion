using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConversorCodificacion.Core.Servicios
{
    /// <summary>
    /// Servicio principal para conversión de archivos entre UTF-8 (sin BOM) y Windows-1252
    /// incluyendo salvaguardas específicas para proyectos VB6.
    /// </summary>
    public static class ConversorDeCodificacion
    {
        // Codificaciones
        private static readonly Encoding Utf8SinBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        private static readonly Encoding Win1252 = Encoding.GetEncoding(1252);

        /// <summary>
        /// Normaliza finales de línea a CRLF (Windows). Recomendado para archivos VB6.
        /// </summary>
        public static string AsegurarCrLf(string texto)
        {
            if (texto is null) return string.Empty;
            var tmp = texto.Replace("\r\n", "\n").Replace("\r", "\n");
            return tmp.Replace("\n", "\r\n");
        }

        /// <summary>
        /// Determina si la ruta corresponde a una extensión típica de VB6.
        /// </summary>
        public static bool EsExtensionVb6(string ruta)
        {
            var ext = Path.GetExtension(ruta).TrimStart('.').ToLowerInvariant();
            return ext is "frm" or "vbp" or "cls" or "bas";
        }

        /// <summary>
        /// Extensiones críticas de VB6 que no deben convertirse a UTF-8.
        /// </summary>
        public static bool EsExtensionCriticaVb6(string ruta)
        {
            var ext = Path.GetExtension(ruta).TrimStart('.').ToLowerInvariant();
            return ext is "frm" or "vbp";
        }

        /// <summary>
        /// Lista archivos con extensiones indicadas. Si recursivo es true, recorre subcarpetas.
        /// </summary>
        public static IEnumerable<string> ListarArchivos(string basePath, IEnumerable<string> extensiones, bool recursivo)
        {
            if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath)) yield break;
            var set = new HashSet<string>(extensiones.Select(e => e.Trim().TrimStart('.').ToLowerInvariant()));
            var opt = recursivo ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var ruta in Directory.EnumerateFiles(basePath, "*", opt))
            {
                var ext = Path.GetExtension(ruta).TrimStart('.').ToLowerInvariant();
                if (set.Contains(ext)) yield return ruta;
            }
        }

        /// <summary>
        /// Parsea extensiones separadas por coma/espacio/punto y devuelve una lista limpia.
        /// </summary>
        public static List<string> ParsearExtensiones(string texto, IEnumerable<string> porDefecto)
        {
            texto = (texto ?? string.Empty).Trim();
            if (texto.Length == 0) return porDefecto.Select(x => x.ToLowerInvariant()).Distinct().ToList();
            var partes = texto.Split(new[] { ',', ';', ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return partes.Select(p => p.Trim().TrimStart('.').ToLowerInvariant()).Where(p => p.Length > 0).Distinct().ToList();
        }

        /// <summary>
        /// Crea un backup .bak si no existe aún.
        /// </summary>
        public static void AsegurarBackupOpcional(string rutaArchivo)
        {
            var bak = rutaArchivo + ".bak";
            if (!File.Exists(bak)) File.Copy(rutaArchivo, bak, overwrite: false);
        }

        /// <summary>
        /// Devuelve true si los bytes representan UTF-8 válido.
        /// </summary>
        public static bool EsUtf8Valido(byte[] bytes)
        {
            try
            {
                _ = Utf8SinBom.GetString(bytes);
                // Si la decodificación no lanza, consideramos válido
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convierte Windows-1252 → UTF-8 sin BOM. Omite .frm/.vbp por seguridad. Normaliza CRLF en VB6.
        /// </summary>
        public static (bool ok, string mensaje) ConvertirAUtf8(string rutaArchivo)
        {
            try
            {
                if (EsExtensionCriticaVb6(rutaArchivo))
                    return (true, "Omitido por seguridad (.frm/.vbp deben permanecer en Windows-1252)");

                var bytes = File.ReadAllBytes(rutaArchivo);
                if (EsUtf8Valido(bytes))
                    return (true, "Sin cambios (ya es UTF-8 válido)");

                var texto = Win1252.GetString(bytes);
                if (EsExtensionVb6(rutaArchivo)) texto = AsegurarCrLf(texto);
                File.WriteAllText(rutaArchivo, texto, Utf8SinBom);
                return (true, "Convertido a UTF-8");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Convierte UTF-8 → Windows-1252. Normaliza CRLF en VB6.
        /// </summary>
        public static (bool ok, string mensaje) ConvertirAWin1252(string rutaArchivo)
        {
            try
            {
                var texto = File.ReadAllText(rutaArchivo, Utf8SinBom);
                if (EsExtensionVb6(rutaArchivo)) texto = AsegurarCrLf(texto);
                var bytes = Win1252.GetBytes(texto);
                File.WriteAllBytes(rutaArchivo, bytes);
                return (true, "Convertido a Windows-1252");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Restaura desde .bak si existe.
        /// </summary>
        public static (bool ok, string mensaje) RestaurarDesdeBak(string rutaArchivo)
        {
            try
            {
                var bak = rutaArchivo + ".bak";
                if (!File.Exists(bak)) return (false, "No existe .bak para este archivo");
                File.Copy(bak, rutaArchivo, overwrite: true);
                return (true, "Restaurado desde .bak");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Heurística simple de conteo de patrones de mojibake.
        /// </summary>
        public static int PuntajeMojibake(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return 0;
            var patrones = new[] { "Ã", "Â", "â", "€", "â€“", "â€”", "â€œ", "â€\u0001", "â€", "â€˜", "â€™", "â€¢", "â€¦" };
            var suma = 0;
            foreach (var p in patrones) suma += texto.Split(p).Length - 1;
            return suma;
        }

        /// <summary>
        /// Intenta reparar una capa de mojibake (UTF-8→CP1252→UTF-8) y guarda si mejora.
        /// </summary>
        public static (bool ok, string mensaje) RepararMojibake(string rutaArchivo)
        {
            try
            {
                var texto = File.ReadAllText(rutaArchivo, Utf8SinBom);
                var s0 = PuntajeMojibake(texto);

                string Paso(string input)
                {
                    // Simula UTF-8→CP1252 (pérdidas) y vuelta a UTF-8
                    var b1252 = Win1252.GetBytes(input);
                    return Utf8SinBom.GetString(b1252);
                }

                var t1 = Paso(texto); var s1 = PuntajeMojibake(t1);
                var mejor = texto; var sMejor = s0; var pasadas = 0;
                if (s1 < s0)
                {
                    mejor = t1; sMejor = s1; pasadas = 1;
                    var t2 = Paso(t1); var s2 = PuntajeMojibake(t2);
                    if (s2 < s1) { mejor = t2; sMejor = s2; pasadas = 2; }
                }

                if (pasadas > 0)
                {
                    if (EsExtensionVb6(rutaArchivo)) mejor = AsegurarCrLf(mejor);
                    var bytes = Win1252.GetBytes(mejor);
                    File.WriteAllBytes(rutaArchivo, bytes);
                    return (true, $"Reparado x{pasadas} (score: {s0} → {sMejor}) y guardado en Windows-1252");
                }

                return (true, "Sin mejora necesaria");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
