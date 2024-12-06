using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeContext : IAnalyzeContext
{
    public string ModuleFilePath { get; private set; }
    public ModuleType ModuleType { get; private set; }

    public List<SyntaxBase> Declarations { get; } = new();

    public List<IDiagnostic> Diagnostics { get; } = new();

    public AnalyzeContext(string moduleFilePath)
    {
        ModuleFilePath = moduleFilePath;
    }

    public IAnalyzeContext AddDiagnostics(List<IDiagnostic> diagnostics)
    {
        Diagnostics.AddRange(diagnostics);
        return this;
    }

    public IAnalyzeContext AddDeclarations(List<SyntaxBase> declarations)
    {
        SetModuleType(); // After Bicep file was successfully parsed
        Declarations.AddRange(declarations);
        return this;
    }

    private void SetModuleType()
    {
        const string AvmDirName = "avm";
        const string MainBicepFileName = "main.bicep";
        const string MainTestBicepFileName = "main.test.bicep";
        const string DependenciesBicepFileName = "dependencies.bicep";

        var fileName = Path.GetFileName(ModuleFilePath);
        if (fileName == MainTestBicepFileName)
        {
            ModuleType = ModuleType.TestModule;
            return;
        }

        if (fileName == DependenciesBicepFileName)
        {
            ModuleType = ModuleType.Dependencies;
            return;
        }

        var names = ModuleFilePath.Split(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        int index = 0;
        foreach (var name in names)
        {
            index++;
            if (name == AvmDirName)
                break;
        }

        if (names.Length == index || index == 0)
            throw new InvalidOperationException($"Not supported module file path: '{ModuleFilePath}'.");

        fileName = names[^1];
        var topDirName = names[index-1];
        var subDirName = names[index];

        if (topDirName == AvmDirName/* && fileName == MainBicepFileName*/)
        {
            if (subDirName == "ptn" || subDirName == "res" || subDirName == "utl")
            {
                ModuleType = ModuleType.RootModule;
                return;
            }

            ModuleType = ModuleType.SubModule; // We are betting on sub module
        }
        else
        {
            ModuleType = ModuleType.NotSpecified;
        }
    }
}
