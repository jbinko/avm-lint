using Bicep.Core.Diagnostics;
using PluralizeService.Core;

internal sealed class AnalyzeRule001 : AnalyzeRuleBase, IAnalyzeRule
{
    public string Code => "AVM001";

    public void Analyze(IAnalyzeContext context)
    {
        // AVM001 | Error
        // The 'name' metadata in the module should be the first metadata defined (without any decorators)
        // and must be in plural form, such as 'Elastic SANs'.

        const int declarationNumber = 0; // Must be 1st

        if (context.Declarations.Count <= declarationNumber)
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        var decl = context.Declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "name", out var msgValue))
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        if (!IsValidPluralForm(msgValue))
        {
            AddDiagnostic(context.Diagnostics, msgValue);
        }
    }

    private bool IsValidPluralForm(string msgValue)
    {
        var lastWord = msgValue.Split(' ').LastOrDefault();
        return !string.IsNullOrWhiteSpace(lastWord) && PluralizationProvider.IsPlural(lastWord);
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The 'name' metadata in the module should be the first metadata defined (without any decorators) and must be in plural form, such as 'Elastic SANs'.",
            msgValue));
    }
}
