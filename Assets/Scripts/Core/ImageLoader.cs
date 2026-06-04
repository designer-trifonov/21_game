using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Загружает изображение с диска и отдаёт в колбэк.
/// Только загрузка — больше ничего.
/// </summary>
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
            return _instance;
        }
    }

    /// <summary>
    /// Загружает спрайт по абсолютному пути и отдаёт в onLoaded.
    /// </summary>
    public static void Load(string absolutePath, Action<Sprite> onLoaded)
    {
        if (string.IsNullOrEmpty(absolutePath) || !File.Exists(absolutePath))
        {
            onLoaded?.Invoke(null);
            return;
        }

        Instance.StartCoroutine(Instance.LoadRoutine(absolutePath, onLoaded));
    }

    IEnumerator LoadRoutine(string path, Action<Sprite> onLoaded)
    {
        string url     = "file:///" + path.Replace("\\", "/");
        var    request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var tex    = DownloadHandlerTexture.GetContent(request);
            var sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
            onLoaded?.Invoke(sprite);
        }
        else
        {
            onLoaded?.Invoke(null);
        }
    }
}
