using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;
using System.IO;

namespace Tester
{
    class CachedSearcher: IDisposable
    {


        private TSHelper _helper;
        public CachedSearcher(string path, TestsLogger logger, bool asyncCache = false)
        {
            _helper = new TSHelper(path, asyncCache, logger);
        }

        public List<Position> findAll(string whatToFind, bool waitCache = true)
        {
            List<Position> Result = new List<Position>();
            Tuple<long, int>[] InternalResult = _helper.findAll(whatToFind, waitCache);

            for (int i = 0; i < InternalResult.Length; i++)
            {
                Result.Add(new Position(InternalResult[i].Item1, InternalResult[i].Item2));
            }

            return Result;
        }

        public void Dispose()
        {
            if (_helper!=null)
            {
                _helper.Dispose();
            }
        }
    }
}
