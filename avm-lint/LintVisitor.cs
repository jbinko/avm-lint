using Bicep.Core.Syntax;
using Bicep.Core.Parsing;

namespace avm_lint;

internal sealed class LintVisitor : AstVisitor // CstVisitor
{
    public List<SyntaxBase> GetDeclarations(Parser parser)
    {
        base.Visit(parser.Program());
        return _declarations;
    }

    public override void VisitProgramSyntax(ProgramSyntax syntax)
    {
        base.VisitProgramSyntax(syntax);
        _declarations = new(syntax.Declarations);
    }

    private List<SyntaxBase> _declarations = new();
}
