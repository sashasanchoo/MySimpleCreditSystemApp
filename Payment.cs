using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    class Payment
    {
        private PaymentEnum choise;
        private string choiseString;
        private bool value;
        public bool Pay()
        {
            Console.WriteLine("1 - pay/0 - ignore");
            choiseString = Console.ReadLine();
            if (Enum.IsDefined(typeof(PaymentEnum), int.Parse(choiseString)))
            {
                choise = (PaymentEnum)Enum.Parse(typeof(PaymentEnum), choiseString);
                if (choise == PaymentEnum.Pay)
                {
                    value = true;
                }
                else if (choise == PaymentEnum.Ignore)
                {
                    value = false;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            return value;
        }
    }
}
