using System;
using System.IO;
using System.Threading.Tasks;

namespace TextSearchHelper
{
    public class TSHelper
    {
        private FileCache _cache;
        private string _rawPathToFile;
        private string _folderPath;
        private string _fileName;
        private long linesCached = 0;
        private long lastPosition = 0;
        private FileSystemWatcher watcher;

        public TSHelper(string path,bool asyncCacheBuilding = false)
        {
            _rawPathToFile = path;
            FileInfo fi = new FileInfo(_rawPathToFile);
            _folderPath = fi.DirectoryName;
            _fileName = fi.Name;
            _cache = new FileCache(fi.Directory.FullName,fi.Name);
            if (asyncCacheBuilding)
            {
                buildCacheAsync();
            }
            else
            {
                buildCache();
                initFSWatcher();
            }
        }

        private void initFSWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = _folderPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;

            watcher.Filter = _fileName;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("---");
            using (FileStream fs = new FileStream(_rawPathToFile,FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs.Seek( lastPosition, SeekOrigin.Begin );
                    while (!sr.EndOfStream)
                    {
                        string currentLine = sr.ReadLine();
                        _cache.cacheLine(linesCached, currentLine);
                        linesCached++;
                    }
                    lastPosition = fs.Length;
                    _cache.flush();
                }
            }
        }

        private long countLines()
        {
            long Result = 0;
            using (FileStream file1 = new FileStream(_rawPathToFile, FileMode.Open))
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

        private async void buildCacheAsync()
        {
            await Task.Run(() =>  buildCache());
            initFSWatcher();
        }

        private void buildCache()
        {
            _cache.resetCache();
            long linesAmount = countLines();
            long currentLine = 0;
            using (FileStream file1 = new FileStream(_rawPathToFile, FileMode.Open))
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
                    lastPosition = file1.Length;
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

        ~TSHelper()
        {
            if (watcher != null)
            {
                watcher.Changed -= OnChanged;
                watcher.Dispose();
            }
        }
    }
}
