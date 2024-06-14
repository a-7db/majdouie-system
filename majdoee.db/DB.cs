using System.Data;
using System.Data.OleDb;

namespace majdoee.db
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class DB
    {
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Programming\majdoee\majdoee.db\bin\x86\Debug\net8.0\Database.accdb";
        private static OleDbConnection connect = new OleDbConnection(connectionString);
        private static OleDbCommand cmd = new OleDbCommand("", connect);

        public readonly static string empCols = "(emp_name)";
        public readonly static string truckCols = "(emp_name)";



        public static void Open()
        {
            if(connect.State == ConnectionState.Closed)
                connect.Open();
        }

        public static void Close()
        {
            if (connect.State == ConnectionState.Open)
                connect.Close();
        }
        public static DataTable Get(string query)
        {
            Open();

            var table = new DataTable();
            cmd.CommandText = query;
            table.Load(cmd.ExecuteReader());

            Close();

            return table;
        }

        public static void Run(string query)
        {
            Open();

            cmd.CommandText= query;
            cmd.ExecuteNonQuery();

            Close();
        }
       
    }
}
