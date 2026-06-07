using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Перехватывает все Debug.Log/LogWarning/LogError и дублирует их в текстовый файл.
/// Инициализируется автоматически до загрузки первой сцены.
/// </summary>
public static class FileLogger
{
    private static string _logPath;
    private static readonly object _lock = new object();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        string logsDir = Path.Combine(Application.dataPath, "Logs");
        Directory.CreateDirectory(logsDir);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _logPath = Path.Combine(logsDir, $"game_{timestamp}.txt");

        // Пишем заголовок сессии
        File.WriteAllText(_logPath, $"=== Сессия {timestamp} ===\n\n");

        Application.logMessageReceived += OnLog;
        Application.quitting += Unsubscribe;
    }

    private static void Unsubscribe()
    {
        Application.logMessageReceived -= OnLog;
    }

    private static void OnLog(string message, string stackTrace, LogType type)
    {
        string prefix = type switch
        {
            LogType.Error     => "[ERROR]  ",
            LogType.Exception => "[EXCEPT] ",
            LogType.Warning   => "[WARN]   ",
            LogType.Assert    => "[ASSERT] ",
            _                 => "[LOG]    "
        };

        string time = DateTime.Now.ToString("HH:mm:ss.fff");
        string line = $"{time} {prefix}{message}";

        if (type == LogType.Error || type == LogType.Exception)
            line += $"\n{stackTrace}";

        lock (_lock)
        {
            try { File.AppendAllText(_logPath, line + "\n"); }
            catch { /* не падаем из-за логгера */ }
        }
    }
}
