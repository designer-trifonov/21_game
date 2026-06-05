public static class DealerStrategyFactory
{
    public static IDealerStrategy Create(Difficulty difficulty)
    {
        var settings = difficulty switch
        {
            Difficulty.Easy   => new DifficultySettings(standThreshold: 15, mistakeChance: 0.25f),
            Difficulty.Normal => new DifficultySettings(standThreshold: 17, mistakeChance: 0f),
            Difficulty.Hard   => new DifficultySettings(standThreshold: 19, mistakeChance: 0f),
            _                 => new DifficultySettings(standThreshold: 17, mistakeChance: 0f),
        };
        return new StandardDealerStrategy(settings);
    }
}
