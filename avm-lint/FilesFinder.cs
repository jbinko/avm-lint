internal sealed class FilesFinder
{
    public static List<string> GetFiles(FileSystemInfo path, bool recursive, string filter)
    {
        return path switch
        {
            FileInfo file => new List<string> { file.FullName },
            DirectoryInfo directory => GetFiles(directory.FullName, recursive, filter),
            _ => throw new InvalidOperationException($"Not supported FileSystemInfo: '{path.GetType().Name}'.")
        };
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
        fileNamesList.AddRange(files);

        if (recursive)
        {
            foreach (var subDir in Directory.GetDirectories(directoryPath))
                GetFilesRecursive(subDir, recursive, filter, ref fileNamesList);
        }
    }
}
