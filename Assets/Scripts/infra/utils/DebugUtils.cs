// NOTE: Define DEBUG_LOG in Scripting Define Symbols to see log messages.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Infra {
public static class DebugUtils {

    [Conditional("DEBUG_LOG")]
    public static void Assert(bool condition, string message = null) {
        if (!condition) throw new Exception(message);
    }
    
    [Conditional("DEBUG_LOG")]
    public static void Log<TKey, TValue>(Dictionary<TKey, TValue> dict) {
        foreach (var entry in dict) {
            Log(entry.Key + " = " + entry.Value);
        }
    }

    [Conditional("DEBUG_LOG")]
    public static void Log(object message) {
        UnityEngine.Debug.Log(Time.realtimeSinceStartup.ToString("0.00") + ": " + message);
    }
    
    [Conditional("DEBUG_LOG")]
    public static void LogError<TKey, TValue>(Dictionary<TKey, TValue> dict) {
        foreach (var entry in dict) {
            LogError(entry.Key + " = " + entry.Value);
        }
    }

    [Conditional("DEBUG_LOG")]
    public static void LogError(object message) {
        UnityEngine.Debug.LogError(Time.realtimeSinceStartup.ToString("0.00") + ": " + message);
    }

    [Conditional("DEBUG_LOG")]
    public static void LogCollection<T>(string name, ICollection<T> collection) {
        var sb = new StringBuilder();
        sb.Append(name)
            .Append("(")
            .Append(collection.Count)
            .AppendLine(")");
        foreach (T item in collection) {
            sb.Append("\t").AppendLine(item.ToString());
        }
        Log(sb.ToString());
    }

    [Conditional("DEBUG_LOG")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color) {
        UnityEngine.Debug.DrawLine(start, end, color);
    }
}
}
