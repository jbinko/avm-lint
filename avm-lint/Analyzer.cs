using Bicep.Core.Syntax;
using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

internal sealed class Analyzer
{
    public static List<IDiagnostic> Analyze(string filePath, IAnalyzeRules analyzeRules)
    {
        var bicepCodeText = File.ReadAllText(filePath);
        var parser = new Parser(bicepCodeText);

        // Collect lexing errors
        var lexingErrors = new List<IDiagnostic>(parser.LexingErrorLookup);
        if (lexingErrors.Count > 0)
            return lexingErrors;

        // Collect parsing errors
        var parsingErrors = new List<IDiagnostic>(parser.ParsingErrorLookup);
        if (parsingErrors.Count > 0)
            return parsingErrors;

        // Collect analyzing errors
        var context = CreateContext(filePath, new LintVisitor().GetDeclarations(parser));
        analyzeRules.Analyze(context);
        return context.Diagnostics;
    }

    private static AnalyzeContext CreateContext(string filePath, List<SyntaxBase> declarations)
    {
        return new AnalyzeContext()
        {
            IsSubmodule = false,
            Declarations = declarations,
            Diagnostics = new List<IDiagnostic>(),
        };
    }
}
