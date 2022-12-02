internal partial class BigBrother
{
    private string GetPath(params string[] paths)
    {
        string abs_path = localPath;
        foreach (string path in paths)
            abs_path = Path.Combine(abs_path, path);
        return abs_path;
    }
}
