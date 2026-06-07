using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Панели — скрипты")]
    public WelcomeScreen     welcome;
    public RulesScreen       rules;
    public GirlSelectScreen  girlSelect;
    public IntroScreen       intro;
    public DifficultyScreen  difficulty;
    public RoundResultScreen roundResult;
    public GameEndScreen     gameEnd;

    [Header("Панели — только объекты (без скрипта)")]
    public GameObject panelGame;
    public GameObject panelAdmin;

    [Header("Контроллер игры")]
    public GameController gameController;

    [Header("Общая кнопка Back")]
    public Button btnBack;

    [Header("Кнопка Главное меню (только во время игры)")]
    public Button btnMainMenu;

    readonly Stack<GameObject> _history = new();
    GameObject _current;

    void Awake()
    {
        Debug.Log("[UIManager] Awake");

        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        DataPaths.EnsureCreated();

        if (btnBack     != null) btnBack    .onClick.AddListener(GoBack);
        else Debug.LogWarning("[UIManager] btnBack не назначен!");

        if (btnMainMenu != null) btnMainMenu.onClick.AddListener(ShowWelcome);
        else Debug.LogWarning("[UIManager] btnMainMenu не назначен!");

        if (btnMainMenu != null) btnMainMenu.gameObject.SetActive(false);

        // Гасим все панели явно — ни одна не должна быть активна при старте
        DeactivateAll();

        if (welcome     == null) Debug.LogError("[UIManager] welcome не назначен!");
        if (rules       == null) Debug.LogError("[UIManager] rules не назначен!");
        if (girlSelect  == null) Debug.LogError("[UIManager] girlSelect не назначен!");
        if (intro       == null) Debug.LogError("[UIManager] intro не назначен!");
        if (difficulty  == null) Debug.LogError("[UIManager] difficulty не назначен!");
        if (roundResult == null) Debug.LogError("[UIManager] roundResult не назначен!");
        if (gameEnd        == null) Debug.LogError("[UIManager] gameEnd не назначен!");
        if (panelGame      == null) Debug.LogError("[UIManager] panelGame не назначен!");
        if (gameController == null) Debug.LogError("[UIManager] gameController не назначен!");

        welcome       ?.Init();
        rules         ?.Init();
        girlSelect    ?.Init();
        intro         ?.Init();
        difficulty    ?.Init();
        roundResult   ?.Init();
        gameEnd       ?.Init();
        gameController?.Init();

        ShowWelcome();
    }

    // ── Навигация ─────────────────────────────────────────────

    public void ShowWelcome()     => Navigate(() => { Show(welcome?.rootPanel);     welcome    ?.Show(); }, clearHistory: true);
    public void ShowRules()       => Navigate(() => { Show(rules?.rootPanel);        rules      ?.Show(); });
    public void ShowGirlSelect()  => Navigate(() => { Show(girlSelect?.rootPanel);   girlSelect ?.Show(); });
    public void ShowIntro()       => Navigate(() => { Show(intro?.rootPanel);        intro      ?.Show(); });
    public void ShowDifficulty()  => Navigate(() => { Show(difficulty?.rootPanel);   difficulty ?.Show(); });
    public void ShowGame()
    {
        Navigate(() => { Show(panelGame); gameController?.StartGame(); });
        if (btnBack     != null) btnBack    .gameObject.SetActive(false);
        if (btnMainMenu != null) btnMainMenu.gameObject.SetActive(true);
    }
    public void ShowRoundResult() => Navigate(() => { Show(roundResult?.rootPanel);  roundResult?.Show(); });
    public void ShowGameEnd()     => Navigate(() => { Show(gameEnd?.rootPanel);      gameEnd    ?.Show(); });

    // ── Назад ────────────────────────────────────────────────

    public void GoBack()
    {
        Debug.Log($"[UIManager] GoBack — история: {_history.Count}");
        if (_history.Count == 0) { Debug.LogWarning("[UIManager] GoBack: история пуста"); return; }
        if (_current != null) _current.SetActive(false);
        _current = _history.Pop();
        if (_current == null) { Debug.LogError("[UIManager] GoBack: поп вернул null!"); return; }
        _current.SetActive(true);
        if (btnBack != null) btnBack.gameObject.SetActive(_history.Count > 0);
        Debug.Log($"[UIManager] GoBack → {_current.name} | осталось в истории: {_history.Count}");
    }

    // ─────────────────────────────────────────────────────────

    void Navigate(Action action, bool clearHistory = false)
    {
        if (action == null) { Debug.LogError("[UIManager] Navigate: action == null!"); return; }

        if (clearHistory)
        {
            Debug.Log("[UIManager] Navigate — очищаем историю");
            _history.Clear();
        }
        else if (_current != null)
        {
            _history.Push(_current);
            Debug.Log($"[UIManager] Navigate — пушим в историю: {_current.name} | размер: {_history.Count}");
        }

        if (_current != null) _current.SetActive(false);

        action.Invoke();

        if (btnBack     != null) btnBack    .gameObject.SetActive(!clearHistory);
        if (btnMainMenu != null) btnMainMenu.gameObject.SetActive(false);
    }

    void Show(GameObject target)
    {
        if (target == null) { Debug.LogError("[UIManager] Show: rootPanel не назначен!"); return; }
        _current = target;
        _current.SetActive(true);
        Debug.Log($"[UIManager] → {target.name}");
    }

    void DeactivateAll()
    {
        void Off(GameObject go) { if (go != null) go.SetActive(false); }

        Off(welcome    ?.rootPanel);
        Off(rules      ?.rootPanel);
        Off(girlSelect ?.rootPanel);
        Off(intro      ?.rootPanel);
        Off(difficulty ?.rootPanel);
        Off(roundResult?.rootPanel);
        Off(gameEnd    ?.rootPanel);
        Off(panelGame);
        Off(panelAdmin);

        Debug.Log("[UIManager] DeactivateAll — все панели скрыты");
    }

    public void Quit()
    {
        Debug.Log("[UIManager] Quit");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
