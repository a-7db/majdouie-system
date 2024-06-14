using majdoee.db;
using System.Globalization;

namespace majdoee.testing
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class Program
    {
        static void Main(string[] args)
        {
            //insertTest();
            //updateTest();
            //deleteTest();
            //fetchTest();

            Console.WriteLine("Enter the second time (hh:mm tt): ");
            string timeStr1 = "06:30 AM";
            string timeStr2 = "04:30 PM";

            // Parse the input times
            DateTime time1 = ParseTime(timeStr1);
            DateTime time2 = ParseTime(timeStr2);

            // Calculate the difference
            TimeSpan difference = time1 > time2 ? time1 - time2 : time2 - time1;

            // Display the result
            Console.WriteLine($"The difference is {difference.Hours} hours and {difference.Minutes} minutes.");
        }

        public static DateTime ParseTime(string timeStr)
        {
            if (DateTime.TryParseExact(timeStr, "hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            {
                return time;
            }
            else
            {
                throw new FormatException("Invalid time format. Please use hh:mm tt.");
            }
        }


        private static void deleteTest()
        {
            DB.Run("DELETE FROM employee WHERE empID = 3");
        }

        private static void updateTest()
        {
            DB.Run("UPDATE employee SET emp_name = 'Nasser' WHERE empID = 3");
        }

        private static void insertTest()
        {
            DB.Run("INSERT INTO employee "+ DB.empCols +" VALUES('Ali')");
        }

        private static void fetchTest()
        {
            var tracks = DB.Get("SELECT * FROM employee");

            for (int i = 0; i < tracks.Rows.Count; i++)
            {
                Console.WriteLine($"{tracks.Rows[i][0]}]- {tracks.Rows[i][1]}");
            }
        }
    }
}
