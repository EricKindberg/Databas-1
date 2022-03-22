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
            bool Admin = false;
            string connection = "Server=pgserver.mau.se;Port=5432;User Id = am3740; Password=cf0py2ta;Database=miint2;";
            Databas databas1 = new Databas();
            currentstate = State.Start;

            //I början bör man få en meny som frågar om man ska logga in etc.
            Start();



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
            void ISLOGGEDIN()
            {
                if (Admin)
                {
                    Console.WriteLine("*Signed in as Admin*\n" +
                        "Choose one of the options below: \n" +
                        "1- Add supplier \n" +
                        "2- Add product \n" +
                        "3- Delete product \n" +
                        "4- Edit quantity of product \n" +
                        "5- Add discount \n" +
                        "6- Add a discount to product \n" +
                        "7- See discount history \n" +
                        "8- See list a list of new orders \n" +
                        "9- See a list of products with maximum orders each months \n" +
                        "10- Confirm order \n");

                    string choice = Console.ReadLine();
                    if (choice.Equals("1")) //ADD SUPPLIER
                    {
                        Console.Clear();
                        Console.Write("Enter Supplier details: (Name, City, Country, Phone number, adress) \n" +
                            "Note: Use commas(,) between the values.\n");
                        string[] details = Console.ReadLine().Split(",");
                        databas1.Add_supplier_to_list(details[0], details[1], details[2], details[3], details[4]);
                        Console.Clear();
                        Console.WriteLine("Supplier " + details[0] + " is added to the list of suppliers\n");
                        Start();
                    }
                    else if (choice.Equals("2")) //ADD PRODUCT
                    {
                        Console.Clear();
                        databas1.Select_All_Suppliers();
                        Console.WriteLine("----------------------");
                        Console.WriteLine("Enter details of product: (name, supplier_name, quantity, base_price) \n" +
                            "Supplier must be of one of the existing suppliers above. \n" +
                            "Note: Use commas(,) between the values. \n");
                        string[] choice2 = Console.ReadLine().Split(",");
                        databas1.Add_product(choice2[0], choice2[1], int.Parse(choice2[2]), float.Parse(choice2[3]));
                        Console.Clear();
                        Console.WriteLine("Product "+ choice2[0]+ " has been added." );
                        ISLOGGEDIN();

                    }
                    else if (choice.Equals("3")) //DELETE PRODUCT
                    {
                        Console.WriteLine("To delete a product type its name: ");
                        string product = Console.ReadLine();
                        databas1.Delete_product(product);
                        Console.Clear();
                        Console.WriteLine("Product: "+ product + " has been deleted.");
                        ISLOGGEDIN();
                    }
                    else if (choice.Equals("4"))
                    {
                        Console.Clear();
                        databas1.View_all_products();
                        Console.WriteLine("To edit the quantity of a product write its name and a positive value to increase and negative to decrease quantity\n" +
                            "Note: Use commas(,) between the values. \n");
                        string[] choices = Console.ReadLine().Split(",");
                        databas1.Update_product_quantity(choices[0], double.Parse(choices[1]));
                        ISLOGGEDIN();
                    }
                    else if (choice.Equals("5"))
                    {
                        Console.Clear();
                        Console.WriteLine("In order to add a discount you need to type in a\n" +
                            "discount code (with numbers only), the percentage (eg. 25.0 (%)), and reason (eg. \"Christmas Sale\")\n" +
                            "Note: Use commas(,) between the values. \n ");
                        string[] choices = Console.ReadLine().Split(",");
                        databas1.Add_discount(int.Parse(choices[0]),float.Parse(choices[1]),choices[2]);
                        ISLOGGEDIN();
                    }
                    else if (choice.Equals("6"))
                    {
                        Console.Clear();
                        databas1.View_all_products();
                        databas1.Select_discount(null,null,null);
                        Console.WriteLine("Add a discount to a product (product_name, discount_code)\n" +
                            "Note: Use commas(,) between the values. \n");
                        string[] choices = Console.ReadLine().Split(",");
                        databas1.Add_discount_to_product(choices[0], int.Parse(choices[1]));
                    }




                }
                else
                {
                    Console.WriteLine("*Signed in*\n" +
                        "Choose one of the options below: \n" +
                        "1- See a list of all products \n" +
                        "2- Add a product to shoppinglist \n" +
                        "3- See your listed items in you shoppinglist \n" +
                        "4- Confirm shoppinglist and add it to orders \n" +
                        "5- Delete order not yet confirmed by admin \n" +
                        "");
                    string choice = Console.ReadLine();

                    if (choice.Equals("1"))
                    {

                    }
                    else if (choice.Equals("2"))
                    {

                    }
                    else if (choice.Equals("3"))
                    {

                    }
                    else if (choice.Equals("4"))
                    {

                    }

                }
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
                    if (rdr.GetBoolean(ordinal: 0))
                    {
                        if (databas1.Is_customer_admin(info[0]))
                        {
                            Admin = true;
                            Console.WriteLine("Welcome Admin: ");//+ databas1.Get_Customer_name(info[0])[0] + databas1.Get_Customer_name(info[0])[1]);
                            ISLOGGEDIN();
                        }
                        else
                        {
                            Console.WriteLine("Welcome: "); //databas1.Get_Customer_name(info[0])[0] + databas1.Get_Customer_name(info[0])[1]);
                            ISLOGGEDIN();
                        }
                    }
                    else
                    {
                        Start();
                    }
                }
            }
            void Register()
            {
                Console.WriteLine("\n Enter your details first (Email, FirstName, LastName, City, Country, Phone number, Adress)." +
                                "\n Note: Use commas(,) between the values.\n");

                string[] details = Console.ReadLine().Split(",");
                databas1.Add_customer(details[0], details[1], details[2], details[3], details[4], details[5], details[6]);

                Console.WriteLine("\n Now choose a password: ");

                string password = Console.ReadLine();
                databas1.Add_password_to_customer(details[0], password);

                Console.WriteLine("Account creation successful! \n Welcome: " + details[1] + " " + details[2]);
                ISLOGGEDIN();
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
        public void Add_supplier_to_list(string name, string city, string country, string phonenr, string adress)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_supplier(@namn,@city,@country,@telnr,@adress)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@namn", name);
            command.Parameters.AddWithValue(parameterName: "@city", city);
            command.Parameters.AddWithValue(parameterName: "@country", country);
            command.Parameters.AddWithValue(parameterName: "@telnr", phonenr);
            command.Parameters.AddWithValue(parameterName: "@adress", adress);
            transaction.Commit();
            con.Close();
        }
        public void Add_customer(string email, string f_namn, string l_namn, string city, string country, string phone_number, string adress)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_customer(@email,@f_namn,@l_namn,@city,@country,@phone_number,@adress)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@email", email);
            command.Parameters.AddWithValue(parameterName: "@f_namn", f_namn);
            command.Parameters.AddWithValue(parameterName: "@l_namn", l_namn);
            command.Parameters.AddWithValue(parameterName: "@city", city);
            command.Parameters.AddWithValue(parameterName: "@country", country);
            command.Parameters.AddWithValue(parameterName: "@phone_number", phone_number);
            command.Parameters.AddWithValue(parameterName: "@adress", adress);
            transaction.Commit();
            con.Close();
        }
        public void Add_discount(int code, float percentage, string reason)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_discount(@code,@percentage,@reason)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@reason", reason);
            command.Parameters.AddWithValue(parameterName: "@code", code);
            command.Parameters.AddWithValue(parameterName: "@percentage", percentage);
            transaction.Commit();
            con.Close();
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
                Console.WriteLine("Supplier name: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Base price: " + rdr.GetInt32(ordinal: 2));
                Console.WriteLine("Quantity: " + rdr.GetInt32(ordinal: 3));
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
                Console.WriteLine("Discount Code/ID: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Percentage: " + rdr.GetFloat(ordinal: 1));
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
        public string[] Get_Customer_name(string email)
        {
            string[] names = new string[2];
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string SQL = "Select customer.f_namn, customer.l_namn from customer where customer.email = " + email;
            using var cmd = new NpgsqlCommand(SQL, con);
            //using NpgsqlDataReader rdr = cmd.ExecuteReader();
            //names[0] = rdr.GetString(ordinal: 0);
            //names[1] = rdr.GetString(ordinal: 1);


            con.Close();

            return names;
        }
        public bool Is_customer_admin(string email)
        {
            bool value = false;
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select is_customer_admin(@email)";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue(parameterName: "@email", email);
            using NpgsqlDataReader rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                value = rdr.GetBoolean(ordinal: 0);
            }
                return value;
        }

        public void Select_All_Suppliers()
        {

            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * from \"Supplier\"";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Supplier: " + rdr.GetString(ordinal: 0));
                Console.WriteLine("City: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Country: " + rdr.GetString(ordinal: 2));
                Console.WriteLine("Phone Number: " + rdr.GetString(ordinal: 3));
                Console.WriteLine("Adress: " + rdr.GetString(ordinal: 4));
            }
            con.Close();

        }

        public void Add_product(string name, string supplier_name, int quantity, float base_price)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_product(@name,@supplier_name,@quantity,@base_price)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@name", name);
            command.Parameters.AddWithValue(parameterName: "@supplier_name", supplier_name);
            command.Parameters.AddWithValue(parameterName: "@quantity", quantity);
            command.Parameters.AddWithValue(parameterName: "@base_price", base_price);
            transaction.Commit();
            con.Close();
        }

        public void Delete_product(string product_name)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call delete_product(@name)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@name", product_name);
            transaction.Commit();
            con.Close();
        }

        public void Add_discount_to_product(string product_name, int discount_code)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call add_discount_to_product(@productname,@code)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@productname", product_name);
            command.Parameters.AddWithValue(parameterName: "@code", discount_code);
            transaction.Commit();
            con.Close();
        }

        /*public void View_all_discounts()
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();
            string sql = "Select * from \"Discount\"";
            NpgsqlCommand command = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Code: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Percentage: " + rdr.GetFloat(ordinal: 1));
                Console.WriteLine("Reason: " + rdr.GetString(ordinal: 2));
                
            }
            con.Close();
        }*/



    }
}
