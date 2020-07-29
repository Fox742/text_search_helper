using System;
using System.IO;
using System.Collections.Generic;

/*
 * Программа для генерации больших файлов с текстом. Генерируемый текст сохраняется в виде файла в форме очень похожей на текст
 *      Слова берутся из списков русских и английских слов. Пока не стал разбивать модуль на классы - не считаю это нужным в данном случае */

namespace Generator
{
    class Program
    {
        //private static long fileSize = 250;
        private static long fileSize = 1000;
        //private static long fileSize = 5000;
        //private static long fileSize = 1;
        private static string fileName = "huge_file.txt";
        private static string EnglishFileName = "eng_dict.txt";
        private static string RussianFileName = "rus_dict.txt";
        private static List<Char> Signs = new List<char>();
        private static List<string> English = new List<string>();
        private static List<string> Russian = new List<string>();
        private static List<Char> Digits = new List<char>();
        

        private static Random r = new Random();

        private static int wordAmountBegin = 3;
        private static int wordAmountEnd = 20;

        private static uint bytesToMBytes(long bytesNumber)
        {
            return (uint)(bytesNumber / 1048576);
        }

        private static string MbytesToString(long bytesNumber)
        {
            string Result = "Undefined";
            Result = System.Convert.ToString(bytesNumber );


            return Result;
        }

        private static void makeSets()
        {
            // Инициализация слов
            English = new List<string>(File.ReadAllText(EnglishFileName).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            Russian = new List<string>(File.ReadAllText(RussianFileName).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // Инициализация словарей цифр и знаков препинания
            for (int i = 0; i < 10; i++)
            {
                Digits.Add(Convert.ToChar(i+(int)'0'));
            }

            Signs.Add('.');
            Signs.Add(',');
            Signs.Add('!');
            Signs.Add('?');
            Signs.Add(')');
            Signs.Add('(');
        }

        private static string makeWord(bool isWord = true)
        {
            string Result = string.Empty;
            // Вычисляем сколько должно быть буков в слове
            uint lettersAmount = (uint)r.Next( 2, 20);
            if (isWord)
            {
                // В половине случаев генерируем английское слово
                if ( r.Next(2)==1 )
                {
                    Result = Russian[r.Next(Russian.Count)];
                }
                else
                {
                    Result = English[r.Next(English.Count)];
                }

                // Поменяем первую букву на заглавную
                if (r.Next(10) == 9)
                {
                    if (Result.Length > 0)
                    {
                        char firstLetter = Result[0];
                        Result = Result.Remove(0,1);
                        Result = firstLetter.ToString().ToUpper() + Result;
                    }
                }

            }
            else
            {
                // Генерируем число из нескольких цифр
                for (uint i = 0; i < lettersAmount; i++)
                {
                    Result += Digits[r.Next(Digits.Count)];
                }
            }
            return Result;
        }

        private static string makeString()
        {
            // Сгенерируем количество слов в строке
            uint wordsNumber = (uint)r.Next(wordAmountBegin, wordAmountEnd);
            string Result = string.Empty;
            for (uint i=0;i<wordsNumber;i++)
            {
                string oneWord = string.Empty;
                // На 30 слов должно приходится одно число
                if (r.Next(30) == 29)
                {
                    oneWord = makeWord(false);
                }
                else
                {
                    oneWord = makeWord();
                }
                // Есть ли после слова знак препинания?
                if (r.Next(5) == 4)
                {
                    oneWord += Signs[r.Next(Signs.Count)];
                }

                // Если слово - не последнее в строке добавляем пробел в конце
                if (i!=wordsNumber-1)
                {
                    oneWord += " ";
                }

                Result += oneWord;
            }

            return Result;
        }

        static void Main(string[] args)
        {
            try
            {
                makeSets();
                uint stringsGenerated = 0;
                using (FileStream file1 = new FileStream(fileName, FileMode.Append))
                {
                    using (StreamWriter sw = new StreamWriter(file1))
                    {
                        FileInfo FI = new FileInfo(fileName);
                        do
                        {
                            string newString = makeString();
                            sw.WriteLine(newString);
                            stringsGenerated++;
                            if (stringsGenerated % 10000 == 0)
                            {
                                FI.Refresh();
                                Console.Clear();
                                Console.WriteLine("Generated {0} from {1} MBytes", MbytesToString(bytesToMBytes(FI.Length)), MbytesToString(fileSize));
                            }
                        }
                        while (bytesToMBytes(FI.Length) < fileSize);
                    }

                }

                Console.Clear();
                Console.WriteLine("Generated file: {0}", fileName);
            }
            catch (Exception exc)
            {
                Console.WriteLine(string.Concat("Error occured: ", exc.Message));
            }
            Console.WriteLine("Press Enter key to quit");
            Console.ReadLine();
        }
    }
}