using System;
using System.IO;
using System.Text.Json;

namespace ConversorCodificacion.Win
{
    /// <summary>
    /// Manejo simple de preferencias de usuario (archivo JSON en carpeta Config junto al ejecutable).
    /// </summary>
    public static class Preferencias
    {
        private class Modelo
        {
            public string EstiloBarra { get; set; } = "Continuous";
            public string ColorBarra { get; set; } = "Verde";
        }

        private static string CarpetaConfig()
        {
            var baseDir = AppContext.BaseDirectory;
            var cfg = Path.Combine(baseDir, "Config");
            Directory.CreateDirectory(cfg);
            return cfg;
        }

        public static string ObtenerRutaConfig() => CarpetaConfig();

        private static string RutaJson() => Path.Combine(CarpetaConfig(), "appsettings.json");
        private static string RutaUltimaRutaJson() => Path.Combine(CarpetaConfig(), "ultima-ruta.json");

        public static string CargarEstiloBarra()
        {
            try
            {
                var ruta = RutaJson();
                if (!File.Exists(ruta)) return "Continuous";
                var json = File.ReadAllText(ruta);
                var modelo = JsonSerializer.Deserialize<Modelo>(json);
                return string.IsNullOrWhiteSpace(modelo?.EstiloBarra) ? "Continuous" : modelo!.EstiloBarra;
            }
            catch
            {
                return "Continuous";
            }
        }

        public static void GuardarEstiloBarra(string estilo)
        {
            try
            {
                var ruta = RutaJson();
                var modelo = LeerExistente();
                modelo.EstiloBarra = estilo;
                var json = JsonSerializer.Serialize(modelo, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ruta, json);
            }
            catch
            {
                // Silenciar errores de guardado de preferencias
            }
        }

        public static string CargarColorBarra()
        {
            try
            {
                var ruta = RutaJson();
                if (!File.Exists(ruta)) return "Verde";
                var json = File.ReadAllText(ruta);
                var modelo = JsonSerializer.Deserialize<Modelo>(json);
                return string.IsNullOrWhiteSpace(modelo?.ColorBarra) ? "Verde" : modelo!.ColorBarra;
            }
            catch { return "Verde"; }
        }

        public static void GuardarColorBarra(string color)
        {
            try
            {
                var ruta = RutaJson();
                var modelo = LeerExistente();
                modelo.ColorBarra = color;
                var json = JsonSerializer.Serialize(modelo, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ruta, json);
            }
            catch { }
        }

        private static Modelo LeerExistente()
        {
            try
            {
                var ruta = RutaJson();
                if (!File.Exists(ruta)) return new Modelo();
                var json = File.ReadAllText(ruta);
                return JsonSerializer.Deserialize<Modelo>(json) ?? new Modelo();
            }
            catch { return new Modelo(); }
        }

        // Última ruta usada (JSON separado)
        private class UltimaRutaModelo { public string Ruta { get; set; } = string.Empty; }

        public static void GuardarUltimaRuta(string ruta)
        {
            try
            {
                var modelo = new UltimaRutaModelo { Ruta = ruta ?? string.Empty };
                var json = JsonSerializer.Serialize(modelo, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(RutaUltimaRutaJson(), json);
            }
            catch { }
        }

        public static string CargarUltimaRuta()
        {
            try
            {
                var archivo = RutaUltimaRutaJson();
                if (!File.Exists(archivo)) return string.Empty;
                var json = File.ReadAllText(archivo);
                var modelo = JsonSerializer.Deserialize<UltimaRutaModelo>(json);
                return modelo?.Ruta ?? string.Empty;
            }
            catch { return string.Empty; }
        }
    }
}
