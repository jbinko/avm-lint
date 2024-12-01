using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule002 : IAnalyzeRule
{
    public static string Code => "AVM002";

    public void Analyze(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM002 | Error
        // The module description metadata must be specified second and must contain
        // the value with the resource name in singular form, starting with the phrase
        // 'This module deploys a'. For example, 'This module deploys an Elastic SAN'.

        const int declarationNumber = 1; // Must be 2nd

        if (declarations.Count <= declarationNumber)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var decl = declarations[declarationNumber];

        if (decl is not MetadataDeclarationSyntax metadata ||
            metadata.Name.IdentifierName != "description" ||
            metadata.Value is not StringSyntax value)
        {
            AddDiagnostic(diagnostics, null);
            return;
        }

        var msgValue = value.TryGetLiteralValue();
        if (string.IsNullOrWhiteSpace(msgValue) || !msgValue.StartsWith("This module deploys a"))
        {
            AddDiagnostic(diagnostics, msgValue);
        }
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The module description metadata must be specified second and must contain the value with the resource name in singular form, starting with the phrase 'This module deploys a'. For example, 'This module deploys an Elastic SAN'.",
            msgValue));
    }
}
