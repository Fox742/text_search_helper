using System;
using TextSearchHelper;
using System.IO;

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
            StreamWriter sw = File.AppendText("../Generator/huge_file.txt");
            sw.WriteLine("Ля ля ля жу жу жу я всё время торможу");
            sw.Flush();
            sw.Dispose();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}