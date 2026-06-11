using UnityEngine;

/// <summary>
/// Глобальное состояние игры. Только remote-девушки, локальная adminка удалена.
/// </summary>
public static class GameState
{
    public static RemoteGirlData   CurrentRemoteGirl { get; private set; }
    public static int              Round             { get; set; } = 1;
    public static int              MaxRounds         { get; set; } = 16;
    public static int              PlayerWins        { get; set; } = 0;
    public static int              ClothingLevel     { get; set; } = 0;
    public static Difficulty       CurrentDifficulty { get; private set; } = Difficulty.Normal;
    public static RoundResult      LastRoundResult   { get; private set; } = RoundResult.Draw;

    public static void StartRemoteGame(RemoteGirlData girl)
    {
        CurrentRemoteGirl = girl;
        Round         = 1;
        PlayerWins    = 0;
        ClothingLevel = 0;
        Debug.Log($"[GameState] StartRemoteGame — id={girl?.id} photos={girl?.photos?.Count} MaxRounds={MaxRounds} Difficulty={CurrentDifficulty}");
    }

    public static void OnPlayerWin()
    {
        LastRoundResult = RoundResult.PlayerWin;
        PlayerWins++;
        ClothingLevel = PlayerWins;
        Round++;
        Debug.Log($"[GameState] OnPlayerWin → PlayerWins={PlayerWins} ClothingLevel={ClothingLevel} Round={Round}");
    }

    public static void OnRoundEnd(RoundResult result)
    {
        LastRoundResult = result;
        Round++;
        Debug.Log($"[GameState] OnRoundEnd → result={result} Round={Round} PlayerWins={PlayerWins}");
    }

    public static bool IsGameOver => Round > MaxRounds;

    public static bool IsPlayerWon
    {
        get
        {
            int count = CurrentRemoteGirl?.photos?.Count ?? 0;
            bool won  = count > 0 && ClothingLevel >= count;
            if (won) Debug.Log($"[GameState] IsPlayerWon=true (ClothingLevel={ClothingLevel} >= photos={count})");
            return won;
        }
    }
}
