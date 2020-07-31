using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace TextSearchHelper
{
    using SubstringPosition = Tuple<long, int>;
    using SearchCache = Dictionary<string, Tuple<long, int>>;

    public class TSHelper: IDisposable
    {
        private FileCache _cache;
        private string _rawPathToFile;
        private string _folderPath;
        private string _fileName;
        private long linesCached = 0;
        private long lastPosition = 0;
        private FileSystemWatcher watcher;
        private SearchCache _searchCache = new SearchCache();
        private bool inited = false;
        private bool cacheBuildCancelled = false;
        private bool isDisposed = false;
        private TSHelperLogger _logger;
        private CancellationTokenSource buildCacheCancel = new CancellationTokenSource();


        public TSHelper(string path,bool asyncCacheBuilding = false, TSHelperLogger logger = null)
        {
            if (logger==null)
            {
                _logger = new TSHelperLogger();
            }
            else
            {
                _logger = logger;
            }
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
                inited = true;
            }
        }

        private void checkDisposed()
        {
            if (isDisposed)
                throw new TextSearchDisposed();
        }

        private void initFSWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = _folderPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnRenameDelete;
            watcher.Renamed += OnRenameDelete;

            watcher.Filter = _fileName;
            watcher.EnableRaisingEvents = true;
        }

        private void OnRenameDelete(object source, FileSystemEventArgs e)
        {
            Dispose();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Console.WriteLine("---");
            using (FileStream fs = new FileStream(_rawPathToFile,FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs.Seek( lastPosition, SeekOrigin.Begin );
                    while (!sr.EndOfStream)
                    {
                        string currentLine = sr.ReadLine();
                        _cache.cacheLine(linesCached, currentLine, buildCacheCancel);
                        if (buildCacheCancel.Token.IsCancellationRequested)
                            return;
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
                        if (Result % 100000 == 0)
                        {
                            _logger.Clear();
                            _logger.WriteLine("Lines counting...");
                            _logger.WriteLine(string.Format("Lines amount: {0}", Result));
                        }
                    }
                }
            }
            return Result;

        }

        private async void buildCacheAsync()
        {
            await Task.Run(() =>  buildCache());
            if (!cacheBuildCancelled)
            {
                initFSWatcher();
                inited = true;
            }

        }

        private bool _waitInitedInternal( bool wait = false )
        {
            while (!inited)
            {
                if (!wait)
                    return false;
            }
            return true;
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
                        _cache.cacheLine(currentLine, line,buildCacheCancel);
                        if (buildCacheCancel.Token.IsCancellationRequested)
                        {
                            cacheBuildCancelled = true;
                            return;
                        }
                        currentLine++;

                        if (currentLine % 100000 == 0)
                        {
                            _logger.Clear();
                            _logger.WriteLine("Cach building...");
                            _logger.WriteLine(string.Format("Lines amount: {0}", currentLine));
                            _logger.WriteLine(string.Format("Lines total: {0}", linesAmount));
                        }
                    }
                    lastPosition = file1.Length;
                }
            }
            linesCached = linesAmount;
            _cache.flush();
        }

        private SubstringPosition[] findInternal(string whatToFind, long stringNumber, int letterNumber, bool waitCaching = true,long entriesNumber=-1)
        {
            List<SubstringPosition> result = new List<SubstringPosition>();

            
            bool cacheReady = _waitInitedInternal(waitCaching);
            if (!cacheReady)
                throw new WaitCacheException();

            // Получим список строк с вхождением первых двух буков
            long[] stringsNumbers = _cache.getStringNumbers(whatToFind);
            int stringsNumberPtr = 0;
            string current;
            using (FileStream fs = new FileStream(_rawPathToFile, FileMode.Open,FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int currentStringNumber = -1;
                    _logger.Clear();
                    _logger.WriteLine("Cache searching process...");
                    while ( !sr.EndOfStream)
                    {
                        currentStringNumber++;
                        current = sr.ReadLine();
                        if (currentStringNumber < stringNumber)
                            continue;
                        while (stringsNumbers[stringsNumberPtr] < currentStringNumber)
                        {
                            stringsNumberPtr++;
                            if (stringsNumberPtr >= stringsNumbers.Length)
                                return result.ToArray();
                        }
                        if (stringsNumbers[stringsNumberPtr] == currentStringNumber)
                        {
                            int currentIndex = -1;
                            if (currentStringNumber==stringNumber)
                            {
                                currentIndex = letterNumber;
                            }

                            while ((currentIndex = current.IndexOf(whatToFind, currentIndex + 1)) >= 0)
                            {
                                SubstringPosition newPosition = new SubstringPosition(currentStringNumber, currentIndex);
                                _searchCache[whatToFind] = newPosition;
                                result.Add(newPosition);
                                if ((entriesNumber>=0) && (result.Count==entriesNumber))
                                {
                                    return result.ToArray();
                                }
                            }

                        }
                    }
                }
            }
            return result.ToArray();
        }

        public bool find(string whatToFind, ref long stringNumber, ref int letterNumber, bool returnPosition = false, bool waitCaching = true)
        {
            checkDisposed();
            SubstringPosition [] result = findInternal(whatToFind,stringNumber,letterNumber,waitCaching,1);

            if ((result.Length>0)&&(returnPosition))
            {
                SubstringPosition sp = _searchCache[whatToFind];
                stringNumber = sp.Item1;
                letterNumber = sp.Item2;
            }
            return result.Length>0;
        }

        public bool find(string whatToFind, bool fromPreviousPosition = false, bool waitCaching = true)
        {
            checkDisposed();
            SubstringPosition[] Result;
            long stringNumber = -1;
            int letterNumber = -1;

            if (_searchCache.ContainsKey(whatToFind)&&fromPreviousPosition)
            {
                stringNumber = _searchCache[whatToFind].Item1;
                letterNumber = _searchCache[whatToFind].Item2;
            }

            Result = findInternal(whatToFind, stringNumber, letterNumber, waitCaching,1);
            
            return Result.Length>0;
        }

        public SubstringPosition [] findAll(string whatToFind, bool waitCaching = true)
        {
            checkDisposed();
            SubstringPosition[] Result;
            long stringNumber = -1;
            int letterNumber = -1;

            Result = findInternal(whatToFind, stringNumber, letterNumber, waitCaching);

            return Result;
        }

        public void resetAllSearch()
        {
            checkDisposed();
            _searchCache = new SearchCache();
        }

        public bool waitCacheBuilt()
        {
            return _waitInitedInternal(true);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                
                if (!inited)
                {
                    buildCacheCancel.Cancel();
                    while (!cacheBuildCancelled){}
                }

                if (watcher != null)
                {
                    watcher.Changed -= OnChanged;
                    watcher.Created -= OnChanged;
                    watcher.Deleted -= OnRenameDelete;
                    watcher.Renamed -= OnRenameDelete;
                    watcher.Dispose();
                }
                if (_cache!=null)
                {
                    _cache.Dispose();
                }

            }
        }

    }
}
