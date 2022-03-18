using Npgsql;
using System;
using System.Data;

namespace Databas_1
{
    class Program
    {
        enum State
        {
            Start,
            Logged_in

        }


        static void Main(string[] args)
        {
            string connection = "Host=localhost;Username=miint2;Password=wbe4ngqu;Database=MAU";
            State currentstate;
            Console.WriteLine("Hello World!");
            Databas databas1 = new Databas();
            string selectedDepCode = null;
            currentstate = State.Logged_in;

            //I början bör man få en meny som frågar om man ska logga in etc.
            switch (currentstate)
            {
                case State.Logged_in:
                    Console.WriteLine("Please Select one of the options below: " +
                        "\n 1- Login" +
                        "\n 2- Register" +
                        "\n 3- Search for product" +
                        "\n 4- See all products");
                    string choice = Console.ReadLine();
                    if (choice.Equals("1"))
                    {
                        Console.WriteLine("Enter your details: (email, password) " +
                            "\n Note: Use commas(,) between the values");

                        string[] info = Console.ReadLine().Split(",");
                        using var con = new NpgsqlConnection(connection);
                        con.Open();
                        string sql = "call function_username_password(@email, @password)";
                        using var cmd = new NpgsqlCommand(sql, con);
                        cmd.Parameters.AddWithValue(parameterName: "@email", info[0]);
                        cmd.Parameters.AddWithValue(parameterName: "@password", info[1]);


                        using NpgsqlDataReader rdr = cmd.ExecuteReader();
                       // rdr.Read();
                        //if ( )

                    }
                    else if (choice.Equals("2"))
                    {
                        Console.WriteLine("\n Enter your details first (Email, FirstName, LastName, City, Adress)." +
                            "\n Note: Use commas(,) between the values.");

                        string[] details = Console.ReadLine().Split(",");
                        databas1.Add_customer(details[0], details[1], details[2], details[3], details[4]);

                        Console.WriteLine("\n Now choose a password: ");

                        string password = Console.ReadLine();
                        databas1.Add_password_to_customer(details[0], password);


                    }



                    break;
                case State.Start:


                    break;




            }


            while (true)
            {
                Console.Write("Please Select one of the options below:" +
                    "\n 1- List all Teachers" +
                    "\n 2- Select department" +
                    "\n 3- List all Teachers in department with selected code/id" +
                    "\n 4- Select Teacher" +
                    "\n 5- Raise Selected Teachers Salary"
                    );
                string choice = Console.ReadLine();
                if (choice.Equals("1"))
                {
                    databas1.PrintAllTeachers();
                }
                else if (choice.Equals("2"))
                {
                    if (selectedDepCode == null)
                    {
                        throw new ArgumentNullException(paramName: nameof(selectedDepCode), message: "Parameter can't be null");
                    }
                    try
                    {
                        databas1.PrintTeachersByDepartment(int.Parse(selectedDepCode));
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.WriteLine("{0} First exception caught.", e);
                    }
                    selectedDepCode = Console.ReadLine();
                    Console.WriteLine("The selected department is: " + selectedDepCode);
                }
                else if (choice.Equals("3"))
                {
                    databas1.PrintTeachersByDepartment(Convert.ToInt32(selectedDepCode));

                }
                else if (choice.Equals("4"))
                {

                }
                else if (choice.Equals("5"))
                {

                }

            }

        }
    }
    class Databas
    {
        string connection = "Host=localhost;Username=miint2;Password=wbe4ngqu;Database=MAU";
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
        public void PrintTeachersByDepartment(int deptCode)
        {
            using var con = new NpgsqlConnection(connection);
            con.Open();

            string sql = "Select * From \"teacher\" where department.dept_code=" + deptCode;
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("ID: " + rdr.GetInt32(ordinal: 0));
                Console.WriteLine("Name: " + rdr.GetString(ordinal: 1));
                Console.WriteLine("Department code: " + rdr.GetInt32(ordinal: 2));
                Console.WriteLine("Salary: " + rdr.GetInt32(ordinal: 3));
            }
            con.Close();


        }
        public void RaiseTeachersSalaries(int deptId, int perc)
        {
            using var con = new NpgsqlConnection(connection);

            con.Open();
            var transaction = con.BeginTransaction();
            string sentencialSQL = "call raiseteachers(@deptId,@percentage)";
            NpgsqlCommand command = new NpgsqlCommand(sentencialSQL, con);
            command.Parameters.AddWithValue(parameterName: "@deptId", deptId);
            command.Parameters.AddWithValue(parameterName: "@percentage", perc);
            int affected = command.ExecuteNonQuery();
            transaction.Commit();
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
            command.Parameters.AddWithValue(parameterName: "@email",email);
            command.Parameters.AddWithValue(parameterName: "@password",password);
            transaction.Commit();
        }


    }
}
