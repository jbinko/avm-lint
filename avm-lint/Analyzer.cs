using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

internal sealed class Analyzer
{
    public static IAnalyzeContext Analyze(string filePath, IAnalyzeRules analyzeRules)
    {
        var context = new AnalyzeContext(filePath);
        var bicepCodeText = File.ReadAllText(filePath);
        var parser = new Parser(bicepCodeText);

        // Collect lexing errors
        var lexingErrors = new List<IDiagnostic>(parser.LexingErrorLookup);
        if (lexingErrors.Count > 0)
            return context.AddDiagnostics(lexingErrors);

        // Collect parsing errors
        var parsingErrors = new List<IDiagnostic>(parser.ParsingErrorLookup);
        if (parsingErrors.Count > 0)
            return context.AddDiagnostics(parsingErrors);

        // Collect analyzing errors
        context.AddDeclarations(new LintVisitor().GetDeclarations(parser));
        analyzeRules.Analyze(context);
        return context;
    }
}
