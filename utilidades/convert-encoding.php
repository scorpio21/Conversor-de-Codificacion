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
    // Lee bytes como Windows-1252 y escribe como UTF-8 sin BOM
    try {
        $bytes = file_get_contents($rutaArchivo);
        if ($bytes === false) {
            throw new RuntimeException('No se pudo leer el archivo');
        }
        // Convertir de CP1252 a UTF-8
        $contenido = iconv('Windows-1252', 'UTF-8//TRANSLIT', $bytes);
        if ($contenido === false) {
            // Fallback con mb
            $contenido = mb_convert_encoding((string)$bytes, 'UTF-8', 'Windows-1252');
        }
        $ok = file_put_contents($rutaArchivo, $contenido);
        if ($ok === false) {
            throw new RuntimeException('No se pudo escribir el archivo');
        }
        return ['ok' => true, 'mensaje' => 'Convertido a UTF-8'];
    } catch (Throwable $e) {
        return ['ok' => false, 'mensaje' => $e->getMessage()];
    }
}

function convertirAWin1252(string $rutaArchivo): array {
    // Lee como UTF-8 y escribe como Windows-1252 (posible transliteración)
    try {
        $contenido = file_get_contents($rutaArchivo);
        if ($contenido === false) {
            throw new RuntimeException('No se pudo leer el archivo');
        }
        // Convertir de UTF-8 a CP1252 con transliteración cuando sea posible
        $bytes = iconv('UTF-8', 'Windows-1252//TRANSLIT//IGNORE', $contenido);
        if ($bytes === false) {
            // Fallback con mb (puede reemplazar caracteres no representables)
            $bytes = mb_convert_encoding($contenido, 'Windows-1252', 'UTF-8');
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
$direccion = 'a_utf8'; // a_utf8 | a_win1252
$recursivo = true;
$hacerBackup = true;

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $basePath = isset($_POST['basePath']) ? limpiarRuta((string)$_POST['basePath']) : $defaultBasePath;
    $direccion = isset($_POST['direccion']) && in_array($_POST['direccion'], ['a_utf8', 'a_win1252'], true)
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
                $r = $direccion === 'a_utf8' ? convertirAUtf8($archivo) : convertirAWin1252($archivo);
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
    <header class="cabecera" style="text-align: center;">
        <h1>Conversor de Codificación</h1>
        <p>Convierte archivos .bas, .cls, .frm, .vbp entre UTF-8 (sin BOM) y Windows-1252, recorriendo subcarpetas.</p>
        <div class="creditos cabecera-creditos" role="note" style="text-align: center;">
            Créditos: <a href="https://github.com/jonathanhecl/vb6-ia-tools" target="_blank" rel="noopener noreferrer">jonathanhecl/vb6-ia-tools</a><br>
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
                <input type="text" id="basePath" name="basePath" placeholder="Ej: D:\\xampp\\htdocs\\AoSpain1.0" value="<?= htmlspecialchars($basePath, ENT_QUOTES, 'UTF-8'); ?>" required>
                <button type="button" id="btn-ruta-proyecto" class="btn-secundario" title="Usar carpeta del proyecto">Proyecto</button>
                <button type="button" id="btn-explorar" class="btn-secundario" title="Elegir carpeta navegando">Elegir carpeta…</button>
            </div>

            <div class="fila">
                <label>Dirección de conversión</label>
                <div class="opciones">
                    <label><input type="radio" name="direccion" value="a_utf8" <?= $direccion === 'a_utf8' ? 'checked' : ''; ?>> Windows-1252 → UTF-8 (sin BOM)</label>
                    <label><input type="radio" name="direccion" value="a_win1252" <?= $direccion === 'a_win1252' ? 'checked' : ''; ?>> UTF-8 → Windows-1252</label>
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
