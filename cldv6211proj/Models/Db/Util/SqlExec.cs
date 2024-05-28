using System.Data.SqlClient;

namespace cldv6211proj.Models.Db.Util
{
    public static class SqlExec
    {
        private const string CON_STR =
            "Server=tcp:cldv6211proj-dbserver.database.windows.net,1433;Initial Catalog=cldv6211proj-db;Persist Security Info=False;User ID=sky;Password=HmmS3cur1ty?!1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private const bool DEBUG_PRINT = true;

        private static Action<string> Dbg => DEBUG_PRINT ? Console.WriteLine : (_) => { };

        public static int NonQuery(string cmdText, bool exec = true)
        {
            Dbg(cmdText);
            if (!exec)
                return 0;
            using var con = new SqlConnection(CON_STR);
            using var cmd = new SqlCommand(cmdText, con);
            con.Open();
            return cmd.ExecuteNonQuery();
        }

        public static int Reader(
            string cmdText,
            Action<SqlDataReader> action,
            int maxReads = int.MaxValue,
            bool exec = true
        )
        {
            Dbg(cmdText);
            if (!exec)
                return 0;
            using var con = new SqlConnection(CON_STR);
            using var cmd = new SqlCommand(cmdText, con);
            con.Open();
            try
            {
                using var reader = cmd.ExecuteReader();
                int readCount = 0;
                while (reader.Read() && maxReads > readCount++)
                    action(reader);
                return readCount;
            }
            catch (SqlException e)
            {
                if (e.Message.Contains("Invalid object name"))
                    Console.WriteLine(
                        $"\nSqlException: {e.Message}\nHas it been created yet?\n\n\n"
                    );
            }
            return 0;
        }
    }
}
