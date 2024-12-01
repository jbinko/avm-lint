using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule003 : AnalyzeRuleBase, IAnalyzeRule
{
    public static string Code => "AVM003";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM003 | Error
        // The 'owner' metadata in the module should be the third metadata defined
        // with the value 'Azure/module-maintainers'.

        const int declarationNumber = 2; // Must be 3rd

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var decl = declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "owner", out var msgValue))
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        if (msgValue != "Azure/module-maintainers")
        {
            AddDiagnostic(diagnostics, msgValue);
        }
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The 'owner' metadata in the module should be the third metadata defined with the value 'Azure/module-maintainers'.",
            msgValue));
    }
}
