using System.CommandLine;

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

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            return 0;
        }
        catch (Exception e)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Internal error: {e.Message}");
            Console.ForegroundColor = c;
            return 1;
        }
    }
}
