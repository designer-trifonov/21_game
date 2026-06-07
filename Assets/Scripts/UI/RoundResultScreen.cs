using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundResultScreen : MonoBehaviour
{
    public GameObject      rootPanel;
    public TextMeshProUGUI txtResult;
    public TextMeshProUGUI txtRound;
    public TextMeshProUGUI txtWins;
    public Button          btnContinue;

    public void Init()
    {
        Debug.Log("[RoundResultScreen] Init");
        if (btnContinue == null) { Debug.LogError("[RoundResultScreen] btnContinue не назначен!"); return; }
        btnContinue.onClick.AddListener(() => { Debug.Log("[RoundResultScreen] Продолжить"); UIManager.Instance.ShowGame(); });
    }

    public void Show()
    {
        Debug.Log($"[RoundResultScreen] Show — результат: {GameState.LastRoundResult} раунд: {GameState.Round - 1} победы: {GameState.PlayerWins}");

        if (txtRound  == null) Debug.LogError("[RoundResultScreen] txtRound не назначен!");
        if (txtWins   == null) Debug.LogError("[RoundResultScreen] txtWins не назначен!");
        if (txtResult == null) Debug.LogError("[RoundResultScreen] txtResult не назначен!");

        if (txtRound  != null) txtRound.text  = $"Раунд {GameState.Round - 1} из {GameState.MaxRounds}";
        if (txtWins   != null) txtWins.text   = $"Побед: {GameState.PlayerWins}";
        if (txtResult != null) txtResult.text = GameState.LastRoundResult switch
        {
            RoundResult.PlayerWin => "Победа!",
            RoundResult.DealerWin => "Дилер выиграл",
            RoundResult.Draw      => "Ничья",
            _                     => ""
        };
    }
}
