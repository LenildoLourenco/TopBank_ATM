using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace ATM.ConsoleApplicationLib
{
    public static class Utility
    {
        private static CultureInfo culture = new CultureInfo("pt-br");
        private static long tranId;

        public static long GetTransactionId()
        {
            return ++tranId;
        }

        public static string GetRawInput(string message)
        {
            Console.Write($"Digite {message}: ");
            return Console.ReadLine();
        }

        public static string GetHiddenConsoleInput(string text)
        {
            bool prompt = true;


            StringBuilder input = new StringBuilder();
            while (true)
            {
                if (prompt)
                    Console.Write(text);

                var key = Console.ReadKey(true);
                prompt = false;

                if (key.Key == ConsoleKey.Enter)
                {
                    if (input.Length == 6)
                    {
                        break;
                    }
                    else
                    {
                        PrintMessage("\nInsira 6 dígitos.", false);
                        prompt = true;
                        input.Clear();
                    }
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        #region UIOutput - UX and output format
        public static void printDotAnimation(int timer = 10)
        {
            for (var x = 0; x < timer; x++)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine();
        }

        public static string FormatAmount(decimal amt)
        {
            return String.Format(culture, "{0:C2}", amt);
        }

        public static void PrintConsoleWriteLine(string msg, bool ConsoleWriteLine = true)
        {
            if (ConsoleWriteLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

            PrintEnterMessage();
        }

        public static void PrintUserInputLabel(string msg, bool ConsoleWriteLine = false)
        {
            if (ConsoleWriteLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

        }

        public static void PrintMessage(string msg, bool success)
        {
            if (success)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(msg);
            Console.ResetColor();
            PrintEnterMessage();
        }
        #endregion

        public static void PrintEnterMessage()
        {
            Console.WriteLine("\nPressione enter para continuar.");
            Console.ReadKey();
        }
    }
}



