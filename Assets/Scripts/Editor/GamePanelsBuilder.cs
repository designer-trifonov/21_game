using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Tools → Game Panels Builder
/// Кидаешь Canvas — создаёт все панели внутри одной кнопкой.
/// </summary>
public class GamePanelsBuilder : EditorWindow
{
    RectTransform _canvas;

    [MenuItem("Tools/Game Panels Builder")]
    public static void Open() => GetWindow<GamePanelsBuilder>("Game Panels Builder");

    void OnGUI()
    {
        GUILayout.Label("Game Panels Builder", EditorStyles.boldLabel);
        GUILayout.Space(8);

        _canvas = (RectTransform)EditorGUILayout.ObjectField(
            "Canvas", _canvas, typeof(RectTransform), true);

        GUILayout.Space(8);
        GUI.enabled = _canvas != null;

        if (GUILayout.Button("Создать все панели", GUILayout.Height(40)))
        {
            var target = _canvas;
            EditorApplication.delayCall += () => BuildAll(target);
        }

        GUI.enabled = true;
    }

    void BuildAll(RectTransform canvas)
    {
        if (canvas == null) return;
        Undo.RegisterFullObjectHierarchyUndo(canvas.gameObject, "Build All Game Panels");

        BuildDifficulty (MakePanel(canvas, "Panel_Difficulty"));
        BuildIntro      (MakePanel(canvas, "Panel_Intro"));
        BuildGame       (MakePanel(canvas, "Panel_Game"));
        BuildRoundResult(MakePanel(canvas, "Panel_RoundResult"));
        BuildGameEnd    (MakePanel(canvas, "Panel_GameEnd"));

        Debug.Log("✅ Все панели созданы");
        EditorUtility.DisplayDialog("Готово",
            "Панели созданы:\n• Panel_Difficulty\n• Panel_Intro\n• Panel_Game\n• Panel_RoundResult\n• Panel_GameEnd\n\nПривяжи их в UIManager.", "ОК");
    }

    // ── Создать панель-контейнер ──────────────────────────────

    RectTransform MakePanel(RectTransform parent, string name)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        // Растягиваем на весь Canvas
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        go.AddComponent<Image>().color = new Color(0, 0, 0, 0); // прозрачный фон
        go.SetActive(false); // все панели выключены по умолчанию
        return rect;
    }

    // ══════════════════════════════════════════════════════════

    void BuildDifficulty(RectTransform p)
    {
        MakeButton(p, "BtnEasy",   "Легко");
        MakeButton(p, "BtnNormal", "Нормально");
        MakeButton(p, "BtnHard",   "Сложно");
    }

    void BuildIntro(RectTransform p)
    {
        var photo = MakeEmpty(p, "PhotoContainer");
        MakeImage(photo, "PhotoImage");
        MakeText (photo, "TxtDescription", "Описание");

        var video = MakeEmpty(p, "VideoContainer");
        var rawGO = new GameObject("VideoDisplay");
        GameObjectUtility.SetParentAndAlign(rawGO, video.gameObject);
        rawGO.AddComponent<RectTransform>();
        rawGO.AddComponent<RawImage>();

        p.gameObject.AddComponent<VideoPlayer>();

        MakeButton(p, "BtnStart", "Начать");
    }

    void BuildGame(RectTransform p)
    {
        MakeText (p, "TxtDealerScore",      "0");
        MakeEmpty(p, "DealerCardsContainer");
        MakeEmpty(p, "DealerHiddenCard").gameObject.AddComponent<Image>();
        MakeEmpty(p, "PlayerCardsContainer");
        MakeText (p, "TxtPlayerScore",      "0");
        MakeButton(p, "BtnHit",   "Ещё");
        MakeButton(p, "BtnStand", "Остаться");
    }

    void BuildRoundResult(RectTransform p)
    {
        MakeText  (p, "TxtResult",   "Победа!");
        MakeText  (p, "TxtRound",    "Раунд 1 из 14");
        MakeText  (p, "TxtWins",     "Побед: 0");
        MakeButton(p, "BtnContinue", "Продолжить");
    }

    void BuildGameEnd(RectTransform p)
    {
        MakeText  (p, "TxtTitle",     "Игра окончена");
        MakeText  (p, "TxtSummary",   "Итог");
        MakeButton(p, "BtnPlayAgain", "Играть снова");
        MakeButton(p, "BtnMainMenu",  "Главное меню");
    }

    // ══════════════════════════════════════════════════════════
    // Хелперы
    // ══════════════════════════════════════════════════════════

    RectTransform MakeEmpty(RectTransform parent, string name)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        return go.AddComponent<RectTransform>();
    }

    void MakeText(RectTransform parent, string name, string text)
    {
        var go  = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        go.AddComponent<RectTransform>();
        var tmp   = go.AddComponent<TextMeshProUGUI>();
        tmp.text  = text;
        tmp.color = Color.black;
    }

    void MakeImage(RectTransform parent, string name)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>();
    }

    void MakeButton(RectTransform parent, string label, string text)
    {
        var go = new GameObject(label);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        go.AddComponent<RectTransform>().sizeDelta = new Vector2(160, 50);
        go.AddComponent<Image>();
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        var rect       = textGO.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var tmp        = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text       = text;
        tmp.color      = Color.black;
        tmp.alignment  = TextAlignmentOptions.Center;
    }
}
