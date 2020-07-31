using System;
using System.Collections.Generic;
using System.Text;
using TextSearchHelper;

namespace Tester
{
    /// <summary>
    /// Класс-логгер, поддерживающий вывод преамбулы.
    /// </summary>
    class TestsLogger:TSHelperLogger
    {
        private bool _silentMode = false;

        public bool silentMode { set; get; }

        List<string> _preamble = new List<string>();

        public void AddToPreamble(string toAdd)
        {
            _preamble.Add(toAdd);
        }

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

        public override void Clear()
        {
            if (!silentMode)
            {
                printPreamble();
            }
        }

        public override void WriteLine(string toPrint)
        {
            if (!silentMode)
            {
                Console.WriteLine(toPrint);
            }
        }

    }
}
