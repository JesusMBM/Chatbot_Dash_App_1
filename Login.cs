using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Chatbot_Application
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        public class User
        {
            public string EmployeeID { get; set; }
            public string First_Name { get; set; }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Create a variable for authentication
            Boolean UserValid;

            // Added New Connection String for SQLServer Express (Note: Provider clause removed)
            string connString = "Data Source=DESKTOP-7TL71ND\\SQLEXPRESS;Initial Catalog=566_DBase;Integrated Security=True";
          
            // Added Try Catch to Check for Issues
            try
            {
                // Use SQL DB to connect with SQLServer Express DB
                //sql connection object (MODIFIED FOR SQLSERVER)
                using (SqlConnection myConnection = new SqlConnection(connString))
                {

                    //Build a select statement to check for user/password combo
                    string query = "SELECT EmployeeID, First_Name from Employee WHERE username = '" + textBox1.Text + "' AND password = '" + textBox2.Text + "'";

                    //define the SqlCommand object
                    SqlCommand cmd = new SqlCommand(query, myConnection);
                    //open connection
                    myConnection.Open();

                    //execute the SQLCommand
                    SqlDataReader readerReturnValue = cmd.ExecuteReader();
                    //string v = readerReturnValue.GetString();
                   

                    // If the User/Password combo was found, then set the return value to 'true'
                    if (readerReturnValue.HasRows == true)
                    {
                        UserValid = true;
                        DataTable table = new DataTable();
                        table.Load(readerReturnValue);

                        var outputQuery = new List<User>();
                        User user;

                        foreach (DataRow dr in table.Rows)
                        {
                            user = new User();
                            user.EmployeeID = dr["EmployeeID"].ToString();
                            user.First_Name = dr["First_Name"].ToString();
                            outputQuery.Add(user);
                        }
                        MessageBox.Show("Valid credentials, welcome to the Chatbot! ");
                        var Mainform = new Chatbot(outputQuery[0].EmployeeID, outputQuery[0].First_Name);
                        Mainform.FormToShowOnClosing = this;
                        Mainform.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Else indicate user not found
                        UserValid = false;
                        MessageBox.Show("Please try again. You UserID or Password is invalid.");
                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    // close connection to prevent zombie DB connection
                    myConnection.Close();
                }
            }
            // Display Error Message for Exceptions
            catch (Exception ex)
            {
                //display error message
                MessageBox.Show("Exception: " + ex.Message);
            }
    }
    }
}
