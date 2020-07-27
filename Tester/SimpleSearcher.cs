using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    class SimpleSearcher:SearchHelper
    {

        public SimpleSearcher(string path):base(path)
        {

        }

        public override List<Position>findAll(string whatToFind)
        {
            long LinesAmount = countLines();
            
            return new List<Position>();
        }
    }
}
