using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class DateSetter
    {
        private DateTime currentDate;
        private int monthesQuantity;
        private DateTime boundaryDate;
        private int allDaysOfUsing;
        public void SetDates(Bill bill)
        {
            Console.WriteLine("Enter monthes quantity:");
            monthesQuantity = int.Parse(Console.ReadLine());
            currentDate = DateTime.Now;
            bill.RegistrationDate = currentDate;
            boundaryDate = currentDate.AddMonths(monthesQuantity);
            bill.BoundaryDate = boundaryDate;
            allDaysOfUsing = boundaryDate.Subtract(DateTime.Now).Days;
            bill.AllDaysOfUsing = allDaysOfUsing;
        }
    }
}
