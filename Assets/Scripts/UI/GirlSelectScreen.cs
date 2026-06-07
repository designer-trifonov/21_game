using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GirlSelectScreen : MonoBehaviour
{
    public GameObject rootPanel;
    public Transform  gridContent;
    public GameObject cardSlotPrefab;

    public void Init() { }

    public void Show()
    {
        Debug.Log("[GirlSelectScreen] Show");
        if (ContentService.Instance == null)
        {
            Debug.LogError("[GirlSelectScreen] ContentService.Instance == null!");
            return;
        }
        StartCoroutine(ContentService.Instance.WaitAndCall(BuildGrid));
    }

    void BuildGrid()
    {
        Debug.Log($"[GirlSelectScreen] BuildGrid — девушек: {ContentService.Instance.Girls?.Count}");

        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        var girls = ContentService.Instance.Girls;
        if (girls == null || girls.Count == 0) return;

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
                    if (slot.photo == null) return;
                    slot.photo.sprite  = sprite;
                    slot.photo.enabled = sprite != null;
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
}
