using UnityEngine;
using UnityEngine.UI;

public class RulesScreen : MonoBehaviour
{
    [Header("Кнопки")]
    public Button btnBack;

    void Start()
    {
        btnBack.onClick.AddListener(() => UIManager.Instance.ShowWelcome());
    }
}
