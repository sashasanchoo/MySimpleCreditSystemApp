using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MyApp
{
    class ProductDoesntExist : ApplicationException
    {
        public ProductDoesntExist(string name) : base($"The product with name \"{name}\" doesn't exist")
        { }
    }
    class ProductAlreadyExists : ApplicationException
    {
        public ProductAlreadyExists(string name) : base($"The product with name \"{name}\" already exist")
        {

        }
    }
    class WrongData : ApplicationException
    {
        public WrongData(string name) : base($"Entered date \"{name}\" is not valid")
        { }
    }
    class DataOperations
    {
        protected SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBForMyApp"].ConnectionString);
        protected SqlCommand sqlCommand = null;
        protected string[] valuesArray = null;
        protected string[] propertyNamesArray = null;
        protected PropertyInfo[] myPropertyInfo = null;
        protected string values = string.Empty;
        protected string propertyNames = string.Empty;
        protected Regex r = null;
        protected string value = string.Empty;
        protected SqlDataReader sqlDataReader = null;
        protected string command = string.Empty;
    }

    class DataDeleter : DataOperations
    {
        string name;
        public DataDeleter(string name)
        {
            this.name = name;
        }
        public void DeleteData()
        {
            sqlConnection.Open();
            command = $"delete from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }
    }
    class DataWriter<T> : DataOperations
    {
        public void WriteData(T tmp)
        {
            sqlConnection.Open();
            myPropertyInfo = tmp.GetType().GetProperties();
            valuesArray = new string[myPropertyInfo.Length];
            propertyNamesArray = new string[myPropertyInfo.Length];
            DataAdapterForSQL dataAdapter;
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                if (myPropertyInfo[i].PropertyType.Name.Contains("DateTime"))
                {
                    dataAdapter = new DataAdapterForSQL();
                    dataAdapter.SQLDateTime(Convert.ToDateTime(myPropertyInfo[i].GetValue(tmp)));
                    value = Convert.ToString(dataAdapter.SQLDateTime(Convert.ToDateTime(myPropertyInfo[i].GetValue(tmp))));
                    value = value.Replace('.', '/');
                }
                else
                {
                    value = Convert.ToString(myPropertyInfo[i].GetValue(tmp));
                }
                valuesArray[i] = "'" + value + "'";
                propertyNamesArray[i] = myPropertyInfo[i].Name;
            }
            values = String.Join(",", valuesArray);
            propertyNames = String.Join(",", propertyNamesArray);
            command = $"insert into[Products] ({propertyNames}) values({values})";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }
    }
    class DataAdapterForSQL : DataOperations
    {
        DateTime sqlDateTime;
        public DateTime SQLDateTime(DateTime dateTime)
        {
            sqlDateTime = new DateTime(dateTime.Year, dateTime.Day, dateTime.Month);
            return sqlDateTime;
        }
    }
    class NameChecker : DataOperations
    {
        private int digit;
        public int CheckName(string name)
        {
            sqlConnection.Open();
            command = $"select count(*) from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            digit = (Int32)sqlCommand.ExecuteScalar();
            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
            return digit;
        }
    }
    class DataReader<T> : DataOperations
    {
        public void ReadData(T tmp, string name)
        {
            sqlConnection.Open();
            myPropertyInfo = tmp.GetType().GetProperties();
            sqlCommand = new SqlCommand();
            propertyNamesArray = new string[myPropertyInfo.Length];
            NameChecker nameChecker = new NameChecker();
            if (nameChecker.CheckName(name) == 0)
            {
                throw new ProductDoesntExist(name);
            }
            command = $"select * from [Products] where Name like '{name}'";
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                for (int i = 0; i < myPropertyInfo.Length; i++)
                {
                    myPropertyInfo[i].SetValue(tmp, sqlDataReader[$"{myPropertyInfo[i].Name}"]);
                }
            }

            if (sqlDataReader != null)
            {
                sqlDataReader.Close();
            }
        }
    }
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
        public string Name { get; set; }//имя/индекс товара
        public decimal PayPerDay { get; set; }//оплата за день
        public int AllDaysOfUsing { get; set; }//количество дней (общее количество дней использования кредитного продукта)
        public decimal PayPerOneDayPenalty { get; set; }//штраф за один день задержки оплаты
        public int PenaltyDaysCount { get; set; }//количество дней просрочки
        public int PayDate { get; set; }//граничная дата внесения ежемесячного платежа
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
    class Payment
    {
        int choise;
        public bool Pay()
        {
            Console.WriteLine("1 - pay/0 - ignore");
            choise = int.Parse(Console.ReadLine());
            if (choise == 1)
            {
                return true;
            }
            else if (choise == 0)
            {
                return false;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    interface IDataOperations<T>
    {
        void SetDates(ref T tmp);
    }
    class DataSetter : IDataOperations<Bill>
    {
        private DateTime currentDate;
        private int monthesQuantity;
        private DateTime boundaryDate;
        private int allDaysOfUsing;
        private int currentMonth = 1;
        public void SetDates(ref Bill bill)
        {
            Console.WriteLine("Enter monthes quantity:");
            monthesQuantity = int.Parse(Console.ReadLine());
            currentDate = DateTime.Now;
            bill.RegistrationDate = currentDate;
            boundaryDate = currentDate.AddMonths(monthesQuantity - currentMonth);
            bill.BoundaryDate = boundaryDate;
            allDaysOfUsing = boundaryDate.Subtract(DateTime.Now).Days;
            bill.AllDaysOfUsing = allDaysOfUsing;
        }
    }
    class CustomDateKeeper
    {
        public DateTime UsersDate { get; set; }
        public CustomDateKeeper(DateTime usersDate)
        {
            this.UsersDate = usersDate;
        }
    }
    class DateChecker
    {
        public DateTime Beginning { get; set; }
        public DateTime End { get; set; }
        public DateTime ToCompare { get; set; }
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
    class PenaltyDaysAccountant
    {
        private DateTime usersDate;
        private int year;
        private int month;
        private int paymentsCount;
        private int payDate;
        private int penaltyDaysCount;
        private int monthesInYear = 12;
        DateTime startPenaltiesDate;
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
            month = month + paymentsCount;
            if (month > monthesInYear)
            {
                year += 1;
                month -= monthesInYear;
            }
            startPenaltiesDate = new DateTime(year, month, payDate);
            penaltyDaysCount = usersDate.Subtract(startPenaltiesDate).Days;
            return penaltyDaysCount;
        }
    }

    class MonthsDifferenceAccountant
    {
        public int DiffBetweenDates(DateTime firstDate, DateTime secondDate)
        {
            return ((firstDate.Year - secondDate.Year) * 12) + firstDate.Month - secondDate.Month;
        }
    }
    class PaymentChecker
    {
        CustomDateKeeper dateKeeper;
        DateChecker dateChecker;
        PenaltyDaysAccountant penaltyDaysAccountant;
        int paymentsCount;
        int penaltyDaysCount;
        public int regularMonth = 30;
        int monthsOfUsingProduct;
        int totalMonths;
        int monthsWithPenaltiesCount;

        public void NewCheckPayments(ref Bill bill, DateTime usersDate)
        {
            dateChecker = new DateChecker(bill.RegistrationDate, bill.BoundaryDate, usersDate);
            dateChecker.CheckDates();
            dateKeeper = new CustomDateKeeper(usersDate);
            totalMonths = new MonthsDifferenceAccountant().DiffBetweenDates(bill.BoundaryDate, bill.RegistrationDate);            
            paymentsCount = bill.PaymentsCount;
            monthsOfUsingProduct = new MonthsDifferenceAccountant().DiffBetweenDates(dateKeeper.UsersDate, bill.RegistrationDate);
            monthsWithPenaltiesCount = monthsOfUsingProduct - paymentsCount;


            if (totalMonths - bill.PaymentsCount == totalMonths - monthsOfUsingProduct)
            {
                if (bill.PayDate >= dateKeeper.UsersDate.Day)
                {
                    bill.SummWithoutPenalties = (bill.AllDaysOfUsing * bill.PayPerDay) / (bill.AllDaysOfUsing / regularMonth);
                    Console.WriteLine($"Required payment: {bill.SummWithoutPenalties}$");
                    if (new Payment().Pay())
                    {
                        bill.payments.Add(true);
                        bill.PenaltiesSumm = 0;
                        bill.SummWithoutPenalties = 0;
                        bill.PenaltyDaysCount = 0;
                        bill.SummWithoutPlusPenalties = 0;
                    }
                }
                else if (bill.PayDate <= dateKeeper.UsersDate.Day)
                {
                    int penaltyDaysCount = dateKeeper.UsersDate.Day - bill.PayDate;
                    bill.PenaltyDaysCount = penaltyDaysCount;
                    bill.SummWithoutPenalties = (bill.AllDaysOfUsing * bill.PayPerDay) / (bill.AllDaysOfUsing / regularMonth);
                    Console.WriteLine($"Required payment: {bill.SummWithoutPlusPenalties}$, which includes {bill.PenaltiesSumm}$ as penalties");
                    if (new Payment().Pay())
                    {
                        bill.payments.Add(true);
                        bill.PenaltiesSumm = 0;
                        bill.SummWithoutPenalties = 0;
                        bill.PenaltyDaysCount = 0;
                        bill.SummWithoutPlusPenalties = 0;
                    }
                }
            }
            else if (totalMonths - bill.PaymentsCount < totalMonths -
                monthsOfUsingProduct)
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
                Console.WriteLine($"Required payment: {bill.SummWithoutPlusPenalties}$, which includes {bill.PenaltiesSumm}$ as penalties");
                if (new Payment().Pay())
                {
                    for (int i = 0; i <= monthsWithPenaltiesCount; i++)
                    {
                        bill.payments.Add(true);
                    }
                    bill.PenaltiesSumm = 0;
                    bill.SummWithoutPenalties = 0;
                    bill.PenaltyDaysCount = 0;
                    bill.SummWithoutPlusPenalties = 0;
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo ch;
            do
            {
                Console.WriteLine("1 - add product");
                Console.WriteLine("2 - create payment");
                try
                {
                    string choise = Console.ReadLine();
                    if (int.Parse(choise) < 1 || int.Parse(choise) > 2)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    switch (choise)
                    {
                        case "1":
                            {
                                try
                                {
                                    Console.WriteLine("Enter product's name:");
                                    string name = Console.ReadLine();
                                    Console.WriteLine("Enter payment for one day of using:");
                                    string payPerDay = Console.ReadLine();
                                    Console.WriteLine("Enter daily penalty:");
                                    string dailyPenalty = Console.ReadLine();
                                    Console.WriteLine("Enter monthly pay date:");
                                    string payDate = Console.ReadLine();
                                    Bill bill = new Bill(name, Convert.ToDecimal(payPerDay), Convert.ToDecimal(dailyPenalty), Convert.ToInt32(payDate));
                                    DataSetter dataSetter = new DataSetter();
                                    dataSetter.SetDates(ref bill);
                                    DataWriter<Bill> dataWriter = new DataWriter<Bill>();
                                    dataWriter.WriteData(bill);
                                }
                                catch (ProductAlreadyExists ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            }
                        case "2":
                            {
                                try
                                {
                                    Console.WriteLine("Enter product's name:");
                                    string name = Console.ReadLine();
                                    Bill bill = new Bill();
                                    DataReader<Bill> dataReader = new DataReader<Bill>();
                                    dataReader.ReadData(bill, name);
                                    Console.WriteLine(bill);
                                    Console.WriteLine("Enter date's year:");
                                    int year = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Enter date's month:");
                                    int month = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Enter date's day");
                                    int day = int.Parse(Console.ReadLine());
                                    PaymentChecker paymentChecker = new PaymentChecker();
                                    paymentChecker.NewCheckPayments(ref bill, new DateTime(year, month, day));
                                    NameChecker nameChecker = new NameChecker();
                                    if (nameChecker.CheckName(name) > 0)
                                    {
                                        new DataDeleter(name).DeleteData();
                                    }
                                    DataWriter<Bill> dataWriter = new DataWriter<Bill>();
                                    dataWriter.WriteData(bill);
                                }
                                catch (ProductDoesntExist ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (WrongData ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            }
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Press any key to continue/ESC to exit");
                ch = Console.ReadKey();
                Console.Clear();
            } while (ch.Key != ConsoleKey.Escape);
        }
    }
}
