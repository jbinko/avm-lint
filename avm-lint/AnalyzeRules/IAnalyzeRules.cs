internal interface IAnalyzeRules
{
    string SetOnlyRules(List<string> rules);
    string SetExcludeRules(List<string> rules);
    void Analyze(IAnalyzeContext context);
    int TotalRulesCount { get; }
    int ActiveRulesCount { get; }
}
