using System.CommandLine;

namespace avm_lint;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var rootCommand = new RootCommand($"Azure Verified Modules Lint [Version {ver}]\nCopyright (c) 2024 Jiri Binko. All rights reserved.");
        return await rootCommand.InvokeAsync(args);
    }
}
