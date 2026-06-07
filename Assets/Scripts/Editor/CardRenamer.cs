using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Tools → Card Renamer
/// Видишь все картинки → назначаешь каждой имя карты → копирует в Resources/Cards/
/// </summary>
public class CardRenamer : EditorWindow
{
    // Целевые имена в стандартном порядке
    static readonly string[] TargetNames =
    {
        "card_back",
        "card_A_clubs",   "card_6_clubs",   "card_7_clubs",   "card_8_clubs",
        "card_9_clubs",   "card_10_clubs",  "card_J_clubs",   "card_Q_clubs",  "card_K_clubs",
        "card_A_diamonds","card_6_diamonds","card_7_diamonds","card_8_diamonds",
        "card_9_diamonds","card_10_diamonds","card_J_diamonds","card_Q_diamonds","card_K_diamonds",
        "card_A_hearts",  "card_6_hearts",  "card_7_hearts",  "card_8_hearts",
        "card_9_hearts",  "card_10_hearts", "card_J_hearts",  "card_Q_hearts",  "card_K_hearts",
        "card_A_spades",  "card_6_spades",  "card_7_spades",  "card_8_spades",
        "card_9_spades",  "card_10_spades", "card_J_spades",  "card_Q_spades",  "card_K_spades",
    };

    string       _sourceFolder = "Assets";
    List<string> _files        = new();
    Vector2      _scroll;
    string       _output       = "Assets/Resources/Cards";

    [MenuItem("Tools/Card Renamer")]
    public static void Open() => GetWindow<CardRenamer>("Card Renamer");

    void OnGUI()
    {
        GUILayout.Label("Card Renamer", EditorStyles.boldLabel);
        GUILayout.Space(6);

        // Папка с исходниками
        GUILayout.BeginHorizontal();
        _sourceFolder = EditorGUILayout.TextField("Папка с картами", _sourceFolder);
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            var path = EditorUtility.OpenFolderPanel("Выбери папку", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                _sourceFolder = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
                LoadFiles();
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Загрузить файлы из папки"))
            LoadFiles();

        GUILayout.Space(6);
        EditorGUILayout.HelpBox(
            $"Файлов найдено: {_files.Count} | Слотов: {TargetNames.Length}\n" +
            "Файлы сопоставляются по порядку слева направо, сверху вниз.",
            MessageType.Info);
        GUILayout.Space(6);

        if (_files.Count == 0) return;

        // Сетка: файл → имя карты
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        int cols = 3;
        for (int i = 0; i < Mathf.Max(_files.Count, TargetNames.Length); i++)
        {
            if (i % cols == 0) GUILayout.BeginHorizontal();

            GUILayout.BeginVertical("box", GUILayout.Width(position.width / cols - 12));

            // Превью
            if (i < _files.Count)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(_files[i]);
                if (tex != null)
                    GUILayout.Label(tex, GUILayout.Width(80), GUILayout.Height(100));
                GUILayout.Label(Path.GetFileName(_files[i]), EditorStyles.miniLabel);
            }
            else
            {
                GUILayout.Label("—", GUILayout.Height(100));
            }

            // Имя цели
            GUI.color = i < TargetNames.Length ? Color.green : Color.red;
            GUILayout.Label(i < TargetNames.Length ? TargetNames[i] : "лишний файл", EditorStyles.boldLabel);
            GUI.color = Color.white;

            GUILayout.EndVertical();

            if (i % cols == cols - 1 || i == Mathf.Max(_files.Count, TargetNames.Length) - 1)
                GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(8);
        _output = EditorGUILayout.TextField("Куда копировать", _output);

        GUI.enabled = _files.Count > 0;
        if (GUILayout.Button("✅ Переименовать и скопировать в Resources/Cards", GUILayout.Height(36)))
            CopyRenamed();
        GUI.enabled = true;
    }

    void LoadFiles()
    {
        _files.Clear();
        if (!Directory.Exists(_sourceFolder)) return;

        var extensions = new[] { ".png", ".jpg", ".jpeg" };
        _files = Directory.GetFiles(_sourceFolder)
            .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
            .OrderBy(f => f)
            .Select(f => f.Replace("\\", "/"))
            .ToList();

        Repaint();
    }

    void CopyRenamed()
    {
        if (!Directory.Exists(_output))
            Directory.CreateDirectory(_output);

        int count = Mathf.Min(_files.Count, TargetNames.Length);

        for (int i = 0; i < count; i++)
        {
            string ext  = Path.GetExtension(_files[i]);
            string dest = Path.Combine(_output, TargetNames[i] + ".png").Replace("\\", "/");
            File.Copy(_files[i], dest, overwrite: true);
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ Скопировано {count} карт в {_output}");
        EditorUtility.DisplayDialog("Готово", $"Скопировано {count} карт в\n{_output}", "ОК");
    }
}
