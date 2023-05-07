using System.Text.RegularExpressions;
using CommandLine;
using comroid.common;
using winbash.util;

namespace winbash.rgx;

public static class RGX
{
    private static readonly ILog log = new Log("rgx");

    static RGX()
    {
        ILog.Detail = DetailLevel.None;
    }

    public static void Main(string[] args)
    {
        CLI.parser.ParseArguments<Arg>(args)
            .WithParsed(Run)
            .WithNotParsed(Error);
    }

    private static void Run(Arg cmd)
    {
        var pattern = new Regex(cmd.pattern, (RegexOptions)cmd.options.Aggregate(0, (x, y) => x | (int)y));

        // handle IO
        while (Console.ReadLine() is { } line)
            if (pattern.IsMatch(line))
                Console.WriteLine(cmd.replacement == null ? line : pattern.Replace(line, cmd.replacement));
    }
    
    private static void Error(IEnumerable<Error> errors)
    {
        foreach (var err in errors)
            log.At(err.StopsProcessing ? LogLevel.Fatal : LogLevel.Error, err);
    }

    private class Arg
    {
        [Value(0, MetaName = "pattern")]
        public string pattern { get; set; } = null!;
        [Value(1, MetaName = "replacement", Required = false)]
        public string? replacement { get; set; }
        [Option(shortName: 'o', longName: "options", Separator = ',')]
        public IEnumerable<RegexOptions> options { get; set; }
    }
}