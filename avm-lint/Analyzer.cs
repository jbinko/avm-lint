using Bicep.Core.Syntax;
using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

namespace avm_lint;

internal sealed class Analyzer
{
    public List<IDiagnostic> Analyze(string filePath)
    {
        var bicepCodeText = File.ReadAllText(filePath);
        var parser = new Parser(bicepCodeText);
        var lexingErrors = new List<IDiagnostic>(parser.LexingErrorLookup);
        var parsingErrors = new List<IDiagnostic>(parser.ParsingErrorLookup);
        var analyzingErrors = AnalyzeInternal(new LintVisitor().GetDeclarations(parser));

        if (lexingErrors.Count > 0)
            return lexingErrors;

        if (parsingErrors.Count > 0)
            return parsingErrors;

        if (analyzingErrors.Count > 0)
            return analyzingErrors;

        return analyzingErrors;
    }

    private List<IDiagnostic> AnalyzeInternal(List<SyntaxBase> declarations)
    {
        List<IDiagnostic> diagnostics = new();

        AnalyzeRule001(declarations, diagnostics);
        AnalyzeRule002(declarations, diagnostics);
        AnalyzeRule003(declarations, diagnostics);

        return diagnostics;
    }

    private IDiagnostic CreateDiagnostic(DiagnosticLevel level, string code, string message, string? value)
    {
        if (!String.IsNullOrWhiteSpace(value))
            message = $"Invalid Value: '{value}'. {message}";
        return new Diagnostic(TextSpan.Nil, level, DiagnosticSource.CoreLinter, code, message);
    }

    private void AnalyzeRule001(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM001 | Error
        // The module name metadata must be specified first and should contain
        // the value with the resource name in plural form. For example, 'Elastic SANs'.

        int declarationNumber = 0; // Must be 1st

        bool isValid = false;
        string? msgValue = null;
        if (declarations.Count > declarationNumber)
        {
            var decl = declarations[declarationNumber];

            if (decl is MetadataDeclarationSyntax metadata)
            {
                if (metadata.Name.IdentifierName == "name" && metadata.Value is StringSyntax value)
                {
                    msgValue = value.TryGetLiteralValue();
                    if (!String.IsNullOrWhiteSpace(msgValue))
                    {
                        // TODO String pattern matching
                        if (msgValue.EndsWith("s"))
                            isValid = true;
                    }
                }
            }
        }

        if (!isValid)
            diagnostics.Add(CreateDiagnostic(DiagnosticLevel.Error, "AVM001", "The module name metadata must be specified first and should contain the value with the resource name in plural form. For example, 'Elastic SANs'.", msgValue));
    }

    private void AnalyzeRule002(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM002 | Error
        // The module description metadata must be specified second and must contain
        // the value with the resource name in singular form, starting with the phrase
        // 'This module deploys a'. For example, 'This module deploys an Elastic SAN'.

        int declarationNumber = 1; // Must be 2nd

        bool isValid = false;
        string? msgValue = null;
        if (declarations.Count > declarationNumber)
        {
            var decl = declarations[declarationNumber];

            if (decl is MetadataDeclarationSyntax metadata)
            {
                if (metadata.Name.IdentifierName == "description" && metadata.Value is StringSyntax value)
                {
                    msgValue = value.TryGetLiteralValue();
                    if (!String.IsNullOrWhiteSpace(msgValue))
                    {
                        // TODO String pattern matching
                        if (msgValue.StartsWith("This module deploys a"))
                            isValid = true;
                    }
                }
            }
        }

        if (!isValid)
            diagnostics.Add(CreateDiagnostic(DiagnosticLevel.Error, "AVM002", "The module description metadata must be specified second and must contain the value with the resource name in singular form, starting with the phrase 'This module deploys a'. For example, 'This module deploys an Elastic SAN'.", msgValue));
    }

    private void AnalyzeRule003(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
    {
        // AVM003 | Error | Module owner is not specified correctly.
        // The module owner is not specified correctly.
        // It must be specified as the third metadata statement
        // with the value "metadata owner = 'Azure/module-maintainers'".

        int declarationNumber = 2; // Must be 3rd

        bool isValid = false;
        string? msgValue = null;
        if (declarations.Count > declarationNumber)
        {
            msgValue = declarations[declarationNumber].ToString();
            if (msgValue == "metadata owner = 'Azure/module-maintainers'")
                isValid = true;
        }

        if (!isValid)
            diagnostics.Add(CreateDiagnostic(DiagnosticLevel.Error, "AVM003", "The module owner is not specified correctly. It must be specified as the third metadata statement with the value \"metadata owner = 'Azure/module-maintainers'\".", msgValue));
    }
}
