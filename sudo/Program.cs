using System.Diagnostics;
using winbash.util;

namespace winbash.sudo;

public static class Program
{
    public static void Main(string[] param)
    {
        if (param.Length == 0)
            return;
        
        var args = new string[param.Length - 1];
        Array.Copy(param, 1, args, 0, args.Length);
        var cmd = PathUtil.Which(param[0]);

        if (cmd == null)
        {
            Console.WriteLine($"sudo: Program not found: {cmd}");
            return;
        }

        var startInfo = new ProcessStartInfo(cmd)
        {
            Arguments = string.Join(" ", args),
            WorkingDirectory = Environment.CurrentDirectory,
            UseShellExecute = true,
            Verb = "runas"
        };
        // todo fixme: does not print to stdout 
        Process.Start(startInfo)!.WaitForExit();
    }
}