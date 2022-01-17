using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class PenaltyDaysAccountant
    {
        private DateTime usersDate;
        private int year;
        private int month;
        private int paymentsCount;
        private int payDate;
        private int penaltyDaysCount;
        private DateTime startPenaltiesDate;
        public PenaltyDaysAccountant(DateTime usersDate, Bill bill)
        {
            this.usersDate = usersDate;
            this.paymentsCount = bill.PaymentsCount;
            this.payDate = bill.PayDate;
            this.year = bill.RegistrationDate.Year;
            this.month = bill.RegistrationDate.Month;
        }
        public int CountPenaltyDays()
        {
            month += paymentsCount;
            if (month > DataOperations.monthesInYear)
            {
                year += 1;
                month -= DataOperations.monthesInYear;
            }
            startPenaltiesDate = new DateTime(year, month, payDate);
            penaltyDaysCount = usersDate.Subtract(startPenaltiesDate).Days;
            return penaltyDaysCount;
        }
    }
}
