using JetBrains.Annotations;

namespace winbash.test;

public static class TestUtil
{
    public static string exec([LanguageInjection("bat")] string cmd)
    {
        return cmd;
    }
}