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

        public CacheGroup()
        {
            _stringsNumbers = new long[_stringNumbersSize];
        }

        public void AddLine(long newStringNumber)
        {

            _stringsNumbers[_stringNumbersPtr] = newStringNumber;
            _stringNumbersPtr++;
        }

        public bool isFull()
        {
            return _stringNumbersPtr >= _stringsNumbers.Length;
        }

        public void flush(string fileToFlush)
        {
            byte[] bytes = new byte[_stringNumbersPtr * sizeof(long)];
            Buffer.BlockCopy(_stringsNumbers, 0, bytes, 0, _stringNumbersPtr * sizeof(long));

            using (FileStream fs = new FileStream(fileToFlush, FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes, 0, bytes.Length);
                }
            }
            _stringNumbersPtr = 0;
        }

    }
}
