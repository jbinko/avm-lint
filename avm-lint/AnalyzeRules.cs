using Bicep.Core.Syntax;
using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

namespace avm_lint;

internal sealed class AnalyzeRules
{
    internal sealed class AnalyzeRuleDefinition
    {
        public bool Execute { get; set; } = true;
        public required Action<List<SyntaxBase>, List<IDiagnostic>> RuleAction { get; set; }
    }

    private Dictionary<string, AnalyzeRuleDefinition> Rules { get; } = new()
    {
        { "AVM001", new AnalyzeRuleDefinition { RuleAction = AnalyzeRule001 } },
        { "AVM002", new AnalyzeRuleDefinition { RuleAction = AnalyzeRule002 } },
        { "AVM003", new AnalyzeRuleDefinition { RuleAction = AnalyzeRule003 } },
    };

    public string SetOnlyRules(List<string> rules)
    {
        // Disable all rules first
        foreach (var rule in Rules)
        {
            rule.Value.Execute = false;
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
            if (Rules.TryGetValue(rule, out var ruleDefinition))
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

        foreach (var rule in Rules)
        {
            if (rule.Value.Execute)
            {
                rule.Value.RuleAction(declarations, diagnostics);
            }
        }

        return diagnostics;
    }

    public int ActiveRulesCount => Rules.Values.Count(x => x.Execute);

    private static IDiagnostic CreateDiagnostic(DiagnosticLevel level, string code, string message, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            message = $"Invalid Value: '{value}'. {message}";
        }
        return new Diagnostic(TextSpan.Nil, level, DiagnosticSource.CoreLinter, code, message);
    }

    private static void AnalyzeRule001(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
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

    private static void AnalyzeRule002(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
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

    private static void AnalyzeRule003(List<SyntaxBase> declarations, List<IDiagnostic> diagnostics)
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
