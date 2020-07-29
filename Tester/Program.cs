using System;
using TextSearchHelper;
using System.IO;
using System.Collections.Generic;

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

            /*
            CachedSearcher cs = new CachedSearcher("../Generator/huge_file.txt");
            StreamWriter sw = File.AppendText("../Generator/huge_file.txt");
            sw.WriteLine("Ля ля ля жу жу жу я всё время торможу");
            sw.Flush();
            sw.Dispose();
            */

            /*
            //CachedSearcher ss = new CachedSearcher("../Generator/huge_file.txt");
            SimpleSearcher ss = new SimpleSearcher("../Generator/huge_file.txt");
            ss.findAll("жу жу жу я всё время торможу");
            */
            SimpleSearcher ss = new SimpleSearcher("../Generator/huge_file.txt");
            List<Position> positionsStandars = ss.findAll("кошка");

            CachedSearcher sc = new CachedSearcher("../Generator/huge_file.txt");
            List<Position> positionsCached = sc.findAll("кошка");


            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}