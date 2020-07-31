using System;
using TextSearchHelper;
using System.IO;
using System.Collections.Generic;
using GeneratorLib;

namespace Tester
{
    class Program
    {
        private static  bool isEqual(List<Position> positions1, List<Position> positions2)
        {
            if (positions1.Count!=positions2.Count)
            {
                return false;
            }

            for (int i=0;i<positions1.Count;i++)
            {
                if (
                    (positions1[i].StringNumber!=positions2[i].StringNumber)
                    ||
                    (positions1[i].LetterNumber != positions2[i].LetterNumber)
                    )
                {
                    return false;
                }
            }

            return true;
        }

        static void Main(string[] args)
        {
            Tests _tests = new Tests();
                _tests.Test1();
                _tests.Test2(false);

                Console.Clear();
                Console.WriteLine("\t\t\tTEST1");
                Console.WriteLine("We have launched two search - standard and cached search with my library. Checking two things:");
                Console.WriteLine("1) Time spent standard search vs Timespent cached search");
                Console.WriteLine("2) Results of searches should be equal");
                Console.WriteLine("Time spent standard search: {0} mlsec and cached search {1} mlsec", _tests.timeStandardTest1, _tests.timeCachedTest1);
                Console.WriteLine("Positions of both search method eqials or not?" + ((_tests.resultsEqualsTest1) ? " " : " NOT ") + "equals");
                Console.WriteLine("\n\n");
                Console.WriteLine("\t\t\tTEST2");
                Console.WriteLine("1) Cached searching substring there is not in file");
                Console.WriteLine("2) Appending string to file");
                Console.WriteLine("3) Cached searching substring");
                Console.WriteLine("4) Appending string to file");
                Console.WriteLine("5) Cached searching substring");
                Console.WriteLine("Number of entries at the beginning of the test:\t{0}",_tests.entriesNumberBeforeTest2);
                Console.WriteLine("Number of entries after step 2 of the test:\t{0}", _tests.entriesNumberAfterTest2);
                Console.WriteLine("Number of entries after step 4 of the test:\t{0}", _tests.entriesNumberAfter2Test2);
                Console.WriteLine("\n\n");

                Console.WriteLine("Please, press Enter key to quit...");
                Console.ReadLine();
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
            /*
            Generator.Generate(250,printLogs:true);


            Timer _timer = new Timer();

            SimpleSearcher ss = new SimpleSearcher("../huge_file.txt");

            _timer.reset();
            List<Position> positionsStandars = ss.findAll("кошка");
            double StTime = _timer.getInterval();

            CachedSearcher sc = new CachedSearcher("../huge_file.txt");
            _timer.reset();
            List<Position> positionsCached = sc.findAll("кошка");
            double CcTime = _timer.getInterval();

            Console.WriteLine("Positions of both search method eqials or not?"+( isEqual(positionsCached,positionsStandars)?" ":" NOT "  )+"equals");
            Console.WriteLine("Hello World!");
            Console.ReadLine();
            */
            /*
            Timer _timer = new Timer();
            _timer.reset();
            CachedSearcher sc = new CachedSearcher("../Generator/huge_file.txt", true);
            Console.WriteLine("Created CachedSearcher -> Calling findAll");
            List<Position> positionsCached = sc.findAll("кошка", true);
            Console.WriteLine("Waiting of cache building took: {0}",_timer.getInterval());
            Console.WriteLine("Hello World!");
            Console.ReadLine();
            */
        }
    }
}