using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TextSearchHelper
{
    internal static class Utils
    {
        /// <summary>
        /// Запись в файл массив long-ов
        /// </summary>
        /// <param name="path">Имя(path) файла</param>
        /// <param name="whatToRecord">Массив, который нужно записать</param>
        /// <param name="numberAmount">Количество элементов массива numberAmount, которое нужно записать в файл</param>
        public static void longArrayToFile(string path, long[] whatToRecord, int numberAmount = -1)
        {
            // Преобразуем массив в массив байтов
            if (numberAmount < 0)
                numberAmount = whatToRecord.Length;
            byte[] bytes = new byte[numberAmount * sizeof(long)];
            Buffer.BlockCopy(whatToRecord, 0, bytes, 0, numberAmount * sizeof(long));

            // Записываем массив байтов в выходной файл
            using (FileStream fs = new FileStream(path, FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(bytes, 0, bytes.Length);
                }
            }
        }

        /// <summary>
        /// Получить массив long-ов из файла
        /// </summary>
        /// <param name="path">Файл, из которого нужно прочитать long-и</param>
        /// <returns></returns>
        public static long[] longArrayFromFile(string path)
        {
            long[] Result = new long[0];

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Читаем массив байтов из файла
                    byte[] bytes = br.ReadBytes((int)fs.Length);

                    // Преобразование массива байтов в выходной массив long-ов
                    long[] temp = new long[bytes.Length / sizeof(long)];
                    Buffer.BlockCopy(bytes, 0, temp, 0, bytes.Length);
                    Result = temp;
                }
            }

            return Result;
        }

    }
}
