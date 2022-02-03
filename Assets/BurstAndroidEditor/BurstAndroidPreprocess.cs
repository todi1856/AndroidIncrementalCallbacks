using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using System.IO;
using Unity.Burst.Editor;
using Unity.Android.Types;
using System.Linq;

// This file should live in Packages\com.unity.burst@1.7.0-pre.1\Editor\, but for simplicity we keep it here
// Add [assembly: InternalsVisibleTo("BurstAndroidEditor")] in Packages\com.unity.burst@1.7.0-pre.1\Editor\BurstAotCompiler.cs
// Comment BurstAndroidGradlePostprocessor in Packages\com.unity.burst@1.7.0-pre.1\Editor\BurstAotCompiler.cs

class BurstAndroidPreprocess : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var aotSettingsForTarget = BurstPlatformAotSettings.GetOrCreateSettings(BuildTarget.Android);

        var settings = new CallbackBurst.CallbackBurstSettings();
        settings.Enabled = aotSettingsForTarget.EnableBurstCompilation;
        if (settings.Enabled)
        {
            var archs = AndroidTargetDeviceType.AllSupported
                .Select(d => d.TargetArchitecture)
                .Where(a => (PlayerSettings.Android.targetArchitectures & (UnityEditor.AndroidArchitecture)a) == (UnityEditor.AndroidArchitecture)a).ToArray();
            settings.Architectures = archs.Select(a => AndroidTargetDeviceType.GetTargetDeviceType(a).ABI).ToArray();
        }
        else
        {
            settings.Architectures = new string[0];
        }
        File.WriteAllText(CallbackBurst.BurstSettingsFile, EditorJsonUtility.ToJson(settings, true));
    }
}