using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tools → Welcome Screen Builder
/// Кидаешь панель — создаёт кнопки внутри.
/// </summary>
public class WelcomeScreenBuilder : EditorWindow
{
    RectTransform _parent;

    [MenuItem("Tools/Welcome Screen Builder")]
    public static void Open() => GetWindow<WelcomeScreenBuilder>("Welcome Screen Builder");

    void OnGUI()
    {
        GUILayout.Label("Welcome Screen Builder", EditorStyles.boldLabel);
        GUILayout.Space(8);

        _parent = (RectTransform)EditorGUILayout.ObjectField(
            "Панель", _parent, typeof(RectTransform), true);

        GUILayout.Space(8);
        GUI.enabled = _parent != null;

        if (GUILayout.Button("Создать кнопки", GUILayout.Height(36)))
        {
            var target = _parent;
            EditorApplication.delayCall += () => Build(target);
        }

        GUI.enabled = true;
    }

    void Build(RectTransform parent)
    {
        if (parent == null) return;

        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Build Welcome Buttons");

        CreateButton(parent, "BtnPlay",  "Играть");
        CreateButton(parent, "BtnRules", "Правила");
        CreateButton(parent, "BtnExit",  "Выйти");
        CreateButton(parent, "BtnAdmin", "⚙");

        Debug.Log("✅ Кнопки Welcome созданы");
        EditorUtility.DisplayDialog("Готово", "Привяжи кнопки в WelcomeScreen (Inspector)", "ОК");
    }

    void CreateButton(RectTransform parent, string name, string label)
    {
        var go = new GameObject(name);
        GameObjectUtility.SetParentAndAlign(go, parent.gameObject);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 50);

        go.AddComponent<Image>();
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        GameObjectUtility.SetParentAndAlign(textGO, go);

        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;

        var tmp       = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
