using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using static System.Console;

namespace CoffeeMaker
{
    static class ConsoleInterface //Interface for Customer for communication with him ! 
    {
        public static void Start() //method for communication with customer
        {
            ConsoleKeyInfo key;
            CoffeeMachine cm = GetCoffeeMachine();

            if (cm.Water < 0.1 || cm.Sugar < 0.1 || cm.Coffee < 0.1 || cm == null) //if ingredients in machine are over
            {
                WriteLine("There are no ingredients to prepare any coffee ");
                WriteLine("We appologise for tecnical inconviniances ");
                WriteLine("Press any key to continue");
                ReadKey();
                return;
            }
            WriteLine("\t\t**** Welcome To My CoffeeMachine ****");
            WriteLine("Only coins 50 , 100 , 200 and 500 are available !");
            WriteLine("Press any key to start !");
            ReadKey();
            Clear();
            do
            {
                WriteLine($"Press '1' to insert a coin or '0' to make order .. (Money in Machine - {cm.MoneyInMachine})");
                WriteLine("Press 'E' to exit !");
                key = ReadKey();

                switch (key.Key)
                {
                    case (ConsoleKey.E):
                        if (cm.MoneyInMachine == 0) return;
                        else WriteLine($"\nTake You money ` {cm.MoneyInMachine}"); //return customers money if it exists
                        return;

                    case (ConsoleKey.NumPad1):
                    case (ConsoleKey.D1):
                        WriteLine("\nEnter the value of coin (50,100,200,500)");

                        int coinvalue;
                        bool ParseSucceeded = Int32.TryParse(ReadLine(), out coinvalue);

                        while ((!(coinvalue == 50 || coinvalue == 100 || coinvalue == 200 || coinvalue == 500)) || !ParseSucceeded) //check correctness of imput
                        {
                            WriteLine("Only coins 50 , 100 , 200 and 500 are available !");
                            WriteLine("Enter proper value !");
                            ParseSucceeded = Int32.TryParse(ReadLine(), out coinvalue);
                        }

                        User.InsertCoin(coinvalue, cm);
                        break;

                    case (ConsoleKey.NumPad0):
                    case (ConsoleKey.D0):
                        if (cm.MoneyInMachine == 0)
                        {
                            WriteLine("\nThere is no money in Machine !");
                            System.Threading.Thread.Sleep(1100);
                            break;
                        }
                        List<Coffee> availablecoffees = GetAvailableCoffees(cm);
                        
                        if (availablecoffees.Count == 0)
                        {
                            WriteLine("\nNo coffees available !");
                            break;
                        }
                        Clear();
                        WriteLine($"You can buy the following coffees... (You have {cm.MoneyInMachine}$) \n");
                        foreach (var coffee in availablecoffees)
                        {
                            WriteLine($"CoffeeNumber - {coffee.ID} , Price - {coffee.Price}$");
                        }

                        WriteLine("\nEnter the number of the coffee you want");
                        int number;
                        bool parsed = int.TryParse(ReadLine(), out number);
                        while (!parsed)
                        {
                            WriteLine("Type only the Number !");
                            parsed = int.TryParse(ReadLine(), out number);
                        }
                        foreach (var cof in availablecoffees)
                        {
                            if (number == cof.ID)
                            {
                                User.Buy(cof, cm);
                                WriteLine($"Thank you ! your change is {cm.MoneyInMachine}$ ");
                                WriteLine("Press Any key to continue ");
                                ReadKey();
                                return;
                            }
                        }
                        WriteLine("Wrong Number !!!"); //inform the customer that he inserted wrong coffee number
                        break;

                    default:
                        WriteLine("\nPress 1 or 2 or 'e' !!!\n");
                        System.Threading.Thread.Sleep(2500);
                        break;
                }
            } while (true);
        }

        private static List<Coffee> GetAvailableCoffees(CoffeeMachine cm)
        {
            List<Coffee> coffees = new List<Coffee>();
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string command = "select * from Coffees";
                SqlCommand com = new SqlCommand(command, connection);

                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["ID"];
                        int price = (int)reader["Price"];

                        if (cm.MoneyInMachine >= price)
                            coffees.Add(new Coffee(id, price));
                    }
                }

                reader.Close();
            }

            foreach (var item in coffees.ToList()) //removing those coffees which cant be prepared because of ingrediend loss
            {
                if (item.WaterQuantity > cm.Water || item.SugarQuantity > cm.Sugar || item.CoffeeQuantity > cm.Coffee)
                {
                    coffees.Remove(item);
                }
            }
            return coffees;
        }

        private static CoffeeMachine GetCoffeeMachine()
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                string command = "select * from Storage";
                SqlCommand com = new SqlCommand(command, connection);
                CoffeeMachine cm = null;
                SqlDataReader reader = com.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        double water = (double)reader["Water"];
                        double sugar = (double)reader["Sugar"];
                        double coffee = (double)reader["Coffee"];
                        cm = new CoffeeMachine(water, sugar, coffee);
                    }
                }
                reader.Close();
                return cm;
            }
        }
    }
}