using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Главный контроллер админки.
/// Левая панель — список карточек девушек.
/// Правая панель — редактор выбранной / новой девушки.
/// </summary>
public class AdminController : MonoBehaviour
{
    // ── Левая панель ─────────────────────────────────────────
    [Header("Левая панель — список")]
    public Transform     girlListContent;   // Content внутри ScrollView
    public GameObject    girlCardPrefab;    // префаб GirlCardUI
    public Button        btnAddGirl;
    public Button        btnDeckConfig;     // → настройки колоды
    public Button        btnBack;

    // ── Правая панель — редактор ──────────────────────────────
    [Header("Правая панель — редактор")]
    public GameObject    editorPanel;       // скрываем пока никто не выбран

    public TMP_InputField inputName;
    public TMP_InputField inputAge;
    public TMP_InputField inputDescription;
    public Image          profilePreview;
    public Button         btnProfilePhoto;
    public Button         btnIntroVideo;
    public Button         btnIntroAudio;

    public TMP_InputField inputClothingCount;
    public Button         btnRefreshSlots;
    public Transform      slotsContent;     // Content для слотов
    public GameObject     slotRowPrefab;    // префаб SlotRowUI

    public Button         btnSave;
    public Button         btnDelete;

    // ── Приватное состояние ───────────────────────────────────
    List<GirlData>   _girls      = new();
    GirlData         _current    = null;
    List<GirlCardUI> _cards      = new();

    // ─────────────────────────────────────────────────────────

    void OnEnable()
    {
        btnAddGirl    .onClick.AddListener(OnAddGirl);
        btnDeckConfig .onClick.AddListener(() => UIManager.Instance.ShowAdminDeck());
        btnBack       .onClick.AddListener(OnBack);
        btnRefreshSlots.onClick.AddListener(OnRefreshSlots);
        btnSave       .onClick.AddListener(OnSave);
        btnDelete     .onClick.AddListener(OnDelete);
        btnProfilePhoto.onClick.AddListener(() => Debug.Log("TODO: выбрать фото профиля"));
        btnIntroVideo  .onClick.AddListener(() => Debug.Log("TODO: выбрать вступительное видео"));
        btnIntroAudio  .onClick.AddListener(() => Debug.Log("TODO: выбрать вступительное аудио"));

        RefreshList();
        editorPanel.SetActive(false);
    }

    void OnDisable()
    {
        btnAddGirl    .onClick.RemoveAllListeners();
        btnDeckConfig .onClick.RemoveAllListeners();
        btnBack       .onClick.RemoveAllListeners();
        btnRefreshSlots.onClick.RemoveAllListeners();
        btnSave       .onClick.RemoveAllListeners();
        btnDelete     .onClick.RemoveAllListeners();
        btnProfilePhoto.onClick.RemoveAllListeners();
        btnIntroVideo  .onClick.RemoveAllListeners();
        btnIntroAudio  .onClick.RemoveAllListeners();
    }

    // ── Список девушек ────────────────────────────────────────

    void RefreshList()
    {
        // очищаем старые карточки
        foreach (Transform child in girlListContent)
            Destroy(child.gameObject);
        _cards.Clear();

        _girls = DataRepository.LoadAllGirls();

        foreach (var girl in _girls)
        {
            var go   = Instantiate(girlCardPrefab, girlListContent);
            var card = go.GetComponent<GirlCardUI>();
            card.Setup(girl, OnGirlSelected);
            _cards.Add(card);
        }
    }

    // ── Выбор девушки ─────────────────────────────────────────

    void OnGirlSelected(GirlData data)
    {
        _current = data;
        PopulateEditor(data);

        // подсвечиваем выбранную карточку
        foreach (var card in _cards)
            card.SetSelected(card.GetComponent<GirlCardUI>() != null);
    }

    // ── Добавить новую ────────────────────────────────────────

    void OnAddGirl()
    {
        var newGirl = new GirlData();   // дефолты: Аноним, 21, 7 слотов
        newGirl.RebuildSlots();
        _current = newGirl;
        PopulateEditor(newGirl);
    }

    // ── Заполнить поля редактора ──────────────────────────────

    void PopulateEditor(GirlData data)
    {
        editorPanel.SetActive(true);

        inputName        .text = data.name;
        inputAge         .text = data.age.ToString();
        inputDescription .text = data.description;
        inputClothingCount.text = data.clothingCount.ToString();

        BuildSlotRows(data);
    }

    void BuildSlotRows(GirlData data)
    {
        foreach (Transform child in slotsContent)
            Destroy(child.gameObject);

        foreach (var slot in data.slots)
        {
            var go  = Instantiate(slotRowPrefab, slotsContent);
            var row = go.GetComponent<SlotRowUI>();
            row.Setup(slot);
        }
    }

    // ── Обновить кол-во слотов ────────────────────────────────

    void OnRefreshSlots()
    {
        if (_current == null) return;

        if (int.TryParse(inputClothingCount.text, out int count))
        {
            _current.clothingCount = Mathf.Clamp(count, 1, 20);
            _current.RebuildSlots();
            BuildSlotRows(_current);
        }
    }

    // ── Сохранить ─────────────────────────────────────────────

    void OnSave()
    {
        if (_current == null) return;

        _current.name        = inputName.text;
        _current.description = inputDescription.text;

        if (int.TryParse(inputAge.text, out int age))
            _current.age = age;

        // слоты уже обновлены через SlotRowUI напрямую

        DataRepository.SaveGirl(_current);
        RefreshList();

        Debug.Log($"Сохранено: {_current.name}");
    }

    // ── Удалить ───────────────────────────────────────────────

    void OnDelete()
    {
        if (_current == null || string.IsNullOrEmpty(_current.id)) return;

        DataRepository.DeleteGirl(_current.id);
        _current = null;
        editorPanel.SetActive(false);
        RefreshList();
    }

    // ── Назад ─────────────────────────────────────────────────

    void OnBack() => UIManager.Instance.ShowWelcome();
}
