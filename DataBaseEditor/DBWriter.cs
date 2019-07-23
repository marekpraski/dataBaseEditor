using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataBaseEditor
{
    class DBWriter
    {
        private string connectionString;
        public DBWriter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void writeToDB(string sqlQuery)
        {
            List<string> queries = new List<string>();
            queries.Add(sqlQuery);
            writeToDB(queries);
        }    
        
        public void writeToDB(List<string> queries)
        {
            SqlConnection dbconnection = new SqlConnection(connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            dbconnection.Open();
            foreach (string query in queries)
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, dbconnection);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    MyMessageBox.display(e.Message, MessageBoxType.Error);
                }
            }
            dbconnection.Close();
        }
    }
}
