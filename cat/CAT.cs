namespace winbash.cat;

public static class CAT
{
    public static void Main(string[] args)
    {
        foreach (var line in File.ReadLines(string.Join(" ", args)))
            Console.WriteLine(line);
    }
}