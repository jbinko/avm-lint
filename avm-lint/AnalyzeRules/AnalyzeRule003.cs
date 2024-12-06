using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule003 : AnalyzeRuleBase, IAnalyzeRule
{
    public string Code => "AVM003";

    public void Analyze(IAnalyzeContext context)
    {
        // AVM003 | Error
        // The 'owner' metadata in the module should be the third metadata defined (without any decorators)
        // with the value 'Azure/module-maintainers'.

        const int declarationNumber = 2; // Must be 3rd

        if (context.Declarations.Count <= declarationNumber)
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        var decl = context.Declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "owner", out var msgValue))
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        if (msgValue != "Azure/module-maintainers")
        {
            AddDiagnostic(context.Diagnostics, msgValue);
        }
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The 'owner' metadata in the module should be the third metadata defined (without any decorators) with the value 'Azure/module-maintainers'.",
            msgValue));
    }
}
