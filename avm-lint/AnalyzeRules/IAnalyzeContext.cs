using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal enum ModuleType
{
    NotSpecified,
    RootModule,
    SubModule,
    TestModule,
    Dependencies,
}

internal interface IAnalyzeContext
{
    string ModuleFilePath { get; }
    ModuleType ModuleType { get; }
    List<SyntaxBase> Declarations { get; }
    List<IDiagnostic> Diagnostics { get; }
}
