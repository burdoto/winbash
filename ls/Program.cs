using System.Security.AccessControl;
using System.Security.Principal;
using CommandLine;
using SymbolicLinkSupport;
using winbash.util;

namespace winbash.ls;

public static class Program
{
    public static void Main(string[] param)
    {
        var args = Parser.Default.ParseArguments<Args>(param).Value;
        var path = PathUtil.Unwrap(args.Path ?? Environment.CurrentDirectory);

        var entries = Directory.EnumerateFileSystemEntries(path);
        if (!args.All && !args.AlmostAll)
            entries = entries.Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == 0);
        if (args.Detailed)
            entries = entries.Select(f => $"{GetMod(f)} " +
                                          $"{GetHardlinkCount(f):##} " +
                                          $"{GetOwner(f).Split("\\")[^1].Trim(8)} " +
                                          $"{GetGroup(f).Split("\\")[^1].Trim(8)} " +
                                          $"{GetSize(f, args.HumanReadableSizes)} " +
                                          $"{GetChangedMonth(f).Trim(3)} " +
                                          $"{GetChangedDOM(f):##} " +
                                          $"{GetChangedTimeDetail(f).Trim(5)} " +
                                          $"{new FileInfo(f).Name}");
        else entries = entries.Select(f => new FileInfo(f).Name);
        
        foreach (var entry in entries)
            Console.WriteLine(entry);
    }

    private static string GetMod(string path)
    {
        string s = string.Empty;
        var file = new FileInfo(path);
        var attr = file.Attributes;

        if ((attr & FileAttributes.Directory) != 0)
            s += 'd';
        else if (file.IsSymbolicLink())
            s += 'l';
        else s += '-';
        s += "rwx";
        Func<FileInfo, FileSystemAccessRule, string> acsTest = (file, acs) =>
        {
            if (file.IsReadOnly)
                return "r--";
            var s = "r";
            s += acs.AccessControlType == AccessControlType.Allow ? 'w' : '-';
            s += file.Extension.EndsWith("exe") ? 'x' : '-';
            return s;
        };
        /*
        s += acsTest(file, file.Directory.GetAccessControl(AccessControlSections.Owner).GetAccessRules(true, false, typeof(SecurityIdentifier))[0]);
        s += acsTest(file, file.Directory.GetAccessControl(AccessControlSections.Group).GetAccessRules(true, false, typeof(SecurityIdentifier))[0]);
        s += acsTest(file, file.Directory.GetAccessControl(AccessControlSections.All).GetAccessRules(true, false, typeof(SecurityIdentifier))[0]);
        */
        s += "------"; // todo
        return s;
    }

    private static int GetHardlinkCount(string path) => 0; // todo

    private static string GetOwner(string path) => new FileInfo(path).GetAccessControl().GetOwner(typeof(NTAccount))?.Value ?? "unknown";

    private static string GetGroup(string path) => new FileInfo(path).GetAccessControl().GetGroup(typeof(NTAccount))?.Value ?? "unknown";

    private static string GetSize(string path, bool humanReadable)
    {
        var amount = new FileInfo(path).Length;
        return humanReadable ? ByteUtil.ReadableAmount(amount) : amount.ToString();
    }

    private static string GetChangedMonth(string path) => File.GetLastWriteTime(path).Month switch
    {
        1 => "Jan",
        2 => "Feb",
        3 => "Mar",
        4 => "Apr",
        5 => "May",
        6 => "Jun",
        7 => "Jul",
        8 => "Aug",
        9 => "Sep",
        10 => "Oct",
        11 => "Nov",
        12 => "Dec",
        _ => throw new ArgumentOutOfRangeException("Invalid month", (Exception?)null)
    };

    private static int GetChangedDOM(string path) => File.GetLastWriteTime(path).Day;

    private static string GetChangedTimeDetail(string path)
    {
        var time = File.GetLastWriteTime(path);
        return time.Year != DateTime.Now.Year ? time.Year.ToString() : $"{time.Hour:00}:{time.Minute:00}";
    }

    private class Args
    {
        [Value(0, Required = false)]
        public string? Path { get; set; }
        
        [Option(shortName: 'a', longName: "all")]
        public bool All { get; set; }
        [Option(shortName: 'A', longName: "almost-all")]
        public bool AlmostAll { get; set; }
        
        [Option(shortName: 'l')]
        public bool Detailed { get; set; }
        
        [Option(shortName: 'h', longName: "human-readable")]
        public bool HumanReadableSizes { get; set; }
    }
}