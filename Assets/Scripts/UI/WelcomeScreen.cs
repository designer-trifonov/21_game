using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    public GameObject rootPanel;
    public Button     btnPlay;
    public Button     btnRules;
    public Button     btnExit;

    public void Init()
    {
        Debug.Log("[WelcomeScreen] Init");
        if (btnPlay  == null) { Debug.LogError("[WelcomeScreen] btnPlay не назначен!");  return; }
        if (btnRules == null) { Debug.LogError("[WelcomeScreen] btnRules не назначен!"); return; }
        if (btnExit  == null) { Debug.LogError("[WelcomeScreen] btnExit не назначен!");  return; }

        btnPlay .onClick.AddListener(() => { Debug.Log("[WelcomeScreen] Играть");  UIManager.Instance.ShowGirlSelect(); });
        btnRules.onClick.AddListener(() => { Debug.Log("[WelcomeScreen] Правила"); UIManager.Instance.ShowRules(); });
        btnExit .onClick.AddListener(() => { Debug.Log("[WelcomeScreen] Выход");   UIManager.Instance.Quit(); });
    }

    public void Show() => Debug.Log("[WelcomeScreen] Show");
}
