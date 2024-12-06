﻿using Bicep.Core.Parsing;
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
        var analyzingErrors = analyzeRules.Analyze(
            CreateContext(filePath),
            new LintVisitor().GetDeclarations(parser));
        return analyzingErrors;
    }

    private static AnalyzeContext CreateContext(string filePath)
    {
        return new AnalyzeContext() { IsSubmodule = false };
    }
}
