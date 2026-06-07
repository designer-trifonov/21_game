using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Счётчик раздач")]
    public TextMeshProUGUI txtRoundCounter;

    [Header("Девушка")]
    public Image girlImage;

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

    [Header("Скрытая карта дилера")]
    public GameObject dealerHiddenCard;

    [Header("Префаб карты")]
    public GameObject cardSlotPrefab;

    RoundSession _session;
    bool         _playerTurnActive;
    bool         _initialized;

    public void Init()
    {
        Debug.Log("[GameController] Init — проверка ссылок:");
        Debug.Log($"  girlImage={girlImage != null}");
        Debug.Log($"  txtRoundCounter={txtRoundCounter != null}");
        Debug.Log($"  txtPlayerScore={txtPlayerScore != null}");
        Debug.Log($"  txtDealerScore={txtDealerScore != null}");
        Debug.Log($"  playerCardsContainer={playerCardsContainer != null}");
        Debug.Log($"  dealerCardsContainer={dealerCardsContainer != null}");
        Debug.Log($"  dealerHiddenCard={dealerHiddenCard != null}");
        Debug.Log($"  btnHit={btnHit != null} btnStand={btnStand != null}");

        if (btnHit   == null) { Debug.LogError("[GameController] btnHit не назначен!"); return; }
        if (btnStand == null) { Debug.LogError("[GameController] btnStand не назначен!"); return; }

        btnHit  .onClick.AddListener(OnHit);
        btnStand.onClick.AddListener(OnStand);

        // Грузим рубашку (0.png) на скрытую карту дилера
        var backPath = DataPaths.FindImage("0");
        if (backPath != null && dealerHiddenCard != null)
        {
            var img = dealerHiddenCard.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                ImageLoader.Load(backPath, s =>
                {
                    Debug.Log($"[GameController] Рубашка загружена: {s != null}");
                    if (img) { img.sprite = s; img.preserveAspect = true; }
                });
            else Debug.LogWarning("[GameController] dealerHiddenCard — нет компонента Image!");
        }
        else Debug.LogWarning($"[GameController] 0.png не найден или dealerHiddenCard null. backPath={backPath}");

        _initialized = true;
        Debug.Log("[GameController] Init завершён");
    }

    public void StartGame()
    {
        if (!_initialized) { Debug.LogError("[GameController] StartGame: Init не был вызван!"); return; }
        Debug.Log("[GameController] StartGame");
        StartRound();
    }

    void StartRound()
    {
        Debug.Log($"[GameController] StartRound — раунд {GameState.Round}");
        var deck     = BuildDeck();
        var strategy = DealerStrategyFactory.Create(GameState.CurrentDifficulty);
        _session     = new RoundSession(deck, strategy);
        _session.Deal();
        _playerTurnActive = true;
        RefreshUI(revealDealer: false);
        SetButtons(true);
    }

    void OnHit()
    {
        if (!_playerTurnActive) return;
        Debug.Log("[GameController] OnHit");
        bool alive = _session.PlayerHit();
        RefreshUI(revealDealer: false);
        if (!alive) { Debug.Log("[GameController] Перебор у игрока"); EndRound(); }
    }

    void OnStand()
    {
        if (!_playerTurnActive) return;
        Debug.Log("[GameController] OnStand");
        _playerTurnActive = false;
        SetButtons(false);
        _session.RunDealer();
        RefreshUI(revealDealer: true);
        EndRound();
    }

    void EndRound()
    {
        _playerTurnActive = false;
        SetButtons(false);
        RefreshUI(revealDealer: true);

        var result = _session.ResolveResult();

        // ── Детальный лог результата ──────────────────────────
        var playerCards = string.Join(", ", _session.PlayerHand.Cards.ConvertAll(c => c.DisplayName));
        var dealerCards = string.Join(", ", _session.DealerHand.Cards.ConvertAll(c => c.DisplayName));
        Debug.Log($"[РЕЗУЛЬТАТ РАУНДА {GameState.Round}] ─────────────────────────");
        Debug.Log($"  Игрок:  {playerCards}  → {_session.PlayerHand.Score} очков  bust={_session.PlayerHand.IsBust}");
        Debug.Log($"  Дилер:  {dealerCards}  → {_session.DealerHand.Score} очков  bust={_session.DealerHand.IsBust}");
        Debug.Log($"  Итог:   {result}");
        Debug.Log($"──────────────────────────────────────────────────────────────");

        switch (result)
        {
            case RoundResult.PlayerWin: GameState.OnPlayerWin();      break;
            case RoundResult.DealerWin: GameState.OnRoundEnd(result); break;
            case RoundResult.Draw:      GameState.OnRoundEnd(result); break;
        }

        Debug.Log($"[GameController] Побед: {GameState.PlayerWins} | Раунд: {GameState.Round}/{GameState.MaxRounds} | ClothingLevel: {GameState.ClothingLevel} | IsGameOver={GameState.IsGameOver} | IsPlayerWon={GameState.IsPlayerWon}");

        if (GameState.IsGameOver || GameState.IsPlayerWon)
            UIManager.Instance.ShowGameEnd();
        else
            UIManager.Instance.ShowRoundResult();
    }

    void RefreshUI(bool revealDealer)
    {
        if (txtRoundCounter != null)
            txtRoundCounter.text = $"{GameState.Round} / {GameState.MaxRounds}";

        if (girlImage == null)
        {
            Debug.LogError("[GameController] girlImage не назначен!");
        }
        else
        {
            var remote = GameState.CurrentRemoteGirl;
            Debug.Log($"[GameController] RefreshUI — ClothingLevel={GameState.ClothingLevel} photos.Count={remote?.photos?.Count}");

            string photoUrl = null;
            if (remote?.photos != null && GameState.ClothingLevel < remote.photos.Count)
                photoUrl = remote.photos[GameState.ClothingLevel];

            if (!string.IsNullOrEmpty(photoUrl))
            {
                Debug.Log($"[GameController] Загружаем фото девушки: {photoUrl}");
                ContentService.Instance.GetSprite(photoUrl, s =>
                {
                    Debug.Log($"[GameController] Фото девушки получено: sprite={s != null}");
                    if (girlImage) girlImage.sprite = s;
                    if (s == null) Debug.LogError($"[GameController] Спрайт NULL для {photoUrl}!");
                });
            }
            else
            {
                Debug.LogWarning($"[GameController] photos пустые или ClothingLevel вне диапазона: {GameState.ClothingLevel}/{remote?.photos?.Count}");
            }
        }

        RebuildCards(playerCardsContainer, _session.PlayerHand.Cards, "Игрок");
        RebuildCards(dealerCardsContainer, _session.DealerHand.Cards, "Дилер");

        if (txtPlayerScore != null) txtPlayerScore.text = _session.PlayerHand.Score.ToString();
        else Debug.LogError("[GameController] txtPlayerScore не назначен!");

        if (revealDealer)
        {
            if (txtDealerScore  != null) txtDealerScore.text = _session.DealerHand.Score.ToString();
            if (dealerHiddenCard != null) dealerHiddenCard.SetActive(false);
            else Debug.LogWarning("[GameController] dealerHiddenCard не назначен!");
        }
        else
        {
            if (txtDealerScore  != null) txtDealerScore.text = "?";
            if (dealerHiddenCard != null) dealerHiddenCard.SetActive(true);
            else Debug.LogWarning("[GameController] dealerHiddenCard не назначен!");
        }
    }

    void RebuildCards(Transform container, List<CardEntry> cards, string owner)
    {
        if (container == null) { Debug.LogError($"[GameController] container для {owner} не назначен!"); return; }
        if (cardSlotPrefab == null) { Debug.LogError("[GameController] cardSlotPrefab не назначен!"); return; }

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            var child = container.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        Debug.Log($"[GameController] RebuildCards {owner} — карт: {cards.Count}");

        foreach (var card in cards)
        {
            var go   = Instantiate(cardSlotPrefab, container);
            go.name  = card.DisplayName;
            Debug.Log($"[GameController] Instantiate '{card.DisplayName}' → parent='{go.transform.parent?.name}' childCount={container.childCount}");

            var slot = go.GetComponent<CardSlotView>();
            if (slot == null) { Debug.LogError($"[GameController] CardSlotView не найден на префабе! Карта: {card.DisplayName}"); continue; }

            if (slot.label  != null) slot.label.text = card.DisplayName;
            if (slot.button != null) slot.button.interactable = false;

            var path = DataPaths.FindImage(card.spriteName);
            Debug.Log($"[GameController] Карта {owner} '{card.DisplayName}' spriteName={card.spriteName} path={path ?? "НЕ НАЙДЕН"}");

            if (!string.IsNullOrEmpty(path) && slot.photo != null)
                ImageLoader.Load(path, s =>
                {
                    Debug.Log($"[GameController] Карта '{card.DisplayName}' спрайт загружен: {s != null}");
                    if (slot.photo == null) return;
                    slot.photo.sprite         = s;
                    slot.photo.preserveAspect = true;
                });
            else if (string.IsNullOrEmpty(path))
                Debug.LogError($"[GameController] Файл карты не найден: spriteName={card.spriteName}");
        }
    }

    static List<CardEntry> BuildDeck()
    {
        var suits  = new[] { "clubs", "diamonds", "hearts", "spades" };
        var ranks  = new[] { "A", "6", "7", "8", "9", "10", "J", "Q", "K" };
        var values = new[] { 11,   6,   7,   8,   9,   10,  10,  10,  10  };
        var deck   = new List<CardEntry>();
        int index  = 1;

        foreach (var suit in suits)
            foreach (var rank in ranks)
                deck.Add(new CardEntry(rank, suit)
                {
                    value      = values[System.Array.IndexOf(ranks, rank)],
                    spriteName = (index++).ToString()
                });

        return deck;
    }

    void SetButtons(bool active)
    {
        if (btnHit   != null) btnHit  .interactable = active;
        else Debug.LogError("[GameController] SetButtons: btnHit null!");
        if (btnStand != null) btnStand.interactable = active;
        else Debug.LogError("[GameController] SetButtons: btnStand null!");
    }
}
