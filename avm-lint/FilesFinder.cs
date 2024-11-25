namespace avm_lint;

internal sealed class FilesFinder
{
    public static List<string> GetFiles(FileSystemInfo path, bool recursive, string filter)
    {
        switch (path)
        {
            case FileInfo file:
                return new List<string> { file.FullName };
            case DirectoryInfo directory:
                return GetFiles(directory.FullName, recursive, filter);
        }

        throw new InvalidOperationException($"Not supported FileSystemInfo: '{path.GetType().Name}'.");
    }

    private static List<string> GetFiles(string directoryPath, bool recursive, string filter)
    {
        var fileNamesList = new List<string>();
        GetFilesRecursive(directoryPath, recursive, filter, ref fileNamesList);
        return fileNamesList;
    }

    private static void GetFilesRecursive(string directoryPath, bool recursive, string filter, ref List<string> fileNamesList)
    {
        var files = Directory.GetFiles(directoryPath, filter);
        foreach (var file in files)
            fileNamesList.Add(file);

        if (recursive)
        {
            foreach (var subDir in Directory.GetDirectories(directoryPath))
                GetFilesRecursive(subDir, recursive, filter, ref fileNamesList);
        }
    }
}
