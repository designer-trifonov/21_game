#define NATIVE_FILE_PICKER_INSTALLED

using System;
using UnityEngine;

/// <summary>
/// Открывает системный проводник на любой платформе.
/// Требует NativeFilePicker (yasirkula):
/// Window → Package Manager → + → Add from git URL:
/// https://github.com/yasirkula/UnityNativeFilePicker.git
/// </summary>

public static class FilePicker
{
    public static void PickImage(Action<string> onPicked) => Pick(onPicked,
        android: "image/*",
        ios:     "public.image",
        desktop: new[] { "image/png", "image/jpeg" });

    public static void PickVideo(Action<string> onPicked) => Pick(onPicked,
        android: "video/*",
        ios:     "public.movie",
        desktop: new[] { "video/mp4", "video/avi" });

    public static void PickAudio(Action<string> onPicked) => Pick(onPicked,
        android: "audio/*",
        ios:     "public.audio",
        desktop: new[] { "audio/mpeg", "audio/wav", "audio/ogg" });

    // ─────────────────────────────────────────────────────────

    static void Pick(Action<string> onPicked, string android, string ios, string[] desktop)
    {
#if NATIVE_FILE_PICKER_INSTALLED
        string[] types;
#if UNITY_ANDROID
        types = new[] { android };
#elif UNITY_IOS
        types = new[] { ios };
#else
        types = desktop;
#endif
        NativeFilePicker.PickFile(path =>
        {
            if (!string.IsNullOrEmpty(path)) onPicked?.Invoke(path);
        }, types);
#else
        Debug.LogWarning("FilePicker: установи NativeFilePicker → https://github.com/yasirkula/UnityNativeFilePicker.git");
#endif
    }
}
