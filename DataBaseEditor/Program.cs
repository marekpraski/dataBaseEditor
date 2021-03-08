using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DataBaseEditor
{
    static class Program
    {
        public static string mainPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.ToString();
        public static int port1 = 100;  //porty do komunikacji TPC z add-inem
        public static int port2 = 101;
        public static bool isAddinConnected = false;    //czy jest połączenie TCP pomiędzy Ewid a addinem
        /// <summary>
        /// Do rozpoznania jaka aplikacja graficzna ma być uruchomiona
        /// </summary>
        public static string aplikacjaCAD;

        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ProgramSettings.userName = args[0];
            ProgramSettings.userPassword = args[1];
            readConfigFile(Application.StartupPath + ProgramSettings.configFilePath + @"\" + ProgramSettings.configFileName);
            Application.Run(new DBEditorMainForm());
        }

        private static void readConfigFile(string configFilePath)
        {
            try
            {
                FileManipulator fm = new FileManipulator();
                string t = fm.readFile(configFilePath);
                XElement parameters = XElement.Parse(t);
                port1 = Convert.ToInt32(parameters.Element("port1").Value);
                port2 = Convert.ToInt32(parameters.Element("port2").Value);
                aplikacjaCAD = getAppFromConfigFile(parameters.Element("aplikacjaCAD").Value);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        private static string getAppFromConfigFile(string s)
        {
            if (s.Contains("station"))
               return "ustation";
            else if (s.Contains("Map"))
                return "MapStandalone";
            return "";
        }
    }
}
