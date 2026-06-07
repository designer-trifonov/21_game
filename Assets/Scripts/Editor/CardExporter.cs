using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// Tools → Card Exporter
/// Копирует карты из Assets/Data/image-parts/ в Data/Image/ как 1.png...36.png
/// Формат исходников: card_СТРОКА-КОЛОНКА_*.png
/// Строки 1-4 = Трефы, Бубны, Червы, Пики
/// Колонки 1-9 = A, 6, 7, 8, 9, 10, J, Q, K
/// </summary>
public class CardExporter : EditorWindow
{
    [MenuItem("Tools/Card Exporter")]
    public static void Open() => GetWindow<CardExporter>("Card Exporter");

    void OnGUI()
    {
        GUILayout.Label("Card Exporter", EditorStyles.boldLabel);
        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Копирует Assets/Data/image-parts/card_РЯД-КОЛ_*.png\n" +
            "→ Data/Image/1.png ... 36.png\n\n" +
            "Строки: 1=Трефы 2=Бубны 3=Червы 4=Пики\n" +
            "Колонки: 1=A 2=6 3=7 4=8 5=9 6=10 7=J 8=Q 9=K",
            MessageType.Info);
        GUILayout.Space(8);

        if (GUILayout.Button("Экспортировать карты", GUILayout.Height(36)))
            Export();
    }

    void Export()
    {
        string src  = Path.Combine(Application.dataPath, "Data", "image-parts");
        string dest = Path.Combine(Application.persistentDataPath, "Data", "Image");

        if (!Directory.Exists(src))  { EditorUtility.DisplayDialog("Ошибка", $"Папка не найдена:\n{src}", "ОК"); return; }
        if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

        var files = Directory.GetFiles(src, "card_*-*.png")
            .OrderBy(f =>
            {
                // Сортируем по строка*10 + колонка
                var name = Path.GetFileNameWithoutExtension(f); // card_1-2_144x208
                var parts = name.Split('_')[1].Split('-');       // ["1","2"]
                int row = int.Parse(parts[0]);
                int col = int.Parse(parts[1]);
                return row * 10 + col;
            })
            .ToArray();

        if (files.Length == 0) { EditorUtility.DisplayDialog("Ошибка", "Карты не найдены в папке", "ОК"); return; }

        for (int i = 0; i < files.Length; i++)
        {
            string destFile = Path.Combine(dest, $"{i + 1}.png");
            File.Copy(files[i], destFile, overwrite: true);
        }

        Debug.Log($"✅ Экспортировано {files.Length} карт → {dest}");
        EditorUtility.DisplayDialog("Готово",
            $"Скопировано {files.Length} карт\n→ {dest}\n\nФайлы: 1.png ... {files.Length}.png", "ОК");
    }
}
