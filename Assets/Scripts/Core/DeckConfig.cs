using System.Collections.Generic;

/// <summary>
/// Конфиг всей колоды. Хранится в JSON через DataRepository.
/// </summary>
[System.Serializable]
public class DeckConfig
{
    public string        cardBackSprite = "";      // имя файла рубашки (без расширения)
    public string        spritesFolder  = "Cards"; // папка внутри Resources/
    public List<CardEntry> cards        = new List<CardEntry>();

    // ── Значения карт для игры в 21 ──────────────────────────
    public static int GetValue(string rank)
    {
        return rank switch
        {
            "A"              => 11,
            "K" or "Q" or "J"=> 10,
            _                => int.TryParse(rank, out int v) ? v : 0
        };
    }
}

[System.Serializable]
public class CardEntry
{
    public string rank;        // A 6 7 8 9 10 J Q K
    public string suit;        // clubs diamonds hearts spades
    public string spriteName;  // имя файла без расширения, например card_A_clubs
    public int    value;       // игровое значение для 21

    // Символ масти
    public string SuitRu => suit switch
    {
        "clubs"    => "♣",
        "diamonds" => "♦",
        "hearts"   => "♥",
        "spades"   => "♠",
        _          => suit
    };

    // Полное русское название: "Туз Треф", "Шесть Червей" и т.д.
    public string DisplayNameRu =>
        $"{RankRu}\n{SuitFullRu}";

    public string RankRu => rank switch
    {
        "A"  => "Туз",
        "6"  => "Шесть",
        "7"  => "Семь",
        "8"  => "Восемь",
        "9"  => "Девять",
        "10" => "Десять",
        "J"  => "Валет",
        "Q"  => "Дама",
        "K"  => "Король",
        _    => rank
    };

    public string SuitFullRu => suit switch
    {
        "clubs"    => "Трефы",
        "diamonds" => "Бубны",
        "hearts"   => "Червы",
        "spades"   => "Пики",
        _          => suit
    };

    public string DisplayName => $"{rank}{SuitRu}";

    // Конструктор для авто-заполнения
    public CardEntry(string rank, string suit)
    {
        this.rank       = rank;
        this.suit       = suit;
        this.spriteName = $"card_{rank}_{suit}";
        this.value      = DeckConfig.GetValue(rank);
    }
}
