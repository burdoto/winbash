using System.Text.RegularExpressions;

namespace winbash.util;

public static class PathUtil
{
    public static string Unwrap(string path)
    {
        bool abs = Regex.IsMatch(path, "([A-Z]:)?/[a-zA-Z0-9-_./]*");
        path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        if (!abs)
            path = Path.Combine(Environment.CurrentDirectory, path);
        return path;
    }

    public static string Wrap(string path)
    {
        return path.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "~");
    }
}