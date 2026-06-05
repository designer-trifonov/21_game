using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Экран результата раунда.
/// </summary>
public class RoundResultScreen : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI txtResult;
    public TextMeshProUGUI txtRound;
    public TextMeshProUGUI txtWins;
    public Button          btnContinue;

    void OnEnable()
    {
        btnContinue.onClick.AddListener(OnContinue);

        txtRound.text = $"Раунд {GameState.Round - 1} из {GameState.MaxRounds}";
        txtWins .text = $"Побед: {GameState.PlayerWins}";
        txtResult.text = GameState.LastRoundResult switch
        {
            RoundResult.PlayerWin => "Победа!",
            RoundResult.DealerWin => "Дилер выиграл",
            RoundResult.Draw      => "Ничья",
            _                     => ""
        };
    }

    void OnDisable() => btnContinue.onClick.RemoveAllListeners();

    void OnContinue() => UIManager.Instance.ShowGame();
}
