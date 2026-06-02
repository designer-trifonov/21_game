using UnityEngine;

/// <summary>
/// Центральный менеджер UI. Одна сцена — переключаем панели.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Все панели")]
    public GameObject panelWelcome;
    public GameObject panelRules;
    public GameObject panelGirlSelect;
    public GameObject panelIntro;
    public GameObject panelGame;
    public GameObject panelRoundResult;
    public GameObject panelGameEnd;
    public GameObject panelAdmin;
    public GameObject panelAdminDeck;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        ShowWelcome();
    }

    // ── Показать нужную панель, остальные скрыть ──

    public void ShowWelcome()      => Show(panelWelcome);
    public void ShowRules()        => Show(panelRules);
    public void ShowGirlSelect()   => Show(panelGirlSelect);
    public void ShowIntro()        => Show(panelIntro);
    public void ShowGame()         => Show(panelGame);
    public void ShowRoundResult()  => Show(panelRoundResult);
    public void ShowGameEnd()      => Show(panelGameEnd);
    public void ShowAdmin()        => Show(panelAdmin);
    public void ShowAdminDeck()    => Show(panelAdminDeck);

    void Show(GameObject target)
    {
        panelWelcome    .SetActive(false);
        panelRules      .SetActive(false);
        panelGirlSelect .SetActive(false);
        panelIntro      .SetActive(false);
        panelGame       .SetActive(false);
        panelRoundResult.SetActive(false);
        panelGameEnd    .SetActive(false);
        panelAdmin      .SetActive(false);
        panelAdminDeck  .SetActive(false);

        target.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
