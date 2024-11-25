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

        var pathOption = new Option<FileSystemInfo>(
                    "--path",
                    "The Bicep file or directory to lint. If a directory is provided, all Bicep files within it are considered unless modified by other options."
                )
        {
            IsRequired = true,
        }.ExistingOnly();

        var recursiveOption = new Option<bool>(
            "--recursive",
            "Search recursively for files within the specified directory and its subdirectories. This is the default behavior."
        );
        recursiveOption.SetDefaultValue(true);

        var fileFilterOption = new Option<string>(
            "--file-filter",
            "A wildcard pattern to select which files to lint. Supports standard wildcard characters such as `*` (matches any sequence of characters) and `?` (matches any single character)."
        );
        fileFilterOption.SetDefaultValue("*main.bicep");

        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var rootCommand = new RootCommand($"Azure Verified Modules Lint [Version {ver}]\nCopyright (c) 2024 Jiri Binko. All rights reserved.")
        {
            pathOption,
            recursiveOption,
            fileFilterOption
        };

        rootCommand.SetHandler((path, recursive, fileFilter) =>
        {
            returnCode = ExecuteRootCommand(path, recursive, fileFilter);
        }, pathOption, recursiveOption, fileFilterOption);

        await rootCommand.InvokeAsync(args);

        return returnCode;
    }

    private static int ExecuteRootCommand(FileSystemInfo path, bool recursive, string fileFilter)
    {
        try
        {
            DateTime start = DateTime.Now;
            var files = FilesFinder.GetFiles(path, recursive, fileFilter);
            AnalyzeAndPrint(files, start);
            return 0;
        }
        catch (Exception e)
        {
            ConsoleError_Error($"Internal error: {e.Message}");
            return 1;
        }
    }

    private static void AnalyzeAndPrint(List<string> files, DateTime start)
    {
        int errorCount = 0, warningCount = 0;

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
                    {
                        errorCount++;
                        ConsoleOut_Error(msg, " => ");
                    }
                    else
                    {
                        warningCount++;
                        ConsoleOut_Warning(msg, " => ");
                    }
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Linting completed in {((DateTime.Now - start).TotalMilliseconds / 1000.0):0.##} seconds.\nFound {errorCount} error(s), {warningCount} warning(s).");
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
