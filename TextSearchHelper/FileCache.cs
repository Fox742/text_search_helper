using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{
    //using EntryList = List<long>;
    //using EntryList = long[];
    internal class FileCache
    {
        private Dictionary<string, long[]> chains = new Dictionary<string, long[]>();
        private Dictionary<string, int> chainsPtr = new Dictionary<string, int>();
        //private Dictionary<string, int> chains = new Dictionary<string, int>();
        private string _cachePathFolder;
        private string _cachePath;
        public FileCache(string path, string Filename)
        {
            _cachePath = path;
            _cachePathFolder = Filename.Replace('.', '_')+"_index";
            resetCache();
            
            /*
            long [] el = new long[10];
            el[0] = 1111111111;
            el[1] = 2222222222;
            el[2] = 3333333333;
            el[3] = 4444444444;

            flush("ts", el,4);

            using (FileStream fs = new FileStream("C:\\Projects\\SSharp\\text_search_helper\\Generator\\huge_file_txt_index\\74_73.dat", FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    Console.WriteLine(br.ReadInt64());
                    Console.WriteLine(br.ReadInt64());
                    Console.WriteLine(br.ReadInt64());
                    Console.WriteLine(br.ReadInt64());
                }
            }
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
        }

        private void createIndex()
        {

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

        private void flush(string code, long[] el,int ptr)
        {
            /*
            string filename = nameToHex(code);
            List<byte> bytes = new List<byte>();
            for (int i=0;i<el.Count;i++)
            {
                bytes.AddRange( new List<byte>( BitConverter.GetBytes(el[i]) ) );
            }

            using (FileStream fs = new FileStream(_cachePath + "\\" + _cachePathFolder+"\\"+ filename + ".dat",FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                { 
                    bw.Write(bytes.ToArray(),0,bytes.Count);
                }
            }
            */

            string filename = nameToHex(code);
            byte[] bytes = new byte[ ptr  * sizeof(long)];
            Buffer.BlockCopy(el, 0, bytes, 0, ptr * sizeof(long));

            using (FileStream fs = new FileStream(_cachePath + "\\" + _cachePathFolder + "\\" + filename + ".dat", FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public void flush()
        {
            foreach(string oneKey in chains.Keys)
            {
                flush(oneKey,chains[oneKey],chainsPtr[oneKey]);
            }
            chains = new Dictionary<string, long[]>();
        }

        public void cacheLine(long lineNumber,string targetLine)
        {
            for (int i =0;i<targetLine.Length-1;i++)
            {
                string onechain = targetLine.Substring(i,2);
                if (!chains.ContainsKey(onechain))
                {
                    chains[onechain] = new long[1000];
                    chainsPtr[onechain] = 0;
                }
                if (chainsPtr[onechain]>= chains[onechain].Length)
                {
                    flush(onechain, chains[onechain], chainsPtr[onechain]);
                    chainsPtr[onechain] = 0;
                }
                //chains[onechain].Add(lineNumber);
                chains[onechain][chainsPtr[onechain]] = lineNumber;
                chainsPtr[onechain]++;
            }
        }

        ~FileCache()
        {
            removeIndexFolder();
        }
    }
}
