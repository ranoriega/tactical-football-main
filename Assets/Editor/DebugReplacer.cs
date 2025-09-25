using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class DebugReplacer : EditorWindow
{
    [MenuItem("Tools/Reemplazar Debug.Log Seguro")]
    public static void ShowWindow()
    {
        GetWindow<DebugReplacer>("Debug Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reemplazar Debug.Log por MyDebug.Log en Scripts", EditorStyles.boldLabel);

        if (GUILayout.Button("Reemplazar en Scripts y subcarpetas"))
        {
            ReplaceDebugLogs();
        }
    }

    private static void ReplaceDebugLogs()
    {
        string scriptsFolder = Path.Combine(Application.dataPath, "Scripts"); // Solo tu carpeta Scripts
        if (!Directory.Exists(scriptsFolder))
        {
            EditorUtility.DisplayDialog("Error", "La carpeta Assets/Scripts no existe.", "OK");
            return;
        }

        string[] scripts = Directory.GetFiles(scriptsFolder, "*.cs", SearchOption.AllDirectories);

        foreach (string scriptPath in scripts)
        {
             // ❌ Ignorar MyDebug.cs para no tocarlo
            if (Path.GetFileName(scriptPath) == "MyDebug.cs") continue;

            string code = File.ReadAllText(scriptPath);
            string originalCode = code;

            // Hacer respaldo automático
            string backupPath = scriptPath + ".bak";
            if (!File.Exists(backupPath))
            {
                File.Copy(scriptPath, backupPath);
            }

            // Reemplazar Debug.Log usando regex para evitar afectar strings o comentarios
            code = Regex.Replace(code, @"\bDebug\.Log\b", "MyDebug.Log");
            code = Regex.Replace(code, @"\bDebug\.LogWarning\b", "MyDebug.LogWarning");
            code = Regex.Replace(code, @"\bDebug\.LogError\b", "MyDebug.LogError");

            if (code != originalCode)
            {
                File.WriteAllText(scriptPath, code);
                Debug.Log($"✅ Reemplazado en: {scriptPath}");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Debug Replacer", "Reemplazo completado con respaldo automático.", "OK");
    }
}
