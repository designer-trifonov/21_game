[System.Serializable]
public class CardEntry
{
    public string rank;
    public string suit;
    public string spriteName;
    public int    value;

    public string SuitRu => suit switch
    {
        "clubs"    => "♣",
        "diamonds" => "♦",
        "hearts"   => "♥",
        "spades"   => "♠",
        _          => suit
    };

    public string DisplayName => $"{rank}{SuitRu}";

    public CardEntry(string rank, string suit)
    {
        this.rank = rank;
        this.suit = suit;
    }
}
