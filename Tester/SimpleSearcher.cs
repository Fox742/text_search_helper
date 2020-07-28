using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tester
{
    class SimpleSearcher:SearchHelper
    {

        public SimpleSearcher(string path):base(path)
        {

        }

        private List<Position>viewLine(long LineNumber,string where,string what)
        {
            List<Position> Result = new List<Position>();
            int pos = -1;
            while ((pos = where.IndexOf(what,pos+1))>=0)
            {
                Result.Add( new Position(LineNumber,pos) );
            }

            return Result;
        }

        public override List<Position>findAll(string whatToFind)
        {
            List<Position> Result = new List<Position>();
            long LinesAmount = countLines();
            long LineNumber = 0;
            using (FileStream file1 = new FileStream(_path, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(file1))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        Result.AddRange( viewLine(LineNumber,line, whatToFind) );
                        if (LineNumber % 100000 == 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Simple search ...");
                            Console.WriteLine("Viewed lines:\t{0}", LineNumber);
                            Console.WriteLine("Total lines:\t{0}", LinesAmount);

                        }
                        LineNumber++;
                    }
                }
            }

            return Result;
        }
    }
}
