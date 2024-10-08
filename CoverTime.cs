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
using System.Globalization;

namespace Chatbot_Application
{
    public partial class CoverTime : Form
    {
        string empID;
        SqlConnection conn = null;   // One Connection to Database
        SqlDataReader readerTaskList = null;  // One Reader for Task List

        public CoverTime(string text)
        {
            InitializeComponent();
            empID = text;
            conn = new SqlConnection("Data Source=DESKTOP-7TL71ND\\SQLEXPRESS;Initial Catalog=566_DBase;Integrated Security=True");
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT DISTINCT ShiftID, Shift_Date, Shift_Start, Shift_End FROM Shift WHERE EmployeeID != " + empID + " AND Worked = " + 0, conn);

            // 11. define parameters used in command object for History
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@EmpID_Par";


            // 12. Get value from ComboBox to populate SQL parameter History    //     dataset selection
            param.Value = empID;

            // 13. add new parameters to command objects
            cmd.Parameters.Add(param);

            // 14. Execute the command for history query
            readerTaskList = cmd.ExecuteReader();

            // 15. Get the new data set and load it as a table for History
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Table1");
            ds.Tables.Add(dt);
            ds.Load(readerTaskList, LoadOption.PreserveChanges, ds.Tables[0]);
            readerTaskList.Close();

            // 16. Stick the new table into the DataGridView for History
            dataGridView1.DataSource = ds.Tables[0];
        }
        public Form FormToShowOnClosing { get; set; }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            textBox1.Text = monthCalendar1.SelectionRange.Start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
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
                    string query = "SELECT Shift_Date from Shift WHERE Shift_Date = '" + textBox1.Text + "' AND EmployeeID != " + empID + " and Worked = " + 0;

                    //define the SqlCommand object
                    SqlCommand cmd = new SqlCommand(query, myConnection);

                    //open connection
                    myConnection.Open();

                    //execute the SQLCommand
                    SqlDataReader readerReturnValue = cmd.ExecuteReader();

                    if (readerReturnValue.HasRows == true)
                    {
                        UserValid = true;
                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                        DialogResult result = MessageBox.Show("Would you like cover the shift on " + textBox1.Text + " ?", "Confirmation", buttons);
                        if (result == DialogResult.No)
                        {
                            this.Close();
                        }
                        else
                        {
                            if (textBox2.Text != null)
                            {
                                myConnection.Close();
                                conn = new SqlConnection(@connString);
                                conn.Open();
                                textBox1.Text = monthCalendar1.SelectionRange.Start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                SqlCommand cmd2 = new SqlCommand("SELECT ShiftID from Shift WHERE Shift_Date = '" + textBox1.Text + "' AND EmployeeID != " + empID + " and Worked = " + 0);
                                cmd2.Connection = conn;
                                SqlDataReader readerReturnValue2 = cmd2.ExecuteReader();
                                DataTable table = new DataTable();
                                table.Load(readerReturnValue2);

                                string x = table.Rows[0][0].ToString();
                                conn.Close();

                                conn = new SqlConnection(@connString);
                                conn.Open();
                                // 3. declare a SQL Command. 
                                SqlCommand cmd3 = new SqlCommand("insert into Request values('" + monthCalendar1.TodayDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "', '" + 1 + "', '" + textBox2.Text + "', '" + x + "')", conn);
                                cmd3.ExecuteNonQuery();
                                SqlCommand cmd4 = new SqlCommand("update Shift set Worked = " + 1 + ", EmployeeID = " + empID + "where ShiftID =" + x, conn);
                                cmd4.ExecuteNonQuery();
                            }
                            else
                            {
                                MessageBox.Show("You need to provide more details for who you're covering for and why");
                            }
                            MessageBox.Show("Confirmed");
                        }
                    }
                    else
                    {
                        // Else indicate user not found
                        UserValid = false;
                        MessageBox.Show("I'm sorry, I can't make that request for you.");
                        textBox1.Text = "";
                    }
                    // close connection to prevent zombie DB connection
                    myConnection.Close();
                    conn.Close();
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
