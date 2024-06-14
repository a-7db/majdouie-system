using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace majdoee.app
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public Form1()
        {
            InitializeComponent();
            instance = this;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var truck = new TruckForm();
            truck.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadImages();
            Load_counters();
        }

        private void LoadImages()
        {
            pictureBox1.Image = Properties.Resources.Logo_Planny;
            truckBtn.Image = Properties.Resources.truck;
            empBtn.Image = Properties.Resources.employee;
            button1.Image = Properties.Resources.materials;
            settingBtn.Image = Properties.Resources.settings;
            pictureBox2.Image = Properties.Resources.copyright;
        }

        public void Load_counters()
        {
            DB.Open();

            var empCounter = DB.Get("SELECT COUNT(*) FROM employee");
            emp_lbl.Text = empCounter.Rows[0][0].ToString().PadLeft(2, '0');

            var truckCounter = DB.Get("SELECT COUNT(*) FROM truck");
            truck_lbl.Text = truckCounter.Rows[0][0].ToString().PadLeft(2, '0');

            var truckInProgress = DB.Get("SELECT COUNT(*) FROM truck WHERE phase IN ('Reduction', 'Stand', 'Assembly')");
            inProgress_lbl.Text = truckInProgress.Rows[0][0].ToString().PadLeft(2, '0');

            var truckDone = DB.Get("SELECT COUNT(*) FROM truck WHERE phase = 'Done'");
            done_lbl.Text = truckDone.Rows[0][0].ToString().PadLeft(2, '0');

            DB.Close();
        }

        private void empBtn_Click(object sender, EventArgs e)
        {
            Employees employees = new Employees();

            employees.ShowDialog();
        }

        DeveloperDetails myDetailsForm = new DeveloperDetails();
        private void label2_Click(object sender, EventArgs e)
        {
            myDetailsForm.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            myDetailsForm.ShowDialog();
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Gray;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            label2.ForeColor = Color.DarkGray;
        }
    }
}
