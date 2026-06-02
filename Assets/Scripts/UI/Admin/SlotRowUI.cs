using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Одна строка слота одежды в редакторе девушки.
/// Создаётся динамически.
/// </summary>
public class SlotRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text    slotIndexText;   // "Слот 0", "Слот 1" ...
    public TMP_InputField labelInput;   // название слота
    public Image       photoPreview;
    public Button      btnPhoto;
    public Button      btnVideo;
    public Button      btnAudio;

    ClothingSlot _slot;

    public void Setup(ClothingSlot slot)
    {
        _slot = slot;

        slotIndexText.text = slot.index == 0 ? "Слот 0 — Одета" : $"Слот {slot.index}";
        labelInput.text    = slot.label;

        labelInput.onEndEdit.AddListener(val => _slot.label = val);

        btnPhoto.onClick.RemoveAllListeners();
        btnVideo.onClick.RemoveAllListeners();
        btnAudio.onClick.RemoveAllListeners();

        btnPhoto.onClick.AddListener(() => Debug.Log($"TODO: выбрать фото слота {slot.index}"));
        btnVideo.onClick.AddListener(() => Debug.Log($"TODO: выбрать видео слота {slot.index}"));
        btnAudio.onClick.AddListener(() => Debug.Log($"TODO: выбрать аудио слота {slot.index}"));
    }

    public ClothingSlot GetSlot() => _slot;
}
