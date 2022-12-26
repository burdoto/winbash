using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace winbash.util
    // ReSharper disable once ArrangeNamespaceBody
{
    public static class PathUtil
    {
        private static readonly string BashHomeDir =
            new DirectoryInfo(typeof(PathUtil).Assembly.Location).Parent!.FullName;

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

        public static string? Which(string cmd) => new[] { Environment.CurrentDirectory, BashHomeDir }
            .Concat(Environment.GetEnvironmentVariable("path")
                ?.Split(Path.PathSeparator) ?? Array.Empty<string>())
            .Select(dir => Path.Combine(dir, cmd + ".exe"))
            .FirstOrDefault(File.Exists);
    }
}
