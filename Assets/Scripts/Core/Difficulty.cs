public enum Difficulty { Easy, Normal, Hard }

[System.Serializable]
public class DifficultySettings
{
    public int   StandThreshold; // дилер останавливается при >= этого значения
    public float MistakeChance;  // вероятность [0..1] что дилер остановится раньше порога

    public DifficultySettings(int standThreshold, float mistakeChance)
    {
        StandThreshold = standThreshold;
        MistakeChance  = mistakeChance;
    }
}
