using System.CommandLine;
using Bicep.Core.Diagnostics;

namespace avm_lint;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        return await ExecuteCommandsAsync(args);
    }

    private static async Task<int> ExecuteCommandsAsync(string[] args)
    {
        int returnCode = 0;

        var sourceOption = new Option<FileSystemInfo>(
                    "--source",
                    "Source file (Bicep) or directory that contains the Bicep files."
                )
        {
            IsRequired = true,
        }.ExistingOnly();

        var recursiveOption = new Option<bool>(
            "--recursive",
            "Search recursively for Bicep files within the specified directory and its subdirectories."
        );
        recursiveOption.SetDefaultValue(true);

        var filterOption = new Option<string>(
            "--filter",
            "The filter string is used to match the names of files, supporting wildcard characters (* and ?)."
        );
        filterOption.SetDefaultValue("*main.bicep");

        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var rootCommand = new RootCommand($"Azure Verified Modules Lint [Version {ver}]\nCopyright (c) 2024 Jiri Binko. All rights reserved.")
        {
            sourceOption,
            recursiveOption,
            filterOption
        };

        rootCommand.SetHandler((source, recursive, filter) =>
        {
            returnCode = ExecuteRootCommand(source, recursive, filter);
        }, sourceOption, recursiveOption, filterOption);

        await rootCommand.InvokeAsync(args);

        return returnCode;
    }

    private static int ExecuteRootCommand(FileSystemInfo source, bool recursive, string filter)
    {
        try
        {
            var files = FilesFinder.GetFiles(source, recursive, filter);
            AnalyzeAndPrint(files);
            return 0;
        }
        catch (Exception e)
        {
            ConsoleError_Error($"Internal error: {e.Message}");
            return 1;
        }
    }

    private static void AnalyzeAndPrint(List<string> files)
    {
        foreach (var filePath in files)
        {
            var findings = new Analyzer().Analyze(filePath);
            if (findings.Count == 0)
                ConsoleOut_OK(filePath);
            else
            {
                var errors = findings.Where(f => f.Level == DiagnosticLevel.Error);
                if (errors.Any())
                    ConsoleOut_Error(filePath);
                else
                    ConsoleOut_Warning(filePath);

                foreach (var finding in findings)
                {
                    var msg = $"{finding.Level}: {finding.Code} - {finding.Message}";
                    if (finding.Level == DiagnosticLevel.Error)
                        ConsoleOut_Error(msg, " => ");
                    else
                        ConsoleOut_Warning(msg, " => ");
                }
            }
        }
    }

    private static void ConsoleError_Error(string message)
    {
        PrintMessageInternal(Console.Error, message, ConsoleColor.Red);
    }

    private static void ConsoleOut_Error(string message, string indent = "")
    {
        PrintMessageInternal(Console.Out, $"{indent}{message}", ConsoleColor.Red);
    }

    private static void ConsoleOut_Warning(string message, string indent = "")
    {
        PrintMessageInternal(Console.Out, $"{indent}{message}", ConsoleColor.Yellow);
    }

    private static void ConsoleOut_OK(string message, string indent = "")
    {
        PrintMessageInternal(Console.Out, $"{indent}{message}", ConsoleColor.Green);
    }

    private static void PrintMessageInternal(TextWriter tw, string message, ConsoleColor c)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = c;
        tw.WriteLine(message);
        Console.ForegroundColor = oldColor;
    }
}
