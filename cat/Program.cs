namespace winbash.cat;

public static class Program
{
    public static void Main(string[] args)
    {
        foreach (var line in File.ReadLines(string.Join(" ", args)))
            Console.WriteLine(line);
    }
}