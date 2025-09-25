
using UnityEngine;

public static class MyDebug
{
    private static bool enableLogsInBuild => Debug.isDebugBuild;

    public static void Log(string message)
    {
        #if UNITY_EDITOR
        PrintLog(message);
        #else
        if (enableLogsInBuild)
            PrintLog(message);
        #endif
    }

    public static void LogWarning(string message)
    {
        #if UNITY_EDITOR
        PrintWarning(message);
        #else
        if (enableLogsInBuild)
            PrintWarning(message);
        #endif
    }

    public static void LogError(string message)
    {
        #if UNITY_EDITOR
        PrintError(message);
        #else
        if (enableLogsInBuild)
            PrintError(message);
        #endif
    }

    private static void PrintLog(string message)
    {
     var stackTrace = new System.Diagnostics.StackTrace(1, true);
        Debug.Log($"{message}\n{stackTrace}");
    }

    private static void PrintWarning(string message)
    {
      var stackTrace = new System.Diagnostics.StackTrace(1, true);
        Debug.LogWarning($"{message}\n{stackTrace}");
    }

    private static void PrintError(string message)
    {
       var stackTrace = new System.Diagnostics.StackTrace(1, true);
        Debug.LogError($"{message}\n{stackTrace}");
    }
}
