using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Одна строка слота одежды в редакторе девушки.
/// </summary>
public class SlotRowUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text       slotIndexText;
    public TMP_InputField labelInput;
    public Image          photoPreview;
    public Button         btnPhoto;
    public Button         btnVideo;
    public Button         btnAudio;

    ClothingSlot _slot;

    public void Setup(ClothingSlot slot)
    {
        _slot = slot;

        if (slotIndexText != null) slotIndexText.text = slot.index == 0 ? "0 — Одета" : $"{slot.index}";
        if (labelInput    != null) labelInput.text    = slot.label;

        labelInput.onEndEdit.RemoveAllListeners();
        labelInput.onEndEdit.AddListener(val => _slot.label = val);

        if (btnPhoto != null) { btnPhoto.onClick.RemoveAllListeners(); btnPhoto.onClick.AddListener(PickPhoto); }
        if (btnVideo != null) { btnVideo.onClick.RemoveAllListeners(); btnVideo.onClick.AddListener(PickVideo); }
        if (btnAudio != null) { btnAudio.onClick.RemoveAllListeners(); btnAudio.onClick.AddListener(PickAudio); }
    }

    public ClothingSlot GetSlot() => _slot;

    void PickPhoto() => FilePicker.PickImage(path  => _slot.photoPath = path);
    void PickVideo() => FilePicker.PickVideo(path  => _slot.videoPath = path);
    void PickAudio() => FilePicker.PickAudio(path  => _slot.audioPath = path);
}
