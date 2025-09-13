<?php
declare(strict_types=1);

// Conversor de codificaciones UTF-8 ↔ Windows-1252
// - Permite elegir una carpeta en el servidor
// - Recorre subcarpetas
// - Aplica filtros por extensión
// - Conversión en ambos sentidos sin BOM para UTF-8

// Reglas de seguridad básicas: este script asume uso en entorno local (XAMPP)
// No expone operaciones de escritura fuera de la ruta base del proyecto a menos que se indique explícitamente

// Configuración por defecto
$defaultBasePath = realpath(dirname(__DIR__)); // raíz del proyecto
$defaultExtensions = ['bas', 'cls', 'frm', 'vbp'];

// Utilidades
function limpiarRuta(string $ruta): string {
    // Normaliza separadores y elimina espacios extremos
    $ruta = trim($ruta);
    $ruta = str_replace(['/', '\\'], DIRECTORY_SEPARATOR, $ruta);
    return rtrim($ruta, DIRECTORY_SEPARATOR);
}

/**
 * Normaliza finales de línea a CRLF (Windows), recomendado para archivos de VB6.
 */
function asegurarCRLF(string $texto): string {
    // Normaliza primero a LF y luego a CRLF
    $tmp = str_replace(["\r\n", "\r"], "\n", $texto);
    return str_replace("\n", "\r\n", $tmp);
}

/**
 * Determina si la extensión corresponde a archivos típicos de VB6.
 */
function esExtensionVB6(string $rutaArchivo): bool {
    $ext = strtolower(pathinfo($rutaArchivo, PATHINFO_EXTENSION));
    return in_array($ext, ['frm', 'vbp', 'cls', 'bas'], true);
}

/**
 * Extensiones críticas de VB6 que deben permanecer en ANSI/CP1252 para evitar corrupción en IDE.
 */
function esExtensionCriticaVB6(string $rutaArchivo): bool {
    $ext = strtolower(pathinfo($rutaArchivo, PATHINFO_EXTENSION));
    return in_array($ext, ['frm', 'vbp'], true);
}

/**
 * Verifica si una cadena de bytes es UTF-8 válida.
 */
function esUtf8Valido(string $bytes): bool {
    return function_exists('mb_check_encoding') ? mb_check_encoding($bytes, 'UTF-8') : false;
}

/**
 * Restaura un archivo desde su copia .bak si existe.
 */
function restaurarDesdeBak(string $rutaArchivo): array {
    try {
        $bak = $rutaArchivo . '.bak';
        if (!file_exists($bak)) {
            return ['ok' => false, 'mensaje' => 'No existe .bak para este archivo'];
        }
        if (!is_readable($bak)) {
            return ['ok' => false, 'mensaje' => 'El .bak no es legible'];
        }
        $contenido = file_get_contents($bak);
        if ($contenido === false) {
            throw new RuntimeException('No se pudo leer el .bak');
        }
        $ok = file_put_contents($rutaArchivo, $contenido);
        if ($ok === false) {
            throw new RuntimeException('No se pudo escribir el archivo de destino');
        }
        return ['ok' => true, 'mensaje' => 'Restaurado desde .bak'];
    } catch (Throwable $e) {
        return ['ok' => false, 'mensaje' => $e->getMessage()];
    }
}

/**
 * Calcula un puntaje de "mojibake" contando patrones típicos como "Ã", "Â", "â", comillas tipográficas rotas, etc.
 * Un número mayor indica más corrupción visible.
 */
function puntajeMojibake(string $texto): int {
    $patrones = [
        'Ã', // aparece cuando un byte 0xC3 de UTF-8 fue mal interpretado
        'Â', // aparece con signos de puntuación y espacios duros
        'â', // familia de comillas y guiones tipográficos rotos
        '€', // suele acompañar secuencias mal decodificadas
        'â€“', 'â€”', 'â€œ', 'â€', 'â€', 'â€˜', 'â€™', 'â€¢', 'â€¦'
    ];
    $suma = 0;
    foreach ($patrones as $p) {
        $suma += substr_count($texto, $p);
    }
    return $suma;
}

/**
 * Intenta reparar una capa de mojibake común:
 *  - Convierte de UTF-8 a Windows-1252 (como bytes) ignorando lo que no mapea
 *  - Vuelve a interpretar esos bytes como UTF-8
 */
function repararTextoMojibakeUnaVez(string $texto): string {
    $paso1 = @iconv('UTF-8', 'Windows-1252//IGNORE', $texto);
    if ($paso1 === false) {
        $paso1 = @mb_convert_encoding($texto, 'Windows-1252', 'UTF-8');
    }
    if ($paso1 === false) { return $texto; }
    $paso2 = @iconv('Windows-1252', 'UTF-8//IGNORE', $paso1);
    if ($paso2 === false) {
        $paso2 = @mb_convert_encoding($paso1, 'UTF-8', 'Windows-1252');
    }
    return $paso2 === false ? $texto : $paso2;
}

/**
 * Repara mojibake en un archivo aplicando 1–2 pasadas si mejoran el puntaje y guarda el resultado en Windows-1252.
 */
function repararMojibake(string $rutaArchivo): array {
    try {
        $raw = file_get_contents($rutaArchivo);
        if ($raw === false) {
            throw new RuntimeException('No se pudo leer el archivo');
        }

        // Interpretamos el contenido como UTF-8 a nivel de string para la heurística.
        // Si no es UTF-8 válido, PHP igualmente lo tratará como bytes y la heurística seguirá contando patrones.
        $s0 = puntajeMojibake($raw);

        $t1 = repararTextoMojibakeUnaVez($raw);
        $s1 = puntajeMojibake($t1);

        $mejor = $raw; $sMejor = $s0; $pasadas = 0;
        if ($s1 < $s0) {
            $mejor = $t1; $sMejor = $s1; $pasadas = 1;
            // Intento de doble capa
            $t2 = repararTextoMojibakeUnaVez($t1);
            $s2 = puntajeMojibake($t2);
            if ($s2 < $s1) { $mejor = $t2; $sMejor = $s2; $pasadas = 2; }
        }

        if ($pasadas > 0) {
            // Guardar como Windows-1252 (recomendado para VB6)
            $bytes1252 = @iconv('UTF-8', 'Windows-1252//TRANSLIT//IGNORE', $mejor);
            if ($bytes1252 === false) {
                $bytes1252 = @mb_convert_encoding($mejor, 'Windows-1252', 'UTF-8');
            }
            if ($bytes1252 === false) {
                throw new RuntimeException('No se pudo convertir el resultado a Windows-1252');
            }
            $ok = file_put_contents($rutaArchivo, $bytes1252);
            if ($ok === false) {
                throw new RuntimeException('No se pudo escribir el archivo');
            }
            return ['ok' => true, 'mensaje' => "Reparado x{$pasadas} (score: {$s0} → {$sMejor}) y guardado en Windows-1252"];
        }

        return ['ok' => true, 'mensaje' => 'Sin mejora necesaria'];
    } catch (Throwable $e) {
        return ['ok' => false, 'mensaje' => $e->getMessage()];
    }
}

function esRutaValida(string $ruta): bool {
    return $ruta !== '' && is_dir($ruta) && is_readable($ruta) && is_writable($ruta);
}

function listarSubcarpetas(string $ruta): array {
    $ruta = limpiarRuta($ruta);
    $subdirs = [];
    if (!is_dir($ruta) || !is_readable($ruta)) {
        return $subdirs;
    }
    $it = new DirectoryIterator($ruta);
    foreach ($it as $f) {
        if ($f->isDot()) { continue; }
        if ($f->isDir()) {
            $subdirs[] = $f->getPathname();
        }
    }
    sort($subdirs, SORT_NATURAL | SORT_FLAG_CASE);
    return $subdirs;
}

function parsearExtensiones(string $texto, array $porDefecto): array {
    $texto = trim($texto);
    if ($texto === '') return $porDefecto;
    $partes = preg_split('/[,;\s]+/', $texto);
    $exts = [];
    foreach ($partes as $p) {
        $p = strtolower(ltrim($p));
        $p = ltrim($p, '.');
        if ($p !== '') { $exts[$p] = true; }
    }
    return array_keys($exts);
}

function listarArchivos(string $basePath, array $extensiones, bool $recursivo = true): array {
    $resultado = [];
    $pattern = '/\.(' . implode('|', array_map('preg_quote', $extensiones)) . ')$/i';

    $iterador = $recursivo
        ? new RecursiveIteratorIterator(new RecursiveDirectoryIterator($basePath, FilesystemIterator::SKIP_DOTS))
        : new IteratorIterator(new DirectoryIterator($basePath));

    foreach ($iterador as $item) {
        if ($item instanceof SplFileInfo && $item->isFile()) {
            $ruta = $item->getPathname();
            if (preg_match($pattern, $ruta)) {
                $resultado[] = $ruta;
            }
        }
    }
    return $resultado;
}

function convertirAUtf8(string $rutaArchivo): array {
    // Convierte Windows-1252 → UTF-8 sin BOM, evitando doble conversión
    try {
        // Seguridad: no convertir a UTF-8 los archivos críticos de VB6 (.frm, .vbp)
        if (esExtensionCriticaVB6($rutaArchivo)) {
            return ['ok' => true, 'mensaje' => 'Omitido por seguridad (.frm/.vbp deben permanecer en Windows-1252)'];
        }
        $contenido = file_get_contents($rutaArchivo);
        if ($contenido === false) {
            throw new RuntimeException('No se pudo leer el archivo');
        }
        // Si ya es UTF-8 válido, no convertimos (evitamos doble conversión)
        if (esUtf8Valido($contenido)) {
            return ['ok' => true, 'mensaje' => 'Sin cambios (ya es UTF-8 válido)'];
        }
        // Convertir de CP1252 a UTF-8
        $convertido = iconv('Windows-1252', 'UTF-8//TRANSLIT', $contenido);
        if ($convertido === false) {
            // Fallback con mb
            $convertido = mb_convert_encoding((string)$contenido, 'UTF-8', 'Windows-1252');
        }
        // Asegurar CRLF en archivos de VB6
        if (esExtensionVB6($rutaArchivo)) {
            $convertido = asegurarCRLF($convertido);
        }
        $ok = file_put_contents($rutaArchivo, $convertido);
        if ($ok === false) {
            throw new RuntimeException('No se pudo escribir el archivo');
        }
        return ['ok' => true, 'mensaje' => 'Convertido a UTF-8'];
    } catch (Throwable $e) {
        return ['ok' => false, 'mensaje' => $e->getMessage()];
    }
}

function convertirAWin1252(string $rutaArchivo): array {
    // Convierte UTF-8 → Windows-1252 con salvaguardas (evita introducir mojibake)
    try {
        $contenido = file_get_contents($rutaArchivo);
        if ($contenido === false) {
            throw new RuntimeException('No se pudo leer el archivo');
        }
        // Si NO es UTF-8 válido y parece mojibake, intentamos reparar primero
        if (!esUtf8Valido($contenido)) {
            // Heurística: si hay patrones de mojibake, reparamos y luego escribimos CP1252
            if (puntajeMojibake($contenido) > 0) {
                $fix = repararMojibake($rutaArchivo);
                if ($fix['ok']) {
                    return $fix; // ya guardó en CP1252 si mejoró
                } else {
                    return ['ok' => false, 'mensaje' => 'Entrada no UTF-8 y con mojibake. Sugerido: usar "Arreglar mojibake". Detalle: ' . $fix['mensaje']];
                }
            } else {
                // No es UTF-8 y no hay señales de mojibake → probablemente ya esté en CP1252
                return ['ok' => true, 'mensaje' => 'Sin cambios (probablemente ya en Windows-1252)'];
            }
        }
        // Convertir de UTF-8 a CP1252 con transliteración cuando sea posible
        $bytes = iconv('UTF-8', 'Windows-1252//TRANSLIT//IGNORE', $contenido);
        if ($bytes === false) {
            // Fallback con mb (puede reemplazar caracteres no representables)
            $bytes = mb_convert_encoding($contenido, 'Windows-1252', 'UTF-8');
        }
        // Asegurar CRLF en archivos de VB6
        if (esExtensionVB6($rutaArchivo)) {
            $bytes = asegurarCRLF($bytes);
        }
        $ok = file_put_contents($rutaArchivo, $bytes);
        if ($ok === false) {
            throw new RuntimeException('No se pudo escribir el archivo');
        }
        return ['ok' => true, 'mensaje' => 'Convertido a Windows-1252'];
    } catch (Throwable $e) {
        return ['ok' => false, 'mensaje' => $e->getMessage()];
    }
}

function asegurarBackupOpcional(string $rutaArchivo): void {
    // Crea un backup .bak solo si no existe ya uno. Simple y opcional.
    $bak = $rutaArchivo . '.bak';
    if (!file_exists($bak)) {
        @copy($rutaArchivo, $bak);
    }
}

// Endpoint JSON para listar subcarpetas (usado por el modal "Elegir carpeta")
if (isset($_GET['listar'])) {
    $dir = isset($_GET['dir']) ? limpiarRuta((string)$_GET['dir']) : '';
    if ($dir === '' || !is_dir($dir)) {
        // Usa la raíz del proyecto como punto de partida si no se pasa una ruta válida
        $dir = $defaultBasePath;
    }

    // Construye respuesta
    $respuesta = [
        'ok' => true,
        'directorio' => $dir,
        'subcarpetas' => listarSubcarpetas($dir),
        'parent' => null,
    ];

    // Determina el directorio padre (si existe y es diferente)
    $padre = dirname($dir);
    if ($padre !== $dir && is_dir($padre)) {
        $respuesta['parent'] = $padre;
    }

    header('Content-Type: application/json; charset=UTF-8');
    echo json_encode($respuesta, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
    exit;
}

// Manejo de POST
$errores = [];
$resultados = [];
$resumen = [
    'procesados' => 0,
    'convertidos' => 0,
    'errores' => 0,
];

$basePath = $defaultBasePath;
$extText = implode(',', $defaultExtensions);
$direccion = 'a_utf8'; // a_utf8 | a_win1252 | fix_mojibake | restore_bak
$recursivo = true;
$hacerBackup = true;

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $basePath = isset($_POST['basePath']) ? limpiarRuta((string)$_POST['basePath']) : $defaultBasePath;
    $direccion = isset($_POST['direccion']) && in_array($_POST['direccion'], ['a_utf8', 'a_win1252', 'fix_mojibake', 'restore_bak'], true)
        ? $_POST['direccion']
        : 'a_utf8';
    $extText = isset($_POST['extensiones']) ? (string)$_POST['extensiones'] : $extText;
    $recursivo = isset($_POST['recursivo']) ? true : false;
    $hacerBackup = isset($_POST['backup']) ? true : false;

    if (!esRutaValida($basePath)) {
        $errores[] = 'La ruta indicada no existe o no tiene permisos de lectura/escritura.';
    } else {
        $extensiones = parsearExtensiones($extText, $defaultExtensions);
        if (empty($extensiones)) {
            $extensiones = $defaultExtensions;
        }

        $archivos = listarArchivos($basePath, $extensiones, $recursivo);
        $resumen['procesados'] = count($archivos);

        if ($resumen['procesados'] === 0) {
            $resultados[] = ['archivo' => '', 'estado' => 'info', 'mensaje' => 'No se encontraron archivos para convertir.'];
        } else {
            foreach ($archivos as $archivo) {
                if ($hacerBackup) {
                    asegurarBackupOpcional($archivo);
                }
                if ($direccion === 'a_utf8') {
                    $r = convertirAUtf8($archivo);
                } elseif ($direccion === 'a_win1252') {
                    $r = convertirAWin1252($archivo);
                } elseif ($direccion === 'restore_bak') {
                    $r = restaurarDesdeBak($archivo);
                } else {
                    $r = repararMojibake($archivo);
                }
                if ($r['ok']) {
                    $resumen['convertidos']++;
                    $resultados[] = ['archivo' => $archivo, 'estado' => 'ok', 'mensaje' => $r['mensaje']];
                } else {
                    $resumen['errores']++;
                    $resultados[] = ['archivo' => $archivo, 'estado' => 'error', 'mensaje' => $r['mensaje']];
                }
            }
        }
    }
}

// Valor seguro para mostrar en el input sin exponer rutas del servidor en carga inicial
// Si hubo POST, mostramos la ruta usada; si no, dejamos vacío
$basePathUI = ($_SERVER['REQUEST_METHOD'] === 'POST') ? $basePath : '';

?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Conversor de Codificación (UTF-8 ↔ Windows-1252)</title>
    <link rel="stylesheet" href="css/convert-encoding.css?v=1">
</head>
<body>
    <header class="cabecera">
        <h1>Conversor de Codificación</h1>
        <p>Convierte archivos .bas, .cls, .frm, .vbp entre UTF-8 (sin BOM) y Windows-1252, recorriendo subcarpetas.</p>
        <div class="creditos cabecera-creditos" role="note">
            Créditos: <a href="https://github.com/jonathanhecl/vb6-ia-tools" target="_blank" rel="noopener noreferrer">jonathanhecl/vb6-ia-tools</a>
            Tools for use IA on VB6 (Visual Basic 6)
        </div>
    </header>

    <main class="contenedor">
        <?php if (!empty($errores)): ?>
            <div class="alerta alerta-error">
                <ul>
                    <?php foreach ($errores as $e): ?>
                        <li><?= htmlspecialchars($e, ENT_QUOTES, 'UTF-8'); ?></li>
                    <?php endforeach; ?>
                </ul>
            </div>
        <?php endif; ?>

        <form method="post" class="formulario" id="form-conversor" autocomplete="off">
            <div class="fila">
                <label for="basePath">Carpeta base</label>
                <input type="text" id="basePath" name="basePath" placeholder="Ej: D:\\xampp\\htdocs\\AoSpain1.0" value="<?= htmlspecialchars($basePathUI, ENT_QUOTES, 'UTF-8'); ?>" data-default-path="<?= htmlspecialchars($defaultBasePath, ENT_QUOTES, 'UTF-8'); ?>" required>
                <button type="button" id="btn-ruta-proyecto" class="btn-secundario" title="Usar carpeta del proyecto">Proyecto</button>
                <button type="button" id="btn-explorar" class="btn-secundario" title="Elegir carpeta navegando">Elegir carpeta…</button>
            </div>

            <div class="fila">
                <label>Dirección de conversión</label>
                <div class="opciones">
                    <label><input type="radio" name="direccion" value="a_utf8" <?= $direccion === 'a_utf8' ? 'checked' : ''; ?>> Windows-1252 → UTF-8 (sin BOM)</label>
                    <label><input type="radio" name="direccion" value="a_win1252" <?= $direccion === 'a_win1252' ? 'checked' : ''; ?>> UTF-8 → Windows-1252</label>
                    <label><input type="radio" name="direccion" value="fix_mojibake" <?= $direccion === 'fix_mojibake' ? 'checked' : ''; ?>> Arreglar mojibake (reparación heurística y guardar en Windows-1252)</label>
                    <label><input type="radio" name="direccion" value="restore_bak" <?= $direccion === 'restore_bak' ? 'checked' : ''; ?>> Restaurar desde .bak (revertir cambios)</label>
                </div>
            </div>

            <div class="fila">
                <label for="extensiones">Extensiones a procesar</label>
                <input type="text" id="extensiones" name="extensiones" value="<?= htmlspecialchars($extText, ENT_QUOTES, 'UTF-8'); ?>" placeholder="Ej: bas, cls, frm, vbp">
                <small>Separadas por coma o espacio. No incluyas el punto.</small>
            </div>

            <div class="fila opciones">
                <label><input type="checkbox" name="recursivo" <?= $recursivo ? 'checked' : ''; ?>> Incluir subcarpetas</label>
                <label><input type="checkbox" name="backup" <?= $hacerBackup ? 'checked' : ''; ?>> Crear backup .bak si no existe</label>
            </div>

            <div class="acciones">
                <button type="submit" class="btn-primario">Convertir</button>
            </div>
        </form>

        <?php if ($_SERVER['REQUEST_METHOD'] === 'POST' && empty($errores)): ?>
            <section class="resumen">
                <h2>Resumen</h2>
                <ul>
                    <li><strong>Archivos procesados:</strong> <?= (int)$resumen['procesados']; ?></li>
                    <li><strong>Archivos convertidos:</strong> <?= (int)$resumen['convertidos']; ?></li>
                    <li><strong>Errores:</strong> <?= (int)$resumen['errores']; ?></li>
                </ul>
            </section>

            <section class="resultados">
                <h2>Resultados</h2>
                <ul class="lista-resultados">
                    <?php foreach ($resultados as $r): ?>
                        <li class="item <?= htmlspecialchars($r['estado'], ENT_QUOTES, 'UTF-8'); ?>">
                            <?php if ($r['archivo'] !== ''): ?>
                                <span class="ruta"><?= htmlspecialchars($r['archivo'], ENT_QUOTES, 'UTF-8'); ?></span>
                            <?php endif; ?>
                            <span class="mensaje"><?= htmlspecialchars($r['mensaje'], ENT_QUOTES, 'UTF-8'); ?></span>
                        </li>
                    <?php endforeach; ?>
                </ul>
            </section>
        <?php endif; ?>
    </main>

    <!-- Modal explorador de carpetas -->
    <div id="modal-picker" class="modal oculto" aria-hidden="true" role="dialog" aria-label="Explorador de carpetas">
        <div class="modal-contenido">
            <div class="modal-cabecera">
                <h3>Elegir carpeta</h3>
                <button type="button" class="btn-cerrar" id="picker-cerrar" aria-label="Cerrar">✕</button>
            </div>
            <div class="modal-cuerpo">
                <div class="picker-ruta">
                    <input type="text" id="picker-ruta-actual" readonly>
                    <div class="acciones">
                        <button type="button" id="picker-subir" class="btn-secundario">Subir un nivel</button>
                        <button type="button" id="picker-seleccionar" class="btn-primario">Usar esta carpeta</button>
                    </div>
                </div>
                <ul id="picker-lista" class="lista"></ul>
                <div id="picker-mensaje" class="picker-mensaje"></div>
            </div>
        </div>
    </div>

    <script src="js/convert-encoding.js?v=1"></script>
</body>
</html>
