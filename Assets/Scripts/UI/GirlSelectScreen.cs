using UnityEngine;
using UnityEngine.UI;

public class GirlSelectScreen : MonoBehaviour
{
    [Header("Кнопки")]
    public Button btnBack;

    void Start()
    {
        btnBack.onClick.AddListener(() => UIManager.Instance.ShowWelcome());
    }
}
