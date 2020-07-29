using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{
    internal static class Utils
    {
        public static void longArrayToFile(string path, long[] whatToRecord, int numberAmount = -1)
        {
            if (numberAmount < 0)
                numberAmount = whatToRecord.Length;
            byte[] bytes = new byte[numberAmount * sizeof(long)];
            Buffer.BlockCopy(whatToRecord, 0, bytes, 0, numberAmount * sizeof(long));

            using (FileStream fs = new FileStream(path, FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static long[] longArrayFromFile(string path)
        {
            long[] Result = new long[0];

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] bytes = br.ReadBytes((int)fs.Length);
                    long[] temp = new long[bytes.Length / sizeof(long)];
                    Buffer.BlockCopy(bytes, 0, temp, 0, bytes.Length);
                    Result = temp;
                }
            }

            return Result;
        }

    }
}
