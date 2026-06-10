using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Экран выбора девушки.
/// Атомарен: если ContentService не загрузил данные — показывает сообщение об ошибке,
/// не падает и не зависает.
/// </summary>
public class GirlSelectScreen : MonoBehaviour
{
    public GameObject       rootPanel;
    public Transform        gridContent;
    public GameObject       cardSlotPrefab;

    [Header("Состояние загрузки (опционально — назначить в инспекторе)")]
    public TextMeshProUGUI  txtError;     // текст ошибки, если девушки не загружены
    public GameObject       loadingIcon;  // спиннер/иконка загрузки (опционально)

    public void Init() { }

    public void Show()
    {
        Debug.Log("[GirlSelectScreen] Show");

        if (ContentService.Instance == null)
        {
            Debug.LogError("[GirlSelectScreen] ContentService.Instance == null!");
            ShowError("Сервис контента недоступен");
            return;
        }

        // Показываем иконку загрузки пока ждём
        if (loadingIcon != null) loadingIcon.SetActive(true);
        if (txtError    != null) txtError.gameObject.SetActive(false);

        StartCoroutine(ContentService.Instance.WaitAndCall(BuildGrid));
    }

    void BuildGrid()
    {
        if (loadingIcon != null) loadingIcon.SetActive(false);

        Debug.Log($"[GirlSelectScreen] BuildGrid — девушек: {ContentService.Instance.Girls?.Count}");

        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        var girls = ContentService.Instance.Girls;

        // Сервер недоступен или вернул пустой список
        if (girls == null || girls.Count == 0)
        {
            var reason = !string.IsNullOrEmpty(ContentService.Instance.Error)
                ? ContentService.Instance.Error
                : "Список девушек пуст";
            Debug.LogError($"[GirlSelectScreen] Нет данных с сервера: {reason}");
            ShowError($"Сервер недоступен.\nПроверьте интернет.\n\n{reason}");
            return;
        }

        if (txtError != null) txtError.gameObject.SetActive(false);

        foreach (var girl in girls)
        {
            var captured = girl;
            var go       = Instantiate(cardSlotPrefab, gridContent);

            var slot = go.GetComponent<CardSlotView>();
            if (slot == null) { Debug.LogError("[GirlSelectScreen] CardSlotView не найден на префабе!"); continue; }

            if (slot.label != null) slot.label.text = $"Девушка {captured.id}";

            string coverUrl = (captured.photos != null && captured.photos.Count > 0)
                ? captured.photos[0]
                : captured.intro?.image_url;

            if (slot.photo != null && !string.IsNullOrEmpty(coverUrl))
                ContentService.Instance.GetSprite(coverUrl, sprite =>
                {
                    try
                    {
                        if (slot == null || slot.photo == null) return;
                        slot.photo.sprite  = sprite;
                        slot.photo.enabled = sprite != null;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"[GirlSelectScreen] Ошибка установки фото: {e.Message}");
                    }
                });

            if (slot.button != null)
                slot.button.onClick.AddListener(() =>
                {
                    Debug.Log($"[GirlSelectScreen] Выбрана девушка id={captured.id}");
                    GameState.StartRemoteGame(captured);
                    UIManager.Instance.ShowIntro();
                });
            else
                Debug.LogError("[GirlSelectScreen] Button не назначен в CardSlotView!");
        }
    }

    void ShowError(string message)
    {
        if (txtError != null)
        {
            txtError.text = message;
            txtError.gameObject.SetActive(true);
        }
        // Даже без txtError — ошибка уже в логе
    }
}
