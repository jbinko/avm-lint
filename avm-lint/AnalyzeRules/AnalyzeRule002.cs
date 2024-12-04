using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;
using PluralizeService.Core;

internal sealed class AnalyzeRule002 : AnalyzeRuleBase, IAnalyzeRule
{
    public string Code => "AVM002";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM002 | Error
        // The 'description' metadata in the module should be the second metadata defined (without any decorators)
        // and must start with 'This module deploys a' followed by the name of the resource
        // in singular form. For example 'This module deploys an Elastic SAN'.

        const int declarationNumber = 1; // Must be 2nd

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var decl = declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "description", out var msgValue))
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        if (!msgValue.StartsWith("This module deploys a"))
        {
            AddDiagnostic(diagnostics, msgValue);
            return;
        }

        if (!IsValidSingularForm(msgValue))
        {
            AddDiagnostic(diagnostics, msgValue);
        }
    }

    private bool IsValidSingularForm(string msgValue)
    {
        var lastWord = msgValue.Split(' ').LastOrDefault();
        return !string.IsNullOrWhiteSpace(lastWord) && !PluralizationProvider.IsPlural(lastWord);
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The 'description' metadata in the module should be the second metadata defined (without any decorators) and must start with 'This module deploys a' followed by the name of the resource in singular form. For example 'This module deploys an Elastic SAN'.",
            msgValue));
    }
}
