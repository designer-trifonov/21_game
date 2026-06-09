<?php
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, OPTIONS');
header('Content-Type: application/json; charset=utf-8');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    exit(0);
}

// Папка с девушками относительно этого файла
define('GIRLS_DIR', __DIR__ . '/data/Girls/');

// Базовый URL — определяется автоматически
$protocol = (!empty($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off') ? 'https' : 'http';
define('GIRLS_URL', $protocol . '://' . $_SERVER['HTTP_HOST'] . '/data/Girls/');

$VIDEO_EXT = ['mp4', 'webm', 'mov'];
$IMAGE_EXT = ['jpg', 'jpeg', 'png', 'webp'];
$AUDIO_EXT = ['mp3', 'ogg', 'wav'];

// Ищет файл с именем $name и одним из расширений, возвращает имя файла или null
function findFile(string $dir, string $name, array $extensions): ?string
{
    foreach ($extensions as $ext) {
        if (file_exists($dir . $name . '.' . $ext)) {
            return $name . '.' . $ext;
        }
    }
    return null;
}

function url(string $girlId, string $filename): string
{
    return GIRLS_URL . $girlId . '/' . $filename;
}

if (!is_dir(GIRLS_DIR)) {
    http_response_code(500);
    echo json_encode(['error' => 'Папка Data/Girls не найдена на сервере', 'girls' => []],
        JSON_UNESCAPED_UNICODE);
    exit;
}

$girls = [];

foreach (scandir(GIRLS_DIR) as $entry) {
    if (!is_numeric($entry)) continue;

    $dir = GIRLS_DIR . $entry . '/';
    if (!is_dir($dir)) continue;

    $id = (int)$entry;
    $girl = ['id' => $id];

    // ---------- ИНТРО ----------
    $introVideo = findFile($dir, 'intro', $VIDEO_EXT);
    $introImage = findFile($dir, 'intro', $IMAGE_EXT);
    $introAudio = findFile($dir, 'intro', $AUDIO_EXT);
    $introText  = file_exists($dir . 'intro.txt')
        ? trim(file_get_contents($dir . 'intro.txt'))
        : null;

    if ($introVideo) {
        // Видео — высший приоритет, картинка и текст игнорируются
        $girl['intro'] = [
            'type' => 'video',
            'url'  => url($entry, $introVideo),
        ];
    } elseif ($introImage && $introText !== null) {
        // Картинка + текст
        $girl['intro'] = [
            'type'      => 'image_text',
            'image_url' => url($entry, $introImage),
            'text'      => $introText,
        ];
    } elseif ($introImage) {
        // Только картинка
        $girl['intro'] = [
            'type'      => 'image_text',
            'image_url' => url($entry, $introImage),
            'text'      => '',
        ];
    } else {
        $girl['intro'] = ['type' => 'none'];
    }

    // Аудио добавляем всегда если есть, независимо от типа интро
    if ($introAudio) {
        $girl['intro']['audio_url'] = url($entry, $introAudio);
    }

    // ---------- ФОТОГРАФИИ (1–7, все обязательны) ----------
    $photos = [];
    $allPhotosFound = true;
    for ($i = 1; $i <= 7; $i++) {
        $photo = findFile($dir, (string)$i, $IMAGE_EXT);
        if ($photo) {
            $photos[] = url($entry, $photo);
        } else {
            $allPhotosFound = false;
            break;
        }
    }
    if (!$allPhotosFound) continue; // нет всех 7 фото — пропускаем девушку
    $girl['photos'] = $photos;

    // ---------- ПОБЕДА (winn) ----------
    $winVideo = findFile($dir, 'winn', $VIDEO_EXT);
    $winImage = findFile($dir, 'winn', $IMAGE_EXT);
    if ($winVideo) {
        $girl['win'] = ['type' => 'video', 'url' => url($entry, $winVideo)];
    } elseif ($winImage) {
        $girl['win'] = ['type' => 'image', 'url' => url($entry, $winImage)];
    } else {
        $girl['win'] = ['type' => 'none'];
    }

    // ---------- ПОРАЖЕНИЕ ----------
    $loseVideo = findFile($dir, 'lose', $VIDEO_EXT);
    $loseImage = findFile($dir, 'lose', $IMAGE_EXT);
    if ($loseVideo) {
        $girl['lose'] = ['type' => 'video', 'url' => url($entry, $loseVideo)];
    } elseif ($loseImage) {
        $girl['lose'] = ['type' => 'image', 'url' => url($entry, $loseImage)];
    } else {
        $girl['lose'] = ['type' => 'none'];
    }

    $girls[$id] = $girl;
}

ksort($girls);

echo json_encode(
    ['girls' => array_values($girls)],
    JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT
);
