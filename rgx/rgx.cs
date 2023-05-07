using System.Text.RegularExpressions;
using CommandLine;
using comroid.common;

namespace winbash.rgx;

public class rgx
{

    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Command>(args)
            .WithParsed(Run)
            .WithNotParsed(Error);
    }

    private static void Run(Command cmd)
    {
        var pattern = new Regex(cmd.pattern);

        // handle IO
        while (Console.ReadLine() is { } line)
            if (pattern.IsMatch(line))
                Console.WriteLine(cmd.replacement == null ? line : pattern.Replace(line, cmd.replacement));
    }
    
    private static void Error(IEnumerable<Error> errors)
    {
        foreach (var err in errors)
            Log<rgx>.At(LogLevel.Error, err);
    }
}

public class Command
{
    [Value(0)] public string pattern { get; set; } = null!;
    [Value(1)] public string? replacement { get; set; }
}