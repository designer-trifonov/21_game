using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Карточка девушки в левом списке админки.
/// Создаётся динамически через AdminController.
/// </summary>
public class GirlCardUI : MonoBehaviour
{
    [Header("UI")]
    public Image       photoImage;
    public TMP_Text    nameText;
    public Button      selectButton;
    public GameObject  selectedBorder;   // outline/highlight при выборе

    GirlData _data;
    System.Action<GirlData> _onSelect;

    public void Setup(GirlData data, System.Action<GirlData> onSelect)
    {
        _data     = data;
        _onSelect = onSelect;

        nameText.text = data.name;
        SetSelected(false);

        // TODO: загрузить превью фото если есть
        // if (!string.IsNullOrEmpty(data.profilePhoto)) LoadPhoto(data.profilePhoto);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClick);
    }

    public void SetSelected(bool on)
    {
        if (selectedBorder != null)
            selectedBorder.SetActive(on);
    }

    void OnClick() => _onSelect?.Invoke(_data);
}
