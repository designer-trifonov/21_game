using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Редакторный инструмент нарезки спрайтшита карт.
/// Меню: Tools → Slice Card Sheet
/// </summary>
public class SliceCards : EditorWindow
{
    // ── Настройки сетки ──────────────────────────────────────
    // Сетка 4 строки × 9 столбцов = 36 карт
    // Строка 0: ♣  Строка 1: ♦  Строка 2: ♥  Строка 3: ♠
    // Столбцы:  A  6  7  8  9  10  J  Q  K

    static readonly string[] RANKS  = { "A", "6", "7", "8", "9", "10", "J", "Q", "K" };
    static readonly string[] SUITS  = { "clubs", "diamonds", "hearts", "spades" };
    static readonly string   BACK   = "card_back"; // позиция (0,0) если это рубашка

    const int COLS = 9;
    const int ROWS = 4;

    string _sheetPath = "Assets/Claude/cards_sheet.jpg";
    string _outputDir = "Assets/Sprites/Cards";
    bool   _firstIsBack = true;  // первая карта (A♣ позиция 0,0) — рубашка?

    // ─────────────────────────────────────────────────────────

    [MenuItem("Tools/Slice Card Sheet")]
    public static void Open() => GetWindow<SliceCards>("Нарезка карт");

    void OnGUI()
    {
        GUILayout.Label("Нарезка спрайтшита карт", EditorStyles.boldLabel);
        GUILayout.Space(8);

        _sheetPath    = EditorGUILayout.TextField("Путь к спрайтшиту", _sheetPath);
        _outputDir    = EditorGUILayout.TextField("Папка вывода",       _outputDir);
        _firstIsBack  = EditorGUILayout.Toggle("Позиция (0,0) — рубашка", _firstIsBack);

        GUILayout.Space(12);

        if (GUILayout.Button("Нарезать!", GUILayout.Height(36)))
            Slice();
    }

    void Slice()
    {
        // Загружаем текстуру
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(_sheetPath);
        if (tex == null) { Debug.LogError($"Текстура не найдена: {_sheetPath}"); return; }

        // Делаем текстуру читаемой
        string fullPath = Path.Combine(Application.dataPath, "..", _sheetPath);
        var importer = (TextureImporter)AssetImporter.GetAtPath(_sheetPath);
        importer.isReadable = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        tex = AssetDatabase.LoadAssetAtPath<Texture2D>(_sheetPath);

        int cardW = tex.width  / COLS;
        int cardH = tex.height / ROWS;

        if (!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);

        int saved = 0;

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                // Первая позиция — рубашка?
                string cardName;
                if (row == 0 && col == 0 && _firstIsBack)
                    cardName = BACK;
                else
                    cardName = $"card_{RANKS[col]}_{SUITS[row]}";

                // Вырезаем пиксели (Unity: Y снизу вверх)
                int x = col  * cardW;
                int y = tex.height - (row + 1) * cardH;

                var pixels = tex.GetPixels(x, y, cardW, cardH);
                var card   = new Texture2D(cardW, cardH);
                card.SetPixels(pixels);
                card.Apply();

                byte[] png  = card.EncodeToPNG();
                string path = Path.Combine(_outputDir, cardName + ".png");
                File.WriteAllBytes(path, png);
                saved++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Нарезано {saved} карт → {_outputDir}");
        EditorUtility.DisplayDialog("Готово!", $"Нарезано {saved} карт.\nПапка: {_outputDir}", "ОК");
    }
}
