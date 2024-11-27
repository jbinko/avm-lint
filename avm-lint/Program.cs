using System.CommandLine;
using System.CommandLine.Parsing;

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
        var analyzeRules = new AnalyzeRules();

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
            "--only-rules",
            "Specifies a list of rule IDs to restrict linting checks exclusively to the specified rules, excluding all other rules. The rules can be provided as a space-separated."
        )
        { AllowMultipleArgumentsPerToken = true };

        onlyRulesOption.AddValidator(option =>
        {
            ValidateExistingRules(option, () => analyzeRules.SetOnlyRules(option.Tokens.Select(t => t.Value).ToList()));
        });

        var excludeRulesOption = new Option<IEnumerable<string>>(
            "--exclude-rules",
            "Excludes specific rules from the linting process. All other rules that are not mentioned will be included by default in the linting process. Specify rule IDs as a space-separated list to exempt them from checks."
        )
        { AllowMultipleArgumentsPerToken = true };

        excludeRulesOption.AddValidator(option =>
        {
            ValidateExistingRules(option, () => analyzeRules.SetExcludeRules(option.Tokens.Select(t => t.Value).ToList()));
        });

        var issueThresholdOption = new Option<UInt32>(
            "--issue-threshold",
            "Specifies the maximum number of issues (including errors and warnings) tolerated before terminating the linting process early."
        );
        // issueThresholdOption.SetDefaultValue(0); // It will be 0 by default and it will not be mentioned in the help

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

        rootCommand.SetHandler((path, recursive, fileFilter, issueThreshold) =>
        {
            returnCode = ExecuteRootCommand(path, recursive, fileFilter, analyzeRules, issueThreshold);
        }, pathOption, recursiveOption, fileFilterOption, issueThresholdOption);

        await rootCommand.InvokeAsync(args);

        return returnCode;
    }

    private static void ValidateExistingRules(OptionResult option, Func<string> rulesFunction)
    {
        var errorMessage = rulesFunction.Invoke();

        if (!String.IsNullOrWhiteSpace(errorMessage))
            option.ErrorMessage = $"One or more specified rules: {errorMessage} do not exist.\nEnsure you are using a space as the separator.";
    }

    private static int ExecuteRootCommand(FileSystemInfo path, bool recursive, string fileFilter, AnalyzeRules analyzeRules, UInt32 issueThreshold)
    {
        try
        {
            DateTime start = DateTime.Now;
            var files = FilesFinder.GetFiles(path, recursive, fileFilter);
            AnalyzeAndPrint(files, analyzeRules, start, issueThreshold);
            return 0;
        }
        catch (Exception e)
        {
            ConsoleError_Error($"Internal error: {e.Message}");
            return 1;
        }
    }

    private static void AnalyzeAndPrint(List<string> files, AnalyzeRules analyzeRules, DateTime start, UInt32 issueThreshold)
    {
        bool issueThresholdReached = false;
        int errorCount = 0, warningCount = 0;

        foreach (var filePath in files)
        {
            if (issueThresholdReached)
                break;

            var findings = new Analyzer().Analyze(filePath, analyzeRules);
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
        Console.WriteLine($"Linting for {analyzeRules.ActiveRulesCount} active rule(s) completed in {((DateTime.Now - start).TotalMilliseconds / 1000.0):0.##} seconds.\nFound {errorCount} error(s), {warningCount} warning(s).");
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
