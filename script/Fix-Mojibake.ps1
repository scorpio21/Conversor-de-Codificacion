<#
.SYNOPSIS
    Repara mojibake típico (Ã, Â, â€¦) producido por conversiones erróneas entre UTF-8 y Windows-1252.
.DESCRIPTION
    Intenta revertir 1 o 2 capas de mojibake aplicando esta transformación:
        textoCorregido = UTF8.GetString(Win1252.GetBytes(textoMojibake))
    Evalúa si después de cada pasada disminuye la presencia de patrones comunes de mojibake.
    Luego guarda el archivo final en Windows-1252 (recomendado para proyectos VB6).
.PARAMETER Path
    Ruta base donde buscar archivos a reparar.
.PARAMETER Include
    Extensiones a procesar (por defecto .bas, .cls, .frm, .vbp).
.PARAMETER WhatIf
    Si se establece, no escribe cambios: solo muestra qué haría.
.EXAMPLE
    .\Fix-Mojibake.ps1 -Path "d:\xampp\htdocs\prueba"
#>

[CmdletBinding(SupportsShouldProcess=$true)]
param (
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Path,

    [string[]]$Include = @("*.bas", "*.cls", "*.frm", "*.vbp"),

    [switch]$WhatIf
)

# Validaciones
if (-not (Test-Path -Path $Path)) {
    Write-Error "El directorio '$Path' no existe."
    exit 1
}

$files = Get-ChildItem -Path $Path -Recurse -File -Include $Include
if ($files.Count -eq 0) {
    Write-Host "No se encontraron archivos para procesar en la ruta especificada." -ForegroundColor Yellow
    exit 0
}

$enc1252 = [System.Text.Encoding]::GetEncoding(1252)
$encUtf8 = [System.Text.Encoding]::UTF8

function Get-MojibakeScore([string]$text) {
    # Cuenta patrones frecuentes de mojibake usando códigos Unicode explícitos
    $patterns = @(
        [string][char]0x00C3, # Ã
        [string][char]0x00C2, # Â
        [string][char]0x00E2, # â
        [string][char]0x20AC, # €
        [string][char]0x2013, # –
        [string][char]0x2014, # —
        [string][char]0x201C, # “
        [string][char]0x201D, # ”
        [string][char]0x2018, # ‘
        [string][char]0x2019, # ’
        [string][char]0x2022, # •
        [string][char]0x2026  # …
    )
    $score = 0
    foreach ($p in $patterns) { $score += ([regex]::Matches($text, [regex]::Escape($p))).Count }
    return $score
}

function Fix-Once([string]$text) {
    # Una pasada: 1252 bytes -> UTF8 string
    $bytes = $enc1252.GetBytes($text)
    return $encUtf8.GetString($bytes)
}

$fixedCount = 0
$skipped = 0
$errors = 0

foreach ($file in $files) {
    try {
        # Lee como 1252 para capturar visualmente el mojibake más común
        $raw = [System.IO.File]::ReadAllText($file.FullName, $enc1252)
        $s0 = Get-MojibakeScore $raw

        # Primera pasada
        $s1Text = Fix-Once $raw
        $s1 = Get-MojibakeScore $s1Text

        # Decide si aceptar la primera pasada
        $bestText = $raw
        $bestScore = $s0
        $passes = 0

        if ($s1 -lt $s0) {
            $bestText = $s1Text
            $bestScore = $s1
            $passes = 1

            # Segunda pasada (para doble mojibake)
            $s2Text = Fix-Once $s1Text
            $s2 = Get-MojibakeScore $s2Text
            if ($s2 -lt $s1) {
                $bestText = $s2Text
                $bestScore = $s2
                $passes = 2
            }
        }

        if ($passes -gt 0) {
            Write-Host "[REPARADO x$passes] $($file.FullName)  (score: $s0 -> $bestScore)" -ForegroundColor Green
            if (-not $WhatIf) {
                # Guardar como Windows-1252
                [System.IO.File]::WriteAllText($file.FullName, $bestText, $enc1252)
            }
            $fixedCount++
        } else {
            Write-Host "[OK] $($file.FullName) sin mejora necesaria (score: $s0)" -ForegroundColor DarkGray
            $skipped++
        }
    }
    catch {
        Write-Host "[ERROR] $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
        $errors++
    }
}

Write-Host "`nResumen:" -ForegroundColor Cyan
Write-Host "- Archivos reparados: $fixedCount" -ForegroundColor Green
Write-Host "- Archivos sin cambios: $skipped" -ForegroundColor Yellow
if ($errors -gt 0) { Write-Host "- Errores: $errors" -ForegroundColor Red }
