using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace TextSearchHelper
{
    /// <summary>
    /// Файл, используемый для построения индекса-кеша на целевой файл
    /// </summary>
    internal class FileCache: IDisposable
    {
        private Dictionary<string, CacheGroup> chains = new Dictionary<string, CacheGroup>();
        private string _cachePathFolder;
        private string _cachePath;
        private static string _groupsCatalogCache = "groups";
        public FileCache(string path, string Filename)
        {
            // Конструируем директорию и имя папки, в котором хранятся файлы индекса-кеша
            _cachePath = path;
            _cachePathFolder = Filename.Replace('.', '_')+"_index";
            resetCache();            
        }

        /// <summary>
        /// Удаляем старый индекс-кеш для данного целевого файла если он существует)
        /// </summary>
        private void removeIndexFolder()
        {
            string cacheFullPath = _cachePath + "\\" + _cachePathFolder;
            if (Directory.Exists(cacheFullPath))
            {
                Directory.Delete(cacheFullPath, true);
            }
        }

        /// <summary>
        /// Создаём директории для нового индекса-кеша
        /// </summary>
        private void createIndexFolder()
        {
            Directory.CreateDirectory(_cachePath + "\\" + _cachePathFolder);
            Directory.CreateDirectory(_cachePath + "\\" + _cachePathFolder+"\\"+_groupsCatalogCache);

        }


        public void resetCache()
        {
            removeIndexFolder();
            createIndexFolder();
        }

        /// <summary>
        /// Функция, кодирующая первые два символа строки в код файла индекса-кеша
        /// </summary>
        /// <param name="whatToConvert"></param>
        /// <returns></returns>
        private string nameToHex(string whatToConvert)
        {
            return System.Convert.ToString(whatToConvert[0], 16) + "_" + System.Convert.ToString(whatToConvert[1], 16);
        }

        /// <summary>
        /// Выполняет сброс данных индекса-кеша на жёсткий диск в каталог индекса-кеша для конкретной пары символов
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupToFlush"></param>
        private void flush(string code, CacheGroup groupToFlush)
        {
            string indexFilePath = _cachePath + "\\" + _cachePathFolder + "\\" + _groupsCatalogCache +"\\" + nameToHex(code) + ".dat";
            groupToFlush.flush(indexFilePath);
        }

        /// <summary>
        /// Выполняет сохранение всего индекса-кеша в специализированный 
        /// </summary>
        public void flush()
        {
            foreach(string oneKey in chains.Keys)
            {
                flush(oneKey,chains[oneKey]);
            }
        }

        /// <summary>
        /// Индексирование-кеширование одной строки
        /// </summary>
        /// <param name="lineNumber">Номер строки, которую мы добавляем в индекс-кеш</param>
        /// <param name="targetLine">Сама строка, добавляемая в индекс</param>
        /// <param name="buildCacheCancel">Токен для отмены</param>
        public void cacheLine(long lineNumber,string targetLine, CancellationTokenSource buildCacheCancel=null)
        {
            // Проходим по символам строки
            for (int i =0;i<targetLine.Length-1;i++)
            {
                if ((buildCacheCancel!=null) && (buildCacheCancel.Token.IsCancellationRequested))
                    return;

                // Берем два символа (текущий и следующий)
                string onechain = targetLine.Substring(i,2);

                // Если в словаре не существует этой пары символов - добавляем в словарь CacheGroup, содержащий список номеров строк, в которых содержится данная пара onechain символов
                if (!chains.ContainsKey(onechain))
                {
                    chains[onechain] = new CacheGroup();
                }
                
                // Один объект CacheGroup не может содержать бесконечное число номеров строк (существует ограничение) и в случае его заполнения, нужно сбрасывать номера в индекс-кеш
                if (chains[onechain].isFull())
                {
                    flush(onechain, chains[onechain]);
                }
                // Добавляем номер строки в соответствующий CacheGroup
                chains[onechain].AddLine(lineNumber);
            }
        }

        /// <summary>
        /// Возвращает номера строк, в которых встречается первая пара символов строки whatToFind
        /// </summary>
        /// <param name="whatToFind"></param>
        /// <returns></returns>
        public long[] getStringNumbers(string whatToFind)
        {
            string code = string.Empty + whatToFind[0] + whatToFind[1];
            string indexFilePath = _cachePath + "\\" + _cachePathFolder + "\\" + _groupsCatalogCache + "\\"+ nameToHex(code) + ".dat";
            return CacheGroup.getStringNumbers(indexFilePath) ; 
        }

        /// <summary>
        /// Удаляем индекс при диспозировании объекта
        /// </summary>
        public void Dispose()
        {
            removeIndexFolder();
        }
    }
}
