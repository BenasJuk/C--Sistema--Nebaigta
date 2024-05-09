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
using static Maisto_uzsakymo_sistema.Form5;
using Mysqlx.Crud;

namespace Maisto_uzsakymo_sistema
{
    public partial class Form2 : Form
    {
        private DataTable orderDataTable; 
        private MySqlConnection connection;
        private MySqlCommand command;
        
        private string connectionString = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

        public Form2()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            command = new MySqlCommand();

            orderDataTable = new DataTable();
            orderDataTable.Columns.Add("ID", typeof(int));
            orderDataTable.Columns.Add("Pavadinimas", typeof(string));
            orderDataTable.Columns.Add("Priedas", typeof(string));
            orderDataTable.Columns.Add("Kiekis", typeof(int));
            orderDataTable.Columns.Add("GalutineKaina", typeof(decimal));
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            InitializeDatabaseConnection(); 

            string query = @"
                            SELECT
                              u.ID,
                              m.MaistoPav AS 'Pavadinimas',
                              p.Priedas AS 'Priedas',
                              u.Kiekis,
                              u.GalutineKaina
                            FROM Uzsakymas u
                            LEFT JOIN Maistas m ON u.MaistoID = m.ID
                            INNER JOIN Priedai p ON u.PasirinktasPriedas = p.ID;";

            using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    mySqlConnection.Open();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, mySqlConnection);
                    adapter.Fill(orderDataTable);

                    dataGridView1.DataSource = orderDataTable;

                    // Skaiciuojama ir rodoma Galutine suma
                    double totalSum = 0.0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Visible) 
                        {
                            if (!Convert.IsDBNull(row.Cells["GalutineKaina"].Value))
                            {
                                double price = Convert.ToDouble(row.Cells["GalutineKaina"].Value);
                                totalSum += price;
                            }
                        }
                    }
                    label1.Text = "Suma: " + totalSum.ToString("0.00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); 
                }
            }
        }

        private void InitializeDatabaseConnection()
        {
            string mysqlCon = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon)) 
            {
                try
                {
                    mySqlConnection.Open();
                  //  MessageBox.Show("Prisijungimas prie duomenu bazes pavyko");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public class DetailedOrderInfo
        {
            public string MaistoPavadinimas { get; set; } // Maisto pav
            public int Kiekis { get; set; } // Kiekis
            public DateTime MokejimoLaikas { get; set; } // Mokejimo laikas
            public double Atstumas { get; set; } // Atstumas
            public bool Complete { get; set; } // Atlikimas
        }

        public List<DetailedOrderInfo> GetOrderDetails(int orderID)
        {
            List<DetailedOrderInfo> orderData = new List<DetailedOrderInfo>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT " +
                        "maistas.Pavadinimas AS MaistoPavadinimas, " +
                        "Kiekis, " +
                        "MokejimoLaikas, " + 
                        "Atstumas, " +    
                        "Atlikimas " +
                        "FROM OrderDetails " +
                        "INNER JOIN maistas ON OrderDetails.maistasID = maistas.ID " +
                        "WHERE OrderDetails.UzsakymoID = @UzsakymasID;";  

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UzsakymasID", orderID);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DetailedOrderInfo data = new DetailedOrderInfo();
                        data.MaistoPavadinimas = reader.GetString("MaistoPavadinimas");
                        data.Kiekis = reader.GetInt32("Kiekis");  

                        data.MokejimoLaikas = reader.GetDateTime("MokejimoLaikas");

                        data.Atstumas = reader.GetDouble("Atstumas");

                        data.Complete = reader.GetBoolean("Atlikimas");

                        orderData.Add(data);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return orderData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            form1.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Užsakymas sumokėtas");

            // Zymi kad uzsakymas apmoketas
            StringBuilder orderDetails = new StringBuilder();

            // Tikrina ar eiles parinktos
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.Visible) 
                    {
                        string foodName = Convert.ToString(row.Cells["Pavadinimas"].Value);
                        string sideDish = Convert.ToString(row.Cells["Priedas"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Kiekis"].Value);
                        orderDetails.AppendFormat("{0}, {1} {2}, ", foodName, sideDish, quantity);

                        int selectedOrderID = Convert.ToInt32(row.Cells["ID"].Value);

                        List<Form5.OrderDisplayData> orderDataList = new List<Form5.OrderDisplayData>();

                        Form5.OrderDisplayData orderData = new Form5.OrderDisplayData();

                        orderDataList.Add(orderData);

                        // Update DataGridView in Form5
                        Form5 form5 = (Form5)Application.OpenForms["Form5"];
                        if (form5 != null)
                        {
                            form5.UpdateDataGridView(orderDataList, selectedOrderID, "", DateTime.Now, 0, true);
                        }
                        else
                        {
                            MessageBox.Show("Form5 not found. Please open Form5 first.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select one or more orders to mark as paid.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Confirmation prompt (optional)
            if (MessageBox.Show("Ar tikrai norite ištrinti visus užsakymus?", "Patvirtinimas", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Connect to the database
                using (MySqlConnection mySqlConnection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        mySqlConnection.Open();

                        // Delete all rows from Uzsakymas table
                        string query = "DELETE FROM Uzsakymas";
                        MySqlCommand command = new MySqlCommand(query, mySqlConnection);
                        command.ExecuteNonQuery();

                        // Clear DataGridView (optional)
                        orderDataTable.Clear();
                        dataGridView1.DataSource = null;

                        // Show success message
                        MessageBox.Show("Užsakymai ištrinti sėkmingai");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();

            form3.Show();

            this.Hide();
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();

            mainForm.Show();

            this.Hide();
        }
    }
}
