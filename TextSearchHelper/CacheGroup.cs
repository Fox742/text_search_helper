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

        public static long[]getStringNumbers( string cacheFilePath )
        {
            long[] Result = new long[0];

            using (FileStream fs = new FileStream(cacheFilePath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    //byte[] bytes = new byte[fs.Length];
                    //for (int i = 0; i < fs.Length; i++)
                    //    bytes[i] = br.ReadByte();
                    byte[] bytes = br.ReadBytes((int)fs.Length); 
                    long[] temp = new long[bytes.Length/ sizeof(long) ];
                    Buffer.BlockCopy(bytes, 0, temp, 0, bytes.Length);
                    Result = temp;
                }
            }

            return Result;
        }

    }
}
