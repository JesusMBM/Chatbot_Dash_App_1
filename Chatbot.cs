using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;

namespace Chatbot_Application
{
    public partial class Chatbot : Form
    {
        string empID;
        private static readonly HttpClient client = new HttpClient();

        public Chatbot(string EmployeeID, string firstName)
        {
            InitializeComponent();
            botReply.Text = "Hi " + firstName + "! I'm the Chatbot, how can I help you?";
            empID = EmployeeID;
        }

        public class UserInfo
        {
            public string EmpID { get; set; }
        }

        public Form FormToShowOnClosing { get; set; }

        // Button click event to process user input and OpenAI integration
        private async void button1_Click(object sender, EventArgs e)
        {
            string userInput = userTextEntry.Text.ToLower();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                botReply.Text = "Please enter something.";
                return;
            }

            // Check for keywords related to time off or covering shifts
            if (userInput.Contains("time") || userInput.Contains("absence") || userInput.Contains("leave"))
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Would you like to take time off?", "Confirmation", buttons);
                if (result == DialogResult.No)
                {
                    botReply.Text = "Ok, how else may I help you?";
                    userTextEntry.Text = String.Empty;
                }
                else
                {
                    userTextEntry.Text = String.Empty;
                    var timeOffForm = new TimeOff(empID);
                    timeOffForm.FormToShowOnClosing = this;
                    timeOffForm.Show();
                }
            }
            else if (userInput.Contains("cover") || userInput.Contains("work for") || userInput.Contains("take over"))
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show("Would you like to cover someone else's shift(s)?", "Confirmation", buttons);
                if (result == DialogResult.No)
                {
                    botReply.Text = "Ok, how else may I help you?";
                    userTextEntry.Text = String.Empty;
                }
                else
                {
                    userTextEntry.Text = String.Empty;
                    var CoverTimeForm = new CoverTime(empID);
                    CoverTimeForm.FormToShowOnClosing = this;
                    CoverTimeForm.Show();
                }
            }
            else
            {
                // Call OpenAI API for other queries
                string openAIResponse = await GetOpenAIResponse(userInput);
                botReply.Text = !string.IsNullOrWhiteSpace(openAIResponse) ? openAIResponse : "I'm sorry, I don't quite understand. Try typing something else like 'time', 'cover', 'absence', or 'work for'";
            }

            userTextEntry.Text = string.Empty; // Clear user input
        }

        // OpenAI API integration
        private async Task<string> GetOpenAIResponse(string prompt)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");  // Ensure your OpenAI API key is stored in environment variables
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new StringContent($"{{\"prompt\": \"{prompt}\", \"max_tokens\": 100}}", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else
            {
                return "Error: Unable to connect to OpenAI.";
            }
        }

        // Handle form closing
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Save employee data
        private void employeeBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.employeeBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this._566_DBaseDataSet2);
        }

        // Load the employee data on form load
        private void Chatbot_Load(object sender, EventArgs e)
        {
            // Load data into the Employee table
            this.employeeTableAdapter.Fill(this._566_DBaseDataSet2.Employee);
        }

        // Open Tableau visualization or custom dashboard
        private void button2_Click_1(object sender, EventArgs e)
        {
            Chatbot_Application.Tableau form = new Chatbot_Application.Tableau();
            form.Show();
        }

        // Placeholder function for future pictureBox interaction
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Add relevant code if needed
        }
    }
}
