using System.Text.RegularExpressions;

namespace winbash.util;

public static class PathUtil
{
    public static readonly string HomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    
    public static string Unwrap(string path)
    {
        bool abs = Regex.IsMatch(path, "([A-Z]:)?/[a-zA-Z0-9-_./]*");
        path = path.Replace("~", HomeDir);
        if (!abs)
            path = Path.Combine(Environment.CurrentDirectory, path);
        return path;
    }

    public static string Wrap(string path)
    {
        return path.Replace(HomeDir, "~");
    }

    public static string? Get(string path)
    {
        path = Unwrap(path);
        if (!Directory.Exists(path))
        {
            Console.WriteLine("Not a directory: " + path);
            return null;
        }
        return path;
    }
}