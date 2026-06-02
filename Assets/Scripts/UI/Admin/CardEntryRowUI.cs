using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Одна строка карты в сетке конфига колоды.
/// </summary>
public class CardEntryRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;       // "A♣", "K♦" и т.д.
    public Image    cardPreview;    // превью спрайта
    public TMP_Text spriteNameText; // текущее имя файла
    public Button   btnPickSprite;  // выбрать спрайт

    CardEntry _entry;
    DeckConfig _config;

    public void Setup(CardEntry entry, DeckConfig config)
    {
        _entry  = entry;
        _config = config;

        nameText.text       = entry.DisplayName;
        spriteNameText.text = string.IsNullOrEmpty(entry.spriteName) ? "—" : entry.spriteName;

        // Загрузить превью
        RefreshPreview();

        btnPickSprite.onClick.RemoveAllListeners();
        btnPickSprite.onClick.AddListener(OnPickSprite);
    }

    void RefreshPreview()
    {
        var sprite = DeckRepository.LoadSprite(_config, _entry.spriteName);
        cardPreview.sprite  = sprite;
        cardPreview.enabled = sprite != null;
    }

    void OnPickSprite()
    {
        // TODO: NativeFilePicker — пока лог
        Debug.Log($"TODO: выбрать спрайт для {_entry.DisplayName}");
    }
}
