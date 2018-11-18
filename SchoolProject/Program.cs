using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathParserLib;

namespace SchoolProject
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.Write(">");
                Queue<Word> words = TextProcessor.ProcessString(Console.ReadLine());
                foreach (Word w in words)
                {
                    Console.WriteLine(w.GetCharacterType().ToString());
                }
                Chunk r = Chunker.Chunkify(words);
                Console.WriteLine(r.Evaluate().ToString());
                Console.ReadKey();
            }
        }
    }
}
