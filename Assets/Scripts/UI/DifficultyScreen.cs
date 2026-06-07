using UnityEngine;
using UnityEngine.UI;

public class DifficultyScreen : MonoBehaviour
{
    public GameObject rootPanel;
    public Button btnEasy;
    public Button btnNormal;
    public Button btnHard;

    public Color colorSelected   = new Color(1f, 0.8f, 0f);
    public Color colorDeselected = new Color(0.3f, 0.3f, 0.3f);

    public void Init()
    {
        Debug.Log("[DifficultyScreen] Init");
        if (btnEasy   == null) { Debug.LogError("[DifficultyScreen] btnEasy не назначен!");   return; }
        if (btnNormal == null) { Debug.LogError("[DifficultyScreen] btnNormal не назначен!"); return; }
        if (btnHard   == null) { Debug.LogError("[DifficultyScreen] btnHard не назначен!");   return; }

        btnEasy  .onClick.AddListener(() => Select(Difficulty.Easy));
        btnNormal.onClick.AddListener(() => Select(Difficulty.Normal));
        btnHard  .onClick.AddListener(() => Select(Difficulty.Hard));
    }

    public void Show()
    {
        Debug.Log($"[DifficultyScreen] Show — текущая сложность: {GameState.CurrentDifficulty}");
        Refresh(GameState.CurrentDifficulty);
    }

    void Select(Difficulty d)
    {
        Debug.Log($"[DifficultyScreen] Выбрана сложность: {d}");
        GameState.SetDifficulty(d);
        Refresh(d);
        UIManager.Instance.ShowGame();
    }

    void Refresh(Difficulty d)
    {
        Highlight(btnEasy,   d == Difficulty.Easy);
        Highlight(btnNormal, d == Difficulty.Normal);
        Highlight(btnHard,   d == Difficulty.Hard);
    }

    void Highlight(Button btn, bool active)
    {
        if (btn == null) { Debug.LogError("[DifficultyScreen] Кнопка null в Highlight!"); return; }
        if (btn.targetGraphic == null) { Debug.LogWarning($"[DifficultyScreen] targetGraphic null у {btn.name}"); return; }
        btn.targetGraphic.color = active ? colorSelected : colorDeselected;
    }
}
