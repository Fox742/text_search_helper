using System;
using System.IO;

namespace TextSearchHelper
{
    public class TSHelper
    {
        private FileCache _cache;
        private string _path;
        private long linesCached = 0;

        public TSHelper(string path,bool asyncCacheBuilding = false)
        {
            _path = path;
            FileInfo fi = new FileInfo(_path);
            _cache = new FileCache(fi.Directory.FullName,fi.Name);

            if (asyncCacheBuilding)
            {

            }
            else
            {
                buildCache();
            }
        }

        private long countLines()
        {
            long Result = 0;
            using (FileStream file1 = new FileStream(_path, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(file1))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        Result++;
                        
                        //----
                        if (Result % 100000 == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Lines counting...");
                            Console.WriteLine("Lines amount: {0}", Result);
                        }
                        //----
                        
                    }
                }
            }
            return Result;

        }
        
        private void buildCache()
        {
            _cache.resetCache();
            long linesAmount = countLines();
            long currentLine = 0;
            using (FileStream file1 = new FileStream(_path, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(file1))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        _cache.cacheLine(currentLine,line);
                        currentLine++;
                        //-----
                        if (currentLine % 100000 == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Caching...");
                            Console.WriteLine("Lines amount: {0}", currentLine);
                            Console.WriteLine("Lines total: {0}", linesAmount);
                        }
                        //-----
                    }
                }
            }
            linesCached = linesAmount;
            _cache.flush();
        }

        public bool find(string whatToFind, ref uint stringNumber, ref uint letterNumber, bool returnPosition = false)
        {
            return false;
        }

        public bool find(string whatToFind, bool fromPreviousPosition = false)
        {
            return false;
        }

        public bool waitCacheBuilt(uint timeoutMSec = 5000)
        {
            return true;
        }
        


    }
}
