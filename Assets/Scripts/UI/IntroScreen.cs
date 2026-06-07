using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
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

        Debug.Log($"[IntroScreen] intro.type={girl.intro.type} url={girl.intro.url} image_url={girl.intro.image_url} text={girl.intro.text}");

        if (girl.intro.type == "video" && !string.IsNullOrEmpty(girl.intro.url))
            ShowVideo(girl.intro.url);
        else
            ShowPhoto(girl.intro.image_url, girl.intro.text);
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

    void ShowPhoto(string imageUrl, string text)
    {
        Debug.Log($"[IntroScreen] ShowPhoto — imageUrl={imageUrl} text={text}");
        if (photoContainer == null) { Debug.LogError("[IntroScreen] photoContainer не назначен!"); return; }

        videoContainer?.SetActive(false);
        photoContainer.SetActive(true);

        if (txtDescription != null)
        {
            txtDescription.text = text ?? string.Empty;
            Debug.Log($"[IntroScreen] Текст установлен: '{txtDescription.text}'");
        }
        else
            Debug.LogError("[IntroScreen] txtDescription не назначен!");

        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("[IntroScreen] imageUrl пустой — фото не будет!");
            return;
        }

        if (photoImage == null)
        {
            Debug.LogError("[IntroScreen] photoImage не назначен!");
            return;
        }

        ContentService.Instance.GetSprite(imageUrl, sprite =>
        {
            Debug.Log($"[IntroScreen] Спрайт получен: {sprite != null} — устанавливаем на photoImage");
            if (photoImage == null) { Debug.LogError("[IntroScreen] photoImage уничтожен!"); return; }
            photoImage.sprite  = sprite;
            photoImage.enabled = sprite != null;
            if (sprite == null) Debug.LogError("[IntroScreen] Спрайт NULL — фото не отобразится!");
        });
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
        Debug.Log("[IntroScreen] Кнопка Старт нажата → ShowDifficulty");
        StopVideo();
        UIManager.Instance.ShowDifficulty();
    }
}
