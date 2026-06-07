using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tools → Game Field Builder
/// Создаёт объекты внутри Panel_Game с позициями по ТЗ.
/// Расчёт под портретный Canvas 1080x1920.
/// </summary>
public class GameFieldBuilder : EditorWindow
{
    RectTransform _panel;

    [MenuItem("Tools/Game Field Builder")]
    public static void Open() => GetWindow<GameFieldBuilder>("Game Field Builder");

    void OnGUI()
    {
        GUILayout.Label("Game Field Builder", EditorStyles.boldLabel);
        GUILayout.Space(8);

        _panel = (RectTransform)EditorGUILayout.ObjectField(
            "Panel_Game", _panel, typeof(RectTransform), true);

        GUILayout.Space(8);
        GUI.enabled = _panel != null;

        if (GUILayout.Button("Создать всё", GUILayout.Height(36)))
        {
            var t = _panel;
            EditorApplication.delayCall += () => Build(t);
        }

        GUI.enabled = true;
    }

    void Build(RectTransform p)
    {
        if (p == null) return;
        Undo.RegisterFullObjectHierarchyUndo(p.gameObject, "Build Game Field");

        // ── Счётчик раздач — левый верхний угол ──────────────
        var roundCounter = MakeText(p, "TxtRoundCounter", "1 / 14");
        Place(roundCounter,
            anchorMin: new Vector2(0, 1), anchorMax: new Vector2(0, 1),
            pivot:     new Vector2(0, 1),
            pos:       new Vector2(20, -20),
            size:      new Vector2(150, 50));

        // ── Фото девушки — сверху по центру ──────────────────
        var girlImg = MakeImage(p, "GirlImage");
        Place(girlImg,
            anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1),
            pivot:     new Vector2(0.5f, 1),
            pos:       new Vector2(0, -20),
            size:      new Vector2(320, 320));

        // ── Очки дилера — слева от карт дилера ───────────────
        var dealerScore = MakeText(p, "TxtDealerScore", "0");
        Place(dealerScore,
            anchorMin: new Vector2(0, 0.62f), anchorMax: new Vector2(0, 0.62f),
            pivot:     new Vector2(0, 0.5f),
            pos:       new Vector2(20, 0),
            size:      new Vector2(80, 50));

        // ── Карты дилера — по центру, верхняя половина ───────
        var dealerCards = MakeCardsContainer(p, "DealerCardsContainer");
        Place(dealerCards,
            anchorMin: new Vector2(0.5f, 0.62f), anchorMax: new Vector2(0.5f, 0.62f),
            pivot:     new Vector2(0.5f, 0.5f),
            pos:       new Vector2(60, 0),
            size:      new Vector2(300, 130));

        // ── Скрытая карта дилера — поверх карт ───────────────
        var hiddenCard = MakeImage(p, "DealerHiddenCard");
        Place(hiddenCard,
            anchorMin: new Vector2(0.5f, 0.62f), anchorMax: new Vector2(0.5f, 0.62f),
            pivot:     new Vector2(0.5f, 0.5f),
            pos:       new Vector2(150, 0),
            size:      new Vector2(80, 120));

        // ── Карты игрока — по центру, нижняя половина ────────
        var playerCards = MakeCardsContainer(p, "PlayerCardsContainer");
        Place(playerCards,
            anchorMin: new Vector2(0.5f, 0.38f), anchorMax: new Vector2(0.5f, 0.38f),
            pivot:     new Vector2(0.5f, 0.5f),
            pos:       new Vector2(60, 0),
            size:      new Vector2(300, 130));

        // ── Очки игрока — слева от карт игрока ───────────────
        var playerScore = MakeText(p, "TxtPlayerScore", "0");
        Place(playerScore,
            anchorMin: new Vector2(0, 0.38f), anchorMax: new Vector2(0, 0.38f),
            pivot:     new Vector2(0, 0.5f),
            pos:       new Vector2(20, 0),
            size:      new Vector2(80, 50));

        // ── Кнопки — снизу по центру ─────────────────────────
        var btnHit = MakeButton(p, "BtnHit", "Ещё");
        Place(btnHit,
            anchorMin: new Vector2(0.5f, 0), anchorMax: new Vector2(0.5f, 0),
            pivot:     new Vector2(1f, 0),
            pos:       new Vector2(-20, 40),
            size:      new Vector2(200, 70));

        var btnStand = MakeButton(p, "BtnStand", "Остаться");
        Place(btnStand,
            anchorMin: new Vector2(0.5f, 0), anchorMax: new Vector2(0.5f, 0),
            pivot:     new Vector2(0f, 0),
            pos:       new Vector2(20, 40),
            size:      new Vector2(200, 70));

        Debug.Log("✅ Panel_Game создан с позициями");
        EditorUtility.DisplayDialog("Готово", "Готово!\nПривяжи поля в GameController.", "ОК");
    }

    // ── Позиционирование ──────────────────────────────────────

    void Place(RectTransform r, Vector2 anchorMin, Vector2 anchorMax,
               Vector2 pivot, Vector2 pos, Vector2 size)
    {
        r.anchorMin        = anchorMin;
        r.anchorMax        = anchorMax;
        r.pivot            = pivot;
        r.anchoredPosition = pos;
        r.sizeDelta        = size;
    }

    // ── Создание объектов ─────────────────────────────────────

    RectTransform MakeCardsContainer(RectTransform parent, string name)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var hlg  = go.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing               = 8;
        hlg.childForceExpandWidth  = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment         = TextAnchor.MiddleCenter;
        go.AddComponent<ContentSizeFitter>().horizontalFit =
            ContentSizeFitter.FitMode.PreferredSize;
        return rect;
    }

    RectTransform MakeText(RectTransform parent, string name, string text)
    {
        var go  = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect  = go.AddComponent<RectTransform>();
        var tmp   = go.AddComponent<TextMeshProUGUI>();
        tmp.text  = text;
        tmp.color = Color.black;
        return rect;
    }

    RectTransform MakeImage(RectTransform parent, string name)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        go.AddComponent<Image>();
        return rect;
    }

    RectTransform MakeButton(RectTransform parent, string name, string label)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        go.AddComponent<Image>();
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        var tr         = textGO.AddComponent<RectTransform>();
        tr.anchorMin   = Vector2.zero;
        tr.anchorMax   = Vector2.one;
        tr.offsetMin   = tr.offsetMax = Vector2.zero;
        var tmp        = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text       = label;
        tmp.color      = Color.black;
        tmp.alignment  = TextAlignmentOptions.Center;
        return rect;
    }
}
