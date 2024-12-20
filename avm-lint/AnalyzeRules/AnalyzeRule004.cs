﻿using Bicep.Core.Syntax;
using Bicep.Core.Diagnostics;

internal sealed class AnalyzeRule004 : AnalyzeRuleBase, IAnalyzeRule
{
    public string Code => "AVM004";

    public void Analyze(IAnalyzeContext context)
    {
        // AVM004 | Error
        // The 'targetScope' (without any decorators) can only be used with 'subscription', 'managementGroup', or 'tenant' value.
        // It cannot be used with 'resourceGroup'. When 'targetScope' is specified, it must be
        // the first statement following the metadata section.

        var targetScopeCount = context.Declarations.Count(ts => ts is TargetScopeSyntax);
        if (targetScopeCount != 0 && targetScopeCount != 1) // It is optional but can be only one when specified
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        if (targetScopeCount == 0) // No targetScope specified - which is fine
        {
            return;
        }

        if (!TryGetTargetScopeAfterMetadata(context.Declarations, out var targetScope))
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        if (!IsValidTargetScopeDeclaration(targetScope!, out var targetScopeValue))
        {
            AddDiagnostic(context.Diagnostics, null);
            return;
        }

        // It cannot be used with 'resourceGroup'
        var allowedValues = new[] { "tenant", "managementGroup", "subscription" };
        if (!allowedValues.Any(targetScopeValue.Contains))
        {
            AddDiagnostic(context.Diagnostics, targetScopeValue);
        }
    }

    private bool TryGetTargetScopeAfterMetadata(List<SyntaxBase> declarations, out TargetScopeSyntax? targetScope)
    {
        targetScope = null;

        // Find the last metadata declaration in the consecutive metadata declarations
        var metadataSectionEndIndex = 0;
        foreach (var declaration in declarations)
        {
            if (declaration is not MetadataDeclarationSyntax)
            {
                if (metadataSectionEndIndex == 0)
                {
                    return false; // Metadata section must exist
                }

                // We found metadata section and first non-metadata declaration statement
                break;
            }
            metadataSectionEndIndex++;
        }

        // Ensure that the next statement is of type TargetScopeSyntax
        if (declarations.Count > metadataSectionEndIndex && declarations[metadataSectionEndIndex] is TargetScopeSyntax targetScopeSyntax)
        {
            targetScope = targetScopeSyntax;
            return true;
        }

        return false;
    }

    private void AddDiagnostic(List<IDiagnostic> diagnostics, string? msgValue)
    {
        diagnostics.Add(DiagnosticFactory.Create(
            DiagnosticLevel.Error,
            Code,
            "The 'targetScope' (without any decorators) can only be used with 'subscription', 'managementGroup', or 'tenant' value. It cannot be used with 'resourceGroup'. When 'targetScope' is specified, it must be the first statement following the metadata section.",
            msgValue));
    }
}
