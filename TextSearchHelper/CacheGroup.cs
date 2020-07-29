using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{

    internal class CacheGroup
    {
        private static int _stringNumbersSize = 1000;
        private int _stringNumbersPtr=0;
        private long[] _stringsNumbers;
        private long lastAddedNumber = 0;
        private bool wasAdded = false;

        public CacheGroup()
        {
            _stringsNumbers = new long[_stringNumbersSize];
        }

        public void AddLine(long newStringNumber)
        {
            if (!wasAdded || ((wasAdded) && (lastAddedNumber != newStringNumber)))
            {
                _stringsNumbers[_stringNumbersPtr] = newStringNumber;
                _stringNumbersPtr++;
                lastAddedNumber = newStringNumber;
                wasAdded = true;
            }
        }

        public bool isFull()
        {
            return _stringNumbersPtr >= _stringsNumbers.Length;
        }

        public void flush(string fileToFlush)
        {
            Utils.longArrayToFile(fileToFlush, _stringsNumbers, _stringNumbersPtr);
            _stringNumbersPtr = 0;
        }

        public static long[]getStringNumbers( string cacheFilePath )
        {
            return Utils.longArrayFromFile(cacheFilePath);
        }

    }
}
