using System;
using System.Security.Cryptography;

namespace HoodWink.Services
{
    public static class WriteService
    {
        public static void Header(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        public static void Header(string msg, string name)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(name);
            Console.ResetColor();
        }
        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Description(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"              -{msg,-30}");
            Console.ResetColor();
        }
        public static void Description(string msg, string des)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"              -{msg, -30} : {des, -30}");
            Console.ResetColor();
        }
        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[+] Success: ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Suggestion(string suggestion, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[+] Suggested {suggestion}: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Progress(string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[+] Building: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);
            Console.ResetColor();
        }

        public static void Error(string err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[!] Error: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(err);
            Console.ResetColor();
        }

        public static void ErrorExit(string err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[!] Exiting: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(err);
            Console.ResetColor();
            System.Environment.Exit(0);
        }
    }
}
