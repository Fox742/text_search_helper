using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;

namespace Tester
{
    class CachedSearcher:SearchHelper
    {
        private TSHelper _helper;
        public CachedSearcher(string path, bool asyncCache = false):base(path)
        {
            _helper = new TSHelper(path,asyncCache);
        }

        public override List<Position> findAll(string whatToFind,bool waitCache = true)
        {
            List<Position> Result = new List<Position>();
            Tuple<long,int> [] InternalResult = _helper.findAll(whatToFind, waitCache);
            
            for (int i=0;i<InternalResult.Length;i++)
            {
                Result.Add(new Position(InternalResult[i].Item1, InternalResult[i].Item2));
            }

            return Result;
        }
    }
}
