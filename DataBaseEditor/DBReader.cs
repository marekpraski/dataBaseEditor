using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataBaseEditor
{
    class DBReader
    {
        
        private string connectionString;
        private QueryData queryData;
        public DBReader(string connectionString)
        {
            this.connectionString = connectionString;
            queryData = new QueryData();
        }
        public QueryData readFromDB(string sqlQuery)
        {
            SqlConnection dbconnection = new SqlConnection(connectionString);

            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, dbconnection);
                dbconnection.Open();
                SqlDataReader sqlReader = sqlCommand.ExecuteReader();

                int numberOfColumns = sqlReader.FieldCount;

                while (sqlReader.Read())
                {
                    object[] rowData = new object[numberOfColumns];
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        rowData[i] = sqlReader.GetValue(i).ToString();
                    }
                    queryData.addQueryData(rowData);
                }

                for (int i = 0; i < sqlReader.FieldCount; i++)
                {
                    queryData.addHeader(sqlReader.GetName(i));
                    queryData.addDataType(sqlReader.GetDataTypeName(i));
                }
                sqlReader.Close();
                sqlCommand.Dispose();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                MyMessageBox.display(e.Message, MessageBoxType.Error);
            }

            // try-catch dotąd
            dbconnection.Close();
            return queryData;
        }
    }
}
