using System;
using System.IO;
using System.Collections.Generic;
using GeneratorLib;

/*
 * Программа для генерации больших файлов с текстом. Генерируемый текст сохраняется в виде файла в форме очень похожей на текст
 *      Слова берутся из списков русских и английских слов. Пока не стал разбивать модуль на классы - не считаю это нужным в данном случае */

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GeneratorLib.Generator.Generate(5000,printLogs:true);
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