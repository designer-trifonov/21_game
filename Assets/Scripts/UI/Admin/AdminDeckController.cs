using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Панель настройки колоды.
/// Рубашка и карты создаются через CardFactory.
/// </summary>
public class AdminDeckController : MonoBehaviour
{
    [Header("Рубашка")]
    public Transform backContainer;  // контейнер для одного слота рубашки

    [Header("Лента карт")]
    public Transform cardsContent;

    [Header("Кнопки")]
    public Button btnSave;

    DeckConfig _config;

    void OnEnable()
    {
        _config = DeckRepository.Load();

        btnSave.onClick.AddListener(OnSave);

        BuildBackSlot();
        BuildCardStrip();
    }

    void OnDisable()
    {
        btnSave.onClick.RemoveAllListeners();
    }

    // ── Рубашка ───────────────────────────────────────────────

    void BuildBackSlot()
    {
        foreach (Transform child in backContainer)
            Destroy(child.gameObject);

        var imagePath = DataPaths.FindImage(_config.cardBackSprite);
        CardFactory.Create(
            parent:    backContainer,
            imagePath: imagePath,
            label:     "Рубашка",
            size:      new Vector2(100, 140),
            onClick:   OnBackClicked
        );
    }

    void OnBackClicked()
    {
        FilePicker.PickImage(path =>
        {
            _config.cardBackSprite = System.IO.Path.GetFileNameWithoutExtension(path);
            BuildBackSlot();
        });
    }

    // ── Лента карт ────────────────────────────────────────────

    void BuildCardStrip()
    {
        foreach (Transform child in cardsContent)
            Destroy(child.gameObject);

        if (_config.cards.Count == 0)
            DeckRepository.AutoFill(_config, DeckRepository.Suits36, DeckRepository.Ranks36);

        foreach (var entry in _config.cards)
        {
            var captured  = entry;
            var imagePath = DataPaths.FindImage(entry.spriteName);
            CardFactory.Create(
                parent:    cardsContent,
                imagePath: imagePath,
                label:     captured.DisplayNameRu,
                size:      new Vector2(100, 140),
                onClick:   () => OnCardClicked(captured)
            );
        }
    }

    void OnCardClicked(CardEntry entry)
    {
        FilePicker.PickImage(path =>
        {
            entry.spriteName = System.IO.Path.GetFileNameWithoutExtension(path);
            BuildCardStrip();
        });
    }

    // ── Сохранить ─────────────────────────────────────────────

    void OnSave()
    {
        DeckRepository.Save(_config);
        Debug.Log("Конфиг колоды сохранён");
    }
}
