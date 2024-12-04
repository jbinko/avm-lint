using Bicep.Core.Syntax;

internal /*sealed*/ class AnalyzeRuleBase
{
    protected bool IsValidMetadataDeclaration(SyntaxBase decl, string identifierName, out string msgValue)
    {
        msgValue = "";

        if (decl is not MetadataDeclarationSyntax metadata ||
            metadata.Decorators.Any() ||
            metadata.Name.IdentifierName != identifierName ||
            metadata.Value is not StringSyntax value)
        {
            return false;
        }

        msgValue = value.TryGetLiteralValue() ?? "";
        return !string.IsNullOrWhiteSpace(msgValue);
    }

    protected bool IsValidTargetScopeDeclaration(SyntaxBase decl, out string scopeValue)
    {
        scopeValue = "";

        if (decl is not TargetScopeSyntax targetScope ||
            targetScope.Decorators.Any() ||
            targetScope.Value is not StringSyntax value)
        {
            return false;
        }

        scopeValue = value.TryGetLiteralValue() ?? "";
        return !string.IsNullOrWhiteSpace(scopeValue);
    }
}
