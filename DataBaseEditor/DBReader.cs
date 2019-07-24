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
        private SqlConnection dbConnection;
        private QueryData queryData;

        public DBReader(SqlConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            queryData = new QueryData();
        }

        public QueryData readFromDB(string sqlQuery)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, dbConnection);
                dbConnection.Open();                                        //System.InvalidOperationException: „Właściwość ConnectionString nie została zainicjowana.”
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

            dbConnection.Close();
            return queryData;
        }
    }
}
