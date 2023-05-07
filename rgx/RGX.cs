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
        using var input = cmd is { split: true, replacement: not null } ? new StringReader(cmd.replacement) : Console.In;
        using var output = cmd.fileOutput is not null ? new StreamWriter(new FileStream(cmd.fileOutput, FileMode.Truncate, FileAccess.Write)) : Console.Out;

        if (cmd.split)
        {
            var newPattern = cmd.pattern;
            if (newPattern.StartsWith('^'))
                // do not match beginning of string
                newPattern = newPattern.Substring(1);
            if (!newPattern.EndsWith('$'))
                newPattern += '$';
            pattern = new Regex(newPattern);
            int c;
            var buf = new StringWriter();
            while ((c = input.Read()) != -1)
            {
                buf.Write((char)c);
                output.Write((char)c);

                var str = buf.ToString();
                if (!pattern.IsMatch(str))
                    continue;
                buf.Close();
                buf = new StringWriter();
                output.WriteLine();
            }
        }
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
        [Value(1)]
        public IEnumerable<string> excess { get; set; }

        [Option(shortName: 'o', longName: "options", Separator = ',', Required = false)]
        public IEnumerable<RegexOptions> options { get; set; } = null!;
        [Option(shortName: 's', longName: "split", Required = false, Default = false)]
        public bool split { get; set; }
        [Option(shortName: 'f', longName: "file", Required = false, Default = null)]
        public string? fileOutput { get; set; }
    }
}