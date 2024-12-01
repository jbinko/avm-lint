using Bicep.Core.Syntax;

internal /*sealed*/ class AnalyzeRuleBase
{
    protected bool IsValidMetadataDeclaration(SyntaxBase decl, string identifierName, out string msgValue)
    {
        msgValue = "";

        if (decl is not MetadataDeclarationSyntax metadata ||
            metadata.Name.IdentifierName != identifierName ||
            metadata.Value is not StringSyntax value)
        {
            return false;
        }

        msgValue = value.TryGetLiteralValue() ?? "";
        return !string.IsNullOrWhiteSpace(msgValue);
    }
}
