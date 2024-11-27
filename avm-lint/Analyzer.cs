using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

namespace avm_lint;

internal sealed class Analyzer
{
    public List<IDiagnostic> Analyze(string filePath, AnalyzeRules analyzeRules)
    {
        var bicepCodeText = File.ReadAllText(filePath);
        var parser = new Parser(bicepCodeText);
        var lexingErrors = new List<IDiagnostic>(parser.LexingErrorLookup);
        var parsingErrors = new List<IDiagnostic>(parser.ParsingErrorLookup);
        var analyzingErrors = analyzeRules.Analyze(new LintVisitor().GetDeclarations(parser));

        if (lexingErrors.Count > 0)
            return lexingErrors;

        if (parsingErrors.Count > 0)
            return parsingErrors;

        if (analyzingErrors.Count > 0)
            return analyzingErrors;

        return analyzingErrors;
    }
}
