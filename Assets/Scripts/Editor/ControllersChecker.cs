using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// Tools → Controllers Checker
/// Находит объекты на сцене и заполняет ссылки в скриптах.
/// </summary>
public class ControllersChecker : EditorWindow
{
    static readonly (string goName, string scriptName)[] Controllers =
    {
        ("WelcomeController",     "WelcomeScreen"),
        ("RulesController",       "RulesScreen"),
        ("GirlSelectController",  "GirlSelectScreen"),
        ("IntroController",       "IntroScreen"),
        ("DifficultyController",  "DifficultyScreen"),
        ("GameController",        "GameController"),
        ("RoundResultController", "RoundResultScreen"),
        ("GameEndController",     "GameEndScreen"),
        ("UIManager",             "UIManager"),
    };

    Vector2 _scroll;

    [MenuItem("Tools/Controllers Checker")]
    public static void Open() => GetWindow<ControllersChecker>("Controllers Checker");

    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        try
        {
            GUILayout.Label("Controllers Checker", EditorStyles.boldLabel);
            GUILayout.Space(6);

            foreach (var (goName, scriptName) in Controllers)
            {
                var go = GameObject.Find(goName);

                GUILayout.BeginHorizontal();
                GUILayout.Label(go != null ? "✅" : "❌", GUILayout.Width(20));
                GUILayout.Label(goName, GUILayout.Width(210));

                GUI.enabled = go != null;
                if (GUILayout.Button("Заполнить", GUILayout.Width(80)))
                    AutoLink(go, scriptName);
                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Заполнить всё", GUILayout.Height(36)))
                foreach (var (g, s) in Controllers)
                {
                    var go = GameObject.Find(g);
                    if (go != null) AutoLink(go, s);
                }
        }
        finally { EditorGUILayout.EndScrollView(); }
    }

    void AutoLink(GameObject go, string scriptName)
    {
        var type = FindType(scriptName);
        if (type == null) { Debug.LogWarning($"Скрипт {scriptName} не найден"); return; }

        var component = go.GetComponent(type) ?? go.AddComponent(type);
        var so        = new SerializedObject(component);
        var fields    = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        int linked    = 0;

        foreach (var field in fields)
        {
            var prop = so.FindProperty(field.Name);
            if (prop == null || prop.objectReferenceValue != null) continue;

            var obj = FindForField(field.Name, field.FieldType);
            if (obj == null) continue;

            prop.objectReferenceValue = obj;
            linked++;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(component);
        Debug.Log($"✅ {scriptName}: привязано {linked} полей");
        Repaint();
    }

    UnityEngine.Object FindForField(string fieldName, Type fieldType)
    {
        foreach (var name in CandidateNames(fieldName))
        {
            var found = GameObject.Find(name);
            if (found == null) continue;
            if (fieldType == typeof(GameObject)) return found;
            var comp = GetComp(found, fieldType);
            if (comp != null) return comp;
        }
        return null;
    }

    string[] CandidateNames(string f) => new[]
    {
        Pascal(f),
        f.StartsWith("panel") ? "Panel_" + Pascal(f.Substring(5)) : null,
        f.StartsWith("btn")   ? "Btn"    + Pascal(f.Substring(3)) : null,
        f.StartsWith("txt")   ? "Txt"    + Pascal(f.Substring(3)) : null,
        f.StartsWith("input") ? "Input"  + Pascal(f.Substring(5)) : null,
    }.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToArray();

    static string Pascal(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

    Component GetComp(GameObject go, Type t)
    {
        if (t == typeof(Button))          return go.GetComponent<Button>();
        if (t == typeof(Image))           return go.GetComponent<Image>();
        if (t == typeof(RawImage))        return go.GetComponent<RawImage>();
        if (t == typeof(TextMeshProUGUI)) return go.GetComponent<TextMeshProUGUI>();
        if (t == typeof(TMP_InputField))  return go.GetComponent<TMP_InputField>();
        if (t == typeof(VideoPlayer))     return go.GetComponent<VideoPlayer>();
        if (t == typeof(Transform))       return go.GetComponent<Transform>();
        if (typeof(Component).IsAssignableFrom(t)) return go.GetComponent(t);
        return null;
    }

    static Type FindType(string name) =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
            .FirstOrDefault(t => t.Name == name && typeof(Component).IsAssignableFrom(t));
}
