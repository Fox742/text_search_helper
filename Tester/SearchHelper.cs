using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tester
{
    struct Position
    {
        public Position(long _StringNumber, long _letterNumber)
        {
            StringNumber = _StringNumber;
            LetterNumber = _letterNumber;
        }
        public readonly long StringNumber;
        public readonly long LetterNumber;
        
    }

    abstract class SearchHelper
    {
        protected string _path = string.Empty;
        
        public SearchHelper(string path)
        {
            _path = path;
        }

        protected long countLines()
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
                        if (Result % 100000 == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Lines counting...");
                            Console.WriteLine("Lines amount: {0}",Result);
                        }
                    }
                }
            }
            return Result;

        }

        public abstract List<Position> findAll(string whatToFind, bool waitCache = true);
    }
}
