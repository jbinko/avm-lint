using Bicep.Core.Syntax;
using Bicep.Core.Parsing;

internal sealed class LintVisitor : AstVisitor
{
    private List<SyntaxBase> _declarations = new();

    public List<SyntaxBase> GetDeclarations(Parser parser)
    {
        Visit(parser.Program());
        return _declarations;
    }

    public override void VisitProgramSyntax(ProgramSyntax syntax)
    {
        base.VisitProgramSyntax(syntax);
        _declarations = new List<SyntaxBase>(syntax.Declarations);
    }
}
