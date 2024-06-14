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
    public partial class Employees : Form
    {
        public Employees()
        {
            InitializeComponent();
        }

        private DataTable dtEmployees = new DataTable();
        private int selectedEmpID = 0;

        private void Employees_Load(object sender, EventArgs e)
        {
            FillEmpData();
        }

        private void FillEmpData()
        {
            dtEmployees.Clear();

            if(dtEmployees.Columns.Count < 1 )
            {
                dtEmployees.Columns.Add("ID");
                dtEmployees.Columns.Add("Name");
                dtEmployees.Columns.Add("Total Hours");
            }

            DB.Open();
            var emps = DB.Get("SELECT * FROM employee");
            var reduction = DB.Get($"SELECT * FROM reduction");
            var stand = DB.Get($"SELECT * FROM stand");
            var assembly = DB.Get($"SELECT * FROM assembly");
            DB.Close();

            for(int i = 0; i < emps.Rows.Count; i++)
            {
                DataRow row = dtEmployees.NewRow();
                var empID = emps.Rows[i][0];

                var redHours = reduction.Select($"emp1 = {empID} or emp2 = {empID}");
                var standHours = stand.Select($"emp1 = {empID} or emp2 = {empID} or emp3 = {empID}");
                var assemblyHours = assembly.Select($"emp1 = {empID} or emp2 = {empID}");

                TimeSpan total = TimeSpan.Zero;
                for (int d = 0; d < redHours.Length; d++)
                {
                    total += TimeSpan.Parse(redHours[d][5].ToString());
                }
                for (int d = 0; d < standHours.Length; d++)
                {
                    total += TimeSpan.Parse(standHours[d][6].ToString());
                }
                for (int d = 0; d < assemblyHours.Length; d++)
                {
                    total += TimeSpan.Parse(assemblyHours[d][5].ToString());
                }

                row[0] = empID;
                row[1] = emps.Rows[i][1];
                row[2] = $"{total.Hours.ToString().PadLeft(2, '0')}:{total.Minutes.ToString().PadLeft(2, '0')}";

                dtEmployees.Rows.Add(row);
            }

            empGrid.DataSource = dtEmployees;
        }


        private void createBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(txtEmpName.Text))
                {
                    MessageBox.Show("Please Fill The Name Field", "Try Again!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DB.Open();
                DB.Run($"INSERT INTO employee (emp_name) VALUES('{txtEmpName.Text}')");
                Form1.instance.Load_counters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error | {ex}", "Something Went Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally 
            {
                DB.Close();
                txtEmpName.Text = "";
                FillEmpData();
            }
        }

        private void empGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            selectedEmpID = Int32.Parse(empGrid[0, e.RowIndex].Value.ToString());
            var name = empGrid[1, e.RowIndex].Value.ToString();

            txtEmpName.Text = name;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(selectedEmpID < 1)
                {
                    MessageBox.Show("Please Select an Employee", "Try Again!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                DB.Open();
                DB.Run($"UPDATE employee SET emp_name = '{txtEmpName.Text}' WHERE empID = {selectedEmpID}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error | {ex}", "Something Went Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally 
            {
                DB.Close();
                txtEmpName.Text = "";
                selectedEmpID = 0;
                FillEmpData();
            }
        }
    }
}
