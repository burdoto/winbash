using System.Text.RegularExpressions;
using CommandLine;
using comroid.common;
using winbash.util;

namespace winbash.rgx;

public static class RGX
{
    private static readonly ILog log = new Log("rgx");
    private static string[] excessArgs;

    static RGX()
    {
        ILog.Detail = DetailLevel.None;
    }

    public static void Main(string[] args)
    {
        var result = CLI.parser.ParseArguments<Arg>(args);
        excessArgs = result.MapResult(_ => Enumerable.Empty<string>(), 
            errs => errs.OfType<UnknownOptionError>().Select(e => e.Token)).ToArray();
        result.WithParsed(Run).WithNotParsed(Error);
    }

    private static void Run(Arg cmd)
    {
        var pattern = new Regex(cmd.pattern, (RegexOptions)cmd.options.Aggregate(0, (x, y) => x | (int)y));
        using var input = excessArgs.Length > 0 ? new StringReader(string.Join(' ', excessArgs)) : Console.In;
        using var output = cmd.fileOutput is not null ? new StreamWriter(new FileInfo(cmd.fileOutput!).OpenWrite()) : Console.Out;

        if (cmd.split)
            foreach (var part in pattern.Split(input.ReadToEnd()))
                output.WriteLine(part);
        else while (input.ReadLine() is { } line)
            if (pattern.IsMatch(line)) 
                output.WriteLine(cmd.replacement == null ? line : pattern.Replace(line, cmd.replacement));
    }

    private static void Error(IEnumerable<Error> errors)
    {
        foreach (var err in errors)
            log.At(err.StopsProcessing ? LogLevel.Fatal : LogLevel.Error, err);
    }

    private class Arg
    {
        [Value(0, MetaName = "pattern", Required = true)]
        public string pattern { get; set; } = null!;

        [Value(1, MetaName = "replacement", Required = false, Default = null)]
        public string? replacement { get; set; }

        [Option(shortName: 'o', longName: "options", Separator = ',', Required = false)]
        public IEnumerable<RegexOptions> options { get; set; } = null!;
        [Option(shortName: 's', longName: "split", Required = false, Default = false)]
        public bool split { get; set; }
        [Option(shortName: 'f', longName: "file", Required = false, Default = null)]
        public string? fileOutput { get; set; }
    }
}