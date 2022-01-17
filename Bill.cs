using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class Bill
    {
        public DateTime RegistrationDate { get; set; }
        public DateTime BoundaryDate { get; set; }
        public int PaymentsCount
        {
            set
            {
                for (int i = 0; i < value; i++)
                {
                    payments.Add(true);
                }
            }
            get { return payments.Count; }
        }

        public List<bool> payments = new List<bool>();//информация о платежах в пределах периода использования кредитного продукта

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == "")
                {
                    throw new NotValidValue(value);
                }
                name = value;
            }
        }//имя/индекс товара

        private decimal payPerDay;
        public decimal PayPerDay
        {
            get
            {
                return payPerDay;
            }

            set
            {
                if (value < 1)
                {
                    throw new NotValidValue(value);
                }
                payPerDay = value;
            }
        }//оплата за день
        public int AllDaysOfUsing { get; set; }//количество дней (общее количество дней использования кредитного продукта)

        private decimal penaltyForOneDay;
        public decimal PayPerOneDayPenalty
        {
            get
            {
                return penaltyForOneDay;
            }
            set
            {
                if (value < 1)
                {
                    throw new NotValidValue(value);
                }
                penaltyForOneDay = value;
            }
        }//штраф за один день задержки оплаты
        public int PenaltyDaysCount { get; set; }//количество дней просрочки

        private int payDate;
        public int PayDate
        {
            get
            {
                return payDate;
            }
            set
            {
                if (value < 0 || value > 25)
                {
                    throw new NotValidValue(value);
                }
                payDate = value;
            }
        }//граничная дата внесения ежемесячного платежа
        public decimal SummWithoutPenalties { get; set; }//сумма к оплате без штрафа

        private decimal penaltiesSumm;
        public decimal PenaltiesSumm
        {
            get
            {
                return PenaltyDaysCount * PayPerOneDayPenalty;
            }
            set
            {
                this.penaltiesSumm = value;
            }
        }//штраф

        public decimal summWithoutPlusPenalties;
        public decimal SummWithoutPlusPenalties
        {
            get
            {
                return PenaltiesSumm + SummWithoutPenalties;
            }
            set
            {
                this.summWithoutPlusPenalties = value;
            }
        }//общая сумма к оплате

        public Bill(string name, decimal payPerDay, decimal penaltyForOneDay, int payDate)
        {
            this.Name = name;
            this.PayPerDay = payPerDay;
            this.PayPerOneDayPenalty = penaltyForOneDay;
            this.PayDate = payDate;
        }
        public Bill()
        { }
        public override string ToString()
        {
            return $"Name {Name}\nRegistration date: {RegistrationDate}\nBoundary date: {BoundaryDate}\nDaily payment: {PayPerDay}" +
                $"\nDaily penalty {PayPerOneDayPenalty}\nMonthly pay date {PayDate}";

        }
    }
}
