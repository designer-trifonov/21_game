using System.IO;
using UnityEngine;

/// <summary>
/// Все пути к медиа-файлам — строго из папки Data.
/// Структура:
///   Data/
///     Images/
///     Video/
///     Audio/
///     Girls/
///     deck_config.json
/// </summary>
public static class DataPaths
{
    // ── Корень ────────────────────────────────────────────────
    public static string Root  => Path.Combine(Application.persistentDataPath, "Data");

    // ── Медиа ─────────────────────────────────────────────────
    public static string Image => Path.Combine(Application.dataPath, "Data", "Images");
    public static string Video => Path.Combine(Root, "Video");
    public static string Audio => Path.Combine(Root, "Audio");

    // ── Данные ────────────────────────────────────────────────
    public static string Girls => Path.Combine(Root, "Girls");

    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Создаёт все папки при первом запуске если их нет.
    /// </summary>
    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Image);
        Directory.CreateDirectory(Video);
        Directory.CreateDirectory(Audio);
        Directory.CreateDirectory(Girls);

        Debug.Log($"[DataPaths] Root: {Root}");
        Debug.Log($"[DataPaths] Image: {Image}");
        Debug.Log($"[DataPaths] Video: {Video}");
        Debug.Log($"[DataPaths] Audio: {Audio}");
    }

    // ── Хелперы поиска файла ──────────────────────────────────

    /// <summary>
    /// Ищет файл по имени (без расширения) в папке Image.
    /// Проверяет .png, .jpg, .jpeg
    /// </summary>
    public static string FindImage(string nameWithoutExt)
    {
        var result = FindFile(Image, nameWithoutExt, ".png", ".jpg", ".jpeg");
        if (result == null)
            Debug.LogError($"[DataPaths] FindImage: файл '{nameWithoutExt}' не найден в {Image}");
        else
            Debug.Log($"[DataPaths] FindImage: {result}");
        return result;
    }

    /// <summary>
    /// Ищет файл по имени в папке Video.
    /// </summary>
    public static string FindVideo(string nameWithoutExt)
    {
        var result = FindFile(Video, nameWithoutExt, ".mp4", ".avi", ".mov", ".webm");
        if (result == null)
            Debug.LogWarning($"[DataPaths] FindVideo: файл '{nameWithoutExt}' не найден в {Video}");
        else
            Debug.Log($"[DataPaths] FindVideo: {result}");
        return result;
    }

    /// <summary>
    /// Ищет файл по имени в папке Audio.
    /// </summary>
    public static string FindAudio(string nameWithoutExt)
    {
        var result = FindFile(Audio, nameWithoutExt, ".mp3", ".wav", ".ogg");
        if (result == null)
            Debug.LogWarning($"[DataPaths] FindAudio: файл '{nameWithoutExt}' не найден в {Audio}");
        else
            Debug.Log($"[DataPaths] FindAudio: {result}");
        return result;
    }

    static string FindFile(string folder, string name, params string[] extensions)
    {
        // Точное совпадение
        foreach (var ext in extensions)
        {
            string path = Path.Combine(folder, name + ext);
            if (File.Exists(path)) return path;
        }
        // Поиск по префиксу: card_1-1 найдёт card_1-1_144x208.png
        if (Directory.Exists(folder))
        {
            foreach (var ext in extensions)
            {
                var files = Directory.GetFiles(folder, name + "*" + ext);
                if (files.Length > 0) return files[0];
            }
        }
        return null;
    }
}
