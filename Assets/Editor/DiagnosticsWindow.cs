using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

public class DiagnosticsWindow : EditorWindow
{
    class SettingsWrapper
    {
        public CallbackReadSettings.Settings settings = new CallbackReadSettings.Settings();
        public string RelativePath { get; }
        public string AbsolutePath { get => Path.Combine(Application.dataPath, "..", RelativePath); }

        public SettingsWrapper(string path)
        {
            RelativePath = path;
        }

    }

    SettingsWrapper[] wrappers = new[]
    {
        new SettingsWrapper("Assets/MyCallback/SettingsNoGUID.json"),
        new SettingsWrapper("Assets/MyCallback/SettingsWithGUID.json")
    };


    [MenuItem("Incremental Callbacks/Show Diagnostics")]
    static void Init()
    {
        DiagnosticsWindow window = (DiagnosticsWindow)EditorWindow.GetWindow(typeof(DiagnosticsWindow));
        window.Show();
    }

    static bool ReferencesUnityEngineAssemblies(Type t)
    {
        var references = t.Assembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(a => a.StartsWith("UnityEngine.") || a.StartsWith("UnityEditor."))
            .ToArray();

        return references.Length > 0;
    }

    private void OnEnable()
    {
        foreach (var w in wrappers)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(w.RelativePath);
            if (asset != null && !string.IsNullOrEmpty(asset.text))
            {
                EditorJsonUtility.FromJsonOverwrite(asset.text, w.settings);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        foreach (var w in wrappers)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Path: {w.RelativePath}");
            GUILayout.Label($"Guid: {AssetDatabase.AssetPathToGUID(w.RelativePath)}");
            w.settings.ModifyManifest = GUILayout.Toggle(w.settings.ModifyManifest, "Modify Manifest");
            GUILayout.Label("ExtraFileContents");
            w.settings.ExtraFileContents = GUILayout.TextField(w.settings.ExtraFileContents);

            if (GUILayout.Button("Save"))
            {
                File.WriteAllText(w.AbsolutePath, EditorJsonUtility.ToJson(w.settings, true));
                AssetDatabase.Refresh();
            }
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();

        var incCallbacks = TypeCache.GetTypesDerivedFrom<Unity.Android.Gradle.AndroidProjectCallbacks>();
        GUILayout.Label($"Incremental Callbacks [{incCallbacks.Count}]:", EditorStyles.boldLabel);
        foreach (var c in incCallbacks)
        {
            GUILayout.Label($"    {c.FullName} from assembly '{c.Assembly.GetName().Name}'");
            GUILayout.Label($"        Location: {c.Assembly.Location}");
            GUILayout.Label($"        References Unity Engine assemblies: " + (ReferencesUnityEngineAssemblies(c) ? "Yes" : "No"));
        }
    }
}