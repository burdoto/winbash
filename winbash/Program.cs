using winbash.util;

namespace winbash;

public static class Program
{
    public static void Main(string[] _)
    {
        bool exiting = false;
        
        AnsiUtil.Init();

        while (!exiting)
        {
            Console.Write($"{AnsiUtil.Green}{Environment.UserName}@{Environment.UserDomainName} {AnsiUtil.Blue}{PathUtil.Wrap(Environment.CurrentDirectory)} $ {AnsiUtil.Reset}");
            var cmd = Console.ReadLine();
            if (string.IsNullOrEmpty(cmd))
                continue;
            var cmds = cmd.Split(" ");
            if (cmds.Length == 0)
                continue;
            var args = new string[cmds.Length - 1];
            Array.Copy(cmds, 1, args, 0, args.Length);
            switch (cmds[0])
            {
                case "exit":
                    exiting = true;
                    break;
                case "ls":
                    break;
                case "cd":
                    var path = string.Join(" ", args);
                    path = PathUtil.Unwrap(path);
                    if (path == null || !Directory.Exists(path))
                    {
                        Console.WriteLine("Not a directory: " + path);
                        break;
                    }
                    Environment.CurrentDirectory = path;
                    break;
            }
        }
    }
}