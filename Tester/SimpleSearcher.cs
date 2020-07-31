using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tester
{
    class SimpleSearcher
    {

        private string _path = string.Empty;


        protected long countLines(TestsLogger logger)
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
                            logger.Clear();
                            logger.WriteLine("Lines counting...");
                            logger.WriteLine(string.Format("Lines amount: {0}", Result));
                        }
                    }
                }
            }
            return Result;
        }

        public SimpleSearcher(string path)
        {
            _path = path;
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

        public List<Position>findAll(string whatToFind, TestsLogger logger)
        {
            List<Position> Result = new List<Position>();
            long LinesAmount = countLines(logger);
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
                            logger.Clear();
                            logger.WriteLine(string.Format("Viewed lines:\t{0}", LineNumber));
                            logger.WriteLine(string.Format("Total lines:\t{0}", LinesAmount));

                        }
                        LineNumber++;
                    }
                }
            }

            return Result;
        }
    }
}
