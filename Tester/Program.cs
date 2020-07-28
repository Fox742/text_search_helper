using System;
using TextSearchHelper;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            SimpleSearcher ss = new SimpleSearcher("../Generator/huge_file.txt");
            ss.findAll("кошка");
            */
            CachedSearcher cs = new CachedSearcher("../Generator/huge_file.txt");

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}