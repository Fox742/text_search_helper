using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{
    /// <summary>
    /// Класс, содержащий список номеров строк, в которых содержатся пары символов
    /// </summary>
    internal class CacheGroup
    {
        private static int _stringNumbersSize = 1000;
        private int _stringNumbersPtr=0;
        private long[] _stringsNumbers;
        private long lastAddedNumber = 0;
        private bool wasAdded = false;
        
        public CacheGroup()
        {
            // Массив для хранения номеров. Инициализируется в конструкторе и не переинициализируется в дальнейшем для повышения производительности
            _stringsNumbers = new long[_stringNumbersSize];
        }

        /// <summary>
        /// Добавить номер строки, в котором есть вхождение пары символов
        /// </summary>
        /// <param name="newStringNumber"></param>
        public void AddLine(long newStringNumber)
        {
            // Проверка, что один и тот же номер не добавляется
            if (!wasAdded || ((wasAdded) && (lastAddedNumber != newStringNumber)))
            {
                _stringsNumbers[_stringNumbersPtr] = newStringNumber;
                _stringNumbersPtr++;
                lastAddedNumber = newStringNumber;
                wasAdded = true;
            }
        }

        /// <summary>
        /// Проверка полон ли список номеров
        /// </summary>
        /// <returns></returns>
        public bool isFull()
        {
            return _stringNumbersPtr >= _stringsNumbers.Length;
        }

        /// <summary>
        /// Сохранить номера строк с вхожденями в индекс-кеш
        /// </summary>
        /// <param name="fileToFlush"></param>
        public void flush(string fileToFlush)
        {
            Utils.longArrayToFile(fileToFlush, _stringsNumbers, _stringNumbersPtr);
            _stringNumbersPtr = 0;
        }

        /// <summary>
        /// Получить массив номеров строк, в которых есть вхождения пары символов
        /// </summary>
        /// <param name="cacheFilePath"></param>
        /// <returns></returns>
        public static long[]getStringNumbers( string cacheFilePath )
        {
            return Utils.longArrayFromFile(cacheFilePath);
        }

    }
}
