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
            
            return new List<Position>();
        }
    }
}
