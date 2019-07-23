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
        string configFileName = "dbEditorConf.xml";
        string connectionStringDelimiter = "server";
        string configFilePath;

        public static string getDBConnectionStringFromFile()
        {
            ProgramSettings ps = new ProgramSettings();
            string currentPath = Application.StartupPath;       //katalog z którego uruchamiany jest program
            ps.configFilePath = currentPath;                   //plik konfiguracyjny jest w tym samym katalogu co program

            return ps.readConnStringFromFile(ref ps, ps.connectionStringDelimiter);
        }

       

        //z pliku tekstowego wyciąga połączenie do serwera na podstawie znacznika "delimiter"
        private string readConnStringFromFile(ref ProgramSettings ps, string delimiter)
        {
            FileManipulator fm = new FileManipulator();
            string configFile = ps.configFilePath + @"\" + ps.configFileName;
            string configFileText = fm.readFile(configFile);

            if (!configFileText.Equals("")) //plik konfiguracyjny istnieje i nie jest pusty 
            {
                TextManipulator tm = new TextManipulator();
                List<int> indexes = tm.getSubstringStartPositions(configFileText, delimiter);
                //plik jest poprawny jeżeli w pliku są dokładnie dwa znaczniki
                if (indexes.Count == 2)
                {
                    int startIndex = indexes[0] + delimiter.Length + 1;   //kompensuję na > po znaczniku
                    int connStringLength = indexes[1] - startIndex - 2;         //kompensuję na </ przed znacznikiem
                    return configFileText.Substring(startIndex, connStringLength);
                }
                else
                //jeżeli w pliku jest błąd i jest za dużo lub za mało znaczników
                {
                    MyMessageBox.display("błąd pliku konfiguracyjnego " + configFile + " dla znacznika " + delimiter, MessageBoxType.Error);
                    return "";
                }
            }
            return "";
        }
    }
}
