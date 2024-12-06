using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal interface IAnalyzeContext
{
    bool IsSubmodule { get; }
    List<SyntaxBase> Declarations { get; }
    List<IDiagnostic> Diagnostics { get; }
}
