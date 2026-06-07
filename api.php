<?php
header('Content-Type: application/json');

$baseDir = __DIR__ . '/Data/Girls/';
$baseUrl = 'http://' . $_SERVER['HTTP_HOST'] . '/Data/Girls/';
$imgExts = ['jpg', 'jpeg', 'png', 'webp'];

$girls = [];

foreach (glob($baseDir . '*', GLOB_ONLYDIR) as $girlDir) {
    $id = (int) basename($girlDir);
    if ($id <= 0) continue;

    $girlUrl = $baseUrl . $id . '/';

    // intro
    $intro = ['type' => 'none'];
    foreach ($imgExts as $ext) {
        if (file_exists($girlDir . '/intro.' . $ext)) {
            $text  = file_exists($girlDir . '/intro.txt') ? trim(file_get_contents($girlDir . '/intro.txt')) : '';
            $intro = ['type' => 'image_text', 'image_url' => $girlUrl . 'intro.' . $ext, 'text' => $text];
            break;
        }
    }

    // photos — 1.jpg, 2.jpg, 3.jpg ...
    $photos = [];
    $i = 1;
    while (true) {
        $found = false;
        foreach ($imgExts as $ext) {
            if (file_exists($girlDir . '/' . $i . '.' . $ext)) {
                $photos[] = $girlUrl . $i . '.' . $ext;
                $found = true;
                break;
            }
        }
        if (!$found) break;
        $i++;
    }

    // win / lose
    $win  = ['type' => 'none'];
    $lose = ['type' => 'none'];
    foreach ($imgExts as $ext) {
        if (file_exists($girlDir . '/winn.' . $ext)) {
            $win = ['type' => 'image', 'url' => $girlUrl . 'winn.' . $ext];
            break;
        }
    }
    foreach ($imgExts as $ext) {
        if (file_exists($girlDir . '/lose.' . $ext)) {
            $lose = ['type' => 'image', 'url' => $girlUrl . 'lose.' . $ext];
            break;
        }
    }

    $girls[] = compact('id', 'intro', 'photos', 'win', 'lose');
}

echo json_encode(['girls' => $girls], JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_PRETTY_PRINT);
