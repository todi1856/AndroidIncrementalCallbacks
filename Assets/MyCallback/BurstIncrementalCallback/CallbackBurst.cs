using Unity.Android.Gradle;
using Unity.Android.Types;
using System;
using System.Collections.Generic;
using System.IO;

public class CallbackBurst : AndroidProjectCallbacks
{
    public const string BurstSettingsFile = "Temp/BurstCallbackSettings_Android.json";
    private const string JniLibs = "unityLibrary/src/main/jniLibs";
    public class CallbackBurstSettings
    {
        public bool Enabled;
        public string[] Architectures;
    }

    private InputFile[] GetBurstSourceLibraries(CallbackBurstSettings settings)
    {
        var files = new List<InputFile>();
        foreach (var a in settings.Architectures)
        {
            files.Add(new InputFile() { Value = Path.Combine("Temp", "StagingArea", "tempburstlibs", a, "lib_burst_generated.so") });
        }
        return files.ToArray();
    }

    public override InputFile[] InputFiles
    {
        get
        {
            var settings = GetSettings();
            var files = new List<InputFile>();
            files.Add(new InputFile() { Value = BurstSettingsFile });

            if (settings.Enabled)
                files.AddRange(GetBurstSourceLibraries(settings));

            return files.ToArray();
        }
    }

    public override string[] OutputFiles
    {
        get
        {
            var settings = GetSettings();
            if (!settings.Enabled)
                return Array.Empty<string>();
            var files = new List<string>();

            foreach (var a in settings.Architectures)
            {
                files.Add(Path.Combine(GradleProjectPath, JniLibs, a, "lib_burst_generated.so"));
            }

            return files.ToArray();
        }
    }

    private CallbackBurstSettings GetSettings()
    {
        // Note: Not validating against input files to avoid recursion
        return FromJson<CallbackBurstSettings>(ReadAllText(new InputFile() { Value = BurstSettingsFile }, false));
    }

    public override void Run()
    {
        var settings = GetSettings();
        var files = GetBurstSourceLibraries(settings);
        foreach (var file in files)
        {
            var arch = Path.GetFileName(Path.GetDirectoryName(file.Value));
            CopyFile(file, Path.Combine(GradleProjectPath, JniLibs, arch, "lib_burst_generated.so"));
        }
    }
}
