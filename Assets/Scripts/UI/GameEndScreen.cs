using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Экран конца игры — победа над девушкой или исчерпаны раунды.
/// </summary>
public class GameEndScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtSummary;
    public Button          btnPlayAgain;
    public Button          btnMainMenu;

    void OnEnable()
    {
        btnPlayAgain.onClick.AddListener(OnPlayAgain);
        btnMainMenu .onClick.AddListener(OnMainMenu);

        if (GameState.IsPlayerWon)
        {
            txtTitle  .text = "Поздравляем!";
            txtSummary.text = $"Ты победил {GameState.CurrentGirl?.name ?? "девушку"}!\nПобед: {GameState.PlayerWins}";
        }
        else
        {
            txtTitle  .text = "Игра окончена";
            txtSummary.text = $"Раунды закончились.\nПобед: {GameState.PlayerWins} из {GameState.MaxRounds}";
        }
    }

    void OnDisable()
    {
        btnPlayAgain.onClick.RemoveAllListeners();
        btnMainMenu .onClick.RemoveAllListeners();
    }

    void OnPlayAgain()
    {
        GameState.StartGame(GameState.CurrentGirl);
        UIManager.Instance.ShowDifficulty();
    }

    void OnMainMenu() => UIManager.Instance.ShowWelcome();
}
