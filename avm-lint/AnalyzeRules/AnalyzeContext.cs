using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeContext : IAnalyzeContext
{
    public required bool IsSubmodule { get; set; }

    public required List<SyntaxBase> Declarations { get; set; }

    public required List<IDiagnostic> Diagnostics { get; set; }
}
