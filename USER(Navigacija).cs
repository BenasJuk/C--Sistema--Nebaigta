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
    public partial class Form3 : Form
    {
        private string locationUrl; // Store the location URL

        public Form3()
        {
            InitializeComponent();
            this.locationUrl = locationUrl; // Assign location URL passed from another form (if applicable)
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //Uzkuomentuota nes neveikia URL taip kaip turetu

            //// Access the location URL from your application logic
            //string locationUrl = "https://www.google.com/maps/place/Senoji+Viduklės+koldūninė?zoom=18.56"; // Adjust zoom level as needed
            //Uri uri = new Uri(locationUrl);
            //webBrowser1.Url = uri;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();

            mainForm.Show();

            this.Hide();
        }
    }
}
