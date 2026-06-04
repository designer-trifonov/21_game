using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Создаёт карточку "картинка + подпись" полностью в коде.
/// Универсальная — используется везде где нужна такая структура.
/// </summary>
public static class CardFactory
{
    public static GameObject Create(
        Transform parent,
        string    imagePath,
        string    label,
        Vector2   size,
        Action    onClick)
    {
        // ── Корень — кнопка ───────────────────────────────────
        var go   = new GameObject(label);
        go.transform.SetParent(parent, false);

        var rect       = go.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        var bg    = go.AddComponent<Image>();
        bg.color  = new Color(0.15f, 0.15f, 0.15f);

        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick?.Invoke());

        // ── Фото ─────────────────────────────────────────────
        var photoGO   = new GameObject("Photo");
        photoGO.transform.SetParent(go.transform, false);
        var photoRect = photoGO.AddComponent<RectTransform>();
        photoRect.anchorMin = Vector2.zero;
        photoRect.anchorMax = Vector2.one;
        photoRect.offsetMin = Vector2.zero;
        photoRect.offsetMax = new Vector2(0, -50);

        var photoImg            = photoGO.AddComponent<Image>();
        photoImg.preserveAspect = true;
        photoImg.enabled        = false;

        if (!string.IsNullOrEmpty(imagePath))
            ImageLoader.Load(imagePath, sprite =>
            {
                if (photoImg == null) return;
                photoImg.sprite  = sprite;
                photoImg.enabled = sprite != null;
            });

        // ── Подпись внизу ─────────────────────────────────────
        var nameGO   = new GameObject("Label");
        nameGO.transform.SetParent(go.transform, false);
        var nameRect = nameGO.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0);
        nameRect.offsetMin = new Vector2(4,  0);
        nameRect.offsetMax = new Vector2(-4, 50);

        var tmp       = nameGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;

        return go;
    }
}
