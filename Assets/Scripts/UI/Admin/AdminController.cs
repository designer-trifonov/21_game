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
    public Transform     girlListContent;
    public Button        btnAddGirl;
    public Button        btnDeckConfig;     // → настройки колоды

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

    public TMP_InputField inputClothingCount; // слоты обновляются автоматически при изменении
    public Transform      slotsContent;      // Content для слотов
    public GameObject     slotRowPrefab;     // префаб SlotRowUI

    public Button         btnSave;
    public Button         btnDelete;

    // ── Приватное состояние ───────────────────────────────────
    List<GirlData>   _girls   = new();
    GirlData         _current = null;

    // ─────────────────────────────────────────────────────────

    void OnEnable()
    {
        btnAddGirl    .onClick.AddListener(OnAddGirl);
        btnDeckConfig.onClick.AddListener(() => UIManager.Instance.ShowAdminDeck());
        btnSave       .onClick.AddListener(OnSave);
        btnDelete     .onClick.AddListener(OnDelete);
        btnProfilePhoto.onClick.AddListener(PickProfilePhoto);
        btnIntroVideo  .onClick.AddListener(PickIntroVideo);
        btnIntroAudio  .onClick.AddListener(PickIntroAudio);

        inputClothingCount.onEndEdit.AddListener(OnClothingCountChanged);

        RefreshList();
        editorPanel.SetActive(false);
    }

    void OnDisable()
    {
        btnAddGirl    .onClick.RemoveAllListeners();
        btnDeckConfig.onClick.RemoveAllListeners();
        btnSave       .onClick.RemoveAllListeners();
        btnDelete     .onClick.RemoveAllListeners();
        btnProfilePhoto.onClick.RemoveAllListeners();
        btnIntroVideo  .onClick.RemoveAllListeners();
        btnIntroAudio  .onClick.RemoveAllListeners();
        inputClothingCount.onEndEdit.RemoveAllListeners();
    }

    // ── Список девушек ────────────────────────────────────────

    void RefreshList()
    {
        foreach (Transform child in girlListContent)
            Destroy(child.gameObject);

        _girls = DataRepository.LoadAllGirls();

        foreach (var girl in _girls)
        {
            var captured = girl;
            CardFactory.Create(
                parent:    girlListContent,
                imagePath: captured.profilePhoto,
                label:     captured.name,
                size:      new Vector2(300, 180),
                onClick:   () => OnGirlSelected(captured)
            );
        }
    }

    // ── Выбор девушки ─────────────────────────────────────────

    void OnGirlSelected(GirlData data)
    {
        _current = data;
        PopulateEditor(data);
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

        inputClothingCount.text = data.clothingCount.ToString();
    }


    // ── Авто-обновление слотов при изменении числа ───────────

    void OnClothingCountChanged(string value)
    {
        if (_current == null) return;

        if (int.TryParse(value, out int count))
        {
            _current.clothingCount = Mathf.Clamp(count, 1, 20);
            inputClothingCount.text = _current.clothingCount.ToString(); // корректируем если вышли за пределы
            _current.RebuildSlots();
            BuildSlotRows(_current);
        }
    }

    // ── Сохранить ─────────────────────────────────────────────

    void OnSave()
    {
        if (_current == null) return;

        // Основные поля
        _current.name        = inputName.text;
        _current.description = inputDescription.text;

        if (int.TryParse(inputAge.text, out int age))
            _current.age = age;

        // Кол-во одежды — на случай если не сработал onEndEdit
        if (int.TryParse(inputClothingCount.text, out int count))
        {
            int clamped = Mathf.Clamp(count, 1, 20);
            if (_current.clothingCount != clamped)
            {
                _current.clothingCount = clamped;
                _current.RebuildSlots(); // пересоздаём если число изменилось
            }
        }

        // Собираем актуальные данные слотов прямо из UI строк
        // (на случай если пользователь не нажал Enter после ввода названия)
        var rows = slotsContent.GetComponentsInChildren<SlotRowUI>();
        foreach (var row in rows)
        {
            var slot = row.GetSlot();
            if (slot != null && slot.index < _current.slots.Count)
                _current.slots[slot.index] = slot;
        }

        DataRepository.SaveGirl(_current);
        RefreshList();

        Debug.Log($"Сохранено: {_current.name} | слотов: {_current.slots.Count}");
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

    // ── Пикеры медиа — строго из Data/ ───────────────────────

    void PickProfilePhoto()
    {
        FilePicker.PickImage(path =>
        {
            if (_current == null) return;
            _current.profilePhoto = path;
            ImageLoader.Load(path, sprite =>
            {
                if (profilePreview == null) return;
                profilePreview.sprite  = sprite;
                profilePreview.enabled = sprite != null;
            });
        });
    }
    void PickIntroVideo()   => FilePicker.PickVideo(path => { if (_current != null) _current.introVideo   = path; });
    void PickIntroAudio()   => FilePicker.PickAudio(path => { if (_current != null) _current.introAudio   = path; });
}
