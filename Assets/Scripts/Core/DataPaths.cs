using System.IO;
using UnityEngine;

/// <summary>
/// Все пути к медиа-файлам — строго из папки Data.
/// Структура:
///   Data/
///     Image/
///     Video/
///     Audio/
///     Girls/       ← JSON девушек
///     deck_config.json
/// </summary>
public static class DataPaths
{
    // ── Корень ────────────────────────────────────────────────
    public static string Root  => Path.Combine(Application.persistentDataPath, "Data");

    // ── Медиа ─────────────────────────────────────────────────
    public static string Image => Path.Combine(Root, "Image");
    public static string Video => Path.Combine(Root, "Video");
    public static string Audio => Path.Combine(Root, "Audio");

    // ── Данные ────────────────────────────────────────────────
    public static string Girls      => Path.Combine(Root, "Girls");
    public static string DeckConfig => Path.Combine(Root, "deck_config.json");

    // ─────────────────────────────────────────────────────────
    /// <summary>
    /// Создаёт все папки при первом запуске если их нет.
    /// Вызывай один раз при старте приложения.
    /// </summary>
    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Image);
        Directory.CreateDirectory(Video);
        Directory.CreateDirectory(Audio);
        Directory.CreateDirectory(Girls);

        Debug.Log($"[DataPaths] Root: {Root}");
    }

    // ── Хелперы поиска файла ──────────────────────────────────

    /// <summary>
    /// Ищет файл по имени (без расширения) в папке Image.
    /// Проверяет .png, .jpg, .jpeg
    /// </summary>
    public static string FindImage(string nameWithoutExt)
    {
        return FindFile(Image, nameWithoutExt, ".png", ".jpg", ".jpeg");
    }

    /// <summary>
    /// Ищет файл по имени в папке Video.
    /// </summary>
    public static string FindVideo(string nameWithoutExt)
    {
        return FindFile(Video, nameWithoutExt, ".mp4", ".avi", ".mov", ".webm");
    }

    /// <summary>
    /// Ищет файл по имени в папке Audio.
    /// </summary>
    public static string FindAudio(string nameWithoutExt)
    {
        return FindFile(Audio, nameWithoutExt, ".mp3", ".wav", ".ogg");
    }

    static string FindFile(string folder, string name, params string[] extensions)
    {
        foreach (var ext in extensions)
        {
            string path = Path.Combine(folder, name + ext);
            if (File.Exists(path)) return path;
        }
        return null;
    }
}
