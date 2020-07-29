using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;

namespace Tester
{
    class CachedSearcher:SearchHelper
    {
        private TSHelper _helper;
        public CachedSearcher(string path):base(path)
        {
            _helper = new TSHelper(path);
        }

        public override List<Position> findAll(string whatToFind)
        {
            List<Position> Result = new List<Position>();
            long str = -1;
            int letter = -1;
            bool isFound = false;
            do
            {
                isFound = _helper.find(whatToFind, ref str, ref letter, true);
                if (isFound)
                {
                    Result.Add(new Position(str,letter));
                }
            }
            while (isFound);

            return Result;
        }
    }
}
