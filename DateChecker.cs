using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DateChecker
    {
        private DateTime Beginning { get; set; }
        private DateTime End { get; set; }
        private DateTime ToCompare { get; set; }
        public DateChecker(DateTime beginning, DateTime end, DateTime ToCompare)
        {
            this.Beginning = beginning;
            this.End = end;
            this.ToCompare = ToCompare;
        }
        public void CheckDates()
        {
            if (ToCompare > End || ToCompare < Beginning)
            {
                throw new WrongData($"{ToCompare}");
            }
        }
    }
}
