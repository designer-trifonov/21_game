using System.Collections.Generic;

/// <summary>
/// Модель данных одной девушки.
/// </summary>
[System.Serializable]
public class GirlData
{
    public string id           = "";
    public string name         = "Аноним";
    public int    age          = 21;
    public string description  = "";
    public string profilePhoto = "";
    public string introVideo   = "";
    public string introAudio   = "";
    public int    clothingCount = 0;

    // слотов будет clothingCount + 1 (0 = одета, 1..N = стадии раздевания)
    public List<ClothingSlot> slots = new List<ClothingSlot>();

    /// <summary>
    /// Пересоздаёт список слотов под текущий clothingCount.
    /// Существующие данные сохраняются по индексу.
    /// </summary>
    public void RebuildSlots()
    {
        int needed = clothingCount;

        // добавляем недостающие
        while (slots.Count < needed)
        {
            int i = slots.Count;
            slots.Add(new ClothingSlot
            {
                index = i,
                label = i == 0 ? "Одета" : $"Слот {i}"
            });
        }

        // обрезаем лишние
        if (slots.Count > needed)
            slots.RemoveRange(needed, slots.Count - needed);
    }
}

[System.Serializable]
public class ClothingSlot
{
    public int    index     = 0;
    public string label     = "";
    public string photoPath = "";
    public string videoPath = "";
    public string audioPath = "";
}
