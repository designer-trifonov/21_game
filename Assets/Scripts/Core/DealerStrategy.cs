using UnityEngine;

public class StandardDealerStrategy : IDealerStrategy
{
    readonly DifficultySettings _settings;

    public StandardDealerStrategy(DifficultySettings settings)
    {
        _settings = settings;
    }

    public bool ShouldHit(int dealerScore)
    {
        if (dealerScore >= 21) return false;

        // На Easy дилер иногда останавливается раньше порога — "ошибка"
        if (_settings.MistakeChance > 0f && Random.value < _settings.MistakeChance)
            return false;

        return dealerScore < _settings.StandThreshold;
    }
}
