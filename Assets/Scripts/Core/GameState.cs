/// <summary>
/// Глобальное состояние игры. Хранит текущую сессию.
/// Сбрасывается при выборе новой девушки.
/// </summary>
public static class GameState
{
    public static GirlData  CurrentGirl   { get; private set; }
    public static int       Round         { get; set; } = 1;
    public static int       MaxRounds     { get; set; } = 14;
    public static int       PlayerWins    { get; set; } = 0;  // кол-во побед игрока
    public static int       ClothingLevel { get; set; } = 0;  // текущий слот одежды

    /// <summary>
    /// Начать новую игру с выбранной девушкой.
    /// </summary>
    public static void StartGame(GirlData girl)
    {
        CurrentGirl   = girl;
        Round         = 1;
        PlayerWins    = 0;
        ClothingLevel = 0;
    }

    /// <summary>
    /// Игрок выиграл раунд — снимаем одежду.
    /// </summary>
    public static void OnPlayerWin()
    {
        PlayerWins++;
        ClothingLevel = PlayerWins;
        Round++;
    }

    /// <summary>
    /// Ничья или поражение — одежда не меняется.
    /// </summary>
    public static void OnRoundEnd()
    {
        Round++;
    }

    public static bool IsGameOver   => Round > MaxRounds;
    public static bool IsPlayerWon  => CurrentGirl != null && ClothingLevel >= CurrentGirl.clothingCount;
}
