using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal interface IAnalyzeRules
{
    string SetOnlyRules(List<string> rules);
    string SetExcludeRules(List<string> rules);
    List<IDiagnostic> Analyze(IAnalyzeContext context, List<SyntaxBase> declarations);
    int TotalRulesCount { get; }
    int ActiveRulesCount { get; }
}
