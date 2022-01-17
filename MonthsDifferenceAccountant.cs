using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class MonthsDifferenceAccountant
    {
        public int DiffBetweenDates(DateTime firstDate, DateTime secondDate)
        {
            return ((firstDate.Year - secondDate.Year) * DataOperations.monthesInYear) + firstDate.Month - secondDate.Month;
        }
    }
}
