using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace majdoee.app
{
    public partial class TruckForm : Form
    {
        public TruckForm()
        {
            InitializeComponent();
        }

        private readonly List<string> Phases = new List<string>() { "Reduction", "Stand", "Assembly" };
        
        private void phase_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (phase.SelectedIndex != 1)
            {
                txtEmp3.Visible = false;
                lbl3.Visible = false;
            }
            else
            {
                txtEmp3.Visible = true;
                lbl3.Visible = true;
            }

            if (string.IsNullOrEmpty(vinNo.Text) || string.IsNullOrWhiteSpace(vinNo.Text))
            {
                return;
            }

            if (!allVinNO.Contains(vinNo.Text))
            {
                return;
            }

            var list = PhaseHelper.GetCurrentTruck(vinNo.Text, Phases);

            if (list.Count() < 2)
            {
                if (phase.SelectedItem.ToString() == Phases[0])
                {
                    txtEmp1.SelectedValue = list[0][1];
                    txtEmp2.SelectedValue = list[0][2];
                    txtEmp3.SelectedItem = null;

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[0][3], list[0][4], list[0][5]);
                }
                else
                {
                    txtEmp1.SelectedItem = null;
                    txtEmp2.SelectedItem = null;
                    txtEmp3.SelectedItem = null;
                    ClearTime();
                }
            }
            else if (list.Count() < 3)
            {
                if (phase.SelectedItem.ToString() == Phases[0])
                {
                    txtEmp1.SelectedValue = list[0][1];
                    txtEmp2.SelectedValue = list[0][2];
                    txtEmp3.SelectedItem = null;

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[0][3], list[0][4], list[0][5]);
                }
                else if(phase.SelectedItem.ToString() == Phases[1])
                {
                    txtEmp1.SelectedValue = list[1][1];
                    txtEmp2.SelectedValue = list[1][2];
                    txtEmp3.SelectedValue = list[1][3];

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[1][4], list[1][5], list[1][6]);
                }
                else
                {
                    txtEmp1.SelectedItem = null;
                    txtEmp2.SelectedItem = null;
                    txtEmp3.SelectedItem = null;
                    ClearTime();
                }
            }
            else
            {
                if (phase.SelectedItem.ToString() == Phases[2])
                {
                    txtEmp1.SelectedValue = list[0][1];
                    txtEmp2.SelectedValue = list[0][2];
                    txtEmp3.SelectedItem = null;

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[0][3], list[0][4], list[0][5]);
                }
                else if (phase.SelectedItem.ToString() == Phases[1])
                {
                    txtEmp1.SelectedValue = list[1][1];
                    txtEmp2.SelectedValue = list[1][2];
                    txtEmp3.SelectedValue = list[1][3];

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[1][4], list[1][5], list[1][6]);
                }
                else
                {
                    txtEmp1.SelectedValue = list[2][1];
                    txtEmp2.SelectedValue = list[2][2];
                    txtEmp3.SelectedItem = null;

                    MoreTimeCbx.Visible = true;
                    InsertTimeToInputs(list[2][3], list[2][4], list[2][5]);
                }
            }

        }

        private void ClearTime()
        {
            FromHour.Value = 6;
            FromMinute.Value = 0;
            FromTime.SelectedIndex = 0;

            ToHour.Value = 4;
            ToMinute.Value = 30;
            ToTime.SelectedIndex = 1;

            MoreTimeCbx.Checked = false;
            MoreTimeCbx.Visible = false;
            CalcTotalHours();
        }

        private void TruckForm_Load(object sender, EventArgs e)
        {
            loadDropDownData();
            CalcTotalHours();

            FillGridByReduction();
            FillGridByStand();
            FillGridByAssembly();
            FillGridByFinshed();
            radioReduction.Checked = true;
        }
        private List<string> allVinNO = new List<string>();
        private void loadDropDownData()
        {
            DB.Open();
            var truck = DB.Get("SELECT * FROM truck");
            var employees1 = DB.Get("SELECT * FROM employee");
            var employees2 = DB.Get("SELECT * FROM employee");
            var employees3 = DB.Get("SELECT * FROM employee");
            DB.Close();

            // Phases -*-*-*-*-*-*-*-*-*
            phase.DataSource = Phases;

            // Vin numbers -*-*-*-*-*-*-*
            setCompoBox(vinNo, truck, "vinNO");
            foreach (DataRow row in truck.Rows)
            {
                allVinNO.Add(row[0].ToString());
            }

            // Employees *-*-*-*-*-*-*-*-
            setCompoBox(txtEmp1, employees1, "emp_name", "empID");
            setCompoBox(txtEmp2, employees2, "emp_name", "empID");
            setCompoBox(txtEmp3, employees3, "emp_name", "empID");
        }

        private void setCompoBox
            (ComboBox txtName, DataTable data, string title, string value = null)
        {
            txtName.DataSource = data;
            txtName.DisplayMember = title;
            if(value != null)
                txtName.ValueMember = value;

            txtName.SelectedItem = null;
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var timeNow = DateTime.Now;
                if (!Validaion('C'))
                {
                    return;
                }

                if (phase.SelectedItem.ToString() != Phases[0])
                {
                    MessageBox.Show($"Sorry, A new truck MUST be in 'Reduction Phase'", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                int emp1ID = Int32.Parse(txtEmp1.SelectedValue.ToString());
                int emp2ID = Int32.Parse(txtEmp2.SelectedValue.ToString());

                DateTime from = ParseTime($"{FromHour.Value.ToString().PadLeft(2, '0')}:{FromMinute.Value.ToString().PadLeft(2, '0')} {FromTime.Text}");
                DateTime to = ParseTime($"{ToHour.Value.ToString().PadLeft(2, '0')}:{ToMinute.Value.ToString().PadLeft(2, '0')} {ToTime.Text}");

                DB.Open();
                DB.Run($"INSERT INTO truck (vinNo, total_hours, phase, createdAt) VALUES('{vinNo.Text}', '{lblTotalHours.Text}', '{Phases[0]}', '{timeNow}')");
                
                DB.Run($"INSERT INTO reduction (emp1, emp2, fromDate, toDate, total, truck_vin) VALUES({emp1ID},{emp2ID},'{from}','{to}','{lblTotalHours.Text}','{vinNo.Text}')");

                Form1.instance.Load_counters();
                MessageBox.Show("New truck has been added", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error | {ex}", "Something Went Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
            finally
            { 
                DB.Close();
            } 
                FillGridByReduction();
        }

        private void Clear()
        {
            vinNo.Text = "";
            txtEmp1.SelectedItem = null;
            txtEmp2.SelectedItem = null;
            txtEmp3.SelectedItem = null;

            FromHour.Value = 6;
            FromMinute.Value = 0;
            FromTime.SelectedIndex = 0;

            ToHour.Value = 4;
            ToMinute.Value = 30;
            ToTime.SelectedIndex = 1;

            phase.SelectedItem = Phases[0];

            doneBtn.Visible = false;
            MoreTimeCbx.Visible = false;
            MoreTimeCbx.Checked = false;

            CalcTotalHours();
        }

        DataTable dtReduction = new DataTable();
        private void FillGridByReduction()
        {
            dtReduction.Clear();

            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE phase = '{Phases[0]}'");
            var reduction = DB.Get("SELECT * FROM reduction");
            var employee = DB.Get("SELECT * FROM employee");
            DB.Close();

            if(dtReduction.Columns.Count < 1) 
            {
                dtReduction.Columns.Add("Vin No");
                dtReduction.Columns.Add("Employee 01");
                dtReduction.Columns.Add("Employee 02");
                dtReduction.Columns.Add("From");
                dtReduction.Columns.Add("To");
                dtReduction.Columns.Add("Total Hours");

                DataColumn[] pks = new DataColumn[1];
                pks[0] = dtReduction.Columns[0];
                dtReduction.PrimaryKey = pks;
            }

            for(int i = 0; i < truck.Rows.Count; i++)
            {
                DataRow row = dtReduction.NewRow();

                var truck_vin = truck.Rows[i][0];
                var redutRow = reduction.Select($"truck_vin = '{truck_vin}'");

                var emp1 = employee.Select($"empID = {redutRow[0][1]}");
                var emp2 = employee.Select($"empID = {redutRow[0][2]}");
                // Vin number
                row[0] = truck_vin;
                // employee 1 / 2
                row[1] = emp1[0][1];
                row[2] = emp2[0][1];
                // from / to
                row[3] = formatedTime(redutRow[0][3].ToString());
                row[4] = formatedTime(redutRow[0][4].ToString());
                // total
                row[5] = redutRow[0][5].ToString();

                if (!dtReduction.Rows.Contains(row))
                {
                    dtReduction.Rows.Add(row);
                }
            }
        }

        private string formatedTime(string date)
        {
            if (string.IsNullOrEmpty(date))
                return "00:00:00 AM";

            return Convert.ToDateTime(date).ToString("hh:mm:ss tt");
        }

        private void radioReduction_CheckedChanged(object sender, EventArgs e)
        {
            truckGrid.Columns.Clear();
            DataView dataView = new DataView(dtReduction);
            dataView.Sort = "From desc";
            truckGrid.DataSource = dataView;
        }

        DataTable dtStand = new DataTable();

        private void FillGridByStand()
        {
            dtStand.Clear();

            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE phase = '{Phases[1]}'");
            var stand = DB.Get("SELECT * FROM stand");
            var employee = DB.Get("SELECT * FROM employee");
            DB.Close();

            if (dtStand.Columns.Count < 1)
            {
                dtStand.Columns.Add("Vin No");
                dtStand.Columns.Add("Employee 01");
                dtStand.Columns.Add("Employee 02");
                dtStand.Columns.Add("Employee 03");
                dtStand.Columns.Add("From");
                dtStand.Columns.Add("To");
                dtStand.Columns.Add("Total Hours");

                DataColumn[] pks = new DataColumn[1];
                pks[0] = dtStand.Columns[0];
                dtStand.PrimaryKey = pks;
            }

            for (int i = 0; i < truck.Rows.Count; i++)
            {
                DataRow row = dtStand.NewRow();
                var truck_vin = truck.Rows[i][0];
                var standRow = stand.Select($"truck_vin = '{truck_vin}'");

                var emp1 = employee.Select($"empID = {standRow[0][1]}");
                var emp2 = employee.Select($"empID = {standRow[0][2]}");
                var emp3 = employee.Select($"empID = {standRow[0][3]}");
                // Vin number
                row[0] = truck_vin;
                // employee 1 / 2 / 3
                row[1] = emp1[0][1];
                row[2] = emp2[0][1];
                row[3] = emp3[0][1];
                // from / to
                row[4] = formatedTime(standRow[0][4].ToString());
                row[5] = formatedTime(standRow[0][5].ToString());
                // total
                row[6] = standRow[0][6].ToString();

                dtStand.Rows.Add(row);
            }
        }


        private void radioStand_CheckedChanged(object sender, EventArgs e)
        {
            truckGrid.Columns.Clear();
            DataView dataView = new DataView(dtStand);
            dataView.Sort = "From desc";
            truckGrid.DataSource = dataView;
        }

        DataTable dtAssembly = new DataTable();
        private void FillGridByAssembly()
        {
            dtAssembly.Clear();

            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE phase = '{Phases[2]}'");
            var assembly = DB.Get("SELECT * FROM assembly");
            var employee = DB.Get("SELECT * FROM employee");
            DB.Close();

            if (dtAssembly.Columns.Count < 1)
            {
                dtAssembly.Columns.Add("Vin No");
                dtAssembly.Columns.Add("Employee 01");
                dtAssembly.Columns.Add("Employee 02");
                dtAssembly.Columns.Add("From");
                dtAssembly.Columns.Add("To");
                dtAssembly.Columns.Add("Total Hours");

                DataColumn[] pks = new DataColumn[1];
                pks[0] = dtAssembly.Columns[0];
                dtAssembly.PrimaryKey = pks;
            }

            for (int i = 0; i < truck.Rows.Count; i++)
            {
                DataRow row = dtAssembly.NewRow();
                var truck_vin = truck.Rows[i][0];
                var assemblyRow = assembly.Select($"truck_vin = '{truck_vin}'");

                var emp1 = employee.Select($"empID = {assemblyRow[0][1]}");
                var emp2 = employee.Select($"empID = {assemblyRow[0][2]}");
                // Vin number
                row[0] = truck_vin;
                // employee 1 / 2
                row[1] = emp1[0][1];
                row[2] = emp2[0][1];
                // from / to
                row[3] = formatedTime(assemblyRow[0][3].ToString());
                row[4] = formatedTime(assemblyRow[0][4].ToString());
                // total
                row[5] = assemblyRow[0][5].ToString();

                dtAssembly.Rows.Add(row);
            }
        }
        private void radioAssembly_CheckedChanged(object sender, EventArgs e)
        {
            truckGrid.Columns.Clear();
            DataView dataView = new DataView(dtAssembly);
            dataView.Sort = "From desc";
            truckGrid.DataSource = dataView;
        }

        DataTable dtFinished = new DataTable();
        private void FillGridByFinshed()
        {
            dtFinished.Clear();

            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE phase = 'Done'");
            var assembly = DB.Get("SELECT * FROM assembly");
            DB.Close();

            if (dtFinished.Columns.Count < 1)
            {
                dtFinished.Columns.Add("Vin No");
                dtFinished.Columns.Add("Starts");
                dtFinished.Columns.Add("Ends");
                dtFinished.Columns.Add("Total Hours");

                DataColumn[] pks = new DataColumn[1];
                pks[0] = dtFinished.Columns[0];
                dtFinished.PrimaryKey = pks;
            }

            for (int i = 0; i < truck.Rows.Count; i++)
            {
                DataRow row = dtFinished.NewRow();
                var truck_vin = truck.Rows[i][0];
                string endsIn = assembly.Select($"truck_vin = '{truck_vin}'")[0][4].ToString();

                row[0] = truck_vin;
                row[1] = truck.Rows[i][3];
                row[2] = endsIn;
                row[3] = truck.Rows[i][1];

                dtFinished.Rows.Add(row);
            }
        }

        private void radioDone_CheckedChanged(object sender, EventArgs e)
        {
            truckGrid.Columns.Clear();
            DataView dataView = new DataView(dtFinished);
            dataView.Sort = "Ends desc";
            truckGrid.DataSource = dataView;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void moreTimeBtn_Click(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void CalcTotalHours()
        {
            var strTotal = "";
            TimeSpan breakTime = TimeSpan.Parse(txtBreakTime.Text);
            //MessageBox.Show(breakTime.ToString());

            DateTime from = ParseTime($"{FromHour.Value.ToString().PadLeft(2, '0')}:{FromMinute.Value.ToString().PadLeft(2, '0')} {FromTime.Text}");
            DateTime to = ParseTime($"{ToHour.Value.ToString().PadLeft(2, '0')}:{ToMinute.Value.ToString().PadLeft(2, '0')} {ToTime.Text}");

            TimeSpan timeSpan = from > to? from - to: to - from;
            timeSpan = timeSpan.Subtract(breakTime);

            strTotal = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}";
            lblTotalHours.Text = strTotal;
        }

        private DateTime ParseTime(string timeStr)
        {
            if (DateTime.TryParseExact(timeStr, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            {
                return time;
            }
            else
            {
                return DateTime.Now;
            }
        }

        private void FromMinute_ValueChanged(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void ToHour_ValueChanged(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void ToMinute_ValueChanged(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void FromHour_ValueChanged(object sender, EventArgs e)
        {
            CalcTotalHours();

        }

        private void txtBreakTime_SelectedItemChanged_1(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void FromTime_SelectedItemChanged(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void ToTime_SelectedItemChanged(object sender, EventArgs e)
        {
            CalcTotalHours();
        }

        private void truckGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            FillToEdit(truckGrid[0, e.RowIndex].Value.ToString());
            tabControl1.SelectedIndex = 0;
        }

        private void FillToEdit(string truckVinNo)
        {
            try
            {
                DB.Open();
                var selectedTruck = DB.Get($"SELECT * FROM truck WHERE VinNo = '{truckVinNo}'");
                var employee = DB.Get("SELECT * FROM employee");

                DataRow phaseData = null;
                MoreTimeCbx.Visible = true;

                vinNo.Text = truckVinNo;
                if (selectedTruck.Rows[0][2].ToString() == Phases[0])
                {
                    phaseData = DB.Get($"SELECT * FROM reduction WHERE truck_vin = '{truckVinNo}'").Rows[0];
                    var emp1 = employee.Select($"empID = {phaseData[1]}")[0][0];
                    var emp2 = employee.Select($"empID = {phaseData[2]}")[0][0];

                    txtEmp1.SelectedValue = emp1;
                    txtEmp2.SelectedValue = emp2;

                    phase.SelectedItem = Phases[0];

                    InsertTimeToInputs(phaseData[3], phaseData[4], phaseData[5].ToString());

                }
                else if (selectedTruck.Rows[0][2].ToString() == Phases[1])
                {
                    phaseData = DB.Get($"SELECT * FROM stand WHERE truck_vin = '{truckVinNo}'").Rows[0];
                    var emp1 = employee.Select($"empID = {phaseData[1]}")[0][0];
                    var emp2 = employee.Select($"empID = {phaseData[2]}")[0][0];
                    var emp3 = employee.Select($"empID = {phaseData[3]}")[0][0];

                    txtEmp1.SelectedValue = emp1;
                    txtEmp2.SelectedValue = emp2;
                    txtEmp3.SelectedValue = emp3;

                    phase.SelectedItem = Phases[1];

                    var fromDate = phaseData[4].ToString().Split(' ');
                    FromHour.Value = Int32.Parse(fromDate[1].Split(':')[0]);
                    FromMinute.Value = Int32.Parse(fromDate[1].Split(':')[1]);
                    FromTime.SelectedItem = fromDate[2];

                    var toDate = phaseData[5].ToString().Split(' ');
                    ToHour.Value = Int32.Parse(toDate[1].Split(':')[0]);
                    ToMinute.Value = Int32.Parse(toDate[1].Split(':')[1]);
                    ToTime.SelectedItem = toDate[2];

                    lblTotalHours.Text = phaseData[6].ToString();
                }
                else if(selectedTruck.Rows[0][2].ToString() == Phases[2])
                {
                    phaseData = DB.Get($"SELECT * FROM assembly WHERE truck_vin = '{truckVinNo}'").Rows[0];
                    var emp1 = employee.Select($"empID = {phaseData[1]}")[0][0];
                    var emp2 = employee.Select($"empID = {phaseData[2]}")[0][0];

                    txtEmp1.SelectedValue = emp1;
                    txtEmp2.SelectedValue = emp2;

                    phase.SelectedItem = Phases[2];
                    doneBtn.Visible = true;

                    var fromDate = phaseData[3].ToString().Split(' ');
                    FromHour.Value = Int32.Parse(fromDate[1].Split(':')[0]);
                    FromMinute.Value = Int32.Parse(fromDate[1].Split(':')[1]);
                    FromTime.SelectedItem = fromDate[2];

                    var toDate = phaseData[4].ToString().Split(' ');
                    ToHour.Value = Int32.Parse(toDate[1].Split(':')[0]);
                    ToMinute.Value = Int32.Parse(toDate[1].Split(':')[1]);
                    ToTime.SelectedItem = toDate[2];

                    lblTotalHours.Text = phaseData[5].ToString();
                }
                else
                {
                    phaseData = DB.Get($"SELECT * FROM assembly WHERE truck_vin = '{truckVinNo}'").Rows[0];
                    var emp1 = employee.Select($"empID = {phaseData[1]}")[0][0];
                    var emp2 = employee.Select($"empID = {phaseData[2]}")[0][0];

                    txtEmp1.SelectedValue = emp1;
                    txtEmp2.SelectedValue = emp2;

                    phase.SelectedItem = Phases[2];
                    doneBtn.Visible = true;

                    var fromDate = phaseData[3].ToString().Split(' ');
                    FromHour.Value = Int32.Parse(fromDate[1].Split(':')[0]);
                    FromMinute.Value = Int32.Parse(fromDate[1].Split(':')[1]);
                    FromTime.SelectedItem = fromDate[2];

                    var toDate = phaseData[4].ToString().Split(' ');
                    ToHour.Value = Int32.Parse(toDate[1].Split(':')[0]);
                    ToMinute.Value = Int32.Parse(toDate[1].Split(':')[1]);
                    ToTime.SelectedItem = toDate[2];

                    lblTotalHours.Text = phaseData[5].ToString();
                    doneBtn.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error | {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            finally 
            {
                DB.Close();
            }
        }

        private void FromHour_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FromMinute.Focus();
            }
        }

        private void FromMinute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FromTime.Focus();
            }
        }

        private void FromTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ToHour.Focus();
            }
        }

        private void ToHour_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ToMinute.Focus();
            }
        }

        private void ToMinute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ToTime.Focus();
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            if (allVinNO.Contains(vinNo.Text))
            {
                FillToEdit(vinNo.Text);
            }
            else
            {
                MessageBox.Show($"Sorry, Cannot find truck with ({vinNo.Text}) number", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            ALLUpdatesInOne();
        }

        private void ALLUpdatesInOne()
        {
            if (!Validaion('U'))
            {
                return;
            }

            int emp1ID = Int32.Parse(txtEmp1.SelectedValue.ToString());
            int emp2ID = Int32.Parse(txtEmp2.SelectedValue.ToString());
            

            DateTime from = ParseTime($"{FromHour.Value.ToString().PadLeft(2, '0')}:{FromMinute.Value.ToString().PadLeft(2, '0')} {FromTime.Text}");
            DateTime to = ParseTime($"{ToHour.Value.ToString().PadLeft(2, '0')}:{ToMinute.Value.ToString().PadLeft(2, '0')} {ToTime.Text}");

            try
            {
                DB.Open();
                var truck = DB.Get($"SELECT * FROM truck WHERE vinNo = '{vinNo.Text}'").Rows[0];
                DB.Close();

                if (truck[2].ToString() == Phases[0] && phase.SelectedItem.ToString() == Phases[0])
                {
                    try
                    {
                        PhaseHelper.UpdateReduction(truck, emp1ID, emp2ID, MoreTimeCbx.Checked, from, to, lblTotalHours.Text);
                        MessageBox.Show($"Truck has been updated successfully", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGridByReduction();
                        Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (truck[2].ToString() == Phases[0] && phase.SelectedItem.ToString() == Phases[1])
                {
                    int emp3ID = Int32.Parse(txtEmp3.SelectedValue.ToString());
                    try
                    {
                        PhaseHelper.AddStand(truck, emp1ID, emp2ID, emp3ID, from, to, lblTotalHours.Text);
                        MessageBox.Show($"Right now, The Truck is in Stand phase", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGridByReduction();
                        FillGridByStand();
                        Clear();
                        Form1.instance.Load_counters();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (truck[2].ToString() == Phases[1] && phase.SelectedItem.ToString() == Phases[1])
                {
                    int emp3ID = Int32.Parse(txtEmp3.SelectedValue.ToString());
                    try
                    {
                        PhaseHelper.UpdateStand(truck, emp1ID, emp2ID, emp3ID, MoreTimeCbx.Checked, from, to, lblTotalHours.Text);
                        MessageBox.Show($"Truck has been updated successfully", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGridByStand();
                        Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (truck[2].ToString() == Phases[1] && phase.SelectedItem.ToString() == Phases[2])
                {
                    //AddAssembly(truck);
                    try
                    {
                        PhaseHelper.AddAssembly(truck, emp1ID, emp2ID, from, to, lblTotalHours.Text);
                        MessageBox.Show($"Right now, The Truck is in Assembly phase", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGridByStand();
                        FillGridByAssembly();
                        Clear();
                        Form1.instance.Load_counters();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (truck[2].ToString() == Phases[2] && phase.SelectedItem.ToString() == Phases[2])
                {
                    //UpdateAssembly(truck);
                    try
                    {
                        PhaseHelper.UpdateAssembly(truck, emp1ID, emp2ID, MoreTimeCbx.Checked, from, to, lblTotalHours.Text);
                        MessageBox.Show($"Truck has been updated successfully", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FillGridByAssembly();
                        Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("This truck MUST jumb to 'Stand' phase first", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Validaion(char operation)
        {
            if (string.IsNullOrEmpty(vinNo.Text) || string.IsNullOrWhiteSpace(vinNo.Text))
            {
                if (operation == 'C')
                {
                    MessageBox.Show($"Please Add Unique VIN Number", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show($"Please Choose a Existing VIN Number", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return false;
            }

            if (operation == 'U')
            {
                if (!allVinNO.Contains(vinNo.Text))
                {
                    MessageBox.Show($"Sorry, Cannot find truck with ({vinNo.Text}) number", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            // Empty Checking
            if (phase.SelectedItem.ToString() == Phases[0] ||
                phase.SelectedItem.ToString() == Phases[2])
            {
                if (txtEmp1.SelectedIndex == -1 || txtEmp2.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Fill The Employee Fields", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else
            {
                if (txtEmp1.SelectedIndex == -1 || txtEmp2.SelectedIndex == -1
                || txtEmp3.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Fill The Employee Fields", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            // Same Checking
            if (((phase.SelectedItem.ToString() == Phases[0] ||
                phase.SelectedItem.ToString() == Phases[2]) &&
                txtEmp1.SelectedIndex == txtEmp2.SelectedIndex) ||
                (phase.SelectedItem.ToString() == Phases[1] &&
                txtEmp1.SelectedIndex == txtEmp2.SelectedIndex ||
                txtEmp1.SelectedIndex == txtEmp3.SelectedIndex ||
                txtEmp2.SelectedIndex == txtEmp3.SelectedIndex))
            {
                MessageBox.Show("Please Choose Different Employees", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private void MoreTimeCbx_CheckedChanged(object sender, EventArgs e)
        {
            FromHour.Value = 6;
            FromMinute.Value = 0;
            FromTime.SelectedIndex = 0;

            ToHour.Value = 4;
            ToMinute.Value = 30;
            ToTime.SelectedIndex = 1;
        }

        private void doneBtn_Click(object sender, EventArgs e)
        {
            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE vinNo = '{vinNo.Text}'").Rows[0];
            DB.Close();

            //Done(truck);
            try
            {
                PhaseHelper.Done(truck);
                MessageBox.Show($"Finally, The Truck Finished", "SUCCESS :)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FillGridByAssembly();
                FillGridByFinshed();
                Clear();
                Form1.instance.Load_counters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Something Went Wrong!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertTimeToInputs(object from, object to, object total)
        {
            var fromDate = from.ToString().Split(' ');
            FromHour.Value = Int32.Parse(fromDate[1].Split(':')[0]);
            FromMinute.Value = Int32.Parse(fromDate[1].Split(':')[1]);
            FromTime.SelectedItem = fromDate[2];

            var toDate = to.ToString().Split(' ');
            ToHour.Value = Int32.Parse(toDate[1].Split(':')[0]);
            ToMinute.Value = Int32.Parse(toDate[1].Split(':')[1]);
            ToTime.SelectedItem = toDate[2];

            lblTotalHours.Text = total.ToString();
        }
    }
}
