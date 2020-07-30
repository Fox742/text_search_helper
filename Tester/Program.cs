using System;
using TextSearchHelper;
using System.IO;
using System.Collections.Generic;

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
            Timer _timer = new Timer();

            SimpleSearcher ss = new SimpleSearcher("../Generator/huge_file.txt");

            _timer.reset();
            List<Position> positionsStandars = ss.findAll("кошка");
            double StTime = _timer.getInterval();

            CachedSearcher sc = new CachedSearcher("../Generator/huge_file.txt");
            _timer.reset();
            List<Position> positionsCached = sc.findAll("кошка");
            double CcTime = _timer.getInterval();

            Console.WriteLine("Positions of both search method eqials or not?"+( isEqual(positionsCached,positionsStandars)?" ":" NOT "  )+"equals");
            */

            Timer _timer = new Timer();
            _timer.reset();
            CachedSearcher sc = new CachedSearcher("../Generator/huge_file.txt", true);
            Console.WriteLine("Created CachedSearcher -> Calling findAll");
            List<Position> positionsCached = sc.findAll("кошка", true);
            Console.WriteLine("Waiting of cache building took: {0}",_timer.getInterval());
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}