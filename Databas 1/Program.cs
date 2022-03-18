using Npgsql;
using System;
using System.Data;
using System.Windows.Input;

namespace Databas_1
{
    class Program
    {
        public enum State
        {
            Start,
            Logged_in,
            login,
            register,
            search,
            view

        }

        enum Accessibilty
        {
            Admin,
            User
        }
        public static State currentstate;
        


        static void Main(string[] args)
        {
            bool Admin=false;
            string connection = "Server=pgserver.mau.se;Port=5432;User Id = am3740; Password=cf0py2ta;Database=miint2;";
            Databas databas1 = new Databas();
            currentstate = State.Start;

            //I början bör man få en meny som frågar om man ska logga in etc.
            switch (currentstate)
            {
                case State.Start:
                    Start();


                    break;


            }
            void Login()
            {
                Console.WriteLine("Enter your details: (email, password) " +
                               "\n Note: Use commas(,) between the values.\n");

                string[] info = Console.ReadLine().Split(",");
                var con = new NpgsqlConnection(connection);
                con.Open();
                string sql = "Select function_username_password(@email, @password)";
                var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue(parameterName: "@email", info[0]);
                cmd.Parameters.AddWithValue(parameterName: "@password", info[1]);
                string[] input = Console.ReadLine().Split(",");

                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine("----------------------"); //kanske console.clear();


                    if (rdr.GetBoolean(ordinal: 0) == true)
                    {
                        Console.WriteLine("Welcome: " + databas1.Get_Customer_name(info[0]));
                    }
                    else
                    {
                        Start();
                    }


                }
            }
            void Start()
            {
                Console.WriteLine("Please Select one of the options below: " +
                    "\n 1- Login" +
                    "\n 2- Register" +
                    "\n 3- Search for product" +
                    "\n 4- See all products\n");
                string choice = Console.ReadLine();
                if (choice.Equals("1"))
                {
                    Login();
                }
                else if (choice.Equals("2"))
                {
                    Register();
                }
                else if (choice.Equals("3"))
                {
                    Search();
                }
                else if (choice.Equals("4"))
                {
                    View();
                }
            }
            void Register()
            {
                Console.WriteLine("\n Enter your details first (Email, FirstName, LastName, City, Adress)." +
                                "\n Note: Use commas(,) between the values.\n");

                string[] details = Console.ReadLine().Split(",");
                databas1.Add_customer(details[0], details[1], details[2], details[3], details[4]);

                Console.WriteLine("\n Now choose a password: ");

                string password = Console.ReadLine();
                databas1.Add_password_to_customer(details[0], password);

                Console.WriteLine("Account creation successful! \n Welcome: " + details[1] + " " + details[2]);
            }

            void Search()
            {
                Console.Clear();
                Console.WriteLine("Type the name of the product you want to search for: \n");
                databas1.View_searched_product(Console.ReadLine());
            }
            void View()
            {
                Console.Clear();
                databas1.View_all_products();
            }
        }
    }
    class Databas
    {
        string connection = "Server=pgserver.mau.se;Port=5432;User Id = am3740; Password=cf0py2ta;Database=miint2;";
        public void PrintAllTeachers()
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * FROM \"teacher\"";
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("ID: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Fullname; " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Department code: " + rdr.GetInt32(ordinal: 2));
                Console.WriteLine("Salary: " + rdr.GetInt32(ordinal: 3));
            }
            con.Close();
        }

        //------------------------------------////------------------------------------//
        public void Add_supplier_to_list(string supplier_name, int phonenr, string adress)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_supplier(@namn,@telnr,@adress)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@namn", supplier_name);
            command.Parameters.AddWithValue(parameterName: "@telnr", phonenr);
            command.Parameters.AddWithValue(parameterName: "@adress", adress);
            transaction.Commit();

        }
        public void Add_customer(string email, string f_namn, string l_namn, string city, string adress)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_customer(@email,@f_namn,@l_namn,@city,@adress)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@email", email);
            command.Parameters.AddWithValue(parameterName: "@f_namn", f_namn);
            command.Parameters.AddWithValue(parameterName: "@l_namn", l_namn);
            command.Parameters.AddWithValue(parameterName: "@city", city);
            command.Parameters.AddWithValue(parameterName: "@adress", adress);
            transaction.Commit();
            con.Close();
        }
        public void Add_discount(string namn, string supplier_namn, int quantity, int base_price)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_product(@namn,@supplier_namn,@quantity,@base_price)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@namn", namn);
            command.Parameters.AddWithValue(parameterName: "@supplier_namn", supplier_namn);
            command.Parameters.AddWithValue(parameterName: "@quantity", quantity);
            command.Parameters.AddWithValue(parameterName: "@base_price", base_price);
            transaction.Commit();
        }

        public void Add_product_to_shoppinglist(string email, string product_namn, string supplier_namn, int quantity)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_product_to_shoppinglist(@email, @product_namn,@supplier_namn,@quantity)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@email", email);
            command.Parameters.AddWithValue(parameterName: "@prioduct_namn", product_namn);
            command.Parameters.AddWithValue(parameterName: "@supplier_namn", supplier_namn);
            command.Parameters.AddWithValue(parameterName: "@quantity", quantity);
            transaction.Commit();
        }

        public void Add_shoppinglist_to_order(int shoplist_id)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_shoplist_to_order(@sl_id)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@sl_id", shoplist_id);
            transaction.Commit();
        }
        public void delete_product_not_sold(string product_namn)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call delete_product_not_sold(@product_namn)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@product_namn", product_namn);
            transaction.Commit();
        }

        public void Update_product_quantity(string product_namn, double variation)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call update_product_quantity(@product_namn,@variation)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@product_namn", product_namn);
            command.Parameters.AddWithValue(parameterName: "@variation", variation);
            transaction.Commit();
        }

        public void View_all_products()
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * FROM view_all_products";
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Product name: " + rdr.GetString(ordinal: 0));
                Console.WriteLine("Base price: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Quantity: " + rdr.GetInt32(ordinal: 2));
            }
            con.Close();
        }
        public void View_all_unconfirmed_products()
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * FROM all_unconfirmed_orders";
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Shoppinglist ID: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Customer email: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Product name: " + rdr.GetString(ordinal: 2));
                Console.WriteLine("Supplier name: " + rdr.GetString(ordinal: 3));
                Console.WriteLine("Quantity: " + rdr.GetInt32(ordinal: 4));
            }
            con.Close();
        }

        public void View_searched_product(string product_namn)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * FROM product where product.namn = " + product_namn;
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Product name: " + rdr.GetString(ordinal: 0));
                Console.WriteLine("Supplier name: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Quantity: " + rdr.GetInt32(ordinal: 2));
                Console.WriteLine("Base price: " + rdr.GetInt32(ordinal: 3));
            }
            con.Close();
        }

        public void Select_discount(int? id, float? percentage, string? reason)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string SQL;
            if (id != null)
            {
                if (percentage != null)
                {

                    if (reason != null)
                    {
                        SQL = "Select * from discount where discount.id = " + id +
                        " and percentage = " + percentage + " and reason like " + reason;
                    }
                    else
                        SQL = "Select * from discount where discount.id = " + id +
                        " and percentage = " + percentage;

                }
                else
                    SQL = "Select * from discount where discount.id = " + id;
            }
            else SQL = "Select * from discount";

            using var cmd = new NpgsqlCommand(SQL, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Discount ID: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Percentage: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Reason: " + rdr.GetString(ordinal: 2));
            }
            con.Close();
        }
        public void Add_password_to_customer(string email, string password)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_password_to_customer(@email, @password)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@email", email);
            command.Parameters.AddWithValue(parameterName: "@password", password);
            transaction.Commit();
        }
        public string Get_Customer_name(string email)
        {
            string fullname = null;
            using var con = new NpgsqlConnection(connection);
            con.Open();
            
            string SQL = "Select customer.f_namn, customer.l_namn from customer where customer.email = " + email;
            using var cmd = new NpgsqlCommand(SQL, con);
            
            /*using NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    fullname = rdr.GetString(ordinal: 0) + " " + rdr.GetString(ordinal: 1);
                }
            }*/

            

            return cmd.ExecuteScalar().ToString();
        }
        public void Is_customer_admin(string email)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            //string sql = //Select 
            //måste göra en table och en function som returnerar true/false.

        }


    }
}
