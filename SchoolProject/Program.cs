using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathParserLib;

namespace SchoolProject
{
    class Program
    {
        static string currentVersion = "0.1";
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to console version of MathParser programing language interpreter");
            Thread.Sleep(500);
            Console.WriteLine("Current Version: v" + currentVersion);
            Console.Beep();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Chunker.SetEnvironment(new ConsoleEnvironment());
            int debugerLevel = 0;
            bool validInput = false;
            while (!validInput)
            {
                try
                {
                    Console.Clear();
                    Console.Write("Set debuger level to: ");
                    debugerLevel = int.Parse(Console.ReadLine());
                }
                finally
                {
                    validInput = true;
                }
            }
            Console.Clear();
            while (true)
            {
                //Console.Clear();
                Console.Write(">");
                Queue<Word> words = TextProcessor.ProcessString(Console.ReadLine());
                if(debugerLevel > 1)
                    foreach (Word w in words)
                    {
                        Console.WriteLine(w.GetCharacterType().ToString());
                    }
                Chunk r = Chunker.Chunkify(words);
                Result res = r.Evaluate();
                if (debugerLevel > 0)
                    Console.WriteLine(res.ToString());
                else if (res.IsUnexpected())
                    Console.WriteLine(res.ToString());
            }
        }
    }

    class ConsoleEnvironment : MathParserLib.Environment
    {
        public override void Print(string text)
        {
            Console.WriteLine(text);
        }
    }
}
