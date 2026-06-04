using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Меню: Tools → Admin UI Builder
/// Кидаешь контейнер — создаёт все элементы внутри.
/// </summary>
public class AdminUIBuilder : EditorWindow
{
    RectTransform _rightPanelContainer;
    RectTransform _deckPanelContainer;
    RectTransform _girlCardContainer;
    RectTransform _slotRowContainer;
    RectTransform _cardEntryRowContainer;

    Vector2 _scroll;

    [MenuItem("Tools/Admin UI Builder")]
    public static void Open() => GetWindow<AdminUIBuilder>("Admin UI Builder");

    void OnGUI()
    {
        // try/finally гарантирует что EndScrollView всегда вызовется
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        try
        {
            DrawContent();
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }
    }

    void DrawContent()
    {
        GUILayout.Label("Admin UI Builder", EditorStyles.boldLabel);
        GUILayout.Space(4);
        EditorGUILayout.HelpBox("Кидай контейнер → жми кнопку → всё создастся внутри.", MessageType.Info);
        GUILayout.Space(10);

        // ── RIGHT PANEL (редактор девушки) ──────────────────
        GUILayout.Label("► RightPanel (редактор девушки)", EditorStyles.boldLabel);
        _rightPanelContainer = (RectTransform)EditorGUILayout.ObjectField(
            "Контейнер (Content)", _rightPanelContainer, typeof(RectTransform), true);

        GUI.enabled = _rightPanelContainer != null;
        if (GUILayout.Button("Создать поля редактора девушки", GUILayout.Height(30)))
        {
            var target = _rightPanelContainer;
            // delayCall — выполняется после OnGUI, безопасно менять иерархию
            EditorApplication.delayCall += () => BuildRightPanel(target);
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        // ── DECK PANEL ───────────────────────────────────────
        GUILayout.Label("► Panel_AdminDeck", EditorStyles.boldLabel);
        _deckPanelContainer = (RectTransform)EditorGUILayout.ObjectField(
            "Контейнер", _deckPanelContainer, typeof(RectTransform), true);

        GUI.enabled = _deckPanelContainer != null;
        if (GUILayout.Button("Создать панель колоды", GUILayout.Height(30)))
        {
            var target = _deckPanelContainer;
            EditorApplication.delayCall += () => BuildDeckPanel(target);
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        // ── ПРЕФАБЫ ──────────────────────────────────────────
        GUILayout.Label("► Префабы", EditorStyles.boldLabel);

        _girlCardContainer = (RectTransform)EditorGUILayout.ObjectField(
            "GirlCard контейнер", _girlCardContainer, typeof(RectTransform), true);
        GUI.enabled = _girlCardContainer != null;
        if (GUILayout.Button("Создать GirlCard", GUILayout.Height(28)))
        {
            var target = _girlCardContainer;
            EditorApplication.delayCall += () => BuildGirlCard(target);
        }
        GUI.enabled = true;

        GUILayout.Space(4);

        _slotRowContainer = (RectTransform)EditorGUILayout.ObjectField(
            "SlotRow контейнер", _slotRowContainer, typeof(RectTransform), true);
        GUI.enabled = _slotRowContainer != null;
        if (GUILayout.Button("Создать SlotRow", GUILayout.Height(28)))
        {
            var target = _slotRowContainer;
            EditorApplication.delayCall += () => BuildSlotRow(target);
        }
        GUI.enabled = true;

        GUILayout.Space(4);

        _cardEntryRowContainer = (RectTransform)EditorGUILayout.ObjectField(
            "CardEntryRow контейнер", _cardEntryRowContainer, typeof(RectTransform), true);
        GUI.enabled = _cardEntryRowContainer != null;
        if (GUILayout.Button("Создать CardEntryRow", GUILayout.Height(28)))
        {
            var target = _cardEntryRowContainer;
            EditorApplication.delayCall += () => BuildCardEntryRow(target);
        }
        GUI.enabled = true;
    }

    // ══════════════════════════════════════════════════════════
    // RIGHT PANEL — поля редактора девушки
    // ══════════════════════════════════════════════════════════

    void BuildRightPanel(RectTransform parent)
    {
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build Right Panel");

        var vlg = EnsureVerticalLayout(parent, spacing: 10, padding: 16);

        MakeLabel     (parent, "── ОСНОВНОЕ ──");
        MakeInputField(parent, "InputName",          "Имя");
        MakeInputField(parent, "InputAge",           "Возраст");
        MakeInputField(parent, "InputDescription",   "Описание", multiline: true);

        MakeLabel (parent, "── МЕДИА ──");
        MakeImage (parent, "ProfilePreview",    200, 280);
        MakeButton(parent, "BtnProfilePhoto",   "📷  Фото профиля");
        MakeButton(parent, "BtnIntroVideo",     "🎬  Видео вступления");
        MakeButton(parent, "BtnIntroAudio",     "🔊  Аудио вступления");

        MakeLabel (parent, "── ОДЕЖДА ──");
        MakeInputField(parent, "InputClothingCount", "Кол-во одежды (слоты обновятся автоматически)");

        MakeLabel(parent, "── СЛОТЫ ──");
        // SlotsContent — пустой контейнер куда будут добавляться SlotRow
        var slotsGO = new GameObject("SlotsContent");
        GameObjectUtility.SetParentAndAlign(slotsGO, parent.gameObject);
        var slotsRect = slotsGO.AddComponent<RectTransform>();
        SetSize(slotsRect, 0, 0);
        var slotVLG = slotsGO.AddComponent<VerticalLayoutGroup>();
        slotVLG.spacing = 6;
        slotVLG.childForceExpandWidth  = true;
        slotVLG.childForceExpandHeight = false;
        slotsGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        MakeSpace(parent, 10);
        MakeButton(parent, "BtnSave",   "💾  Сохранить",  new Color(0.2f, 0.7f, 0.3f));
        MakeButton(parent, "BtnDelete", "🗑  Удалить",    new Color(0.8f, 0.2f, 0.2f));

        Debug.Log("✅ RightPanel создан");
        EditorUtility.DisplayDialog("Готово", "RightPanel создан!\nПривяжи поля в AdminController.", "ОК");
    }

    // ══════════════════════════════════════════════════════════
    // DECK PANEL
    // ══════════════════════════════════════════════════════════

    void BuildDeckPanel(RectTransform parent)
    {
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build Deck Panel");

        EnsureVerticalLayout(parent, spacing: 10, padding: 16);

        MakeLabel (parent, "── РУБАШКА ──");
        MakeImage (parent, "BackPreview", 120, 180);
        MakeLabel (parent, "BackNameText");
        MakeButton(parent, "BtnPickBack", "🂠  Выбрать рубашку");

        MakeLabel (parent, "── СПРАЙТЫ ──");
        MakeInputField(parent, "InputFolder", "Папка в Resources (Cards)");
        MakeButton(parent, "BtnAutoFill", "⚡  Авто-заполнить по именам файлов");

        MakeLabel(parent, "── КАРТЫ ──");
        var cardsGO   = new GameObject("CardsContent");
        GameObjectUtility.SetParentAndAlign(cardsGO, parent.gameObject);
        var cardsRect = cardsGO.AddComponent<RectTransform>();
        SetSize(cardsRect, 0, 0);
        var cVLG = cardsGO.AddComponent<VerticalLayoutGroup>();
        cVLG.spacing = 4;
        cVLG.childForceExpandWidth  = true;
        cVLG.childForceExpandHeight = false;
        cardsGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        MakeSpace(parent, 10);
        MakeButton(parent, "BtnSave", "💾  Сохранить",  new Color(0.2f, 0.7f, 0.3f));
        MakeButton(parent, "BtnBack", "←  Назад");

        Debug.Log("✅ DeckPanel создан");
        EditorUtility.DisplayDialog("Готово", "DeckPanel создан!\nПривяжи поля в AdminDeckController.", "ОК");
    }

    // ══════════════════════════════════════════════════════════
    // ПРЕФАБ: GirlCard
    // ══════════════════════════════════════════════════════════

    void BuildGirlCard(RectTransform parent)
    {
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build GirlCard");

        // Корень карточки — кнопка
        var root = MakeButton(parent, "SelectButton", "");
        SetSize(root.GetComponent<RectTransform>(), 320, 180);

        // Фото на весь фон
        var photo = new GameObject("PhotoImage");
        GameObjectUtility.SetParentAndAlign(photo, root.gameObject);
        var photoRect = photo.AddComponent<RectTransform>();
        Stretch(photoRect);
        photo.AddComponent<Image>().preserveAspect = true;

        // Имя снизу
        var nameGO = new GameObject("NameText");
        GameObjectUtility.SetParentAndAlign(nameGO, root.gameObject);
        var nameRect = nameGO.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.3f);
        nameRect.offsetMin = nameRect.offsetMax = Vector2.zero;
        var tmp = nameGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = "Имя";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 22;
        tmp.color     = Color.white;

        // Рамка выбора (выключена)
        var border = new GameObject("SelectedBorder");
        GameObjectUtility.SetParentAndAlign(border, root.gameObject);
        var borderRect = border.AddComponent<RectTransform>();
        Stretch(borderRect);
        var borderImg  = border.AddComponent<Image>();
        borderImg.color = new Color(1f, 0.85f, 0f, 0.6f);
        border.SetActive(false);

        Debug.Log("✅ GirlCard создан");
    }

    // ══════════════════════════════════════════════════════════
    // ПРЕФАБ: SlotRow
    // ══════════════════════════════════════════════════════════

    void BuildSlotRow(RectTransform parent)
    {
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build SlotRow");

        var root = new GameObject("SlotRow");
        GameObjectUtility.SetParentAndAlign(root, parent.gameObject);
        var rootRect = root.AddComponent<RectTransform>();
        SetSize(rootRect, 0, 60);
        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.padding = new RectOffset(8, 8, 4, 4);
        root.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        MakeTextChild (root.transform, "SlotIndexText", "Слот 0", 18, 140);
        MakeInputChild(root.transform, "LabelInput",    "Название", 160);
        MakeImageChild(root.transform, "PhotoPreview",  50, 60);
        MakeButtonChild(root.transform, "BtnPhoto",  "Фото",  80);
        MakeButtonChild(root.transform, "BtnVideo",  "Видео", 80);
        MakeButtonChild(root.transform, "BtnAudio",  "Аудио", 80);

        root.AddComponent<SlotRowUI>();

        Debug.Log("✅ SlotRow создан");
    }

    // ══════════════════════════════════════════════════════════
    // ПРЕФАБ: CardEntryRow
    // ══════════════════════════════════════════════════════════

    void BuildCardEntryRow(RectTransform parent)
    {
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build CardEntryRow");

        var root = new GameObject("CardEntryRow");
        GameObjectUtility.SetParentAndAlign(root, parent.gameObject);
        var rootRect = root.AddComponent<RectTransform>();
        SetSize(rootRect, 0, 50);
        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth  = false;
        hlg.padding = new RectOffset(8, 8, 4, 4);

        MakeTextChild (root.transform, "NameText",       "A♣",              28, 50);
        MakeImageChild(root.transform, "CardPreview",    40, 50);
        MakeTextChild (root.transform, "SpriteNameText", "card_A_clubs",    16, 180);
        MakeButtonChild(root.transform, "BtnPickSprite", "Выбрать",         90);

        Debug.Log("✅ CardEntryRow создан");
    }

    // ══════════════════════════════════════════════════════════
    // Хелперы создания элементов
    // ══════════════════════════════════════════════════════════

    VerticalLayoutGroup EnsureVerticalLayout(RectTransform parent, float spacing, int padding)
    {
        if (parent == null || parent.gameObject == null)
        {
            Debug.LogError("EnsureVerticalLayout: parent is null!");
            return null;
        }
        var vlg = parent.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = parent.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = spacing;
        vlg.padding = new RectOffset(padding, padding, padding, padding);
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;

        var csf = parent.GetComponent<ContentSizeFitter>();
        if (csf == null) csf = parent.gameObject.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return vlg;
    }

    void MakeLabel(RectTransform parent, string name, string text = null)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var le   = go.AddComponent<LayoutElement>();
        le.preferredHeight = 28;

        var tmp  = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text ?? name;
        tmp.fontSize  = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color     = new Color(0.7f, 0.7f, 0.7f);
    }

    void MakeSpace(RectTransform parent, float h)
    {
        var go = new GameObject("Space");
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        go.AddComponent<RectTransform>();
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = h;
    }

    void MakeInputField(RectTransform parent, string name, string placeholder, bool multiline = false)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var le   = go.AddComponent<LayoutElement>();
        le.preferredHeight = multiline ? 80 : 50;

        go.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);
        var field = go.AddComponent<TMP_InputField>();

        // Text area child
        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        var textRect = textGO.AddComponent<RectTransform>();
        Stretch(textRect, 8);
        var textTMP   = textGO.AddComponent<TextMeshProUGUI>();
        textTMP.color = Color.white;
        textTMP.fontSize = 20;

        // Placeholder child
        var phGO = new GameObject("Placeholder");
        GameObjectUtility.SetParentAndAlign(phGO, go);
        var phRect = phGO.AddComponent<RectTransform>();
        Stretch(phRect, 8);
        var phTMP   = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text  = placeholder;
        phTMP.color = new Color(0.5f, 0.5f, 0.5f);
        phTMP.fontSize = 20;
        phTMP.fontStyle = FontStyles.Italic;

        field.textComponent   = textTMP;
        field.placeholder     = phTMP;
        field.lineType        = multiline
            ? TMP_InputField.LineType.MultiLineNewline
            : TMP_InputField.LineType.SingleLine;
    }

    void MakeImage(RectTransform parent, string name, float w, float h)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var le   = go.AddComponent<LayoutElement>();
        le.preferredWidth  = w;
        le.preferredHeight = h;
        SetSize(rect, w, h);
        var img  = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f);
    }

    GameObject MakeButton(RectTransform parent, string name, string label, Color? color = null)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var le   = go.AddComponent<LayoutElement>();
        le.preferredHeight = 50;

        var img = go.AddComponent<Image>();
        img.color = color ?? new Color(0.2f, 0.45f, 0.7f);
        var btn = go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        var textRect = textGO.AddComponent<RectTransform>();
        Stretch(textRect);
        var tmp  = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 22;
        tmp.color     = Color.white;

        return go;
    }

    // ── Child-хелперы для горизонтальных строк ────────────────

    void MakeTextChild(Transform parent, string name, string text, int fontSize, float width)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var rect = go.AddComponent<RectTransform>();
        var le   = go.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        var tmp  = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color     = Color.white;
    }

    void MakeInputChild(Transform parent, string name, string placeholder, float width)
    {
        var go   = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le   = go.AddComponent<LayoutElement>();
        le.preferredWidth = width;
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);
        var field = go.AddComponent<TMP_InputField>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);
        Stretch(textGO.AddComponent<RectTransform>(), 4);
        var textTMP = textGO.AddComponent<TextMeshProUGUI>();
        textTMP.color = Color.white; textTMP.fontSize = 18;

        var phGO = new GameObject("Placeholder");
        GameObjectUtility.SetParentAndAlign(phGO, go);
        Stretch(phGO.AddComponent<RectTransform>(), 4);
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder; phTMP.color = new Color(0.5f,0.5f,0.5f); phTMP.fontSize = 18;
        phTMP.fontStyle = FontStyles.Italic;

        field.textComponent = textTMP;
        field.placeholder   = phTMP;
    }

    void MakeImageChild(Transform parent, string name, float w, float h)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = w; le.preferredHeight = h;
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
    }

    void MakeButtonChild(Transform parent, string name, string label, float width)
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
        tmp.text = label; tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 18; tmp.color = Color.white;
    }

    // ── Утилиты ───────────────────────────────────────────────

    void SetSize(RectTransform rect, float w, float h)
    {
        rect.sizeDelta = new Vector2(w, h);
    }

    void Stretch(RectTransform rect, float offset = 0)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2( offset,  offset);
        rect.offsetMax = new Vector2(-offset, -offset);
    }
}
