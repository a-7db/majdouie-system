using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace majdoee.app
{
    internal static class PhaseHelper
    {
        internal static void AddAssembly(DataRow truck, int emp1ID, int emp2ID, DateTime from, DateTime to, string total)
        {
            var strTotal = NewTotalHours(truck[1].ToString(), total);

            DB.Open();
            DB.Run($"UPDATE truck SET total_hours = '{strTotal}', phase = 'Assembly' WHERE vinNo = '{truck[0]}'");

            DB.Run($"INSERT INTO assembly (emp1, emp2, fromDate, toDate, total, truck_vin) " +
                $"VALUES({emp1ID}, {emp2ID}, '{from}', '{to}', '{total}', '{truck[0]}')");
            DB.Close();
        }

        internal static void AddStand(DataRow truck, int emp1ID, int emp2ID, int emp3ID, DateTime from, DateTime to, string total)
        {
            var strTotal = NewTotalHours(truck[1].ToString(), total);

            DB.Open();
            DB.Run($"UPDATE truck SET total_hours = '{strTotal}', phase = 'Stand' WHERE vinNo = '{truck[0]}'");

            DB.Run($"INSERT INTO stand (emp1, emp2, emp3, fromDate, toDate, total,truck_vin) " +
                $"VALUES({emp1ID}, {emp2ID}, {emp3ID}, '{from}', '{to}', '{total}', '{truck[0]}')");
            DB.Close();
        }

        internal static void Done(DataRow truck)
        {
            DB.Open();

            DB.Run($"UPDATE truck SET phase = 'Done' WHERE vinNO = '{truck[0]}'");

            DB.Close();
        }

        internal static List<DataRow> GetCurrentTruck(string text, List<string> Phases)
        {
            List<DataRow> list = new List<DataRow>();
            DB.Open();
            var truck = DB.Get($"SELECT * FROM truck WHERE vinNo = '{text}'").Rows[0];

            if (truck[2].ToString() == Phases[0])
            {
                list.Add(DB.Get($"SELECT * FROM reduction WHERE truck_vin = '{text}'").Rows[0]);
            }
            else if (truck[2].ToString() == Phases[1])
            {
                list.Add(DB.Get($"SELECT * FROM reduction WHERE truck_vin = '{text}'").Rows[0]);
                list.Add(DB.Get($"SELECT * FROM stand WHERE truck_vin = '{text}'").Rows[0]);
            }
            else
            {
                list.Add(DB.Get($"SELECT * FROM reduction WHERE truck_vin = '{text}'").Rows[0]);
                list.Add(DB.Get($"SELECT * FROM stand WHERE truck_vin = '{text}'").Rows[0]);
                list.Add(DB.Get($"SELECT * FROM assembly WHERE truck_vin = '{text}'").Rows[0]);
            }
            DB.Close();

            return list;
        }

        internal static void UpdateAssembly(DataRow truck, int emp1ID, int emp2ID, bool includeDuration, DateTime from, DateTime to, string total)
        {
            DB.Open();
            if (includeDuration)
            {
                var assembly = DB.Get($"SELECT * FROM assembly WHERE truck_vin = '{truck[0]}'").Rows[0];
                var strTotal = NewTotalHours(truck[1].ToString(), total);
                var assemblyTotal = NewTotalHours(assembly[5].ToString(), total);

                DB.Run($"UPDATE truck SET total_hours = '{strTotal}' WHERE vinNo = '{truck[0]}'");

                DB.Run($"UPDATE assembly SET emp1 = {emp1ID}, emp2 = {emp2ID}," +
                    $"fromDate = '{from}', toDate = '{to}', total = '{assemblyTotal}' WHERE truck_vin = '{truck[0]}'");
            }
            else
            {
                DB.Run($"UPDATE truck SET total_hours = '{total}' WHERE vinNo = '{truck[0]}'");
                DB.Run($"UPDATE assembly SET emp1 = {emp1ID}, emp2 = {emp2ID}, fromDate = '{from}', toDate = '{to}', total = '{total}' WHERE truck_vin = '{truck[0]}'");
            }

            DB.Close();
        }

        internal static void UpdateReduction
            (DataRow truck, int emp1, int emp2, bool includeDuration,
            DateTime from, DateTime to, string total)
        {
            // Sum new total with truck total
            DB.Open();
            if (includeDuration)
            {
                var strTotal = NewTotalHours(truck[1].ToString(), total);

                DB.Run($"UPDATE truck SET total_hours = '{strTotal}' WHERE vinNo = '{truck[0]}'");

                DB.Run($"UPDATE reduction SET emp1 = {emp1}, emp2 = {emp2}," +
                    $"fromDate = '{from}', toDate = '{to}', total = '{strTotal}' WHERE truck_vin = '{truck[0]}'");
            }
            else
            {
                DB.Run($"UPDATE truck SET total_hours = '{total}' WHERE vinNo = '{truck[0]}'");
                DB.Run($"UPDATE reduction SET emp1 = {emp1}, emp2 = {emp2}, fromDate = '{from}', toDate = '{to}', total = '{total}' WHERE truck_vin = '{truck[0]}'");
            }

            DB.Close();
        }

        internal static void UpdateStand(DataRow truck, int emp1ID, int emp2ID, int emp3ID, bool includeDuration, DateTime from, DateTime to, string total)
        {
            DB.Open();
            if (includeDuration)
            {
                var stand = DB.Get($"SELECT * FROM stand WHERE truck_vin = '{truck[0]}'").Rows[0];
                var strTotal = NewTotalHours(truck[1].ToString(), total);
                var standTotal = NewTotalHours(stand[6].ToString(), total);

                DB.Run($"UPDATE truck SET total_hours = '{strTotal}' WHERE vinNo = '{truck[0]}'");

                DB.Run($"UPDATE stand SET emp1 = {emp1ID}, emp2 = {emp2ID}, emp3 = {emp3ID}," +
                    $"fromDate = '{from}', toDate = '{to}', total = '{standTotal}' WHERE truck_vin = '{truck[0]}'");
            }
            else
            {
                DB.Run($"UPDATE truck SET total_hours = '{total}' WHERE vinNo = '{truck[0]}'");
                DB.Run($"UPDATE stand SET emp1 = {emp1ID}, emp2 = {emp2ID}, emp3 = {emp3ID}, fromDate = '{from}', toDate = '{to}', total = '{total}' WHERE truck_vin = '{truck[0]}'");
            }

            DB.Close();
        }

        private static string NewTotalHours(string oldHours, string newHours)
        {
            TimeSpan timeSpan = TimeSpan.Parse(oldHours) + TimeSpan.Parse(newHours);
            var strTotal = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}";
            return strTotal;
        }

    }
}
