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
        List<string> _preamble = new List<string>();

        public void AddToPreamble(string toAdd)
        {
            _preamble.Add(toAdd);
        }

        public void ResetPreamble()
        {
            _preamble.Clear();
        }

        public override void Clear()
        {
            Console.Clear();
            foreach (string aString in _preamble)
            {
                Console.WriteLine(aString);
            }
        }

        public override void WriteLine(string toPrint)
        {
            Console.WriteLine(toPrint);
        }

    }
}
