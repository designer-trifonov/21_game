using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Один раунд игры. Управляет колодой, руками игрока и дилера.
/// Не знает про UI — только логика.
/// </summary>
public class RoundSession
{
    public Hand PlayerHand { get; } = new();
    public Hand DealerHand { get; } = new();

    readonly List<CardEntry>  _deck;
    readonly IDealerStrategy  _dealerStrategy;
    int _drawIndex;

    public RoundSession(List<CardEntry> deck, IDealerStrategy dealerStrategy)
    {
        _deck           = Shuffle(deck);
        _dealerStrategy = dealerStrategy;
        Debug.Log($"[RoundSession] Создан — карт в колоде: {_deck.Count} стратегия: {dealerStrategy?.GetType().Name}");
    }

    // ── Раздача ───────────────────────────────────────────────

    public void Deal()
    {
        PlayerHand.Clear();
        DealerHand.Clear();
        _drawIndex = 0;

        PlayerHand.Add(Draw());
        DealerHand.Add(Draw());
        PlayerHand.Add(Draw());
        DealerHand.Add(Draw());

        Debug.Log($"[RoundSession] Deal — игрок: {CardsLog(PlayerHand)} ({PlayerHand.Score}) | дилер: {CardsLog(DealerHand)} ({DealerHand.Score})");
    }

    // ── Ход игрока ────────────────────────────────────────────

    /// <summary>Игрок берёт карту. Возвращает false если перебор.</summary>
    public bool PlayerHit()
    {
        var card = Draw();
        PlayerHand.Add(card);
        bool alive = !PlayerHand.IsBust;
        Debug.Log($"[RoundSession] PlayerHit — карта: {card.DisplayName} итого: {PlayerHand.Score} bust={PlayerHand.IsBust}");
        return alive;
    }

    // ── Ход дилера ────────────────────────────────────────────

    public void RunDealer()
    {
        int hits = 0;
        while (_dealerStrategy.ShouldHit(DealerHand.Score))
        {
            DealerHand.Add(Draw());
            hits++;
        }
        Debug.Log($"[RoundSession] RunDealer — взял карт: {hits} | итого: {CardsLog(DealerHand)} ({DealerHand.Score}) bust={DealerHand.IsBust}");
    }

    // ── Результат ─────────────────────────────────────────────

    public RoundResult ResolveResult()
    {
        bool playerBust = PlayerHand.IsBust;
        bool dealerBust = DealerHand.IsBust;
        int  ps         = PlayerHand.Score;
        int  ds         = DealerHand.Score;

        RoundResult result;

        if      (playerBust)  result = RoundResult.DealerWin;
        else if (dealerBust)  result = RoundResult.PlayerWin;
        else if (ps > ds)     result = RoundResult.PlayerWin;
        else if (ds > ps)     result = RoundResult.DealerWin;
        else                  result = RoundResult.Draw;

        Debug.Log($"[RoundSession] ResolveResult — игрок={ps}(bust={playerBust}) дилер={ds}(bust={dealerBust}) → {result}");
        return result;
    }

    // ── Вспомогательное ──────────────────────────────────────

    CardEntry Draw()
    {
        if (_drawIndex >= _deck.Count)
        {
            Debug.LogWarning("[RoundSession] Колода закончилась — перетасовываем.");
            for (int i = _deck.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
            }
            _drawIndex = 0;
        }
        return _deck[_drawIndex++];
    }

    static List<CardEntry> Shuffle(List<CardEntry> source)
    {
        var deck = new List<CardEntry>(source);
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
        return deck;
    }

    static string CardsLog(Hand hand) =>
        string.Join(", ", hand.Cards.ConvertAll(c => c.DisplayName));
}
