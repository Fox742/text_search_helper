﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TextSearchHelper
{
    using SubstringPosition = Tuple<long, int>;
    using SearchCache = Dictionary<string, Tuple<long, int>>;

    public class TSHelper
    {
        private FileCache _cache;
        private string _rawPathToFile;
        private string _folderPath;
        private string _fileName;
        private long linesCached = 0;
        private long lastPosition = 0;
        private FileSystemWatcher watcher;
        private SearchCache _searchCache = new SearchCache();

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



        private bool findInternal(string whatToFind, long stringNumber, int letterNumber, bool waitCaching = true)
        {
            bool result = false;

            // Лочим мьютекс

            // Получим список строк с вхождением первых двух буков
            long[] stringsNumbers = _cache.getStringNumbers(whatToFind);
            int stringsNumberPtr = 0;
            string current;
            using (FileStream fs = new FileStream(_rawPathToFile, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    int currentStringNumber = -1;
                    
                    while ( !sr.EndOfStream)
                    {
                        currentStringNumber++;
                        current = sr.ReadLine();
                        if (currentStringNumber < stringNumber)
                            continue;
                        while (stringsNumbers[stringsNumberPtr] < currentStringNumber)
                            stringsNumberPtr++;
                        if (stringsNumbers[stringsNumberPtr] == currentStringNumber)
                        {
                            int currentIndex = -1;
                            if (currentStringNumber==stringNumber)
                            {
                                currentIndex = letterNumber;
                            }
                            
                            if ( (currentIndex = current.IndexOf(whatToFind,currentIndex+1))>=0 )
                            {
                                // Нашли следующее вхождение строки!!!!
                                result = true;
                                // Кеширование номера строки и 
                                _searchCache[whatToFind] = new SubstringPosition(currentStringNumber,currentIndex);
                                // Выход из цикла поиска
                                break;
                            }
                            // else - ничего не делаем, не нашли в текущей строке, поэтому надо идти к следующей строке

                        }
                    }
                }
            }


            // Освобождаем мьютекс

            return result;
        }

        public bool find(string whatToFind, ref long stringNumber, ref int letterNumber, bool returnPosition = false, bool waitCaching = true)
        {
            bool Result = findInternal(whatToFind,stringNumber,letterNumber,waitCaching);

            if ((Result)&&(returnPosition))
            {
                SubstringPosition sp = _searchCache[whatToFind];
                stringNumber = sp.Item1;
                letterNumber = sp.Item2;
            }
            return Result;
        }

        public bool find(string whatToFind, bool fromPreviousPosition = false, bool waitCaching = true)
        {
            bool Result = false;
            long stringNumber = -1;
            int letterNumber = -1;

            if (_searchCache.ContainsKey(whatToFind)&&fromPreviousPosition)
            {
                stringNumber = _searchCache[whatToFind].Item1;
                letterNumber = _searchCache[whatToFind].Item2;
            }

            Result = findInternal(whatToFind, stringNumber, letterNumber, waitCaching);
            
            return Result;
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
