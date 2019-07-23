using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBaseEditor
{
    //zmienia układ przekazanego formularza, raczej nie jest uniwersalna tylko chodziło mi o oddzielenie tej funkcjonalności
    public class FormFormatter
    {
        int oldButtonXLocation = 464;       //położenie buttonów przy 4 kolumnach
        int originalDatagridWidth = 444;    //szerokość datagridu mającego 4 kolumny to 444
        int dataGridPadding = 44;           //szerokość datagridu mającego 0 kolumn
        int originalFormWidth = 565;        //szerokość formularza dla datagridu mającego 4 kolumny
        int displayButtonYLocation = 123;
        int undoButtonYLocation = 169;
        int saveButtonYLocation = 219;
        int defaultNrOfDatagridColumns = 4;

        private List<DataGridViewColumn> columnsAdded;
        int dataGridWidth;
        public FormFormatter ()
        {
            columnsAdded = new List<DataGridViewColumn>();
        }

        public void changeNumberOfColumnsInDatagrid(ref DataGridView dataGrid, int numberOfHeaders)
        {
            resetDatagrid(ref dataGrid);

            //datagrid ma przynajmniej tyle kolumn ile określa zmienna defaultNrOfDatagridColumns, więc po resecie datagridu kolumny mogę tylko dodać, jeżeli nagłówków jest więcej niż ta liczba
            if (numberOfHeaders > defaultNrOfDatagridColumns)
            {
                int numberOfAddedColumn = 0;      //zmienna użyta do nazywania kolejnych dodawanych kolumn
                for (int i = 0; i < numberOfHeaders - defaultNrOfDatagridColumns; i++)
                {                    
                    DataGridViewColumn col = new DataGridViewTextBoxColumn();
                    dataGrid.Columns.Add(col);
                    col.HeaderText = "added";
                    col.Name = "added" + numberOfAddedColumn;
                    columnsAdded.Add(col);
                    numberOfAddedColumn++;
                }
                if (dataGridPadding + numberOfHeaders * 100 > 1046)     //ograniczam max szerokość tworzonego datagrida do szerokości potrzebnej dla 10 kolumn
                {
                    dataGridWidth = 1046;
                    dataGrid.Width = dataGridWidth;     //celowa redundancja, dla debugowania
                }
                else
                {
                    dataGridWidth = dataGridPadding + numberOfHeaders * 100;   //standardowa szerokość kolumny to 100
                    dataGrid.Width = dataGridWidth;
                }
            }
            else
            {
                dataGrid.Width = originalDatagridWidth;
            }
        }

        //wraca  datagrid do jego pierwotnych rozmiarów, tj do tylu kolumn ile określa zmienna defaultNrOfDatagridColumns
        private void resetDatagrid(ref DataGridView dataGrid)
        {
            if (columnsAdded.Count>0)   //w czasie tej sesji dodane zostały columny
            {
                dataGrid.Width = originalDatagridWidth;
                foreach (DataGridViewColumn col in columnsAdded)
                {
                    dataGrid.Columns.Remove(col);
                } 
            }
            columnsAdded.Clear();
            //zmieniam nazwy kolumn na oryginalne, bo w razie gdyby obecna kwerenda miała mniej niż 4 kolumny, to zostaną w nich stare napisy
            for(int colNr = 0; colNr<defaultNrOfDatagridColumns; colNr++)
            {
                dataGrid.Columns[colNr].HeaderText = "Column " + colNr;
            }
        }

        public void changeUndoButtonLocation(ref Button button)
        {
            Point newUndoButtonLocation = new Point(dataGridWidth + (oldButtonXLocation - originalDatagridWidth), undoButtonYLocation);
            button.Location = newUndoButtonLocation;
        }

        public void changeSaveButtonLocation(ref Button button)
        {
            Point newSaveButtonLocation = new Point(dataGridWidth + (oldButtonXLocation - originalDatagridWidth), saveButtonYLocation);
            button.Location = newSaveButtonLocation;
        }

        public void changeDisplayButtonLocation(ref Button button)
        {
            Point newDisplayButtonLocation = new Point(dataGridWidth + (oldButtonXLocation - originalDatagridWidth), displayButtonYLocation);
            button.Location = newDisplayButtonLocation;
        }

        public int calculateFormWidth()
        {
            return dataGridWidth + (originalFormWidth - originalDatagridWidth);
        }

    }
}
