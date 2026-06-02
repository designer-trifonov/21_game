using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Панель настройки колоды в админке.
/// Авто-заполнение + ручной выбор рубашки и спрайтов.
/// </summary>
public class AdminDeckController : MonoBehaviour
{
    [Header("Рубашка")]
    public Image      backPreview;
    public TMP_Text   backNameText;
    public Button     btnPickBack;

    [Header("Авто-заполнение")]
    public TMP_InputField inputFolder;   // папка в Resources (по умолч. "Cards")
    public Button         btnAutoFill;   // заполнить по именам файлов

    [Header("Список карт")]
    public Transform  cardsContent;      // Content ScrollView
    public GameObject cardRowPrefab;     // префаб CardEntryRowUI

    [Header("Кнопки")]
    public Button btnSave;
    public Button btnBack;

    DeckConfig _config;

    // ─────────────────────────────────────────────────────────

    void OnEnable()
    {
        _config = DeckRepository.Load();

        inputFolder.text = _config.spritesFolder;

        btnAutoFill.onClick.AddListener(OnAutoFill);
        btnPickBack.onClick.AddListener(OnPickBack);
        btnSave    .onClick.AddListener(OnSave);
        btnBack    .onClick.AddListener(OnBack);

        RefreshBackPreview();
        BuildCardRows();
    }

    void OnDisable()
    {
        btnAutoFill.onClick.RemoveAllListeners();
        btnPickBack.onClick.RemoveAllListeners();
        btnSave    .onClick.RemoveAllListeners();
        btnBack    .onClick.RemoveAllListeners();
    }

    // ── Авто-заполнить список карт ────────────────────────────

    void OnAutoFill()
    {
        _config.spritesFolder = inputFolder.text.Trim();
        DeckRepository.AutoFill(_config, DeckRepository.Suits36, DeckRepository.Ranks36);
        BuildCardRows();
        Debug.Log("Авто-заполнено 36 карт");
    }

    // ── Строим сетку карт ─────────────────────────────────────

    void BuildCardRows()
    {
        foreach (Transform child in cardsContent)
            Destroy(child.gameObject);

        foreach (var entry in _config.cards)
        {
            var go  = Instantiate(cardRowPrefab, cardsContent);
            var row = go.GetComponent<CardEntryRowUI>();
            row.Setup(entry, _config);
        }
    }

    // ── Рубашка ───────────────────────────────────────────────

    void OnPickBack()
    {
        // TODO: NativeFilePicker
        Debug.Log("TODO: выбрать рубашку");
    }

    void RefreshBackPreview()
    {
        backNameText.text = string.IsNullOrEmpty(_config.cardBackSprite)
            ? "не выбрана"
            : _config.cardBackSprite;

        var sprite = DeckRepository.LoadCardBack(_config);
        backPreview.sprite  = sprite;
        backPreview.enabled = sprite != null;
    }

    // ── Сохранить ─────────────────────────────────────────────

    void OnSave()
    {
        _config.spritesFolder = inputFolder.text.Trim();
        DeckRepository.Save(_config);
        Debug.Log("Конфиг колоды сохранён");
    }

    // ── Назад ─────────────────────────────────────────────────

    void OnBack() => UIManager.Instance.ShowAdmin();
}
