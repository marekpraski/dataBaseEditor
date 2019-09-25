using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseEditor
{
    public class ProgramSettings
    {
        //public static string rezerwerConnection = @"Data Source=laptop08\sqlexpress;Initial Catalog=dbrezerwer_test;User ID=marek;Password=root";
        //ustawienia pliku konfiguracyjnego
        public static string configFileName = "dbEditorConf.xml";
        public static string connectionStringDelimiter = "server";
        public static string databaseNameDelimiter = "db_name";
        public static string configFilePath = @"";
        public static string userName = "";
        public static string userPassword = "";

        public static int numberOfRowsToLoad = 50;     //ile wierszy ładuje się do datagrida w jednej operacji
    }
}
