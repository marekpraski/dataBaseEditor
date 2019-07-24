using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DataBaseEditor
{
    //służy do tworzenia połaczenia do bazy danych na podstawie elementów składowych, tj. nazwa serwera, nazwa bazy, użytkownik i hasło
    //testuje wygenerowane połaczenie i przekazuje jako obiekt 

    public enum ConnectionTypes { sqlAuthorisation, windowsAuthorisation }
    public enum ConnectionSources { wholeConnectionInFile, serverNameInFile }

   
    public class DBConnector
    {
        private SqlConnection dbConnection;
        private string sqlQuery;
        private string dbName;
        private string configFilePath;
        string configFileText;
        string dbConnectionString;
        private string serverName;
        private bool configFileValidated = true;
        private bool configFileValidationWasDone = false;       //jest to bezpiecznik, gdybym w programie analizę pliku konfiguracyjnego dał zanim plik został zwalidowany, bo z metody analizującej ściągnąłem wszystkie zabezpieczenia

        public DBConnector ()
        {
        }

        private void generateConnection()
        {
            
        }
        private void testConnection()
        {

        }

        public string getDBName(ref string sqlQuery)
        {
            this.sqlQuery = sqlQuery;
            TextManipulator tm = new TextManipulator();
            dbName = extractDBName(ref tm);
            return dbName;
        }

        //wyciąga nazwę db z kwerendy wpisanej przez użytkownika
        private string extractDBName(ref TextManipulator tm)
        {
            //znajduję położenie wyrazu kluczowego "from" w kwerendzie
            List<int> keyWordFromPosition = tm.getSubstringStartPositions(sqlQuery, "from");
            try
            {
                //wywala bład gdy kwerenda jest na tyle bezsensowna, że nie potrafi wyłuskać sensownego wyrazu, który mógłby być nazwą bazy danych
                string textAfterFrom = sqlQuery.Substring(keyWordFromPosition[0] + 5);  //dodaję długość wyrazu from i jedną spację
                int firstSpacePosition = textAfterFrom.IndexOf(" ");
                if (firstSpacePosition == -1)   //brak spacji
                {
                    dbName = textAfterFrom;
                }
                else
                {
                    dbName = textAfterFrom.Substring(0, firstSpacePosition);
                }

                return dbName;
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                MyMessageBox.display("Błąd w kwerendzie", MessageBoxType.Error);
            }
            return dbName;
        }

        public ref SqlConnection getDBConnection (ConnectionSources source, ConnectionTypes type)
        {
                switch (source)
                {
                    case ConnectionSources.wholeConnectionInFile:
                        readConnStringFromFile(ProgramSettings.connectionStringDelimiter);
                        break;
                    case ConnectionSources.serverNameInFile:
                        getServerNameFromFile(ProgramSettings.connectionStringDelimiter);
                        generateConnection();
                        break;
                }
            dbConnection = new SqlConnection(dbConnectionString);
            return ref dbConnection;
        }

        private void getServerNameFromFile(string delimiter)
        {
            serverName = "";
        }

        public bool validateConfigFile()
        {
            string currentPath = Application.StartupPath;       //katalog z którego uruchamiany jest program
            if (ProgramSettings.configFilePath.Equals(""))      //nie zdefiniowano alternatywnej ścieżki dla pliku konfiguracyjnego
            {
                configFilePath = currentPath;                      //plik konfiguracyjny jest w tym samym katalogu co program
            }
            else
            {
                configFilePath = ProgramSettings.configFilePath;
            }

            FileManipulator fm = new FileManipulator();
            string configFile = configFilePath + @"\" + ProgramSettings.configFileName;
            configFileText = fm.readFile(configFile);
            if (!configFileText.Equals(""))                     //plik konfiguracyjny istnieje i nie jest pusty 
            {
                TextManipulator tm = new TextManipulator();
                List<int> indexes = tm.getSubstringStartPositions(configFileText, ProgramSettings.connectionStringDelimiter);

                //jeżeli w pliku jest błąd i jest za dużo lub za mało znaczników
                if (indexes.Count != 2)
                {
                    MyMessageBox.display("błąd pliku konfiguracyjnego " + configFile + " dla znacznika " + ProgramSettings.connectionStringDelimiter, MessageBoxType.Error);
                    configFileValidated= false;
                }
            }
            else
            {
                configFileValidated = false;       //plik jest pusty lub go nie ma
            }
            configFileValidationWasDone = true;
            return configFileValidated;             //domyślnie jest true
        }

        //z pliku tekstowego wyciąga połączenie do serwera na podstawie znacznika "delimiter"
        private void readConnStringFromFile(string delimiter)
        {
            if (configFileValidationWasDone)
            {
                TextManipulator tm = new TextManipulator();
                List<int> indexes = tm.getSubstringStartPositions(configFileText, delimiter);
                int startIndex = indexes[0] + delimiter.Length + 1;         //kompensuję na > po znaczniku
                int connStringLength = indexes[1] - startIndex - 2;         //kompensuję na </ przed znacznikiem
                dbConnectionString = configFileText.Substring(startIndex, connStringLength);
            }
            else
            {
                dbConnectionString = "";
            }
        }
    }
}
