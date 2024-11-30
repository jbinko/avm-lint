using System.CommandLine.Parsing;

internal sealed class ParseRuleIDs
{
    public IAnalyzeRules AnalyzeRules { get; } = new AnalyzeRules();

    public List<string> ParseOnlyRulesIDs(ArgumentResult arg)
    {
        return ParseAndValidate(arg, AnalyzeRules.SetOnlyRules);
    }

    public List<string> ParseExcludeRulesIDs(ArgumentResult arg)
    {
        return ParseAndValidate(arg, AnalyzeRules.SetExcludeRules);
    }

    private List<string> ParseAndValidate(ArgumentResult arg, Func<List<string>, string> setRulesAction)
    {
        var ruleIDs = arg.Tokens
            .SelectMany(token => token.Value.Split(','))
            .Select(tokenItem => tokenItem.Trim())
            .Where(tokenItemValue => !string.IsNullOrWhiteSpace(tokenItemValue))
            .ToList();

        var errorMessage = setRulesAction.Invoke(ruleIDs);

        if (!string.IsNullOrWhiteSpace(errorMessage))
            arg.ErrorMessage = $"One or more specified rules: {errorMessage} do not exist.";

        return ruleIDs;
    }
}
