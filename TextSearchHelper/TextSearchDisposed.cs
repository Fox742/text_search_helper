using System;
using System.Collections.Generic;
using System.Text;

namespace TextSearchHelper
{
    public class TextSearchDisposed: System.Exception
    {
        public TextSearchDisposed():base("The object of TSHelper is already disposed and can't be used due file renaming or deleting. Please recreate TSHelper")
        {

        }
    }
}
