using System.Diagnostics;
using comroid.common;
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
            Console.Title = $"{Environment.UserName}@{Environment.UserDomainName}:{PathUtil.Wrap(Environment.CurrentDirectory)}";
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
                case "cd":
                    if (PathUtil.Get(string.Join(" ", args)) is { } path1)
                        Environment.CurrentDirectory = path1;
                    break;
                case "which":
                    if (PathUtil.Which(cmds[1]) is { } exe0) 
                        Console.WriteLine(exe0);
                    else Console.WriteLine($"which: {cmds[1]}: Command not found");
                    break;
                default:
                    if (PathUtil.Which(cmds[0]) is not { } exe1)
                    {
                        Console.WriteLine($"{cmds[0]}: Command not found");
                        break;
                    }
                    var startInfo = new ProcessStartInfo(exe1) { Arguments = string.Join(" ", args) };
                    var process = new Process() { StartInfo = startInfo };
                    process.Start();
                    process.WaitForExit();
                    Console.WriteLine($"{cmds[0]}: Exited with code {process.ExitCode} (0x{process.ExitCode:x8})");
                    process.Close();
                    break;
            }
        }
    }
}