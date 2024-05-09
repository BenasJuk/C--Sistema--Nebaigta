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

namespace Maisto_uzsakymo_sistema
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private MySqlCommand command;
        private string mysqlCon = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

        public Form1()
        {
            InitializeComponent();
            connection = new MySqlConnection(mysqlCon);
            command = new MySqlCommand();

        }

     
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            string mysqlCon = "server=127.0.0.1; user=root; database=maisto_sistema; password=";

            using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon)) 
            {
                try
                {
                    mySqlConnection.Open();
                    MessageBox.Show("Prisijungimas prie duomenu bazes pavyko");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.Show();

            this.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int kiekis; 

                // Gaunamas maisto kiekis per Message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox1.Checked = false; // Jei vedimas klaidingas, unchecked
                    return;
                }

                // Gaunama maisto kaina is duomenu bazes
                decimal kainaVienetas = 0; 

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID";
                        command.Parameters.AddWithValue("@MaistoID", 1);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString());
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox1.Checked = false;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox1.Checked = false;
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Apskaiciuojama galutine kaina
                decimal galutineKaina = kiekis * kainaVienetas;

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 1);
                        command.Parameters.AddWithValue("@MaistoID", 1);
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 1);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina);
                        command.Parameters.AddWithValue("@IsChecked", checkBox1.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox1.Checked = false;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // Trinimas
            {
                
                int orderID = 1;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox2.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 2);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox2.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox2.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 2); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 2); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 2);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox2.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox2.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 2;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox3.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 3);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox3.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox3.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 3); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 3); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 3);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox3.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox3.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 3;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox4.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 4);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox4.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox4.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 4); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 4); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 4);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox4.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox4.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 4;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox5.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 5);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox5.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox5.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 5); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 5); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 5);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox5.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox5.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 5;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox6.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 6);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox6.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox6.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 6); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 6); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 4);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox6.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox6.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 6;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox7.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 7);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox7.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox7.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 7); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 7); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 6);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox7.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox7.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 7;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox8.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 8);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox8.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox8.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 8); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 8); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 7);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox8.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox8.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 8;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox9.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 9);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox9.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox9.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 9); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 9); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 6);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox9.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox9.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 9;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox10.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 10);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox10.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox10.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 10); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 10); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 7);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox10.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox10.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 10;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                int kiekis; // Variable to store the user's quantity input

                // Get user input for quantity using a message box
                string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Įveskite norimą kiekį:", "Kiekis");

                if (!int.TryParse(quantityInput, out kiekis))
                {
                    MessageBox.Show("Neteisingas kiekio formatas. Įveskite tik skaičius.");
                    checkBox11.Checked = false; // Uncheck the checkbox if input is invalid
                    return;
                }

                // Retrieve price per unit from database based on MaistoID (assuming you know it)
                decimal kainaVienetas = 0; // Variable to store the price per unit

                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        // Modify the query to retrieve GalutineKaina (price) for the specific MaistoID (replace with your logic to get MaistoID)
                        command.CommandText = "SELECT Kaina FROM Maistas WHERE ID = @MaistoID"; // Replace "Maistas" with your table name
                        command.Parameters.AddWithValue("@MaistoID", 11);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                kainaVienetas = decimal.Parse(reader["Kaina"].ToString()); // Convert retrieved value to decimal
                            }
                            else
                            {
                                MessageBox.Show("Nepavyko gauti kainos iš duomenų bazės.");
                                checkBox11.Checked = false; // Uncheck the checkbox if price retrieval fails
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox11.Checked = false; // Uncheck the checkbox if an error occurs
                        return;
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }

                // Calculate final price
                decimal galutineKaina = kiekis * kainaVienetas;

                // Update the code to insert the GalutineKaina (final price) into the Uzsakymas table
                using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                {
                    try
                    {
                        mySqlConnection.Open();
                        command = new MySqlCommand();
                        command.Connection = mySqlConnection;

                        command.CommandText = "INSERT INTO Uzsakymas (ID, MaistoID, PasirinktasPriedas, Kiekis, GalutineKaina, IsChecked)" +
                                                 " VALUES (@ID, @MaistoID, @PasirinktasPriedas, @Kiekis, @GalutineKaina, @IsChecked)";

                        command.Parameters.AddWithValue("@ID", 11); // Replace with appropriate values if needed
                        command.Parameters.AddWithValue("@MaistoID", 11); // Assuming you have MaistoID
                        command.Parameters.AddWithValue("@PasirinktasPriedas", 7);
                        command.Parameters.AddWithValue("@Kiekis", kiekis);
                        command.Parameters.AddWithValue("@GalutineKaina", galutineKaina); // Use the calculated final price
                        command.Parameters.AddWithValue("@IsChecked", checkBox11.Checked);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Užsakymas pateiktas!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        checkBox11.Checked = false; // Uncheck the checkbox if an error occurs
                    }
                    finally
                    {
                        mySqlConnection.Close();
                    }
                }
            }
            else // When checkbox is unchecked, remove the item from the order
            {
                // Assuming you have an order ID or some way to identify the specific order for this checkbox
                int orderID = 11;

                if (orderID > 0)
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(mysqlCon))
                    {
                        try
                        {
                            mySqlConnection.Open();
                            command = new MySqlCommand();
                            command.Connection = mySqlConnection;

                            // Modify the query to delete the order based on the order ID
                            command.CommandText = "DELETE FROM Uzsakymas WHERE ID = @OrderID";
                            command.Parameters.AddWithValue("@OrderID", orderID);

                            command.ExecuteNonQuery();
                            MessageBox.Show("Užsakymas pašalintas!"); // Confirmation message for removal
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        finally
                        {
                            mySqlConnection.Close();
                        }
                    }
                }
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
