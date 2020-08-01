using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tester
{
    /// <summary>
    /// Класс, осуществляющий стандартный полнотекстовый поиск по файлу, реализованный с помощью стандартной библиотеки .NET
    /// </summary>
    class SimpleSearcher
    {

        private string _path = string.Empty;
        
        /// <summary>
        /// Посчитаем количество строк в целевом файле
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Имя (путь) файла, в котором будем искать</param>
        public SimpleSearcher(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Просмотр одной строки файла на предмет вхождения искомой строки
        /// </summary>
        /// <param name="LineNumber">Номер строки (для вывода результата)</param>
        /// <param name="where">Строка файла</param>
        /// <param name="what">Искомая строка</param>
        /// <returns></returns>
        private List<Position>viewLine(long LineNumber,string where,string what)
        {
            List<Position> Result = new List<Position>();
            int pos = -1;
            // До тех пор пока IndexOf выдаёт положительное число, сохраняем номера этих позиции
            while ((pos = where.IndexOf(what,pos+1))>=0)
            {
                Result.Add( new Position(LineNumber,pos) );
            }

            return Result;
        }

        /// <summary>
        /// Функция стандартного поиска
        /// </summary>
        /// <param name="whatToFind">Что искать</param>
        /// <param name="logger">Логгер</param>
        /// <returns></returns>
        public List<Position>findAll(string whatToFind, TestsLogger logger)
        {
            List<Position> Result = new List<Position>();
            long LinesAmount = countLines(logger);
            long LineNumber = 0;
            
            // Проходим по целевому файлу
            using (FileStream file1 = new FileStream(_path, FileMode.Open))
            {
                using (StreamReader r = new StreamReader(file1))
                {
                    string line;
                    // Считываем целевой файл построчно
                    while ((line = r.ReadLine()) != null)
                    {
                        // Добавляем в результирующий массив все вхождения искомой строки в текущей строке
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
