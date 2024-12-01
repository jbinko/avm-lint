using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal interface IAnalyzeRule
{
    static string Code { get; } = "";
    void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics);
}
