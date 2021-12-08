using Unity.Android.Gradle;
using Unity.Android.Types;
using System.IO;

// TODO: add example with conditional output file
public class CallbackReadSettings : AndroidProjectCallbacks
{
    public class Settings
    {
        public bool ModifyManifest;
        public string ExtraFileContents;
    }

    // Note: This only works if settings file path stays the same. 
    //       Meaning it's not very good for asset store packages, where asset path can vary. In those case a guid approach must be used
    private InputFile SettingsFileNoGUID => new InputFile() { IsGUID = false, Value = Path.Combine(UnityProjectPath, "Assets/MyCallback/SettingsNoGUID.json") };

    // The guid acquired from SettingsWithGUID.json.meta
    private InputFile SettingsFileWithGUID => new InputFile() { IsGUID = true, Value = "acfd24c76e1b1064fa1f5034136abac8" };

    public override InputFile[] InputFiles
    {
        get
        {
            return new[]
            {
                SettingsFileNoGUID,
                SettingsFileWithGUID
            };
        }
    }

    private string TestFileNoGUID => Path.Combine(GradleProjectPath, "ExtraFileFromSettingsNoGUID.txt");
    private string TestFileWithGUID => Path.Combine(GradleProjectPath, "ExtraFileFromSettingsWithGUID.txt");
    public override string[] OutputFiles => new[]
    {
        TestFileNoGUID,
        TestFileWithGUID
    };

    private void ModifyProjectUsingSettingsFrom(InputFile file, string testFile)
    {
        var contents = ReadAllText(file);
        var settings = FromJson<Settings>(contents);

        if (settings.ModifyManifest)
            LibraryManifest.AddApplicationMetaDataAttribute(Path.GetFileNameWithoutExtension(testFile), "Test");
        WriteAllText(testFile, settings.ExtraFileContents);
    }

    public override void Run()
    {
        ModifyProjectUsingSettingsFrom(SettingsFileNoGUID, TestFileNoGUID);
        ModifyProjectUsingSettingsFrom(SettingsFileWithGUID, TestFileWithGUID);
    }
}
