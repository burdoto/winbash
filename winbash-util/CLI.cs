using System;
using System.Globalization;
using CommandLine;

namespace winbash.util;

public static class CLI
{
    public static readonly Parser parser = new(cfg =>
    {
        cfg.CaseSensitive = false;
        cfg.CaseInsensitiveEnumValues = true;
        cfg.HelpWriter = Console.Out;
        cfg.IgnoreUnknownArguments = false;
        cfg.AutoHelp = true;
        cfg.AutoVersion = true;
        cfg.ParsingCulture = CultureInfo.InvariantCulture;
        cfg.EnableDashDash = false;
        cfg.MaximumDisplayWidth = Console.WindowWidth;
    });
}