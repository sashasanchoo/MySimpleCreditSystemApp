using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DataAdapterForSQL:DataOperations
    {
        private string result;
        public string SQLDateTime(DateTime dateTime)
        {
            result = $"{dateTime.Month}/{dateTime.Day}/{dateTime.Year} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}";
            return result;
        }
    }
}
