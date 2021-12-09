using Unity.Android.Gradle;

/// <summary>
/// The most simplest incremental friendly callback.
/// No input files
/// No output files
/// Callback is only reexecuted, when its assembly changes, for ex., after changes in script.
/// </summary>
public class CallbackModifiesExistingFiles : AndroidProjectCallbacks
{
    public override void Run()
    {
        LibraryManifest.AddApplicationMetaDataAttribute("CalledWhenAssemblyIsRecompiled", "sdsd");
    }
}
