namespace winbash.grep;

public static class Program
{
    public static void Main(string[] args)
    {
        while (Console.ReadLine() is {} line)
            if (args.Any(line.Contains))
                Console.WriteLine(line);
    }
}