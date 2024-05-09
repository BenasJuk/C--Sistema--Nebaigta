using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using Mysqlx.Crud;
using static Maisto_uzsakymo_sistema.Form2;
using System.Reflection;

namespace Maisto_uzsakymo_sistema
{
    public partial class Form5 : Form
    {
        private string connectionString = "server=127.0.0.1; user=root; database=maisto_sistema; password=";
        private MySqlConnection connection; 

        public Form5()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
        }

        private bool IsOrderComplete(int orderID)
        {

            bool isComplete = false;

            string query = "SELECT Status FROM Uzsakymai WHERE ID = @UzsakymasID";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UzsakymasID", orderID);

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string status = reader.GetString("Status");
                    isComplete = status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isComplete;
        }

        private DateTime GetTimeOfPaymentForOrderID(int orderID)
        {

            DateTime timeOfPayment = DateTime.MinValue; 

            string query = "SELECT MokejimoLaikas FROM Uzsakymai WHERE ID = @UzsakymasID"; 
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UzsakymasID", orderID);

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    timeOfPayment = reader.GetDateTime("MokejimoLaikas"); 
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close(); 
            }

            return timeOfPayment;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            InitializeDatabaseConnection();

            DataTable bendrasuzsakymas = new DataTable();
            bendrasuzsakymas.Columns.Add("Uzsakymo Nr", typeof(int));
            bendrasuzsakymas.Columns.Add("Uzsakymas", typeof(string)); 
            bendrasuzsakymas.Columns.Add("Mokejimo Laikas", typeof(string));
            bendrasuzsakymas.Columns.Add("Atstumas", typeof(double));
            bendrasuzsakymas.Columns.Add("Atlikimas", typeof(bool));

            string query = "SELECT ID, UzsakymasID, MokejimoLaikas, Atstumas, Atlikimas FROM bendrasuzsakymas"; 

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<OrderDisplayData> orderDataList = new List<OrderDisplayData>();
                while (reader.Read())
                {
                    int orderID = reader.GetInt32("UzsakymasID");
                    DateTime mokejimoLaikas = reader.GetDateTime("MokejimoLaikas");
                    double atstumas = reader.GetDouble("Atstumas");
                    bool atlikimas = reader.GetBoolean("Atlikimas");

                    string formattedTime = mokejimoLaikas.ToString("HH:mm");

                    List<OrderDisplayData> uzsakymasDetailsList = GetOrderDetailsFromForm2(orderID);

                    DateTime timeOfPayment = GetTimeOfPaymentForOrderID(orderID);
                    bool isComplete = IsOrderComplete(orderID);

                    UpdateDataGridView(uzsakymasDetailsList, orderID, formattedTime, timeOfPayment, atstumas, isComplete); 
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close(); 
            }

            dataGridView1.DataSource = bendrasuzsakymas;
        }

        private void InitializeDatabaseConnection()
        {
            //string mysqlCon = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

            //using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon)) // Use MySqlConnection
            //{
            //    try
            //    {
            //        mySqlConnection.Open();
            //        MessageBox.Show("Prisijungimas prie duomenu bazes pavyko");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }

        public delegate void OrderPaidEventHandler(int orderID);
        public event OrderPaidEventHandler OrderPaid;

        public void UpdateBendrasUzsakymas(int orderID)
        {
            string query = "UPDATE bendrasuzsakymas SET MokejimoLaikas = @currentTime WHERE ID = @UzsakymasID";

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@currentTime", DateTime.Now);
                command.Parameters.AddWithValue("@UzsakymasID", orderID);
                command.ExecuteNonQuery();

                OrderPaid?.Invoke(orderID); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        public class OrderDisplayData
        {
            public int OrderNumber { get; set; }
            public string UzsakymoNr { get; set; }
            public string MaistoPavadinimas { get; set; }
            public string PriedoPavadinimas { get; set; } 
            public int Kiekis { get; set; }
            public DateTime MokejimoLaikas { get; set; }
            public double Atstumas { get; set; }
            public bool Complete { get; set; }
        }

        public class UzsakymasDetails 
        {
            public int UzsakymasID { get; set; } 
            public int MaistoID { get; set; } 
            public string MaistoPavadinimas { get; set; }
            public int Kiekis { get; set; } 
            public DateTime? MokejimoLaikas { get; set; } 
            public double? Atstumas { get; set; } 
            public bool Complete { get; set; }
        }

        private List<OrderDisplayData> GetOrderDetailsFromForm2(int orderID)
        {
            Form2 form2 = GetForm2Instance();
            if (form2 != null)
            {
                List<DetailedOrderInfo> detailedOrders = form2.GetOrderDetails(orderID);
                List<OrderDisplayData> orderDataList = new List<OrderDisplayData>();

                foreach (DetailedOrderInfo detailedOrder in detailedOrders)
                {
                    OrderDisplayData displayData = new OrderDisplayData();
                    displayData.MaistoPavadinimas = detailedOrder.MaistoPavadinimas; 
                    displayData.Kiekis = detailedOrder.Kiekis; 

                    if (detailedOrder.MokejimoLaikas != null)
                    {
                        displayData.MokejimoLaikas = detailedOrder.MokejimoLaikas;
                    }

                    displayData.Atstumas = detailedOrder.Atstumas; 

                    displayData.Complete = detailedOrder.Complete;

                    orderDataList.Add(displayData);
                }

                return orderDataList;
            }
            else
            {
                MessageBox.Show("Form2 instance not found.");
                return null;
            }
        }

        private Form2 GetForm2Instance()
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Form2)
                {
                    return (Form2)openForm;
                }
            }
            return null;
        }

        public bool UpdatePaymentInformation(OrderDisplayData orderData, DateTime timeOfPayment)
        {
            string connectionString = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string updateQuery = "UPDATE bendrasuzsakymas SET MokejimoLaikas = @paymentTime WHERE UzsakymasID = @orderId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                    updateCommand.Parameters.AddWithValue("@orderId", orderData.UzsakymoNr);
                    updateCommand.Parameters.AddWithValue("@paymentTime", timeOfPayment);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {

                    Console.WriteLine("Error updating payment information: " + ex.Message);
                    return false;
                }
            }
        }



        public bool MarkOrderComplete(OrderDisplayData orderData)
        {
            string connectionString = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string updateQuery = "UPDATE bendrasuzsakymas SET Atlikimas = 'Taip' WHERE UzsakymasID = @orderId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                    updateCommand.Parameters.AddWithValue("@orderId", orderData.UzsakymoNr);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {

                    Console.WriteLine("Error marking order complete: " + ex.Message);
                    return false;
                }
            }
        }



        public void UpdateDataGridView(List<OrderDisplayData> orderDataList, int orderID, string formattedTime, DateTime timeOfPayment, double atstumas, bool isComplete)
        {
            
            if (orderDataList.Count == 0)
            {

                MessageBox.Show("Order data list is empty. Please refresh data.");
                return;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int currentOrderID;

                try
                {
                    currentOrderID = Convert.ToInt32(row.Cells["Uzsakymo Nr"].Value); 
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Error converting Uzsakymo Nr to integer: " + ex.Message);
                    continue; 
                }

                OrderDisplayData matchingOrderData = orderDataList.FirstOrDefault(data =>
                    data.UzsakymoNr != null && data.UzsakymoNr.ToString() == currentOrderID.ToString());

                if (matchingOrderData == null)
                {
                    Console.WriteLine("No matching order found for UzsakymoNr: " + currentOrderID);
                    continue; 
                }

                row.Cells["Mokejimo Laikas"].Value = formattedTime;

                string orderDetailsString = GetOrderDetailsString(currentOrderID);
                row.Cells["Uzsakymas"].Value = orderDetailsString;

                bool paymentSuccessful = UpdatePaymentInformation(matchingOrderData, timeOfPayment); 
                if (!paymentSuccessful)
                {
                    MessageBox.Show("Failed to update payment information.");
                }

                bool orderCompleted = MarkOrderComplete(matchingOrderData); 
                if (!orderCompleted)
                {
                    MessageBox.Show("Failed to mark order complete. Please check the logs for details.");
                }
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Mokejimo Laikas"].Value = timeOfPayment; 
                row.Cells["Atstumas"].Value = atstumas; 
                row.Cells["Atlikimas"].Value = isComplete; 
            }
        }

        private List<OrderDisplayData> GetOrderDetailsList(int orderID)
        {

            List<OrderDisplayData> orderDetailsList = new List<OrderDisplayData>();

            // Assuming you have database connection established

            string query = "SELECT * FROM Uzsakymo_Detales WHERE UzsakymoID = @UzsakymasID"; 
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UzsakymasID", orderID);

            try
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderDisplayData orderData = new OrderDisplayData();
                    orderData.MaistoPavadinimas = reader.GetString("Pavadinimas");
                    orderData.Kiekis = reader.GetInt32("Kiekis");
                    orderData.PriedoPavadinimas = reader.GetString("PriedoPavadinimas"); 

                    orderDetailsList.Add(orderData);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close(); 
            }

            return orderDetailsList;
        }

        private string GetOrderDetailsString(int orderID)
        {
            List<OrderDisplayData> orderData = GetOrderDetailsList(orderID);  
            if (orderData != null && orderData.Count > 0)
            {
                StringBuilder orderDetailsBuilder = new StringBuilder();
                foreach (OrderDisplayData data in orderData)
                {
                    orderDetailsBuilder.AppendLine($"{data.MaistoPavadinimas} {data.PriedoPavadinimas} ({data.Kiekis})");
                }
                return orderDetailsBuilder.ToString().Trim();
            }
            else
            {
                return "Details not available";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();

            mainForm.Show();

            this.Hide();
        }
    }
}
