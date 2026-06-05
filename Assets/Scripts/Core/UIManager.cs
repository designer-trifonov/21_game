using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Центральный менеджер UI. Одна сцена — переключаем панели.
/// Стек навигации — кнопка Back всегда возвращает на предыдущую.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Панели")]
    public GameObject panelWelcome;
    public GameObject panelRules;
    public GameObject panelGirlSelect;
    public GameObject panelDifficulty;
    public GameObject panelIntro;
    public GameObject panelGame;
    public GameObject panelRoundResult;
    public GameObject panelGameEnd;
    public GameObject panelAdmin;
    public GameObject panelAdminDeck;

    [Header("Общая кнопка Back")]
    public Button btnBack;  // одна на всё приложение, висит поверх всего

    readonly Stack<GameObject> _history = new();
    GameObject _current;

    // ─────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DataPaths.EnsureCreated();

        if (btnBack != null)
            btnBack.onClick.AddListener(GoBack);

        ShowWelcome();
    }

    // ── Навигация ─────────────────────────────────────────────

    public void ShowWelcome()     => Show(panelWelcome,     clearHistory: true);
    public void ShowRules()       => Show(panelRules);
    public void ShowGirlSelect()  => Show(panelGirlSelect);
    public void ShowDifficulty()  => Show(panelDifficulty);
    public void ShowIntro()       => Show(panelIntro);
    public void ShowGame()        => Show(panelGame);
    public void ShowRoundResult() => Show(panelRoundResult);
    public void ShowGameEnd()     => Show(panelGameEnd);
    public void ShowAdmin()       => Show(panelAdmin);
    public void ShowAdminDeck()   => Show(panelAdminDeck);

    // ── Назад ────────────────────────────────────────────────

    public void GoBack()
    {
        if (_history.Count == 0) return;
        var prev = _history.Pop();
        ShowDirect(prev);
    }

    // ─────────────────────────────────────────────────────────

    void Show(GameObject target, bool clearHistory = false)
    {
        if (clearHistory)
            _history.Clear();
        else if (_current != null)
            _history.Push(_current);

        ShowDirect(target);
    }

    void ShowDirect(GameObject target)
    {
        if (_current != null) _current.SetActive(false);
        _current = target;
        _current.SetActive(true);

        // Кнопка Back видна только если есть куда возвращаться
        if (btnBack != null)
            btnBack.gameObject.SetActive(_history.Count > 0);
    }

    // ─────────────────────────────────────────────────────────

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
