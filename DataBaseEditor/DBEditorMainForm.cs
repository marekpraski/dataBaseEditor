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

        public DBEditorMainForm()
        {
            InitializeComponent();
            connector = new DBConnector();
            configFileValidated = connector.validateConfigFile();
            label2.Visible = !configFileValidated;
            dg1Handler = new DataGridHandler();  //każdy datagrid musi mieć swoją instancję DataGridHandlera
            formatter = new FormFormatter();

        }


        private void populateDatagrid(ref DataGridView dataGrid)
        {
 
            //pierwsza kolumna nie jest do edycji, w niej musi być primaryKey;
            dataGrid.Columns[0].ReadOnly = true;
            dataGrid.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;

            DBReader reader = new DBReader(dbConnection);
            queryData = reader.readFromDB(sqlQuery);

            //jeżeli kwerenda błędna to nie zwróci wyników
            //przypadki błędnej kwerendy obsługiwane są przez DBReader
            if (queryData.getHeaders().Count != 0)
            {

                List<string> columnHeaders = queryData.getHeaders();

                //dopasowuję formatkę do wyników nowej kwerendy
                
                changeMainFormLayout(columnHeaders.Count, ref dataGrid);

                //nazywam nagłówki
                for (int i = 0; i < queryData.getHeaders().Count; i++)
                {
                    dataGrid.Columns[i].HeaderText = columnHeaders[i];  //System.ArgumentOutOfRangeException: „Indeks był spoza zakresu. Musi mieć wartość nieujemną i mniejszą niż rozmiar kolekcji.
                                                                        // Nazwa parametru: index”

                }

                //wypełniam datagrid danymi
                List<object[]> dbData = queryData.getQueryData();
                int rowIndex = 0;
                foreach (object[] row in dbData)
                {
                    dataGrid.Rows.Add(row);
                    object primaryKey = row[0];
                    dg1Handler.addDataGridIndex(rowIndex, primaryKey);
                    if (dataGrid.AllowUserToAddRows)
                    {
                        rowIndex = dataGrid.Rows.Count - 1;   //gdy użytkownik ma możliwość dodawania wierszy w datagridzie, datagrid posiada dodatkowo jeden pusty wiersz na końcu
                    }
                    else
                    {
                        rowIndex = dataGrid.Rows.Count;
                    }
                }
            }
        }

        //przyjmuje liczbę nagłówków z kwerendy oraz datagrid, w którym trzeba dopasować liczbę kolumn
        private void changeMainFormLayout(int numberOfHeaders, ref DataGridView dataGrid)
        {
            formatter.changeNumberOfColumnsInDatagrid(ref dataGrid, numberOfHeaders);
            formatter.changeDisplayButtonLocation(ref displayButton);
            formatter.changeSaveButtonLocation(ref saveButton);
            formatter.changeUndoButtonLocation(ref undoButton);
            this.Width = formatter.calculateFormWidth();
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

        private string generateUpdateQuery(string dbName, DataGridCell cell)
        {
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            CellConverter cellConverter = new CellConverter();
            string columnName = queryData.getHeaders()[columnIndex];
            string primaryKeyColumnName = queryData.getHeaders()[0];    //kluczem głównym MUSI być pierwsza kolumna
            object primaryKey = dg1Handler.getCellPrimaryKey(cell);
            string updateQuery = "update " + dbName + " set " + columnName + "=" + cellConverter.getConvertedValue(ref cell) + " where " + primaryKeyColumnName + "='" + primaryKey.ToString() + "'";
            return updateQuery;
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

        private void changeCellTextColour(DataGridCell cell, Color colour)
        {
            int rowIndex = cell.getCellIndex(cellIndexTypes.rowIndex);
            int columnIndex = cell.getCellIndex(cellIndexTypes.columnIndex);
            dataGridView1.Rows[rowIndex].Cells[columnIndex].Style.ForeColor = colour;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            changedCell = new DataGridCell();
            object oldCellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            changedCell.setCellValue(cellValueTypes.oldValue, oldCellValue);
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

        //button, którego kliknięcie wypełnia danymi z kwerendy główny datagrid
        private void displayButton_Click(object sender, EventArgs e)
        {
            //jest to pierwszy przycisk, który użytkownik może nacisnąć po wpisaniu kwerendy w pole tekstowe
            //przekazuję kwerendę do DBConnectora w celu utworzenia połaczenia, wyciągam od razu nazwę bazy danych, jest potrzebna później

            if (configFileValidated)
            {
                sqlQuery = sqlQueryTextBox.Text;
                dbName = connector.getDBName(ref sqlQuery);
                dbConnection = connector.getDBConnection(ConnectionSources.wholeConnectionInFile, ConnectionTypes.sqlAuthorisation);

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
                    populateDatagrid(ref dataGridView1);
                }
            }
        }

        private void PomocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pomoc = "(*) pierwsza kolumna wyników kwerendy MUSI zawierać ID (lub inny klucz główny)" + 
                "\r\nidentyfikujący jednoznacznie wiersz wyników kwerendy zasilającej datagrid";
            MyMessageBox.display(pomoc, MessageBoxType.Information);
        }

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
    }
}
