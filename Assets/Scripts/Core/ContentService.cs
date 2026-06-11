using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Игнорируем проверку SSL — нужно для бесплатных хостингов с общим сертификатом
public class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}

public class ContentService : MonoBehaviour
{
    public static ContentService Instance { get; private set; }

    const string API_URL = "http://cards21.atwebpages.com/api.php";

    public List<RemoteGirlData> Girls    { get; private set; } = new();
    public bool                 IsLoaded { get; private set; }
    public string               Error    { get; private set; }

    readonly Dictionary<string, Sprite> _cache = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => StartCoroutine(LoadAll());

    IEnumerator LoadAll()
    {
        const int MAX_RETRIES = 5;
        UnityWebRequest req = null;

        for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            Debug.Log($"[ContentService] Запрос манифеста (попытка {attempt}/{MAX_RETRIES}): {API_URL}");
            req = UnityWebRequest.Get(API_URL);
            req.certificateHandler = new AcceptAllCertificates();
            req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success) break;

            Debug.LogWarning($"[ContentService] Попытка {attempt} неудачна: {req.error}. Повтор через 2 сек...");
            req.Dispose();
            req = null;
            yield return new WaitForSeconds(2f);
        }

        if (req == null || req.result != UnityWebRequest.Result.Success)
        {
            Error    = req?.error ?? "Нет ответа";
            IsLoaded = true;
            Debug.LogError($"[ContentService] ОШИБКА манифеста после {MAX_RETRIES} попыток: {Error}");
            req?.Dispose();
            yield break;
        }

        Debug.Log($"[ContentService] Манифест получен:\n{req.downloadHandler.text}");

        var response = JsonUtility.FromJson<RemoteGirlsResponse>(req.downloadHandler.text);
        Girls = response?.girls ?? new List<RemoteGirlData>();
        Debug.Log($"[ContentService] Девушек в манифесте: {Girls.Count}");

        var urls = new List<string>();
        foreach (var g in Girls)
        {
            Debug.Log($"[ContentService] Девушка id={g.id}: photos={g.photos?.Count}");

            if (!string.IsNullOrEmpty(g.intro?.image_url)) urls.Add(g.intro.image_url);
            if (g.photos != null) foreach (var p in g.photos) if (!string.IsNullOrEmpty(p)) urls.Add(p);
            if (g.win?.type  == "image" && !string.IsNullOrEmpty(g.win.url))  urls.Add(g.win.url);
            if (g.lose?.type == "image" && !string.IsNullOrEmpty(g.lose.url)) urls.Add(g.lose.url);
        }

        Debug.Log($"[ContentService] Всего изображений: {urls.Count}");
        foreach (var url in urls)
            yield return EnsureSprite(url);

        IsLoaded = true;
        Debug.Log($"[ContentService] Готово. В кэше: {_cache.Count}");
    }

    // Грузит спрайт: сначала с диска, если нет — скачивает и сохраняет
    IEnumerator EnsureSprite(string url)
    {
        if (_cache.ContainsKey(url)) yield break;

        var localPath = LocalPath(url);

        if (File.Exists(localPath))
        {
            Debug.Log($"[ContentService] С диска: {localPath}");
            yield return LoadFromDisk(localPath, url);
        }
        else
        {
            Debug.Log($"[ContentService] Скачиваю: {url}");
            yield return DownloadAndSave(url, localPath);
        }
    }

    IEnumerator LoadFromDisk(string localPath, string cacheKey)
    {
        using var req = UnityWebRequestTexture.GetTexture("file:///" + localPath.Replace('\\', '/'));
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var tex = DownloadHandlerTexture.GetContent(req);
            _cache[cacheKey] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            Debug.Log($"[ContentService] ✓ Загружено с диска: {localPath}");
        }
        else
        {
            Debug.LogError($"[ContentService] ✗ Ошибка чтения с диска: {localPath} — {req.error}");
        }
    }

    IEnumerator DownloadAndSave(string url, string localPath)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        req.certificateHandler = new AcceptAllCertificates();
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var tex = DownloadHandlerTexture.GetContent(req);
            _cache[url] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            Debug.Log($"[ContentService] ✓ Скачано: {url} ({tex.width}x{tex.height})");

            // Сохраняем на диск
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                var ext = Path.GetExtension(localPath).ToLower();
                byte[] bytes = ext == ".png" ? tex.EncodeToPNG() : tex.EncodeToJPG(90);
                File.WriteAllBytes(localPath, bytes);
                Debug.Log($"[ContentService] Сохранено на диск: {localPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ContentService] Не удалось сохранить на диск: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"[ContentService] ✗ Ошибка скачивания: {url} — {req.error}");
        }
    }

    // URL → локальный путь: http://host/Data/Girls/1/2.jpg → persistentDataPath/Data/Girls/1/2.jpg
    static string LocalPath(string url)
    {
        var uri  = new Uri(url);
        var rel  = uri.AbsolutePath.TrimStart('/'); // Data/Girls/1/2.jpg
        return Path.Combine(Application.persistentDataPath, rel);
    }

    public void GetSprite(string url, Action<Sprite> onDone)
    {
        if (string.IsNullOrEmpty(url)) { onDone?.Invoke(null); return; }

        if (_cache.TryGetValue(url, out var cached))
        {
            onDone?.Invoke(cached);
            return;
        }

        StartCoroutine(FetchAndReturn(url, onDone));

    }

    IEnumerator FetchAndReturn(string url, Action<Sprite> onDone)
    {
        yield return EnsureSprite(url);
        _cache.TryGetValue(url, out var sprite);
        onDone?.Invoke(sprite);
    }

    public IEnumerator WaitAndCall(Action onReady)
    {
        while (!IsLoaded) yield return null;
        onReady?.Invoke();
    }
}
