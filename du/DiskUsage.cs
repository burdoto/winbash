using System.ComponentModel;
using System.Runtime.InteropServices;
using CommandLine;
using comroid.common;
using winbash.util;

namespace winbash.du;

public static class DiskUsage
{
    private static Args args = null!;
    
    public static void Main(string[] param)
    {
        args = Parser.Default.ParseArguments<Args>(param).Value;
        var path = PathUtil.Unwrap(args.Path ?? Environment.CurrentDirectory);
        if (File.Exists(path))
            path = new FileInfo(path).DirectoryName!;

        var size = 0L;
        ListFileSizes_Rec(path, ref size);
        PrintFileSize(size, true);
    }

    private static void ListFileSizes_Rec(string path, ref long totalSize)
    {
        foreach (var file in Directory.EnumerateFiles(path))
        {
            var size = GetFileSizeOnDisk(file);
            totalSize += size;
            PrintFileSize(size);
            Console.WriteLine($" {file}");
        }
        foreach (var dir in Directory.EnumerateDirectories(path))
            ListFileSizes_Rec(dir, ref totalSize);
    }

    private static void PrintFileSize(long size, bool endl = false) => Console.Write(
        $"{(args.HumanReadable ? (size * Units.Bytes).Normalize() : size)}\t{(endl ? Environment.NewLine : string.Empty)}");

    // https://stackoverflow.com/questions/3750590/get-size-of-file-on-disk
    private static long GetFileSizeOnDisk(string file)
    {
        FileInfo info = new FileInfo(file);
        uint dummy, sectorsPerCluster, bytesPerSector;
        int result = GetDiskFreeSpaceW(info.Directory!.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
        if (result == 0) throw new Win32Exception();
        uint clusterSize = sectorsPerCluster * bytesPerSector;
        uint hosize;
        uint losize = GetCompressedFileSizeW(file, out hosize);
        long size;
        size = (long)hosize << 32 | losize;
        return ((size + clusterSize - 1) / clusterSize) * clusterSize;
    }

    [DllImport("kernel32.dll")]
    private static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
        [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

    [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
    private static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
        out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
        out uint lpTotalNumberOfClusters);
    
    public class Args
    {
        [Value(0, Required = false)]
        public string? Path { get; set; }
        
        [Option(shortName: 'h', longName: "human-readable")]
        public bool HumanReadable { get; set; }
    }
}