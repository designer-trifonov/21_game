using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using TMPro;

public class IntroScreen : MonoBehaviour
{
    public GameObject       rootPanel;
    public Image            photoImage;
    public TextMeshProUGUI  txtDescription;
    public GameObject       photoContainer;
    public VideoPlayer      videoPlayer;
    public RawImage         videoDisplay;
    public GameObject       videoContainer;
    public Button           btnStart;
    public AudioSource      audioSource;   // назначь в инспекторе

    RenderTexture _rt;

    public void Init()
    {
        if (btnStart == null) { Debug.LogError("[IntroScreen] btnStart не назначен!"); return; }
        btnStart.onClick.AddListener(OnStart);
        Debug.Log("[IntroScreen] Init выполнен");
    }

    public void Show()
    {
        var girl = GameState.CurrentRemoteGirl;
        Debug.Log($"[IntroScreen] Show — girl={(girl == null ? "NULL" : $"id={girl.id}")}");

        if (girl == null || girl.intro == null)
        {
            Debug.LogError("[IntroScreen] CurrentRemoteGirl или intro == null!");
            return;
        }

        Debug.Log($"[IntroScreen] type={girl.intro.type} image_url={girl.intro.image_url} text={girl.intro.text} audio_url={girl.intro.audio_url}");

        if (girl.intro.type == "video" && !string.IsNullOrEmpty(girl.intro.url))
            ShowVideo(girl.intro.url);
        else
            ShowPhoto(girl.intro.image_url, girl.intro.text, girl.intro.audio_url);
    }

    void ShowVideo(string url)
    {
        Debug.Log($"[IntroScreen] ShowVideo: {url}");
        if (videoContainer == null) { Debug.LogError("[IntroScreen] videoContainer не назначен!"); return; }
        if (videoPlayer    == null) { Debug.LogError("[IntroScreen] videoPlayer не назначен!");    return; }
        if (videoDisplay   == null) { Debug.LogError("[IntroScreen] videoDisplay не назначен!");   return; }

        videoContainer.SetActive(true);
        photoContainer?.SetActive(false);

        _rt = new RenderTexture(1080, 1920, 0);
        videoDisplay.texture      = _rt;
        videoPlayer.targetTexture = _rt;
        videoPlayer.url           = url;
        videoPlayer.isLooping     = false;
        videoPlayer.Play();
        Debug.Log("[IntroScreen] Видео запущено");
    }

    void ShowPhoto(string imageUrl, string text, string audioUrl)
    {
        Debug.Log($"[IntroScreen] ShowPhoto — imageUrl={imageUrl} text={text} audioUrl={audioUrl}");
        if (photoContainer == null) { Debug.LogError("[IntroScreen] photoContainer не назначен!"); return; }

        videoContainer?.SetActive(false);
        photoContainer.SetActive(true);

        // Текст — показываем если есть
        if (txtDescription != null)
        {
            txtDescription.text = text ?? string.Empty;
            txtDescription.gameObject.SetActive(!string.IsNullOrEmpty(text));
            Debug.Log($"[IntroScreen] Текст: '{txtDescription.text}'");
        }

        // Фото
        if (!string.IsNullOrEmpty(imageUrl) && photoImage != null)
        {
            ContentService.Instance.GetSprite(imageUrl, sprite =>
            {
                if (photoImage == null) return;
                photoImage.sprite  = sprite;
                photoImage.enabled = sprite != null;
                Debug.Log($"[IntroScreen] Фото загружено: {sprite != null}");
            });
        }

        // Аудио — автозапуск
        if (!string.IsNullOrEmpty(audioUrl))
            StartCoroutine(PlayAudio(audioUrl));
    }

    IEnumerator PlayAudio(string url)
    {
        Debug.Log($"[IntroScreen] Загружаю аудио: {url}");
        using var req = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
        ((DownloadHandlerAudioClip)req.downloadHandler).streamAudio = true;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[IntroScreen] Ошибка загрузки аудио: {req.error}");
            yield break;
        }

        var clip = DownloadHandlerAudioClip.GetContent(req);
        if (audioSource == null) { Debug.LogError("[IntroScreen] audioSource не назначен!"); yield break; }

        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log("[IntroScreen] Аудио запущено");
    }

    void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.targetTexture = null;
        }
        if (_rt != null)
        {
            _rt.Release();
            Object.Destroy(_rt);
            _rt = null;
        }
    }

    void OnStart()
    {
        Debug.Log("[IntroScreen] Кнопка Старт нажата → ShowGame");
        StopVideo();
        StopAudio();
        UIManager.Instance.ShowGame();
    }
}
