using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class PaymentChecker
    {
        private CustomDateKeeper dateKeeper;
        private DateChecker dateChecker;
        private PenaltyDaysAccountant penaltyDaysAccountant;
        private int paymentsCount;
        private int daysOfUsingProductCount;
        private int penaltyDaysCount;
        private int regularMonth = 30;
        private int monthsOfUsingProduct;
        private int totalMonths;
        private int monthsWithPenaltiesCount;

        public void NewCheckPayments(Bill bill, DateTime usersDate)
        {
            dateChecker = new DateChecker(bill.RegistrationDate, bill.BoundaryDate, usersDate);
            dateChecker.CheckDates();
            dateKeeper = new CustomDateKeeper(usersDate);
            totalMonths = new MonthsDifferenceAccountant().DiffBetweenDates(bill.BoundaryDate, bill.RegistrationDate);
            daysOfUsingProductCount = dateKeeper.UsersDate.Subtract(bill.RegistrationDate).Days;//количество дней пользования кредитным продуктом
            paymentsCount = bill.PaymentsCount;
            monthsOfUsingProduct = new MonthsDifferenceAccountant().DiffBetweenDates(dateKeeper.UsersDate, bill.RegistrationDate);
            monthsWithPenaltiesCount = monthsOfUsingProduct - paymentsCount;


            if (totalMonths - bill.PaymentsCount == totalMonths - monthsOfUsingProduct)
            {
                if (bill.PayDate >= dateKeeper.UsersDate.Day)
                {
                    bill.SummWithoutPenalties = (bill.AllDaysOfUsing * bill.PayPerDay) / (bill.AllDaysOfUsing / regularMonth);
                    Console.WriteLine($"Required payment: {Math.Round(bill.SummWithoutPenalties, 2)}$");
                    if (new Payment().Pay())
                    {
                        bill.payments.Add(true);
                    }
                    bill.SummWithoutPenalties = 0;
                }
                else if (bill.PayDate <= dateKeeper.UsersDate.Day)
                {
                    int penaltyDaysCount = dateKeeper.UsersDate.Day - bill.PayDate;
                    bill.PenaltyDaysCount = penaltyDaysCount;
                    bill.SummWithoutPenalties = (bill.AllDaysOfUsing * bill.PayPerDay) / (bill.AllDaysOfUsing / regularMonth);
                    Console.WriteLine($"Required payment: {Math.Round(bill.SummWithoutPlusPenalties, 2)}$, which includes {Math.Round(bill.PenaltiesSumm, 2)}$ as penalties");
                    if (new Payment().Pay())
                    {
                        bill.payments.Add(true);
                    }
                    bill.SummWithoutPenalties = 0;
                    bill.PenaltyDaysCount = 0;
                }
            }
            else if (totalMonths - bill.PaymentsCount < totalMonths - monthsOfUsingProduct)
            {
                Console.WriteLine("No required payments this month");
            }
            else if (totalMonths - bill.PaymentsCount > totalMonths - monthsOfUsingProduct)
            {
                penaltyDaysAccountant = new PenaltyDaysAccountant(dateKeeper.UsersDate, bill);
                penaltyDaysCount = penaltyDaysAccountant.CountPenaltyDays();
                bill.PenaltyDaysCount = penaltyDaysCount;
                bill.SummWithoutPenalties = (bill.AllDaysOfUsing * bill.PayPerDay) / (bill.AllDaysOfUsing / regularMonth);
                bill.SummWithoutPenalties = bill.SummWithoutPenalties + bill.SummWithoutPenalties * monthsWithPenaltiesCount;
                Console.WriteLine($"Required payment: {Math.Round(bill.SummWithoutPlusPenalties, 2)}$, which includes {Math.Round(bill.PenaltiesSumm)}$ as penalties");
                if (new Payment().Pay())
                {
                    for (int i = 0; i <= monthsWithPenaltiesCount; i++)
                    {
                        bill.payments.Add(true);
                    }
                    bill.SummWithoutPenalties = 0;
                    bill.PenaltyDaysCount = 0;
                }
            }
        }
    }
}
