namespace winbash.grep;

public static class GREP
{
    public static void Main(string[] args)
    {
        while (Console.ReadLine() is {} line)
            if (args.Any(line.Contains))
                Console.WriteLine(line);
    }
}