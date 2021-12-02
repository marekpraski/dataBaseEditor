using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using DatabaseInterface;
using System.Windows.Forms;
using System.Data.SqlClient;
using MapInterface;
using System.Diagnostics;
using MapInterfaceObjects;
using InterProcessCommunication;
using System.Xml.Linq;
using System.IO;
using UtilityTools;

namespace DataBaseEditor
{
    public partial class DBEditorMainForm : Form
    {
        private enum ApplicationType { insert, update}

        private ApplicationType appType = ApplicationType.update;       //ustawić odpowiednio dla kompilacji dla PRGW (insert) lub Bogdanka (update)
        private UtilityTools.NumberHandler numberHandler = new UtilityTools.NumberHandler();
        private DataGridHandler dg1Handler;
        private FormFormatter formatter;
        private DataGridCell changedCell;
        private DBConnector connector;
        private bool configFileValidated;
        private string sqlQueryForDisplayInDatagrid;
        private QueryData queryData;
        private SqlConnection dbConnection;
        private string tableName = "";
        private readonly string displayLevelNameUpdate = "linieCentralneMapaObiektowa";

        private List<object[]> dbData;

        private int datagridRowIndex = 0;
        private int rowsLoaded = 0;

        private IPCReceiver ipcReceiver;
        private IPCSender ipcSender;

        public DBEditorMainForm()
        {
            InitializeComponent();
            connector = new DBConnector(ProgramSettings.userName, ProgramSettings.userPassword);
            configFileValidated = connector.validateConfigFile(Application.StartupPath);
            label2.Visible = !configFileValidated;
            dg1Handler = new DataGridHandler();  //każdy datagrid musi mieć swoją instancję DataGridHandlera
            formatter = new FormFormatter();
            initialSettings();
            populateComboKolor();
        }

        #region zdarzenia podczas uruchamiania i zamykania okna
        private void initialSettings()
        {
            if(this.appType == ApplicationType.insert)
            {
                tbSqlQuery.Text = getQueryFromTxtFile();
                tsWyswietlOryginalne.Visible = false;
                labelZatwierdzone.Visible = false;
                cbOryginalneCzyZmienione.Visible = false;
                cbZatwierdzone.Visible = false;
                tbLike.Enabled = false;
                cbFituj.Visible = false;
                cbKolor.Visible = false;
            }
            else if (this.appType == ApplicationType.update)
            {
                //TODO kwerendę zmienić po zmianie bazy danych i struktury
                tbSqlQuery.Text = @"Select WyrobiskaLinieCentralne.idLinieCentralne, MaincoalWyrobiska.id_wyrobiska,cast(WyrobiskaLinieCentralne.wyrobiskoNumerExcel as int) wyrobiskoNumerExcel, 
                                    MaincoalWyrobiska.nazwaWyrobiska,WyrobiskaLinieCentralne.odcinekNumer,WyrobiskaLinieCentralne.odcinekMetrStart, WyrobiskaLinieCentralne.odcinekMetrKoniec, 
                                    WyrobiskaLinieCentralne.wyrobiskoDlugoscExcel, MaincoalWyrobiska.rodzajWyrobiska, MaincoalWyrobiska.id_poziomu, MaincoalWyrobiska.id_pokladu, WyrobiskaLinieCentralne.zatwierdzone
	                                 from WyrobiskaLinieCentralne
                                    inner join MaincoalWyrobiska on WyrobiskaLinieCentralne.id_wyrobiska = MaincoalWyrobiska.id_wyrobiska 
                                    where WyrobiskaLinieCentralne.zatwierdzone =  ";
                tbSqlQuery.ReadOnly = true;
                cbZatwierdzone.SelectedIndex = 0;
                cbOryginalneCzyZmienione.SelectedIndex = 0;
                undoButton.Visible = true;
                saveButton.Visible = true;
                dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                label4.Visible = false;
                tbOrderBy.Visible = false;
            }                
            sqlQueryTextBox_TextChangedEvent(null, null);
        }

        private void populateComboKolor()
        {
            ComboboxItem[] comboItems = new ComboboxItem[8];
            comboItems[0] = new ComboboxItem("czerwony", -16776961);
            comboItems[1] = new ComboboxItem("zielony", -16711936);
            comboItems[2] = new ComboboxItem("żółty", -16711681);
            comboItems[3] = new ComboboxItem("jasnoniebieski", -256);
            comboItems[4] = new ComboboxItem("ciemny niebieski", -65536);
            comboItems[5] = new ComboboxItem("różowy", -65281);
            comboItems[6] = new ComboboxItem("brąz-pomarańcza", -16744193);
            comboItems[7] = new ComboboxItem("fioletowy", -65408);

            cbKolor.ComboBox.DataSource = comboItems;
            cbKolor.ComboBox.DisplayMember = "displayText";
            cbKolor.ComboBox.ValueMember = "value";
            cbKolor.SelectedIndex = 0;
        }

        private void DBEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MapTools mt = new MapTools(Program.platformaGraficzna);
            mt.unloadAddin();
            if(this.appType == ApplicationType.insert)
                saveQueryToTxtFile();
        }

        #endregion

        #region zapisywanie kwerendy sql do pliku tekstowego i czytanie z pliku

        private string getQueryFromTxtFile()
        {
            string txt = "";
            try
            {
                txt = File.ReadAllText("kwerenda.txt");
            }
            catch (Exception e)
            {
                //TODO kwerendę zmienić po zmianie bazy danych i struktury
                MessageBox.Show("błąd odczytu ustawień z pliku kwerenda.txt " + e.Message + e.StackTrace);
                txt = @"SELECT * FROM MaincoalWyrobiska  where id_wyrobiska not in (select id_wyrobiska from WyrobiskaLinieCentralne)";
            }
            return txt;
        }

        private void saveQueryToTxtFile()
        {
            string textToSave = tbSqlQuery.Text;
            string fileName = "kwerenda.txt";
            try
            {
                File.WriteAllText(fileName, textToSave);
            }
            catch (Exception e)
            {
                MessageBox.Show("błąd zapisu ustawień do pliku  " + fileName + "   " + e.Message + e.StackTrace);
            }
        }


        #endregion

        #region zdarzenia na interakcję z użytkownikiem

        private void cbZatwierdzone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbZatwierdzone.SelectedIndex == 0)
            {
                cbOryginalneCzyZmienione.SelectedIndex = 0;
                cbOryginalneCzyZmienione.Enabled = false;
            }
            else
            {
                cbOryginalneCzyZmienione.SelectedIndex = 1;
                cbOryginalneCzyZmienione.Enabled = true;
            }
        }

        //button, którego kliknięcie wypełnia danymi z kwerendy główny datagrid
        //jest to pierwszy przycisk, który użytkownik może nacisnąć po wpisaniu kwerendy w pole tekstowe
        private void btnWyswietl_Click(object sender, EventArgs e)
        {
            //przekazuję kwerendę do DBConnectora w celu utworzenia połaczenia, wyciągam od razu nazwę bazy danych, jest potrzebna później
            this.sqlQueryForDisplayInDatagrid = constructQueryForDisplayInDatagrid();

            if (configFileValidated)
            {
                //tbSqlQuery.Text = this.sqlQuery;      
                
                //sql nie widzi różnicy pomiędzy lower i upper case a ma to znaczenie przy wyszukiwaniu słow kluczowych w kwerendzie
                tableName = connector.getTableNameFromQuery(this.sqlQueryForDisplayInDatagrid);
                dbConnection = connector.getDBConnection(ConnectionDataSource.serverAndDatabaseNamesInFile, ConnectionTypes.sqlAuthorisation);

                if (dbConnection == null)
                    return;
                if (dg1Handler.checkChangesExist())
                {
                    if (MyMessageBox.display("Czy zapisać zmiany?", MessageBoxType.YesNo) == MessageBoxResults.Yes)
                    {
                        //zaimplementować 
                    }
                }
                else
                {
                    dg1Handler.Dispose();               //likwiduję starą instancję utworzoną w konstruktorze, bo jest to de facto wyświetlenie od zera i operacje na datagridzie od zera
                    dg1Handler = new DataGridHandler();  //każdy datagrid musi mieć swoją instancję DataGridHandlera
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                    datagridRowIndex = 0;
                    loadNextButton.Visible = false;
                    setUpDatagrid();
                }
            }
            tbLike.Text = "";   //jeżeli nie wyczyszczę, to przy próbie wyświetlenia na mapie wyrobisk zaznaczonych w datagridzie tworzy kwerendę, która nie przechodzi i wywala błąd
            if (this.appType == ApplicationType.update)
                dataGridView1.Columns[0].Visible = false;
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            //DataGridCell recoveredCell = new DataGridCell();
            DataGridCell recoveredCell = dg1Handler.getLastCellChangedAndUndoChanges();

            object oldCellValue = recoveredCell.getCellValue(cellValueTypes.oldValue);
            int rowIndex = recoveredCell.getCellIndex(cellIndexTypes.rowIndex);
            int columnIndex = recoveredCell.getCellIndex(cellIndexTypes.columnIndex);
            dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = oldCellValue;

            changeCellTextColour(recoveredCell, Color.Black);

            if (!dg1Handler.checkChangesExist())
            {
                undoButton.Enabled = false;
                saveButton.Enabled = false;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DBWriter writer = new DBWriter(dbConnection);
            string query;
            while (dg1Handler.checkChangesExist())
            {
                DataGridCell cell = dg1Handler.getLastCellChangedAndUndoChanges();
                query = generateUpdateQuery(tableName, cell);
                writer.executeQuery(query);
                changeCellTextColour(cell, Color.Black);
            }
            //blokuję przyciski zapisu i cofania, bo po zapisaniu zmian już nie ma czego zapisać ani cofnąć
            undoButton.Enabled = false;
            saveButton.Enabled = false;
        }


        private void LoadNexButton_Click(object sender, EventArgs e)
        {
            if (dbData != null && datagridRowIndex <= dbData.Count)
            {
                loadRowPacket();
            }
        }


        private void PomocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pomoc = @"        (1) wpisać kwerendę; pierwsze pole musi być id_wyrobiska
        (2) wyświetlić dane w datagridzie
        (3) uruchomić moduł graficzny
        (4) zaznaczyć żądany wiersz w datagridzie
        (5) zaznaczyć linię centralną na mapie i nacisnąć Przypisz
        (6) na mapie wyświetlana jest wczytana linia, zaznaczony wiersz znika z datagrida
        (7) opcje off-setów i zmiany kierunku są wyłączone
        (8) przycisk informacja otwiera okno informacji i wyświetla info o wyrobisku wczytanym do bazy (trzeba zaznaczyć żądaną zieloną linię)";

            if (this.appType == ApplicationType.update)
                pomoc = @"        (1) pole kwerendy jest już wypełnione, nie można edytować, można wpisać warunek nazwy wyrobiska
        (2) wyświetlić dane w datagridzie przyciskiem „Wyświetl” po prawej
        (3) uruchomić moduł graficzny przyciskiem na pasku górnym
        (4) wyświetlić wyrobiska na mapie (wyświetla te, które są widoczne w datagridzie) przyciskiem na pasku górnym; w zależności od wyboru opcji „zatwierdzone”,  wyświetlane są linie zmodyfikowane bądź oryginalne
        (5) zaznaczyć żądaną linię wyświetloną z bazy danych, nacisnąć przycisk Informacja, co wyświetla nazwę wyrobiska i id
        (6) zatwierdzić wybór klikając ppm w Microstation, co pokazuje kierunek przebiegu wyrobiska
        (7) wpisać żądane wartości offsetów; wartości offsetów są ograniczone do +-10 metrów, pozostawienie pól pustych oznacza 0.
        (8) w razie potrzeby odwrócić kierunek biegu linii; UWAGA: offsety liczone są w stosunku do oryginalnych punktów początku i końca!
        (9) kliknąć Zatwierdź
        (10) na mapie wyświetlana jest na zielono zatwierdzona linia; jeżeli wpisane były jakieś offsety, długość linii jest różna od linii oryginalnej; 0 oznacza start a 1 koniec linii
        (11) z datagrida znika wyrobisko zatwierdzone; uzyskuje ono status zatwierdzone=1, więc w razie potrzeby można je ponownie wczytać do datagrida wybierając 1 z listy wyboru w warunku w oknie głównym
        (12) jeżeli wyrobisko jest źle przypisane (tj. ta linia centralna nie odpowiada wyrobisku o tej nazwie) można je usunąć z bazy danych przyciskiem Usuń";

            MyMessageBox.display(pomoc, MessageBoxType.Information);
        }


        //tj użytkownik wpisał kwerendę w polu tekstowym
        private void sqlQueryTextBox_TextChangedEvent(object sender, EventArgs e)
        {
            if (tbSqlQuery.Text != "") // && dbConnection != null)
            {
                btnWyswietl.Enabled = true;
            }
            else
            {
                btnWyswietl.Enabled = false;
            }
        }


        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            changedCell = new DataGridCell();
            object oldCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            changedCell.setCellValue(cellValueTypes.oldValue, oldCellValue);
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object newCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            //changedCell została utworzona gdy użytkownik rozpoczął edycję, metoda dataGridView1_CellBeginEdit
            changedCell.setCellValue(cellValueTypes.newValue, newCellValue);

            changedCell.setCellIndex(cellIndexTypes.rowIndex, e.RowIndex);
            changedCell.setCellIndex(cellIndexTypes.columnIndex, e.ColumnIndex);

            changedCell.DataTypeName = (queryData.getDataTypes()[e.ColumnIndex]);

            if (dg1Handler.addChangedCell(changedCell))
            {
                changeCellTextColour(changedCell, Color.Red);
                undoButton.Enabled = true;
                saveButton.Enabled = true;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = changedCell.getCellValue(cellValueTypes.oldValue);
            }
        }

        #endregion

        #region tworzenie kwerendy do wyświetlania danych w datagridzie

        private string constructQueryForDisplayInDatagrid()
        {
            string queryPart1 = constructMainQueryBody();
            string queryPart2 = constructQueryConditionSection(queryPart1);
            string queryPart3 = constructOrderByQuerySection();
            return queryPart1 + queryPart2 + queryPart3;
        }

        private string constructMainQueryBody()
        {
            string query = tbSqlQuery.Text;
            if (this.appType == ApplicationType.update)
                query += cbZatwierdzone.Text;

            return query;
        }

        private string constructQueryConditionSection(string queryPart1)
        {
            if (this.appType == ApplicationType.insert)
                return "";
            if (!String.IsNullOrEmpty(tbLike.Text))
            {
                if (queryPart1.ToLower().Contains("where"))
                    return " and MaincoalWyrobiska.nazwaWyrobiska like '%" + tbLike.Text + "%' ";
                else
                    return " where MaincoalWyrobiska.nazwaWyrobiska like '%" + tbLike.Text + "%' ";
            }
            return "";
        }

        private string constructOrderByQuerySection()
        {
            if (this.appType == ApplicationType.insert)
            {
                if (!String.IsNullOrEmpty(tbOrderBy.Text))
                    return " order by " + tbOrderBy.Text;
            }
            if (this.appType == ApplicationType.update)
                return " order by wyrobiskoNumerExcel";
            return "";
        }

        #endregion

        #region metody do edycji i zapisu danych w datagridzie

        private void setUpDatagrid()
        {

            //pierwsza kolumna nie jest do edycji, w niej musi być primaryKey;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;

            DBReader reader = new DBReader(dbConnection);
            queryData = reader.readFromDB(this.sqlQueryForDisplayInDatagrid);
            if (queryData == null)
                return;

            //jeżeli kwerenda błędna to nie zwróci wyników
            //przypadki błędnej kwerendy obsługiwane są przez DBReader
            if (queryData.getHeaders().Count != 0)
            {
                dbData = queryData.getQueryData();
                List<string> columnHeaders = queryData.getHeaders();

                //dopasowuję formatkę do wyników nowej kwerendy

                changeMainFormLayout(columnHeaders.Count, ref dataGridView1);

                //nazywam nagłówki
                for (int i = 0; i < queryData.getHeaders().Count; i++)
                {
                    dataGridView1.Columns[i].HeaderText = columnHeaders[i];
                    dataGridView1.Columns[i].Name = columnHeaders[i];
                }
            }
            if (dbData != null)
            {
                loadRowPacket();

                if (rowsLoaded < dbData.Count)
                {
                    int rowsRemaining = dbData.Count - rowsLoaded;

                    loadNextButton.Visible = true;
                    loadNextButton.Enabled = true;
                    remainingRowsLabel.Visible = true;
                    remainingRowsLabel.Text = "zostało " + rowsRemaining;

                    if (rowsRemaining > ProgramSettings.numberOfRowsToLoad)
                    {
                        loadNextButton.Text = "+" + ProgramSettings.numberOfRowsToLoad;
                        
                    }
                }
            }
        }

        private void loadRowPacket()
        {
            for (int i = datagridRowIndex; i< ProgramSettings.numberOfRowsToLoad + rowsLoaded; i++)
            {
                if (i < dbData.Count)
                {
                    object[] row = dbData[i];
                    dataGridView1.Rows.Add(row);
                    object primaryKey = row[0];
                    dg1Handler.addDataGridIndex(i, primaryKey);
                }
            }


            if (dataGridView1.AllowUserToAddRows)
            {
                datagridRowIndex = dataGridView1.Rows.Count - 1;   //gdy użytkownik ma możliwość dodawania wierszy w datagridzie, datagrid posiada dodatkowo jeden pusty wiersz na końcu
            }
            else
            {
                datagridRowIndex = dataGridView1.Rows.Count;
            }
            rowsLoaded = datagridRowIndex;
            int rowsRemaining = dbData.Count - rowsLoaded;

            if ((rowsRemaining) < ProgramSettings.numberOfRowsToLoad)
            {
                loadNextButton.Text = "+" + rowsRemaining;
            }

            if (rowsLoaded == dbData.Count)
            {
                loadNextButton.Enabled = false;
                loadNextButton.Text = "+0";
                remainingRowsLabel.Visible = false;
            }
            else
            {
                remainingRowsLabel.Text = "zostało " + rowsRemaining;
            }
        }

        //przyjmuje liczbę nagłówków z kwerendy oraz datagrid, w którym trzeba dopasować liczbę kolumn
        private void changeMainFormLayout(int numberOfHeaders, ref DataGridView dataGrid)
        {
            List<int> colWidths = queryData.getColumnWidths(dataGrid.Font,30);         //szerokości kolummn datagridu z danych z kwerendy
            formatter.formatDatagrid(ref dataGrid, numberOfHeaders, colWidths);
            formatter.changeDisplayButtonLocation(ref btnWyswietl);
            formatter.changeSaveButtonLocation(ref saveButton);
            formatter.changeUndoButtonLocation(ref undoButton);
            formatter.changeLoadNextButtonLocation(loadNextButton);
            formatter.changeRemainingRowsLabelLocation(remainingRowsLabel);
            formatter.setTextboxSize(ref tbSqlQuery);
            this.Width = formatter.calculateFormWidth();
        }

 
        private string generateUpdateQuery(string tableName, DataGridCell cell)
        {
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            CellConverter cellConverter = new CellConverter();
            string columnName = queryData.getHeaders()[columnIndex];
            string primaryKeyColumnName = queryData.getHeaders()[0];    //kluczem głównym MUSI być pierwsza kolumna
            object primaryKey = dg1Handler.getCellPrimaryKey(cell);
            string newValue = cellConverter.getConvertedValue(ref cell);
            if (newValue == null)
            {
                return "update " + tableName + " set " + columnName + "= null" + " where " + primaryKeyColumnName + "='" + primaryKey.ToString() + "'";
            }
            return "update " + tableName + " set " + columnName + "=" + cellConverter.getConvertedValue(ref cell) + " where " + primaryKeyColumnName + "='" + primaryKey.ToString() + "'"; ;
        }


        private void changeCellTextColour(DataGridCell cell, Color colour)
        {
            int rowIndex = cell.getCellIndex(cellIndexTypes.rowIndex);
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            dataGridView1.Rows[rowIndex].Cells[columnIndex].Style.ForeColor = colour;
        }
        #endregion

        #region start add-ina, obsługa komunikacji z modułem add-in, w tym metody wykonujące konkretne zadania na konkretne żądania ze stronu add-in
        /**
         * przebieg procesu uruchamiania add-ina i ustanawiania połączenia przez TCP z programem głównym:
         * 1. program główny za pośrednictwem MapTools wywołuje key-in "mdl load BentleyAddin,,BIDomain " + port1 + " " + port2;
         * 2. add-in wykorzystuje przesłane porty do ustanowienia serwera i klienta TCP, wysyła do programu głównego żądanie przesłania danych logowania do bazy danych
         * 3. dopiero po otrzymaniu żądania program główny uruchamia klienta TCP a następnie przesyła dane logowania
         * 4. add-in ustanawia połączenie do bazy i wysyła do programu głównego potwierdzenie, że jest w pełni funkcjonalny
         * 5. program główny uruchamia pasek narzędziowy add-ina wywołując właściwy key-in
         **/
        private void mapaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ipcReceiver = IPCReceiver.getInstance();
            if (this.ipcReceiver.getDataReceivedEventInvocationList() == 0)
            {    //inaczej zdarzenie dodane jest przy każdym kliknięciu
                this.ipcReceiver.dataReceived += onDataReceived;
            }
            startAddin();
        }

        private void startAddin()
        {
            Process[] proc = Process.GetProcessesByName(Program.aplikacjaCAD);

            if (proc.Length == 0)
            {
                MessageBox.Show("Należy włączyć Microstation");
            }
            else
            {
                MapTools mt = new MapTools(Program.platformaGraficzna);
                if (!Program.isAddinConnected)
                    mt.startAddin(Program.port1, Program.port2);
                else
                    startMaincoalTools(mt);
            }
        }


        private void onDataReceived(object sender, IPCEventArgs args)
        {
            SenderFunction senderFunction = args.senderFunc;
            IPCCodec codec = new IPCCodec();
            switch (senderFunction)
            {
                case SenderFunction.Handshake:
                    handleHandshake();
                    break;
                case SenderFunction.ConfirmationAddinOnline:
                    startMaincoalTools(new MapTools(Program.platformaGraficzna));
                    break;
                case SenderFunction.MaincoalDataSaved:
                    actionWhenDataSaved(codec.getString(args.data));
                    break;
            }
        }


        private void handleHandshake()
        {
            this.ipcSender = IPCSender.getInstance();
            DBConnectionData connectionData = new DBConnectionData()
            {
                serverName = ProgramSettings.Server,
                dbName = ProgramSettings.Database,
                login = ProgramSettings.userName,
                password = ProgramSettings.userPassword,
                applicationStartupPath = Program.mainPath
            };
            this.ipcSender.sendConnectionData(connectionData);
            Program.isAddinConnected = true;
        }


        private void startMaincoalTools(MapTools mt)
        {
            mt.startMaincoalTools();
        }

        public delegate void dataSavedMethodDelegate(string idWyrobiska);
        private void actionWhenDataSaved(string idWyrobiska)
        {
            if (this.InvokeRequired)
                this.Invoke(new dataSavedMethodDelegate(actionWhenDataSaved), idWyrobiska);
            else
            {
                if (this.appType == ApplicationType.insert)
                    actionWhenDataInserted();
                else if (this.appType == ApplicationType.update)
                    actionWhenDataUpdated(idWyrobiska);
            }
        }

        private void actionWhenDataInserted()
        {
            int rowIndex = this.dataGridView1.SelectedRows[0].Index;
            this.dataGridView1.SelectedRows[0].Visible = false;
            if (rowIndex + 1 < this.dataGridView1.Rows.Count - 1)
            {
                this.dataGridView1.Rows[rowIndex + 1].Selected = true;
                dataGridView1_RowHeaderMouseClick(null, null);
            }
        }

        private void actionWhenDataUpdated(string idWyrobiska)
        {
            int rowIndex = getRowIndexForUpdatedWyrobisko(idWyrobiska);
            if (rowIndex == -1)
                MessageBox.Show("Nie znaleziono wyrobiska w datagridzie ???");
            else
                this.dataGridView1.Rows[rowIndex].Visible = false;
        }

        private int getRowIndexForUpdatedWyrobisko(string idWyrobiska)
        {
            for (int i = 0; i < this.dataGridView1.RowCount; i++)
            {
                if (this.dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(idWyrobiska))
                    return i;
            }
            return -1;
        }

        #endregion

        #region interakcja użytkownika, kliknięcia nagłówka wiersza datagrida

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.appType == ApplicationType.update)
                return;
            try
            {
                int selectedRow = dataGridView1.SelectedRows[0].Index;
                int columnNazwaIndex = dataGridView1.Columns["nazwaWyrobiska"].Index;
                int columnIDIndex = dataGridView1.Columns["id_wyrobiska"].Index;
                string idWyrobiska = dataGridView1.Rows[selectedRow].Cells[columnIDIndex].Value.ToString();
                string nazwaWyrobiska = dataGridView1.Rows[selectedRow].Cells[columnNazwaIndex].Value.ToString();
                string info = idWyrobiska + ";" + nazwaWyrobiska;
                if (this.ipcSender != null)
                    this.ipcSender.sendMessage(info, SenderFunction.InformacjaOObiekcie);
                else
                    MessageBox.Show("należy najpierw uruchomić narzędzie mapy  ");
            }
            catch (NullReferenceException exe)
            {
                MessageBox.Show("Błąd kwerendy. Wynik kwerendy musi zawierać pola 'id_wyrobiska' i 'nazwaWyrobiska'");
            }
        }

        #endregion

        #region wyświetlanie linii centralnych i znaczników na mapie przyciskiem Wyświetl na mapie

        private void tsWyswietlNaMapie_Click(object sender, EventArgs e)
        {
            if (ipcSender == null)
            {
                MessageBox.Show("należy najpierw wyświetlić dane w datagridzie i uruchomić narzędzie mapy  ");
                return;
            }
            if(dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("należy zaznaczyć przynajmniej jedno wyrobisko do wyświetlenia  ");
                return;
            }

            IEnumerable<IElementGraficzny> elems = getElementsToDisplay();
            ipcSender.sendElementyGraficzne(elems, getDisplayParams().getDisplayParametersAsXml().ToString(), SenderFunction.DisplayReceivedObjectsOnMap);
        }

        private DisplayParams getDisplayParams()
        {
            return new DisplayParams()
            {
                CADColourAsInt = getColourFromCombo(),
                levelName = displayLevelNameUpdate,
                lineThickness = 1,
                lineStyleName = "0",
                clearLevelBeforeDisplay = cbFituj.Checked,
                focusLevelAfterDisplay = cbFituj.Checked
            };
        }

        private int getColourFromCombo()
        {
            ComboboxItem selectedItem = cbKolor.SelectedItem as ComboboxItem;
            return numberHandler.tryGetInt(selectedItem.value);
        }

        private IEnumerable<IElementGraficzny> getElementsToDisplay()
        {
            QueryData liniaCentralnaQueryData = getLinestringData();
            MapObjectsTools tools = new MapObjectsTools(this.dbConnection);
            List<IElementGraficzny> elems = tools.getLinestringsFromSqlData(liniaCentralnaQueryData, (int)TypObiektuMapy.LiniaCentralnaWyrobiska);
            modifyDblinks(elems, liniaCentralnaQueryData);
            return elems;
        }

        private void modifyDblinks(List<IElementGraficzny> elems, QueryData liniaCentralnaQueryData)
        {
            for (int i = 0; i < elems.Count; i++)
            {
                DBLink link = elems[i].dbLink;
                link.elementId = getUniqueIdOdcinka(liniaCentralnaQueryData, link.elementId);
            }
        }

        private int getUniqueIdOdcinka(QueryData liniaCentralnaQueryData, int elementId)
        {
            return getUniqueIdOdcinkaFromDataRow(liniaCentralnaQueryData, getDataRowIndexOdcinka(liniaCentralnaQueryData, elementId));
        }

        private int getDataRowIndexOdcinka(QueryData liniaCentralnaQueryData, int elementId)
        {
            for (int i = 0; i < liniaCentralnaQueryData.dataRowsNumber; i++)
            {
                int id = Convert.ToInt32(liniaCentralnaQueryData.getDataValue(i, "id"));
                if (elementId == id)
                    return i;
            }
            return 0;
        }

        private int getUniqueIdOdcinkaFromDataRow(QueryData liniaCentralnaQueryData, int rowIndex)
        {
            OdcinekLiniiCentralnej odcinek = new OdcinekLiniiCentralnej()
            {
                idWyrobiska = liniaCentralnaQueryData.getDataValue(rowIndex, "id_wyrobiska").ToString(),
                numerKolejny = Convert.ToInt32(liniaCentralnaQueryData.getDataValue(rowIndex, "odcinekNumer"))
            };
            return odcinek.uniqueId;
        }

        private QueryData getLinestringData()
        {
            string query = "select geometriaLiniiCentralnej as geometryString, idLinieCentralne as id, id_wyrobiska, odcinekNumer FROM WyrobiskaLinieCentralne " + getQueryCondition();
            DBReader reader = new DBReader(this.dbConnection);
            return reader.readFromDB(query);
        }

        private string getQueryCondition()
        {
            string whereCondition = " where WyrobiskaLinieCentralne.id_wyrobiska in (@idZaznaczonychWyrobisk)";
            return whereCondition.Replace("@idZaznaczonychWyrobisk", getIdsOfSelectedWyrobiska());
        }

        private string getIdsOfSelectedWyrobiska()
        {
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                ids.Add(row.Cells["id_wyrobiska"].Value.ToString());
            }
            return String.Join(",", ids);
        }

        #endregion

    }
}
