using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Сохранение / загрузка конфига колоды.
/// Файл: persistentDataPath/deck_config.json
/// </summary>
public static class DeckRepository
{
    static string FilePath => DataPaths.DeckConfig;

    // Масти и ранги для 36-карточной колоды
    public static readonly string[] Suits36 = { "clubs", "diamonds", "hearts", "spades" };
    public static readonly string[] Ranks36 = { "A", "6", "7", "8", "9", "10", "J", "Q", "K" };

    // ── Загрузить ─────────────────────────────────────────────

    public static DeckConfig Load()
    {
        if (!File.Exists(FilePath))
            return CreateDefault();

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<DeckConfig>(json) ?? CreateDefault();
        }
        catch
        {
            return CreateDefault();
        }
    }

    // ── Сохранить ─────────────────────────────────────────────

    public static void Save(DeckConfig config)
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(config, prettyPrint: true));
    }

    // ── Создать дефолтный конфиг (36 карт, авто-имена) ───────

    public static DeckConfig CreateDefault()
    {
        var config = new DeckConfig();
        AutoFill(config, Suits36, Ranks36);
        return config;
    }

    /// <summary>
    /// Заполняет список карт по именам файлов вида card_RANK_SUIT.
    /// Существующие записи перезаписываются.
    /// </summary>
    public static void AutoFill(DeckConfig config, string[] suits, string[] ranks)
    {
        config.cards.Clear();
        foreach (var suit in suits)
            foreach (var rank in ranks)
                config.cards.Add(new CardEntry(rank, suit));
    }

    // ── Загрузить спрайт карты из Resources ───────────────────

    public static UnityEngine.Sprite LoadSprite(DeckConfig config, string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        return Resources.Load<UnityEngine.Sprite>($"{config.spritesFolder}/{spriteName}");
    }

    public static UnityEngine.Sprite LoadCardBack(DeckConfig config)
        => LoadSprite(config, config.cardBackSprite);
}
