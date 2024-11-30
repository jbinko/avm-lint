using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule001 : IAnalyzeRule
{
    public string Code => "AVM001";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM001 | Error
        // The module name metadata must be specified first and should contain
        // the value with the resource name in plural form. For example, 'Elastic SANs'.

        const int declarationNumber = 0; // Must be 1st

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var decl = declarations[declarationNumber];

        if (decl is not MetadataDeclarationSyntax metadata ||
            metadata.Name.IdentifierName != "name" ||
            metadata.Value is not StringSyntax value)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var msgValue = value.TryGetLiteralValue();
        if (string.IsNullOrWhiteSpace(msgValue) || !msgValue.EndsWith("s"))
        {
            AddDiagnostic(diagnostics, msgValue);
        }
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The module name metadata must be specified first and should contain the value with the resource name in plural form. For example, 'Elastic SANs'.",
            msgValue));
    }
}
