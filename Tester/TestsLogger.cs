using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;

namespace Tester
{
    /// <summary>
    /// Класс-логгер для класса TSHelper, поддерживающий вывод преамбулы
    /// 
    /// Преамбула здесь это некоторый набор строк строк, который автоматически выводится на экран после вызова Clear().
    ///     Таким образом сразу после вызова Clear() на экране оказывается строки преамбулы
    /// 
    /// </summary>
    class TestsLogger:TSHelperLogger
    {
        /// <summary>
        /// Свойство, позвояющее отключать и включать вывод логов из TSHelper-а
        /// </summary>
        public bool silentMode { set; get; }

        List<string> _preamble = new List<string>();

        /// <summary>
        /// Добавить строку в преамбулу
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddToPreamble(string toAdd)
        {
            _preamble.Add(toAdd);
        }

        /// <summary>
        /// Удалить все строки из преамбулы
        /// </summary>
        public void ResetPreamble()
        {
            _preamble.Clear();
        }

        public void printPreamble()
        {
                Console.Clear();
                foreach (string aString in _preamble)
                {
                    Console.WriteLine(aString);
                }
        }

        /// <summary>
        /// Метод Clear вызываемый внутри класса TSHelper
        /// </summary>
        public override void Clear()
        {
            if (!silentMode)
            {
                printPreamble();
            }
        }

        /// <summary>
        /// Метод WriteLine вызываемый из класса TSHelper
        /// </summary>
        /// <param name="toPrint"></param>
        public override void WriteLine(string toPrint)
        {
            if (!silentMode)
            {
                Console.WriteLine(toPrint);
            }
        }

    }
}
