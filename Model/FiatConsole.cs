using System;

namespace Fiat
{
    public static class FiatConsole
    {
        internal static int ErrorCount { get; private set; }

        internal static int WarnCount { get; private set; }

        internal static void WriteSuccess(string text)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(text);

            Console.ForegroundColor = originalColor;
        }

        internal static void WriteWarn(string text)
        {
            WarnCount++;

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine(text);

            Console.ForegroundColor = originalColor;
        }

        internal static void WriteError(string text, bool writeGithubLabel = false)
        {
            ErrorCount++;

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(text);

            if (writeGithubLabel)
            {
                Console.WriteLine(" ");
                Console.WriteLine(
                    "Check documentation and examples at https://github.com/diegosiao/fiat");
            }

            Console.ForegroundColor = originalColor;
        }

        internal static void WriteSummary()
        {
            if (ErrorCount == 0 && WarnCount == 0)
                WriteSuccess("\r\n\r\nFinnished successfully!");

            else if (ErrorCount == 0 && WarnCount > 0)
                WriteWarn("\r\n\r\nFinnished with warnings.");

            else
                WriteError("\r\n\r\nFinnished with errors.");
        }
    }
}
