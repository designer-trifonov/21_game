using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Оркестратор одного раунда. Живёт на панели panelGame.
/// Знает про UI этой панели — больше ни о чём.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Очки")]
    public TextMeshProUGUI txtPlayerScore;
    public TextMeshProUGUI txtDealerScore;

    [Header("Карты игрока")]
    public Transform playerCardsContainer;

    [Header("Карты дилера")]
    public Transform dealerCardsContainer;

    [Header("Кнопки")]
    public Button btnHit;
    public Button btnStand;

    [Header("Скрытая карта дилера (заглушка)")]
    public GameObject dealerHiddenCard;

    RoundSession _session;
    bool         _playerTurnActive;

    // ─────────────────────────────────────────────────────────

    void OnEnable()
    {
        btnHit  .onClick.AddListener(OnHit);
        btnStand.onClick.AddListener(OnStand);
        StartRound();
    }

    void OnDisable()
    {
        btnHit  .onClick.RemoveAllListeners();
        btnStand.onClick.RemoveAllListeners();
    }

    // ── Старт раунда ─────────────────────────────────────────

    void StartRound()
    {
        var deck     = DeckRepository.Load().cards;
        var strategy = DealerStrategyFactory.Create(GameState.CurrentDifficulty);
        _session     = new RoundSession(deck, strategy);
        _session.Deal();

        _playerTurnActive = true;

        RefreshUI(revealDealer: false);
        SetButtons(true);
    }

    // ── Действия игрока ──────────────────────────────────────

    void OnHit()
    {
        if (!_playerTurnActive) return;

        bool alive = _session.PlayerHit();
        RefreshUI(revealDealer: false);

        if (!alive)
            EndRound(); // перебор у игрока
    }

    void OnStand()
    {
        if (!_playerTurnActive) return;
        _playerTurnActive = false;
        SetButtons(false);

        _session.RunDealer();
        RefreshUI(revealDealer: true);
        EndRound();
    }

    // ── Конец раунда ─────────────────────────────────────────

    void EndRound()
    {
        _playerTurnActive = false;
        SetButtons(false);
        RefreshUI(revealDealer: true);

        var result = _session.ResolveResult();

        switch (result)
        {
            case RoundResult.PlayerWin: GameState.OnPlayerWin();        break;
            case RoundResult.DealerWin: GameState.OnRoundEnd(result);   break;
            case RoundResult.Draw:      GameState.OnRoundEnd(result);   break;
        }

        if (GameState.IsGameOver || GameState.IsPlayerWon)
            UIManager.Instance.ShowGameEnd();
        else
            UIManager.Instance.ShowRoundResult();
    }

    // ── Обновление UI ────────────────────────────────────────

    void RefreshUI(bool revealDealer)
    {
        RebuildCards(playerCardsContainer, _session.PlayerHand.Cards);
        RebuildCards(dealerCardsContainer, _session.DealerHand.Cards);

        txtPlayerScore.text = _session.PlayerHand.Score.ToString();

        if (revealDealer)
        {
            txtDealerScore.text = _session.DealerHand.Score.ToString();
            if (dealerHiddenCard != null) dealerHiddenCard.SetActive(false);
        }
        else
        {
            txtDealerScore.text = "?";
            if (dealerHiddenCard != null) dealerHiddenCard.SetActive(true);
        }
    }

    void RebuildCards(Transform container, List<CardEntry> cards)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var card in cards)
        {
            var go   = new GameObject(card.DisplayName);
            go.transform.SetParent(container, false);

            var rect       = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 120);

            var img   = go.AddComponent<Image>();
            img.color = new Color(0.9f, 0.9f, 0.9f);

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var labelRect       = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var tmp       = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = card.DisplayName;
            tmp.fontSize  = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color     = (card.suit == "hearts" || card.suit == "diamonds")
                            ? Color.red : Color.black;
        }
    }

    void SetButtons(bool active)
    {
        btnHit  .interactable = active;
        btnStand.interactable = active;
    }
}
