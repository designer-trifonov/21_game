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
        DealerHand.Add(Draw()); // вторая карта дилера закрыта до его хода
    }

    // ── Ход игрока ────────────────────────────────────────────

    /// <summary>Игрок берёт карту. Возвращает false если перебор.</summary>
    public bool PlayerHit()
    {
        PlayerHand.Add(Draw());
        return !PlayerHand.IsBust;
    }

    // ── Ход дилера ────────────────────────────────────────────

    /// <summary>
    /// Дилер доигрывает по стратегии.
    /// Вызывать после того как игрок остановился.
    /// </summary>
    public void RunDealer()
    {
        while (_dealerStrategy.ShouldHit(DealerHand.Score))
            DealerHand.Add(Draw());
    }

    // ── Результат ─────────────────────────────────────────────

    public RoundResult ResolveResult()
    {
        bool playerBust = PlayerHand.IsBust;
        bool dealerBust = DealerHand.IsBust;

        if (playerBust) return RoundResult.DealerWin;
        if (dealerBust) return RoundResult.PlayerWin;

        int ps = PlayerHand.Score;
        int ds = DealerHand.Score;

        if (ps > ds) return RoundResult.PlayerWin;
        if (ds > ps) return RoundResult.DealerWin;
        return RoundResult.Draw;
    }

    // ── Вспомогательное ──────────────────────────────────────

    CardEntry Draw()
    {
        if (_drawIndex >= _deck.Count)
        {
            Debug.LogWarning("[RoundSession] Колода закончилась — перетасовываем.");
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
}
