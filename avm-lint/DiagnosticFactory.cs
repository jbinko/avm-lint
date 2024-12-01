using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;

internal sealed class DiagnosticFactory
{
    public static IDiagnostic Create(DiagnosticLevel level, string code, string message, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            message = $"Invalid value: '{value}'. {message}";
        }

        return new Diagnostic(TextSpan.Nil, level, DiagnosticSource.CoreLinter, code, message);
    }
}
