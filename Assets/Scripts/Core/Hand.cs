using System.Collections.Generic;

public class Hand
{
    public List<CardEntry> Cards { get; } = new();

    public void Add(CardEntry card) => Cards.Add(card);
    public void Clear()             => Cards.Clear();

    /// <summary>
    /// Сумма очков. Туз считается как 11, если не приводит к перебору — иначе как 1.
    /// </summary>
    public int Score
    {
        get
        {
            int total = 0;
            int aces  = 0;

            foreach (var card in Cards)
            {
                total += card.value;
                if (card.rank == "A") aces++;
            }

            // Понижаем тузы с 11 до 1 пока есть перебор
            while (total > 21 && aces > 0)
            {
                total -= 10;
                aces--;
            }

            return total;
        }
    }

    public bool IsBust       => Score > 21;
    public bool IsBlackjack  => Cards.Count == 2 && Score == 21;
}
