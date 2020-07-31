using System;
using System.Collections.Generic;
using System.Text;
using GeneratorLib;
using System.IO;
using System.Threading;
using TextSearchHelper;

namespace Tester
{
    class Tests
    {

        private string _Filename;
        public double timeStandardTest1;
        public double timeCachedTest1;
        public bool resultsEqualsTest1;
        public int entriesNumberBeforeTest2 = -1;
        public int entriesNumberAfterTest2 = -1;
        public int entriesNumberAfter2Test2 = -1;
        public TextSearchDisposed disposedException3 = null;
        public TextSearchDisposed disposedException4 = null;
        public WaitCacheException waitingException5 = null;



        private TestsLogger logger;

        private static bool isEqual(List<Position> positions1, List<Position> positions2)
        {
            if (positions1.Count != positions2.Count)
            {
                return false;
            }

            for (int i = 0; i < positions1.Count; i++)
            {
                if (
                    (positions1[i].StringNumber != positions2[i].StringNumber)
                    ||
                    (positions1[i].LetterNumber != positions2[i].LetterNumber)
                    )
                {
                    return false;
                }
            }

            return true;
        }

        public Tests(string Filename = "../huge_file.txt")
        {
            _Filename = Filename;
            logger = new TestsLogger();
        }
        public void Test1(bool regenerateFile = false)
        {
            if (regenerateFile)
            {
                Generator.Generate(250);
            }
            logger.ResetPreamble();
            string testProcess = "TEST1 Process...";
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("Standard search processing...");

            SimpleSearcher simple = new SimpleSearcher(_Filename);

            Timer _timer = new Timer();

            _timer.reset();
            List<Position> positionsStandars = simple.findAll("кошка",logger);
            timeStandardTest1 = _timer.getInterval();

            logger.ResetPreamble();
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("We are testing the Cached search now...");
            using (CachedSearcher cached = new CachedSearcher(_Filename, logger))
            {
                _timer.reset();
                List<Position> positionsCached = cached.findAll("кошка");
                timeCachedTest1 = _timer.getInterval();
            
            resultsEqualsTest1 = isEqual(positionsStandars,positionsCached);
            logger.ResetPreamble();
            }
        }

        public void Test2(bool regenerateFile = false)
        {
            if (regenerateFile)
            {
                Generator.Generate(250);
            }
            logger.ResetPreamble();
            string testProcess = "TEST2 Process...";
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("Cached searching first (there is not substring)...");

            using (CachedSearcher cached = new CachedSearcher(_Filename, logger))
            {


                entriesNumberBeforeTest2 = cached.findAll("мыла раму").Count;
                using (StreamWriter sw = File.AppendText(_Filename))
                {
                    sw.WriteLine("Мама мыла раму");
                    sw.Flush();
                }
                Thread.Sleep(2000);
                logger.ResetPreamble();
                logger.AddToPreamble(testProcess);
                logger.AddToPreamble("Cached searching second (there is one substring entry)...");


                entriesNumberAfterTest2 = cached.findAll("мыла раму").Count;
                using (StreamWriter sw = File.AppendText(_Filename))
                {
                    sw.WriteLine("Мама мыла раму");
                    sw.Flush();
                }

                Thread.Sleep(2000);
                logger.ResetPreamble();
                logger.AddToPreamble(testProcess);
                logger.AddToPreamble("Cached searching third (there is two substring entry)...");


                entriesNumberAfter2Test2 = cached.findAll("мыла раму").Count;
            }
        }

        public void Test3(bool regenerateFile = false)
        {
            if (regenerateFile)
            {
                Generator.Generate(250);
            }
            logger.ResetPreamble();
            string testProcess = "TEST3 Process...";
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("Getting DisposedException after cached file renaming");

            using (CachedSearcher cached = new CachedSearcher(_Filename, logger))
            {
                try
                {
                    File.Move(_Filename, _Filename + "_renamed");
                    Thread.Sleep(2000);
                    List<Position> positionsCached = cached.findAll("кошка");
                }
                catch (TextSearchDisposed tsd)
                {
                    disposedException3 = tsd;
                }
            }
            // Вернули на место переименованный файл
            File.Move(_Filename + "_renamed", _Filename);
        }

        public void Test4(bool regenerateFile = false)
        {
            if (regenerateFile)
            {
                Generator.Generate(250);
            }
            logger.ResetPreamble();
            string testProcess = "TEST4 Process...";
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("Getting DisposedException after cached file deleting");

            using (CachedSearcher cached = new CachedSearcher(_Filename, logger))
            {
                try
                {
                    File.Delete(_Filename);
                    Thread.Sleep(2000);
                    List<Position> positionsCached = cached.findAll("кошка");
                }
                catch (TextSearchDisposed tsd)
                {
                    disposedException4 = tsd;
                }
            }
            // Вернули на место удалённый файл
            Generator.Generate(250);
        }

        public void Test5(bool regenerateFile = false)
        {
            if (regenerateFile)
            {
                Generator.Generate(250);
            }
            logger.ResetPreamble();
            string testProcess = "TEST5 Process...";
            logger.AddToPreamble(testProcess);
            logger.AddToPreamble("Getting WaitCacheException while cache building");

            using (CachedSearcher cached = new CachedSearcher(_Filename, logger,true))
            {
                try
                {
                    logger.silentMode = true;
                    List<Position> positionsCached = cached.findAll("кошка", false);
                }
                catch (WaitCacheException wce)
                {
                    waitingException5 = wce;
                }
                // Ожидаем окончания построения кеша
                logger.AddToPreamble("Waiting cache will be built");
                logger.printPreamble();
                cached.waitCacheBuilt();
                logger.silentMode = false;
            }
        }

        }
}
