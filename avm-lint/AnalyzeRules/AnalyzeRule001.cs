﻿using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;
using PluralizeService.Core;

internal sealed class AnalyzeRule001 : AnalyzeRuleBase, IAnalyzeRule
{
    public static string Code => "AVM001";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM001 | Error
        // The 'name' metadata in the module should be the first metadata defined
        // and it should be in plural form, for example, 'Elastic SANs'.

        const int declarationNumber = 0; // Must be 1st

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var decl = declarations[declarationNumber];

        if (!IsValidMetadataDeclaration(decl, "name", out var msgValue))
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        if (!IsValidPluralForm(msgValue))
        {
            AddDiagnostic(diagnostics, msgValue);
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
            "The 'name' metadata in the module should be the first metadata defined and it should be in plural form, for example, 'Elastic SANs'.",
            msgValue));
    }
}
