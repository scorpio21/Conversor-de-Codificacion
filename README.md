# Conversor de Codificación (UTF-8 ↔ Windows-1252)

Tools for use IA on VB6 (Visual Basic 6)

## Créditos

Este proyecto agradece y reconoce la inspiración y herramientas del repositorio:

- [jonathanhecl/vb6-ia-tools](https://github.com/jonathanhecl/vb6-ia-tools)

## Scripts de conversión

### Versión PowerShell (Windows)

- `script/ConvertTo-Utf8.ps1` - Convierte archivos de Windows-1252 a UTF-8
- `script/ConvertTo-Win1252.ps1` - Convierte archivos de UTF-8 a Windows-1252

### Versión PHP (XAMPP/Apache)

- `utilidades/convert-encoding.php` - Interfaz web para convertir archivos entre Windows-1252 y UTF-8 (sin BOM) recorriendo subcarpetas.

## Uso de la versión PHP

1. Inicia Apache en XAMPP.
2. Abre en el navegador:
   - `http://localhost/Conversor-de-Codificacion/index.php`
3. En el formulario:
   - Indica la carpeta base a procesar.
   - Elige la dirección de conversión: Windows-1252 → UTF-8 o UTF-8 → Windows-1252.
   - Define extensiones (por defecto: `bas, cls, frm, vbp`).
   - Marca si quieres incluir subcarpetas y crear backups `.bak` (opcional).
4. Pulsa "Convertir" para ejecutar.

## Recursos estáticos

- CSS: `css/convert-encoding.css`
- JS: `js/convert-encoding.js`

## Uso de los scripts de PowerShell

### Convertir a UTF-8

```powershell
.\script\ConvertTo-Utf8.ps1 [-Path "ruta\al\directorio"]
```

### Convertir a Windows-1252

```powershell
.\script\ConvertTo-Win1252.ps1 [-Path "ruta\al\directorio"]
```

### Ejemplos

1. Convertir archivos en el directorio actual:

   ```powershell
   .\script\ConvertTo-Utf8.ps1
   ```

2. Convertir archivos en un directorio específico:

   ```powershell
   .\script\ConvertTo-Utf8.ps1 -Path "C:\ruta\a\mi\proyecto"
   ```

## Características

- Procesa archivos .bas, .cls, .frm y .vbp de forma recursiva
- Muestra un resumen de la conversión
- Manejo de errores detallado
- Compatible con PowerShell 5.1 y versiones posteriores

## Notas

- Los scripts de PowerShell están diseñados para Windows.
- Los scripts de bash requieren `iconv` instalado.

## Agradecimientos

Este proyecto agradece e inspira su trabajo en el repositorio:

- [jonathanhecl/vb6-ia-tools](https://github.com/jonathanhecl/vb6-ia-tools): Tools for use IA on VB6 (Visual Basic 6)
