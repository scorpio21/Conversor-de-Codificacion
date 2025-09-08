<#
.SYNOPSIS
    Convierte archivos de UTF-8 a Windows-1252.
.DESCRIPTION
    Este script convierte archivos de texto de codificación UTF-8 a Windows-1252.
    Por defecto, busca archivos .bas, .cls, .frm y .vbp de forma recursiva.
.PARAMETER Path
    Ruta al directorio que contiene los archivos a convertir. Por defecto es el directorio actual.
.EXAMPLE
    .\ConvertTo-Win1252.ps1
    Convierte todos los archivos en el directorio actual.
.EXAMPLE
    .\ConvertTo-Win1252.ps1 -Path "C:\mi\proyecto"
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

Write-Host "Iniciando conversión a Windows-1252..." -ForegroundColor Cyan
Write-Host "Archivos encontrados: $($files.Count)" -ForegroundColor Cyan

$converted = 0
$errors = 0

foreach ($file in $files) {
    try {
        # Leer el archivo como UTF-8
        $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
        
        # Convertir a Windows-1252
        $win1252 = [System.Text.Encoding]::GetEncoding(1252)
        $contentBytes = $win1252.GetBytes($content)
        
        # Escribir el archivo
        [System.IO.File]::WriteAllBytes($file.FullName, $contentBytes)
        
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
