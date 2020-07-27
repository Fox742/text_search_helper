using System;
using System.Collections.Generic;
using System.Text;

namespace Tester
{
    class Timer
    {
        private DateTime StartTimespan;

        public void reset()
        {
            StartTimespan = DateTime.Now;
        }

        public double getInterval()
        {
            return DateTime.Now.Subtract(StartTimespan).TotalMilliseconds;
        }

    }
}
