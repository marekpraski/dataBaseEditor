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

namespace DataBaseEditor
{
    public partial class DBEditorMainForm : Form
    {
        private enum ApplicationType { insert, update}

        private ApplicationType appType = ApplicationType.update;       //ustawić odpowiednio dla kompilacji dla PRGW (insert) lub Bogdanka (update)

        private DataGridHandler dg1Handler;
        private FormFormatter formatter;
        private DataGridCell changedCell;
        private DBConnector connector;
        private bool configFileValidated;
        private string sqlQuery;
        private QueryData queryData;
        private SqlConnection dbConnection;
        private string tableName = "";

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
        }

        private void initialSettings()
        {
            if(this.appType == ApplicationType.insert)
            {
                tsWyswietlNaMapie.Visible = false;
                labelZatwierdzone.Visible = false;
                cbZatwierdzone.Visible = false;
            }
            else if (this.appType == ApplicationType.update)
            {
                tbSqlQuery.Text = @"Select Wyrobiska.id_wyrobiska, Wyrobiska.nazwa, Wyrobiska.typWyrob, Wyrobiska.id_poziomu, Wyrobiska.id_pokladu, WyrobiskaLinieCentralne.zatwierdzone
	                                 from WyrobiskaLinieCentralne
                                    inner join Wyrobiska on WyrobiskaLinieCentralne.id_wyrobiska = Wyrobiska.id_wyrobiska 
                                    where WyrobiskaLinieCentralne.zatwierdzone = ";
                tbSqlQuery.ReadOnly = true;
                cbZatwierdzone.SelectedIndex = 0;
                labelWhereAnd.Text = "and";
                sqlQueryTextBox_TextChangedEvent(null, null);
            }                
        }

        private void DBEditorMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MapTools mt = new MapTools(Program.platformaGraficzna);
            mt.unloadAddin();
        }


        #region Region - zdarzenia na interakcję z użytkownikiem


        //button, którego kliknięcie wypełnia danymi z kwerendy główny datagrid
        //jest to pierwszy przycisk, który użytkownik może nacisnąć po wpisaniu kwerendy w pole tekstowe
        private void displayButton_Click(object sender, EventArgs e)
        {
            //przekazuję kwerendę do DBConnectora w celu utworzenia połaczenia, wyciągam od razu nazwę bazy danych, jest potrzebna później
            this.sqlQuery = constructQuery();

            if (configFileValidated)
            {
                //tbSqlQuery.Text = this.sqlQuery;      
                
                //sql nie widzi różnicy pomiędzy lower i upper case a ma to znaczenie przy wyszukiwaniu słow kluczowych w kwerendzie
                tableName = connector.getTableNameFromQuery(sqlQuery);
                dbConnection = connector.getDBConnection(ConnectionDataSource.serverAndDatabaseNamesInFile, ConnectionTypes.sqlAuthorisation);

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
        }

        private string constructQuery()
        {
            string queryPart1 = tbSqlQuery.Text;
            string queryPart2 = "";
            string queryPart3 = "";
            if (this.appType == ApplicationType.update)
                queryPart1 += cbZatwierdzone.Text;
            if(!String.IsNullOrEmpty(tbLike.Text))
                if(queryPart1.ToLower().Contains("where"))
                    queryPart2 = " and " + tbNazwa.Text + " like '%" + tbLike.Text + "%' ";
            else
                    queryPart2 = " where " + tbNazwa.Text + " like '%" + tbLike.Text + "%' ";
            if (!String.IsNullOrEmpty(tbOrderBy.Text))
                queryPart3 = " order by " + tbOrderBy.Text;
            return queryPart1 + queryPart2 + queryPart3;
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
            string pomoc = "(*) pierwsza kolumna wyników kwerendy MUSI zawierać ID (lub inny klucz główny)" +
                "\r\nidentyfikujący jednoznacznie wiersz wyników kwerendy zasilającej datagrid";
            MyMessageBox.display(pomoc, MessageBoxType.Information);
        }


        //tj użytkownik wpisał kwerendę w polu tekstowym
        private void sqlQueryTextBox_TextChangedEvent(object sender, EventArgs e)
        {
            if (tbSqlQuery.Text != "") // && dbConnection != null)
            {
                displayButton.Enabled = true;
            }
            else
            {
                displayButton.Enabled = false;
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

        #region metody do edycji i zapisu danych w datagridzie

        private void setUpDatagrid()
        {

            //pierwsza kolumna nie jest do edycji, w niej musi być primaryKey;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;

            DBReader reader = new DBReader(dbConnection);
            queryData = reader.readFromDB(sqlQuery);

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
            formatter.changeDisplayButtonLocation(ref displayButton);
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
                case SenderFunction.DataSaved:
                    actionWhenDataSaved();
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

        public delegate void voidMethodDelegate();
        private void actionWhenDataSaved()
        {
            if (this.InvokeRequired)
                this.Invoke(new voidMethodDelegate(actionWhenDataSaved));
            else
            {
                int rowIndex = this.dataGridView1.SelectedRows[0].Index;
                this.dataGridView1.SelectedRows[0].Visible = false;
                if (rowIndex + 1 < this.dataGridView1.Rows.Count - 1)
                {
                    this.dataGridView1.Rows[rowIndex + 1].Selected = true;
                    dataGridView1_RowHeaderMouseClick(null, null);
                }
            }
        }

        #endregion

        #region interakcja użytkownika, kliknięcia nagłówka wiersza datagrida

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                int selectedRow = dataGridView1.SelectedRows[0].Index;
                int columnNazwaIndex = dataGridView1.Columns["nazwa"].Index;
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
                MessageBox.Show("Błąd kwerendy. Wynik kwerendy musi zawierać pola 'id_wyrobiska' i 'nazwa'");
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

            KeyinParameters kp = new KeyinParameters() { displayParams = getDisplayParams(), queryParams = getQueryParams(), typObiektu = TypObiektuMapy.LiniaCentralnaWyrobiska };
            ipcSender.sendMessage(kp.toXmlString(), SenderFunction.DisplayObjectsOnMap);
        }

        private QueryInputParams getQueryParams()
        {
            QueryInputParams qd = new QueryInputParams()
            {
                textTableName = this.tableName,
                textIdentityColumn = "id_wyrobiska",
                textGeometryColumn = "geometriaPunktStart",
                textColumn = "znacznikPunktStart",
                linestringTableName = this.tableName,
                linestringIdentityColumn = "id_wyrobiska",
                linestringGeometryColumn = "geometriaLiniiCentralnej",
                selectCondition = extractQueryCondition(this.sqlQuery)
            };
            return qd;
        }

        private string extractQueryCondition(string sqlQuery)
        {
            if (!sqlQuery.ToLower().Contains("where"))
                return "";

            int startIndex = sqlQuery.ToLower().IndexOf("where");
            int substringLength = sqlQuery.Length - startIndex;
            if (sqlQuery.ToLower().Contains("order by"))
                substringLength = sqlQuery.ToLower().IndexOf("order by") - startIndex;

            return sqlQuery.Substring(startIndex, substringLength);
        }

        private DisplayParams getDisplayParams()
        {
            return new DisplayParams()
            {
                CADColourAsInt = 0,
                levelName = "osieWyrobiskWBazie",
                lineThickness = 0,
                lineStyleName = "0"
            };
        }

        #endregion
    }
}
