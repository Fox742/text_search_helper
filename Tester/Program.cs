using System;
using TextSearchHelper;
using System.IO;
using System.Collections.Generic;
using GeneratorLib;

namespace Tester
{
    class Program
    {

        static void Main(string[] args)
        {
                Tests _tests = new Tests();
                _tests.Test1(true);
                _tests.Test2();
                _tests.Test3();
                _tests.Test4();
                _tests.Test5();


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

                Console.WriteLine("\t\t\tTEST3");
                Console.WriteLine("1) Creating TSHelper");
                Console.WriteLine("3) Renaming target file");
                Console.WriteLine("4) Attempting find call");
                Console.WriteLine(string.Format("Should getting TextSearchDisposed exception: {0}", _tests.disposedException3==null?"Not caught":"Caught -> "+_tests.disposedException3.Message ));
                Console.WriteLine("\n\n");

                Console.WriteLine("\t\t\tTEST4");
                Console.WriteLine("1) Creating TSHelper");
                Console.WriteLine("3) Deleting target file");
                Console.WriteLine("4) Attempting find call");
                Console.WriteLine(string.Format("Should getting TextSearchDisposed exception: {0}", _tests.disposedException4 == null ? "Not caught" : "Caught -> " + _tests.disposedException4.Message));
                Console.WriteLine("\n\n");

                Console.WriteLine("\t\t\tTEST5");
                Console.WriteLine("1) Creating TSHelper with asynchronous cache building");
                Console.WriteLine("3) Trying to find something at the time cache building");
                Console.WriteLine(string.Format("Should getting WaitCache exception: {0}", _tests.waitingException5 == null ? "Not caught" : "Caught -> " + _tests.waitingException5.Message));
                Console.WriteLine("\n\n");

                Console.WriteLine("Please, press Enter key to quit...");
                Console.ReadLine();
        }
    }
}