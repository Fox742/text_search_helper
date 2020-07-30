using System;
using System.Collections.Generic;
using System.Text;

namespace TextSearchHelper
{
    public class WaitCacheException: System.Exception
    {
        public WaitCacheException():base("Find call at the time of cache building. You can use \"waitCaching\" to wait cache silently")
        {

        }

    }
}
