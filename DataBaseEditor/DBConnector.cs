using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataBaseEditor
{
    //służy do tworzenia połaczenia do bazy danych na podstawie elementów składowych, tj. nazwa serwera, nazwa bazy, użytkownik i hasło
    //testuje wygenerowane połaczenie i przekazuje jako obiekt 
    public class DBConnector
    {
        private SqlConnection dbconnection { get; }
        private string sqlQuery;

        public DBConnector (ref string sqlQuery)
        {
            this.sqlQuery = sqlQuery;
        }

        private void generateConnection()
        {
            TextManipulator tm = new TextManipulator();
          //  string dbName = getDBName(ref tm);
        }
    }
}
