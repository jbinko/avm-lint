using System.CommandLine;
using System.CommandLine.Parsing;

using Bicep.Core.Diagnostics;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        return await ExecuteCommandsAsync(args);
    }

    private static async Task<int> ExecuteCommandsAsync(string[] args)
    {
        var parseRuleIDs = new ParseRuleIDs();

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

        var onlyRulesOption = new Option<IEnumerable<string>>(
            name: "--only-rules",
            description: "Specifies a list of rule IDs to restrict linting checks exclusively to the specified rules, excluding all other rules. The rules can be provided as a comma-separated.",
            parseArgument: parseRuleIDs.ParseOnlyRulesIDs
        )
        { AllowMultipleArgumentsPerToken = true };

        var excludeRulesOption = new Option<IEnumerable<string>>(
            name: "--exclude-rules",
            description: "Excludes specific rules from the linting process. All other rules that are not mentioned will be included by default in the linting process. Specify rule IDs as a comma-separated list to exempt them from checks.",
            parseArgument: parseRuleIDs.ParseExcludeRulesIDs
        )
        { AllowMultipleArgumentsPerToken = true };

        var issueThresholdOption = new Option<uint>(
            "--issue-threshold",
            "Specifies the maximum number of issues (including errors and warnings) tolerated before terminating the linting process early."
        );

        var ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var rootCommand = new RootCommand($"Azure Verified Modules Lint [Version {ver}]\nCopyright (c) 2024 Jiri Binko. All rights reserved.")
        {
            pathOption,
            recursiveOption,
            fileFilterOption,
            onlyRulesOption,
            excludeRulesOption,
            issueThresholdOption
        };

        int returnCode = 0;
        rootCommand.SetHandler((path, recursive, fileFilter, issueThreshold) =>
        {
            returnCode = ExecuteRootCommand(path, recursive, fileFilter, parseRuleIDs.AnalyzeRules, issueThreshold);
        }, pathOption, recursiveOption, fileFilterOption, issueThresholdOption);

        await rootCommand.InvokeAsync(args);
        return returnCode;
    }

    private static int ExecuteRootCommand(FileSystemInfo path, bool recursive, string fileFilter, IAnalyzeRules analyzeRules, uint issueThreshold)
    {
        try
        {
            var start = DateTime.Now;
            var files = FilesFinder.GetFiles(path, recursive, fileFilter);
            AnalyzeAndPrint(files, analyzeRules, start, issueThreshold);
            return 0;
        }
        catch (Exception e)
        {
            PrintMessage(Console.Error, $"Internal error: {e.Message}", ConsoleColor.Red);
            return 1;
        }
    }

    private static void AnalyzeAndPrint(List<string> files, IAnalyzeRules analyzeRules, DateTime start, uint issueThreshold)
    {
        var errorCount = 0;
        var warningCount = 0;
        var issueThresholdReached = false;

        foreach (var filePath in files)
        {
            if (issueThresholdReached)
                break;

            var findings = Analyzer.Analyze(filePath, analyzeRules);
            if (findings.Count == 0)
            {
                PrintMessage(Console.Out, filePath, ConsoleColor.Green);
            }
            else
            {
                var errors = findings.Where(f => f.Level == DiagnosticLevel.Error);
                if (errors.Any())
                {
                    PrintMessage(Console.Out, filePath, ConsoleColor.Red);
                }
                else
                {
                    PrintMessage(Console.Out, filePath, ConsoleColor.Yellow);
                }

                foreach (var finding in findings)
                {
                    var msg = $"{finding.Level}: {finding.Code} - {finding.Message}";
                    if (finding.Level == DiagnosticLevel.Error)
                    {
                        errorCount++;
                        PrintMessage(Console.Out, msg, ConsoleColor.Red, " => ");
                    }
                    else
                    {
                        warningCount++;
                        PrintMessage(Console.Out, msg, ConsoleColor.Yellow, " => ");
                    }

                    if (issueThreshold != 0 && (errorCount + warningCount) >= issueThreshold)
                    {
                        issueThresholdReached = true;
                        break;
                    }
                }
            }
        }

        if (issueThresholdReached)
        {
            Console.WriteLine();
            Console.WriteLine($"Issue threshold of {issueThreshold} reached. Terminating linting process early.");
        }

        Console.WriteLine();
        Console.WriteLine($"Linting for {analyzeRules.ActiveRulesCount} active rule(s) out of {analyzeRules.TotalRulesCount} rules. Completed in {((DateTime.Now - start).TotalMilliseconds / 1000.0):0.##} seconds.");
        Console.WriteLine($"Found {errorCount} error(s), {warningCount} warning(s).");
    }

    private static void PrintMessage(TextWriter tw, string message, ConsoleColor color, string indent = "")
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        tw.WriteLine($"{indent}{message}");
        Console.ForegroundColor = oldColor;
    }
}
