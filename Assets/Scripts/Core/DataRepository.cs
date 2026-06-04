using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Сохранение и загрузка всех данных через JSON в persistentDataPath.
/// </summary>
public static class DataRepository
{
    static string GirlsFolder => DataPaths.Girls;

    // ── Девушки ──────────────────────────────────────────────

    public static List<GirlData> LoadAllGirls()
    {
        var list = new List<GirlData>();
        if (!Directory.Exists(GirlsFolder)) return list;

        foreach (var file in Directory.GetFiles(GirlsFolder, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);
                var girl = JsonUtility.FromJson<GirlData>(json);
                if (girl != null) list.Add(girl);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Ошибка загрузки {file}: {e.Message}");
            }
        }
        return list;
    }

    public static void SaveGirl(GirlData girl)
    {
        if (!Directory.Exists(GirlsFolder))
            Directory.CreateDirectory(GirlsFolder);

        if (string.IsNullOrEmpty(girl.id))
            girl.id = Guid.NewGuid().ToString("N");

        string path = Path.Combine(GirlsFolder, girl.id + ".json");
        File.WriteAllText(path, JsonUtility.ToJson(girl, prettyPrint: true));
    }

    public static void DeleteGirl(string id)
    {
        string path = Path.Combine(GirlsFolder, id + ".json");
        if (File.Exists(path)) File.Delete(path);
    }

    // ── Медиа: путь к файлу девушки ──────────────────────────

    public static string GetMediaFolder(string girlId)
    {
        string folder = Path.Combine(Application.persistentDataPath, "media", girlId);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        return folder;
    }
}
