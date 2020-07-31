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


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Путь к файлу, по содержимому которого необходимо будет осуществлять полнотекстовый поиск. Может быть относительным или абсолютным</param>
        /// <param name="asyncCacheBuilding">    Флаг как строить индекс-кеш, синхронно или асинхронно.
        ///                                             Синхронно - не выходим из конструктора, пока он не будет построен, 
        ///                                             Асинхронно - запускаем построение индекса, 
        /// </param>
        /// <param name="logger">Логгер. Для вывода информации через логгер необходимо пронаследовать от класса TSHelperLogger (для примера см. класс TestsLogger в проекте Tester)</param>
        public TSHelper(string path,bool asyncCacheBuilding = false, TSHelperLogger logger = null)
        {
            // Инициализируем логгер
            if (logger==null)
            {
                _logger = new TSHelperLogger();
            }
            else
            {
                _logger = logger;
            }

            // Инициализируем дополнительные пути, которые нам нужны будут (например путь к папке, в которой сожержится файл)
            _rawPathToFile = path;
            FileInfo fi = new FileInfo(_rawPathToFile);
            _folderPath = fi.DirectoryName;
            _fileName = fi.Name;

            // Строим или запускаем построение индекса-кеша
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

        /// <summary>
        /// Метод добавляет FileSystemWatcher для целевого файла
        /// </summary>
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

        /// <summary>
        /// Обработчик событий Rename и Delete для целевого файла
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenameDelete(object source, FileSystemEventArgs e)
        {
            Dispose(); // При переименовывании и удалении целевого файла TSHelper становится недействительным и им больше пользоваться нельзя
        }

        /// <summary>
        /// В файле что-то изменилось - нам нужно добавить в индекс записанные данные
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Открываем целевой файл
            using (FileStream fs = new FileStream(_rawPathToFile,FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    // Считываем его не с начала, а с той позиции, на которой закончили индексироввание в прошлый раз
                    fs.Seek( lastPosition, SeekOrigin.Begin );
                    while (!sr.EndOfStream)
                    {
                        string currentLine = sr.ReadLine();
                        // Для каждой новой строки вызываем функцию индексации строки
                        _cache.cacheLine(linesCached, currentLine, buildCacheCancel);
                        // Если был вызван Dispose и нужно отменить индексирование
                        if (buildCacheCancel.Token.IsCancellationRequested)
                            return;
                        linesCached++;
                    }
                    // Сохраняем новую самую последнюю позицию, начиная с которой будет индексировать в следущий раз
                    lastPosition = fs.Length;
                    _cache.flush();
                }
            }

        }
        
        /// <summary>
        /// Метод подсчёта строк в целевом файле
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Метод, запускающий асинхронное построение индекса-кеша
        /// </summary>
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

        /// <summary>
        /// Построить кеш-индекс
        /// </summary>
        private void buildCache()
        {
            _cache.resetCache();
            // Определим сколько строк в файле
            long linesAmount = countLines();
            long currentLine = 0;
            // Считываем целевой файл построчно
            using (FileStream file1 = new FileStream(_rawPathToFile, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(file1))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        // Вызываем функцию кеширования для каждой строки целевого файлп
                        _cache.cacheLine(currentLine, line,buildCacheCancel);
                        
                        // Проверяем токен отмены (это нужно на тот случай, если происходит построение индекса-кеша, но был вызван Dispose до окончания процесса)
                        if (buildCacheCancel.Token.IsCancellationRequested)
                        {
                            cacheBuildCancelled = true;
                            return;
                        }
                        currentLine++;

                        // Выводим информацию в логгер
                        if (currentLine % 100000 == 0)
                        {
                            _logger.Clear();
                            _logger.WriteLine("Cach building...");
                            _logger.WriteLine(string.Format("Lines amount: {0}", currentLine));
                            _logger.WriteLine(string.Format("Lines total: {0}", linesAmount));
                        }
                    }
                    // Запоминаем конечную позицию, чтобы при изменении содержимого файла продолжить индексирование вновь доавленной информации
                    lastPosition = file1.Length;
                }
            }
            linesCached = linesAmount;
            
            // Говорим индексу, чтобы он всё сохранил на файловую систему
            _cache.flush();
        }

        /// <summary>
        /// Основной метод для поиска строки в целевом файле. Открытые методы find/findAll должны вызывать эту функци
        /// </summary>
        /// <param name="whatToFind">Строка, которую надо искать</param>
        /// <param name="stringNumber">Номер строки целевого файла, начиная с которой нужно искать</param>
        /// <param name="letterNumber">     Номер позиции в строке, начиная с которой надо искать в строке stringNumber. Во всех строках, номер которых больше, чем stringNumber,
        ///                                 поиск будет осуществляться с нулевой позиции</param>
        /// <param name="waitCaching">Нужно ли ожидать завершения построения индекса-кеша в том случае, если к моменту вызова он не был ещё достроен</param>
        /// <param name="entriesNumber">Сколько вхождений искомой строки нужно вернуть. При значении -1 нужно вернуть все вхождения, которые есть в целевом файле</param>
        /// <returns></returns>
        private SubstringPosition[] findInternal(string whatToFind, long stringNumber, int letterNumber, bool waitCaching = true,long entriesNumber=-1)
        {
            List<SubstringPosition> result = new List<SubstringPosition>();
            
            bool cacheReady = _waitInitedInternal(waitCaching);
            if (!cacheReady)
                throw new WaitCacheException();

            // Получаем список номеров строк, в котором есть вхождения первых двух буков искомой строки и которые нужно проверить методом IndexOf.
            //              Строки, в которых нет таких вхождений мы проверять не будем
            long[] stringsNumbers = _cache.getStringNumbers(whatToFind);
            int stringsNumberPtr = 0; // Указатель на номер в массиве строк в которых есть вхождения первых двух буков строки whatToFind, которую мы будем проверять с помощью IndexOf
            string current;
            // Открываем целевой файл
            using (FileStream fs = new FileStream(_rawPathToFile, FileMode.Open,FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {

                    int currentStringNumber = -1;
                    _logger.Clear();
                    _logger.WriteLine("Cache searching process...");
                    // Читаем построчно целевой файл
                    while ( !sr.EndOfStream)
                    {
                        currentStringNumber++;
                        current = sr.ReadLine();
                        // Если текущая строка меньше минимального номера строки, которой мы вообще рассматриваем, то не продолжаем итерацию
                        if (currentStringNumber < stringNumber)
                            continue;

                        // Прокручиваем указатель stringsNumberPtr до тех пор, пока он не будет указывать на первую строку, номер которой больше, чем номер считанной на данной итерации из целевого файла
                        while (stringsNumbers[stringsNumberPtr] < currentStringNumber)
                        {
                            stringsNumberPtr++;
                            // Если мы достигла конца массива строк с вхождениями, то мы должны выйти из функции - мы больше ничего не найдём
                            if (stringsNumberPtr >= stringsNumbers.Length)
                                return result.ToArray();
                        }
                        // Если номер строки, прочитанной из целевого файла равен номеру из массива строк с вхождениями первых двух буков искомой строки,
                        //        мы должны её обработать и проанализировать есть ли вхождения не только первых двух буков искомой строки, но и всей строки whatToFind
                        if (stringsNumbers[stringsNumberPtr] == currentStringNumber)
                        {
                            // Начинаем с начала строки
                            int currentIndex = -1;
                            // Если строка первая, которую мы рассматрвиаем - то позицию буквы в строке берём из параметра
                            if (currentStringNumber==stringNumber)
                            {
                                currentIndex = letterNumber;
                            }

                            // До тех пор пока есть вхождения искомой строки в строке, прочитанной из файла
                            while ((currentIndex = current.IndexOf(whatToFind, currentIndex + 1)) >= 0)
                            {
                                // Инициализируем структуру с найденной позицией (номер строки, номер буквы)
                                SubstringPosition newPosition = new SubstringPosition(currentStringNumber, currentIndex);

                                // Записываем эту позицию в поисковый кеш, чтобы в следующий раз начать поиск с этой позиции
                                _searchCache[whatToFind] = newPosition;
                                result.Add(newPosition);

                                // Првоеряем сколько позиций вхождения искомой строки нам нужно вернуть и если достаточно - возвращаемся из функции
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

        /// <summary>
        /// Поиск строки whatToFind начиная со строки stringNumber и буквы с номером letterNumber
        /// </summary>
        /// <param name="whatToFind">Строка, которую необходимо найти</param>
        /// <param name="stringNumber">Номер строки, начиная с которого надо искать</param>
        /// <param name="letterNumber">Номер буквы в строке stringNumber, начиная с которого надо искать</param>
        /// <param name="returnPosition">Флаг указывающия нужно ли в параметрах stringNumber и letterNumber возвращать позицию найденного вхождения строки whatToFind</param>
        /// <param name="waitCaching">Нужно ли ожидать завершения построения индекса-кеша в том случае, если к моменту вызова он не был ещё достроен</param>
        /// <returns></returns>
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

        /// <summary>
        /// Поиск строки whatToFind
        /// </summary>
        /// <param name="whatToFind">Строка, которую необходимо найти</param>
        /// <param name="fromPreviousPosition">Флаг, который говорит что нужно ли искать вхождение строк с нулевой строки и 
        ///                                     нулевой буквы или начинать поесле позиции последнего найденного вхождения данной строки</param>
        /// <param name="waitCaching">Нужно ли ожидать завершения построения индекса-кеша в том случае, если к моменту вызова он не был ещё достроен (или выбросить исклюение)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Найти все вхождения строки whatToFind в целевом файле
        /// </summary>
        /// <param name="whatToFind">Строка, которую необходимо найти</param>
        /// <param name="waitCaching">Нужно ли ожидать завершения построения индекса-кеша в том случае, если к моменту вызова он не был ещё достроен (или выбросить исклюение)</param>
        /// <returns></returns>
        public SubstringPosition [] findAll(string whatToFind, bool waitCaching = true)
        {
            checkDisposed();
            SubstringPosition[] Result;
            long stringNumber = -1;
            int letterNumber = -1;

            Result = findInternal(whatToFind, stringNumber, letterNumber, waitCaching);

            return Result;
        }

        /// <summary>
        /// Сбросить поисковый кеш (то есть "забыть" последние позиции вхождений всех строк, когда либо найденных)
        /// </summary>
        public void resetAllSearch()
        {
            checkDisposed();
            _searchCache = new SearchCache();
        }

        /// <summary>
        /// Подождать пока построится индекс-кеш если он ещё не был построен
        /// </summary>
        /// <returns></returns>
        public bool waitCacheBuilt()
        {
            checkDisposed();
            return _waitInitedInternal(true);
        }

        /// <summary>
        /// Метод Dispose для TSHelper-а
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                
                // Если флаг inited не равен true, это значит, что индекс-кеш ещё строится (причём асинхронно). Чтобы предотвратить undefined behavior нам нужно остановить построение индекса-кеша
                if (!inited)
                {
                    buildCacheCancel.Cancel();
                    while (!cacheBuildCancelled){}
                }

                if (watcher != null)
                {
                    // Отписываем FileSystemWatcher-а от целевого файла и вызываем ему Dispose
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
