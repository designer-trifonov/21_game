using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    static ImageLoader _instance;

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
    /// Загружает спрайт по пути (локальный) или URL (http/https) и отдаёт в onLoaded.
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

        bool isRemote = path.StartsWith("http://") || path.StartsWith("https://");

        if (!isRemote && !File.Exists(path))
        {
            Debug.LogError($"[ImageLoader] Файл не найден на диске: {path}");
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
            Debug.Log($"[ImageLoader] Успешно загружено: {url} ({tex.width}x{tex.height})");
            onLoaded?.Invoke(sprite);
        }
        else
        {
            Debug.LogError($"[ImageLoader] Ошибка загрузки: {url} | {request.responseCode} | {request.error}");
            onLoaded?.Invoke(null);
        }
    }
}
