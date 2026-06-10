using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class GameEndScreen : MonoBehaviour
{
    public GameObject      rootPanel;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtSummary;
    public Button          btnPlayAgain;
    public Button          btnMainMenu;

    [Header("Медиа победы/поражения")]
    public Image       mediaImage;
    public RawImage    mediaVideoDisplay;
    public VideoPlayer mediaVideoPlayer;
    public GameObject  mediaImageContainer;
    public GameObject  mediaVideoContainer;

    RenderTexture _rt;

    public void Init()
    {
        Debug.Log("[GameEndScreen] Init");
        if (btnPlayAgain == null) { Debug.LogError("[GameEndScreen] btnPlayAgain не назначен!"); return; }
        if (btnMainMenu  == null) { Debug.LogError("[GameEndScreen] btnMainMenu не назначен!");  return; }

        btnPlayAgain.onClick.AddListener(OnPlayAgain);
        btnMainMenu .onClick.AddListener(() => { Debug.Log("[GameEndScreen] Главное меню"); StopMedia(); UIManager.Instance.ShowWelcome(); });
    }

    public void Show()
    {
        Debug.Log($"[GameEndScreen] Show — IsPlayerWon={GameState.IsPlayerWon} PlayerWins={GameState.PlayerWins} MaxRounds={GameState.MaxRounds}");

        if (txtTitle   == null) Debug.LogError("[GameEndScreen] txtTitle не назначен!");
        if (txtSummary == null) Debug.LogError("[GameEndScreen] txtSummary не назначен!");

        if (GameState.IsPlayerWon)
        {
            if (txtTitle   != null) txtTitle  .text = "Поздравляем!";
            if (txtSummary != null) txtSummary.text = $"Ты победил!\nПобед: {GameState.PlayerWins}";
            Debug.Log($"[GameEndScreen] Победа — текст установлен");
        }
        else
        {
            if (txtTitle   != null) txtTitle  .text = "Игра окончена";
            if (txtSummary != null) txtSummary.text = $"Раунды закончились.\nПобед: {GameState.PlayerWins} из {GameState.MaxRounds}";
            Debug.Log($"[GameEndScreen] Проигрыш — текст установлен");
        }

        var girl  = GameState.CurrentRemoteGirl;
        var media = girl == null ? null : (GameState.IsPlayerWon ? girl.win : girl.lose);
        ShowMedia(media);
    }

    void ShowMedia(RemoteMedia media)
    {
        StopMedia();
        mediaImageContainer?.SetActive(false);
        mediaVideoContainer?.SetActive(false);

        if (media == null || media.type == "none" || string.IsNullOrEmpty(media.url))
        {
            Debug.Log("[GameEndScreen] Медиа отсутствует");
            return;
        }

        if (media.type == "video")
        {
            if (mediaVideoPlayer == null || mediaVideoDisplay == null) { Debug.LogWarning("[GameEndScreen] videoPlayer/videoDisplay не назначены"); return; }
            mediaVideoContainer?.SetActive(true);
            _rt = new RenderTexture(1080, 1920, 0);
            mediaVideoDisplay.texture      = _rt;
            mediaVideoPlayer.targetTexture = _rt;
            mediaVideoPlayer.url           = media.url;
            mediaVideoPlayer.isLooping     = false;
            mediaVideoPlayer.Play();
            Debug.Log($"[GameEndScreen] Видео запущено: {media.url}");
        }
        else if (media.type == "image")
        {
            if (mediaImage == null) { Debug.LogWarning("[GameEndScreen] mediaImage не назначен"); return; }
            mediaImageContainer?.SetActive(true);
            ContentService.Instance.GetSprite(media.url, sprite =>
            {
                if (mediaImage == null) return;
                mediaImage.sprite  = sprite;
                mediaImage.enabled = sprite != null;
                Debug.Log($"[GameEndScreen] Картинка загружена: {sprite != null}");
            });
        }
    }

    void OnPlayAgain()
    {
        Debug.Log("[GameEndScreen] Играть снова");
        StopMedia();
        if (GameState.CurrentRemoteGirl != null)
        {
            Debug.Log($"[GameEndScreen] Рестарт remote-игры id={GameState.CurrentRemoteGirl.id}");
            GameState.StartRemoteGame(GameState.CurrentRemoteGirl);
        }
        else
        {
            Debug.Log($"[GameEndScreen] Рестарт локальной игры girl={GameState.CurrentGirl?.name}");
            GameState.StartGame(GameState.CurrentGirl);
        }
        UIManager.Instance.ShowGame();
    }

    void StopMedia()
    {
        Debug.Log($"[GameEndScreen] StopMedia — videoPlaying={mediaVideoPlayer?.isPlaying}");
        if (mediaVideoPlayer != null)
        {
            mediaVideoPlayer.Stop();
            mediaVideoPlayer.targetTexture = null;
        }
        if (_rt != null)
        {
            _rt.Release();
            Object.Destroy(_rt);
            _rt = null;
        }
    }
}
