using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Экран выбора сложности. Показывается после выбора девушки, перед игрой.
/// </summary>
public class DifficultyScreen : MonoBehaviour
{
    [Header("Кнопки сложности")]
    public Button btnEasy;
    public Button btnNormal;
    public Button btnHard;

    [Header("Подсветка выбранной кнопки")]
    public Color colorSelected   = new Color(1f, 0.8f, 0f);
    public Color colorDeselected = new Color(0.3f, 0.3f, 0.3f);

    void OnEnable()
    {
        Refresh(GameState.CurrentDifficulty);

        btnEasy  .onClick.AddListener(() => Select(Difficulty.Easy));
        btnNormal.onClick.AddListener(() => Select(Difficulty.Normal));
        btnHard  .onClick.AddListener(() => Select(Difficulty.Hard));
    }

    void OnDisable()
    {
        btnEasy  .onClick.RemoveAllListeners();
        btnNormal.onClick.RemoveAllListeners();
        btnHard  .onClick.RemoveAllListeners();
    }

    void Select(Difficulty d)
    {
        GameState.SetDifficulty(d);
        Refresh(d);
        UIManager.Instance.ShowGame();
    }

    void Refresh(Difficulty d)
    {
        SetHighlight(btnEasy,   d == Difficulty.Easy);
        SetHighlight(btnNormal, d == Difficulty.Normal);
        SetHighlight(btnHard,   d == Difficulty.Hard);
    }

    void SetHighlight(Button btn, bool active)
    {
        var img = btn.GetComponent<Image>();
        if (img != null)
            img.color = active ? colorSelected : colorDeselected;
    }
}
