using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;
using System.IO;

namespace Tester
{
    /// <summary>
    /// Класс-обёртка над TSHelp-ером
    /// </summary>
    class CachedSearcher: IDisposable
    {
        private TSHelper _helper;
        public CachedSearcher(string path, TestsLogger logger, bool asyncCache = false)
        {
            _helper = new TSHelper(path, asyncCache, logger);
        }

        /// <summary>
        /// Функция поиска
        /// </summary>
        /// <param name="whatToFind">Что искать</param>
        /// <param name="waitCache">Нужно ли ожидать конца построения индекса-кеша</param>
        /// <returns></returns>
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

        public bool waitCacheBuilt()
        {
            return _helper.waitCacheBuilt();
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
