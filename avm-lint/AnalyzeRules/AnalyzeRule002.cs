using Bicep.Core.Diagnostics;
using PluralizeService.Core;

internal sealed class AnalyzeRule002 : AnalyzeRuleBase, IAnalyzeRule
{
    public string Code => "AVM002";

    public void Analyze(IAnalyzeContext context)
    {
        // AVM002 | Error
        // The 'description' metadata in the module should be the second metadata defined (without any decorators)
        // and must start with 'This module deploys a' followed by the name of the resource
        // in singular form. For example 'This module deploys an Elastic SAN'.

        const int declarationNumber = 1; // Must be 2nd

        if (context.Declarations.Count <= declarationNumber)
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        var decl = context.Declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "description", out var msgValue))
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        if (!msgValue.StartsWith("This module deploys a"))
        {
            AddDiagnostic(context.Diagnostics, msgValue);
            return;
        }

        if (!IsValidSingularForm(msgValue))
        {
            AddDiagnostic(context.Diagnostics, msgValue);
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
