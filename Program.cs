using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace MyApp
{
    enum Menu
    {
        AddProduct = 1,
        CreatePayment
    }
    enum PaymentEnum
    {
        Ignore,
        Pay
    }

    class Program
    {
        static void Main(string[] args)
        {
            Menu pointOfMenu;
            ConsoleKeyInfo ch;
            do
            {
                Console.WriteLine("1 - add product");
                Console.WriteLine("2 - create payment");
                try
                {
                    string choise = Console.ReadLine();
                    if (Enum.IsDefined(typeof(Menu), int.Parse(choise)))
                    {
                        pointOfMenu = (Menu)Enum.Parse(typeof(Menu), choise);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    switch (pointOfMenu)
                    {
                        case Menu.AddProduct:
                            {
                                try
                                {
                                    Console.WriteLine("Enter product's name:");
                                    string name = Console.ReadLine();
                                    NameChecker nameChecker = new NameChecker();
                                    if (nameChecker.CheckName(name) > 0)
                                    {
                                        throw new ProductAlreadyExists(name);
                                    }
                                    Console.WriteLine("Enter payment for one day of using:");
                                    string payPerDay = Console.ReadLine();
                                    Console.WriteLine("Enter daily penalty:");
                                    string dailyPenalty = Console.ReadLine();
                                    Console.WriteLine("Enter monthly pay date:");
                                    string payDate = Console.ReadLine();
                                    Bill bill = new Bill(name, Convert.ToDecimal(payPerDay), Convert.ToDecimal(dailyPenalty), Convert.ToInt32(payDate));
                                    DateSetter dataSetter = new DateSetter();
                                    dataSetter.SetDates(bill);
                                    DataWriter<Bill> dataWriter = new DataWriter<Bill>();
                                    dataWriter.WriteData(bill);
                                }
                                catch (ProductAlreadyExists ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                catch (NotValidValue ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            }
                        case Menu.CreatePayment:
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
                                    paymentChecker.NewCheckPayments(bill, new DateTime(year, month, day));
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
