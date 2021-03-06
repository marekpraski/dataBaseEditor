using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace DataBaseEditor
{
    public partial class DBEditorMainForm : Form
    {
        private DataGridHandler dg1Handler;
        private FormFormatter formatter;
        private DataGridCell changedCell;
        private DBConnector connector;
        private bool configFileValidated;
        private string sqlQuery;
        private QueryData queryData;
        private SqlConnection dbConnection;
        private string dbName = "";

        private List<object[]> dbData;

        private int datagridRowIndex = 0;
        private int rowsLoaded = 0;

        public DBEditorMainForm()
        {
            InitializeComponent();
            connector = new DBConnector();
            configFileValidated = connector.validateConfigFile();
            label2.Visible = !configFileValidated;
            dg1Handler = new DataGridHandler();  //każdy datagrid musi mieć swoją instancję DataGridHandlera
            formatter = new FormFormatter();

        }


        #region Region - zdarzenia na interakcję z użytkownikiem


        //button, którego kliknięcie wypełnia danymi z kwerendy główny datagrid
        //jest to pierwszy przycisk, który użytkownik może nacisnąć po wpisaniu kwerendy w pole tekstowe
        private void displayButton_Click(object sender, EventArgs e)
        {
            //przekazuję kwerendę do DBConnectora w celu utworzenia połaczenia, wyciągam od razu nazwę bazy danych, jest potrzebna później

            if (configFileValidated)
            {
                sqlQuery = sqlQueryTextBox.Text;      
                
                //sql nie widzi różnicy pomiędzy lower i upper case a ma to znaczenie przy wyszukiwaniu słow kluczowych w kwerendzie
                dbName = connector.getTableName(sqlQuery.ToLower());
                dbConnection = connector.getDBConnection(ConnectionSources.serverNameInFile, ConnectionTypes.sqlAuthorisation);

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
                query = generateUpdateQuery(dbName, cell);
                writer.writeToDB(query);
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
            if (sqlQueryTextBox.Text != "") // && dbConnection != null)
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
            formatter.setTextboxSize(ref sqlQueryTextBox);
            this.Width = formatter.calculateFormWidth();
        }

 
        private string generateUpdateQuery(string dbName, DataGridCell cell)
        {
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            CellConverter cellConverter = new CellConverter();
            string columnName = queryData.getHeaders()[columnIndex];
            string primaryKeyColumnName = queryData.getHeaders()[0];    //kluczem głównym MUSI być pierwsza kolumna
            object primaryKey = dg1Handler.getCellPrimaryKey(cell);
            string newValue = cellConverter.getConvertedValue(ref cell);
            if (newValue == null)
            {
                return "update " + dbName + " set " + columnName + "= null" + " where " + primaryKeyColumnName + "='" + primaryKey.ToString() + "'";
            }
            return "update " + dbName + " set " + columnName + "=" + cellConverter.getConvertedValue(ref cell) + " where " + primaryKeyColumnName + "='" + primaryKey.ToString() + "'"; ;
        }



        private void changeCellTextColour(DataGridCell cell, Color colour)
        {
            int rowIndex = cell.getCellIndex(cellIndexTypes.rowIndex);
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            dataGridView1.Rows[rowIndex].Cells[columnIndex].Style.ForeColor = colour;
        }
    }
}
