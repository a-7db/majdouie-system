using System.Data;
using System.Data.OleDb;

namespace majdoee.app
{
    public static class DB
    {
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb";
        private static OleDbConnection connect = new OleDbConnection(connectionString);
        private static OleDbCommand cmd = new OleDbCommand("", connect);

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
            var table = new DataTable();
            cmd.CommandText = query;
            table.Load(cmd.ExecuteReader());

            return table;
        }

        public static void Run(string query)
        {
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

        }
       
    }
}
