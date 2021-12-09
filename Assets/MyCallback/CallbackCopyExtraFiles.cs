using Unity.Android.Gradle;
using System.IO;
using Unity.Android.Types;

public class CallbackCopyExtraFile : AndroidProjectCallbacks
{    
    private InputFile SourceExtraFile => new InputFile() { IsGUID = true, Value = "7f22081b2d41d44468b29920f49807d7" };

    public override InputFile[] InputFiles => new[] { SourceExtraFile };

    private string RandomExtraFile => Path.Combine(GradleProjectPath, "RandomExtraFile.txt");
    public override string[] OutputFiles => new[]
    {
        RandomExtraFile
    };

    public override void Run()
    {
        CopyFile(SourceExtraFile, RandomExtraFile);
    }
}
