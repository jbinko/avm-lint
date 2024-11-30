using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRules : IAnalyzeRules
{
    internal sealed class AnalyzeRuleDefinition
    {
        public bool Execute { get; set; } = true;
        public required IAnalyzeRule Rule { get; set; }
    }

    private readonly Dictionary<string, AnalyzeRuleDefinition> _rules = new()
    {
        { "AVM001", new AnalyzeRuleDefinition { Rule = new AnalyzeRule001() } },
        { "AVM002", new AnalyzeRuleDefinition { Rule = new AnalyzeRule002() } },
        { "AVM003", new AnalyzeRuleDefinition { Rule = new AnalyzeRule003() } },
    };

    public string SetOnlyRules(List<string> rules)
    {
        // Disable all rules first
        foreach (var rule in _rules.Values)
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
            if (_rules.TryGetValue(rule, out var ruleDefinition))
            {
                ruleDefinition.Execute = execute;
            }
            else
            {
                invalidRules.Add(rule);
            }
        }

        return string.Join(",", invalidRules.Select(v => $"'{v}'"));
    }

    public List<IDiagnostic> Analyze(List<SyntaxBase> declarations)
    {
        var diagnostics = new List<IDiagnostic>();

        foreach (var rule in _rules.Values)
        {
            if (rule.Execute)
            {
                rule.Rule.Analyze(declarations, diagnostics);
            }
        }

        return diagnostics;
    }

    public int ActiveRulesCount => _rules.Values.Count(x => x.Execute);
}
