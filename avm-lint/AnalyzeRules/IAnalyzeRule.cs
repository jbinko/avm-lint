internal interface IAnalyzeRule
{
    string Code { get; }
    void Analyze(IAnalyzeContext context);
}
