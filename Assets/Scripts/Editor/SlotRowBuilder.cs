using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Меню: Tools → Slot Row Builder
/// Кидаешь контейнер — создаёт SlotRow внутри.
/// </summary>
public class SlotRowBuilder : EditorWindow
{
    RectTransform _container;

    [MenuItem("Tools/Slot Row Builder")]
    public static void Open() => GetWindow<SlotRowBuilder>("Slot Row Builder");

    void OnGUI()
    {
        GUILayout.Label("Slot Row Builder", EditorStyles.boldLabel);
        GUILayout.Space(8);

        _container = (RectTransform)EditorGUILayout.ObjectField(
            "Контейнер", _container, typeof(RectTransform), true);

        GUILayout.Space(8);
        GUI.enabled = _container != null;

        if (GUILayout.Button("Создать SlotRow", GUILayout.Height(36)))
        {
            var target = _container;
            EditorApplication.delayCall += () => Build(target);
        }

        GUI.enabled = true;
    }

    void Build(RectTransform parent)
    {
        if (parent == null) return;

        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build SlotRow");

        var root = new GameObject("SlotRow");
        GameObjectUtility.SetParentAndAlign(root, parent.gameObject);

        var rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(0, 64);

        var le = root.AddComponent<LayoutElement>();
        le.preferredHeight = 64;

        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing            = 8;
        hlg.padding            = new RectOffset(8, 8, 6, 6);
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.childAlignment     = TextAnchor.MiddleLeft;

        // 1. Номер слота
        AddText(root.transform,       "SlotIndexText", "Слот 0", fontSize: 18, width: 130);

        // 2. Название
        AddInputField(root.transform, "LabelInput",    "Название...",           width: 170);

        // 3. Превью фото
        AddImage(root.transform,      "PhotoPreview",                   w: 44, h: 52);

        // 4. Кнопки
        AddButton(root.transform, "BtnPhoto", "Фото",  width: 80);
        AddButton(root.transform, "BtnVideo", "Видео", width: 80);
        AddButton(root.transform, "BtnAudio", "Аудио", width: 80);

        root.AddComponent<SlotRowUI>();

        Selection.activeGameObject = root;
        Debug.Log("✅ SlotRow создан");
    }

    // ── Хелперы ───────────────────────────────────────────────

    void AddText(Transform parent, string name, string text, int fontSize, float width)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
    }

    void AddInputField(Transform parent, string name, string placeholder, float width)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);
        var field = go.AddComponent<TMP_InputField>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        Stretch(textGO.AddComponent<RectTransform>(), 4);
        var textTMP = textGO.AddComponent<TextMeshProUGUI>();
        textTMP.color    = Color.white;
        textTMP.fontSize = 18;

        var phGO = new GameObject("Placeholder");
        GameObjectUtility.SetParentAndAlign(phGO, go);
        Stretch(phGO.AddComponent<RectTransform>(), 4);
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text      = placeholder;
        phTMP.color     = new Color(0.5f, 0.5f, 0.5f);
        phTMP.fontSize  = 18;
        phTMP.fontStyle = FontStyles.Italic;

        field.textComponent = textTMP;
        field.placeholder   = phTMP;
    }

    void AddImage(Transform parent, string name, float w, float h)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth  = w;
        le.preferredHeight = h;
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
    }

    void AddButton(Transform parent, string name, string label, float width)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.2f, 0.45f, 0.7f);
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        Stretch(textGO.AddComponent<RectTransform>());
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 18;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    void Stretch(RectTransform rect, float offset = 0)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2( offset,  offset);
        rect.offsetMax = new Vector2(-offset, -offset);
    }
}
