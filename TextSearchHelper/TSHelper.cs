using System;

namespace TextSearchHelper
{
    public class TSHelper
    {
        public TSHelper(string path,bool asyncCacheBuilding = false)
        {

        }

        public bool find(string whatToFind, ref uint stringNumber, ref uint letterNumber, bool returnPosition = false)
        {
            return false;
        }

        public bool find(string whatToFind, bool fromPreviousPosition = false)
        {
            return false;
        }

        public bool waitCacheBuilt(uint timeoutMSec = 5000)
        {
            return true;
        }
        


    }
}
