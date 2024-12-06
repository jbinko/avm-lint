using Bicep.Core.Syntax;

internal sealed class AnalyzeRules : IAnalyzeRules
{
    internal sealed class AnalyzeRuleDefinition
    {
        public bool Execute { get; set; } = true;
        public required IAnalyzeRule Rule { get; set; }
    }

    private readonly List<AnalyzeRuleDefinition> _rules = new()
    {
        { new AnalyzeRuleDefinition { Rule = new AnalyzeRule001() } },
        { new AnalyzeRuleDefinition { Rule = new AnalyzeRule002() } },
        { new AnalyzeRuleDefinition { Rule = new AnalyzeRule003() } },
        { new AnalyzeRuleDefinition { Rule = new AnalyzeRule004() } },
    };

    public string SetOnlyRules(List<string> rules)
    {
        // Disable all rules first
        foreach (var rule in _rules)
        {
            rule.Execute = false;
        }

        return SetExecuteForSpecificRules(rules, true);
    }

    public string SetExcludeRules(List<string> rules)
    {
        return SetExecuteForSpecificRules(rules, false);
    }

    private string SetExecuteForSpecificRules(List<string> rules, bool execute)
    {
        var invalidRules = new List<string>();
        foreach (var rule in rules)
        {
            var ruleDefinition = _rules.FirstOrDefault(v => v.Rule.Code == rule);
            if (ruleDefinition == null)
            {
                invalidRules.Add(rule);
            }
            else
            {
                ruleDefinition.Execute = execute;
            }
        }

        return string.Join(",", invalidRules.Select(v => $"'{v}'"));
    }

    public void Analyze(IAnalyzeContext context)
    {
        foreach (var rule in _rules)
        {
            if (rule.Execute)
            {
                rule.Rule.Analyze(context);
            }
        }
    }

    public int TotalRulesCount => _rules.Count;
    public int ActiveRulesCount => _rules.Count(x => x.Execute);
}
