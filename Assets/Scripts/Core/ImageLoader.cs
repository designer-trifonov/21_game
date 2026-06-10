using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Атомарный загрузчик локальных изображений.
/// Полностью независим от ContentService и сетевого стека.
/// Имеет кеш — повторный запрос одного файла возвращает кешированный спрайт.
/// </summary>
public class ImageLoader : MonoBehaviour
{
    static ImageLoader _instance;

    readonly Dictionary<string, Sprite> _cache = new();

    public static ImageLoader Instance
    {
        get
        {
            if (_instance != null) return _instance;
            var go = new GameObject("ImageLoader");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<ImageLoader>();
            Debug.Log("[ImageLoader] Создан экземпляр");
            return _instance;
        }
    }

    /// <summary>
    /// Загружает спрайт по локальному пути или URL.
    /// Повторный вызов с тем же путём вернёт кешированный спрайт мгновенно.
    /// Никогда не бросает исключений — при ошибке вызывает onLoaded(null).
    /// </summary>
    public static void Load(string path, Action<Sprite> onLoaded)
    {
        Debug.Log($"[ImageLoader] Load: {path}");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("[ImageLoader] path пустой!");
            onLoaded?.Invoke(null);
            return;
        }

        // Кеш
        if (Instance._cache.TryGetValue(path, out var cached))
        {
            Debug.Log($"[ImageLoader] Из кеша: {path}");
            onLoaded?.Invoke(cached);
            return;
        }

        bool isRemote = path.StartsWith("http://") || path.StartsWith("https://");

        if (!isRemote && !File.Exists(path))
        {
            Debug.LogError($"[ImageLoader] Файл не найден: {path}");
            onLoaded?.Invoke(null);
            return;
        }

        Instance.StartCoroutine(Instance.LoadRoutine(path, isRemote, onLoaded));
    }

    IEnumerator LoadRoutine(string path, bool isRemote, Action<Sprite> onLoaded)
    {
        string url = isRemote ? path : "file:///" + path.Replace("\\", "/");
        Debug.Log($"[ImageLoader] Загружаем: {url}");

        var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var tex    = DownloadHandlerTexture.GetContent(request);
            var sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));

            _cache[path] = sprite; // кешируем
            Debug.Log($"[ImageLoader] Успешно: {url} ({tex.width}x{tex.height})");
            onLoaded?.Invoke(sprite);
        }
        else
        {
            Debug.LogError($"[ImageLoader] Ошибка: {url} | {request.responseCode} | {request.error}");
            onLoaded?.Invoke(null);
        }
    }
}
