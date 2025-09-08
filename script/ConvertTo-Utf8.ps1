<#
.SYNOPSIS
    Convierte archivos de Windows-1252 a UTF-8 sin BOM.
.DESCRIPTION
    Este script convierte archivos de texto de codificación Windows-1252 a UTF-8 sin BOM.
    Por defecto, busca archivos .bas, .cls, .frm y .vbp de forma recursiva.
.PARAMETER Path
    Ruta al directorio que contiene los archivos a convertir. Por defecto es el directorio actual.
.EXAMPLE
    .\ConvertTo-Utf8.ps1
    Convierte todos los archivos en el directorio actual.
.EXAMPLE
    .\ConvertTo-Utf8.ps1 -Path "C:\mi\proyecto"
    Convierte todos los archivos en el directorio especificado.
#>

[CmdletBinding()]
param (
    [Parameter(Position = 0)]
    [string]$Path = "."
)

# Verificar que el directorio exista
if (-not (Test-Path -Path $Path)) {
    Write-Error "El directorio '$Path' no existe."
    exit 1
}

# Obtener archivos a convertir
$files = Get-ChildItem -Path $Path -Include @("*.bas", "*.cls", "*.frm", "*.vbp") -Recurse -File

if ($files.Count -eq 0) {
    Write-Host "No se encontraron archivos para convertir en la ruta especificada." -ForegroundColor Yellow
    exit 0
}

Write-Host "Iniciando conversión a UTF-8..." -ForegroundColor Cyan
Write-Host "Archivos encontrados: $($files.Count)" -ForegroundColor Cyan

$converted = 0
$errors = 0

foreach ($file in $files) {
    try {
        # Leer el archivo como bytes
        $content = [System.IO.File]::ReadAllBytes($file.FullName)
        
        # Convertir de Windows-1252 a string
        $contentStr = [System.Text.Encoding]::GetEncoding(1252).GetString($content)
        
        # Escribir como UTF-8 sin BOM
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($file.FullName, $contentStr, $utf8NoBom)
        
        Write-Host "Convertido: $($file.FullName)" -ForegroundColor Green
        $converted++
    }
    catch {
        Write-Host "Error al convertir: $($file.FullName)" -ForegroundColor Red
        Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
        $errors++
    }
}

# Mostrar resumen
Write-Host "`nResumen de la conversion:" -ForegroundColor Cyan
Write-Host "- Archivos procesados: $($files.Count)" -ForegroundColor Cyan
Write-Host "- Archivos convertidos: $converted" -ForegroundColor Green

if ($errors -gt 0) {
    Write-Host "- Errores: $errors" -ForegroundColor Red
}

Write-Host "`nProceso completado." -ForegroundColor Cyan

# Pausa para que el usuario pueda ver los resultados
Write-Host "`nPresiona cualquier tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
