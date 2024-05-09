using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maisto_uzsakymo_sistema
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();

            form1.Show();

            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();

            form2.Show();

            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();

            form3.Show();

            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();

            form4.Show();

            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();

            form5.Show();

            this.Hide();
        }
    }
}
