using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule003 : IAnalyzeRule
{
    public string Code => "AVM003";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM003 | Error | Module owner is not specified correctly.
        // The module owner is not specified correctly.
        // It must be specified as the third metadata statement
        // with the value "metadata owner = 'Azure/module-maintainers'".

        const int declarationNumber = 2; // Must be 3rd

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var msgValue = declarations[declarationNumber].ToString();
        if (msgValue != "metadata owner = 'Azure/module-maintainers'")
        {
            AddDiagnostic(diagnostics, msgValue);
        }
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The module owner is not specified correctly. It must be specified as the third metadata statement with the value \"metadata owner = 'Azure/module-maintainers'\".",
            msgValue));
    }
}
