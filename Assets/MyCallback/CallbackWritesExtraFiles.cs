using Unity.Android.Gradle;
using System.IO;

public class CallbackWritesExtraFiles : AndroidProjectCallbacks
{
    private string TestFile => Path.Combine(GradleProjectPath, "test.txt");
    public override string[] OutputFiles => new[]
    {
        TestFile
    };

    public override void Run()
    {
        WriteAllText(TestFile, "MyTest");
    }
}
