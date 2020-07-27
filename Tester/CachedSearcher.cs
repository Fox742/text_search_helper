using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    class CachedSearcher:SearchHelper
    {
        public CachedSearcher(string path):base(path)
        {

        }

        public override List<Position> findAll(string whatToFind)
        {
            return new List<Position>();
        }
    }
}
