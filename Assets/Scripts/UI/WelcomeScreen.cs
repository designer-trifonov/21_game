using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    [Header("Кнопки")]
    public Button btnPlay;
    public Button btnRules;
    public Button btnExit;
    public Button btnAdmin;

    void Start()
    {
        btnPlay .onClick.AddListener(() => UIManager.Instance.ShowGirlSelect());
        btnRules.onClick.AddListener(() => UIManager.Instance.ShowRules());
        btnExit .onClick.AddListener(() => UIManager.Instance.Quit());
        btnAdmin.onClick.AddListener(() => UIManager.Instance.ShowAdmin());
    }
}
