using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{
    internal class FileCache
    {
        //private Dictionary<string, long[]> chains = new Dictionary<string, long[]>();
        //private Dictionary<string, int> chainsPtr = new Dictionary<string, int>();
        private Dictionary<string, CacheGroup> chains = new Dictionary<string, CacheGroup>();
        private string _cachePathFolder;
        private string _cachePath;
        private static string _groupsCatalogCache = "groups";
        public FileCache(string path, string Filename)
        {
            _cachePath = path;
            _cachePathFolder = Filename.Replace('.', '_')+"_index";
            resetCache();

            /*
            //---
            CacheGroup cg = new CacheGroup();

            cg.AddLine(11111111111);
            cg.AddLine(22222222222);
            cg.AddLine(33333333333);
            cg.AddLine(44444444444);

            flush("ts", cg);
            long[] test = CacheGroup.getStringNumbers("C:\\Projects\\SSharp\\text_search_helper\\Generator\\huge_file_txt_index\\74_73.dat");
            for (int i = 0; i < test.Length; i++)
                Console.WriteLine(test[i]);
                */
            
        }

        private void removeIndexFolder()
        {
            string cacheFullPath = _cachePath + "\\" + _cachePathFolder;
            if (Directory.Exists(cacheFullPath))
            {
                Directory.Delete(cacheFullPath, true);
            }
        }

        private void createIndexFolder()
        {
            Directory.CreateDirectory(_cachePath + "\\" + _cachePathFolder);
            Directory.CreateDirectory(_cachePath + "\\" + _cachePathFolder+"\\"+_groupsCatalogCache);

        }

        public void resetCache()
        {
            removeIndexFolder();
            createIndexFolder();
        }

        private string nameToHex(string whatToConvert)
        {
            return System.Convert.ToString(whatToConvert[0], 16) + "_" + System.Convert.ToString(whatToConvert[1], 16);
        }

        private void flush(string code, CacheGroup groupToFlush)
        {
            string indexFilePath = _cachePath + "\\" + _cachePathFolder + "\\" + _groupsCatalogCache +"\\" + nameToHex(code) + ".dat";
            groupToFlush.flush(indexFilePath);
        }

        public void flush()
        {
            foreach(string oneKey in chains.Keys)
            {
                flush(oneKey,chains[oneKey]);
            }
        }

        public void cacheLine(long lineNumber,string targetLine)
        {
            for (int i =0;i<targetLine.Length-1;i++)
            {
                string onechain = targetLine.Substring(i,2);
                if (!chains.ContainsKey(onechain))
                {
                    chains[onechain] = new CacheGroup();
                }
                if (chains[onechain].isFull())
                {
                    flush(onechain, chains[onechain]);
                }
                chains[onechain].AddLine(lineNumber);
            }
        }

        public long[] getStringNumbers(string whatToFind)
        {
            string code = string.Empty + whatToFind[0] + whatToFind[1];
            string indexFilePath = _cachePath + "\\" + _cachePathFolder + "\\" + _groupsCatalogCache + "\\"+ nameToHex(code) + ".dat";
            return CacheGroup.getStringNumbers(indexFilePath) ; 
        }

        ~FileCache()
        {
            removeIndexFolder();
        }
    }
}
