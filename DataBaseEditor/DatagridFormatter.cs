
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataBaseEditor
{
    public class DatagridFormatter
    {
        //szerokości kolumn datagridu
        private int dafaultColWidth = 100;
        private int minColumnWidth = 50;                 //szerokości kolumn są dopasowane do zawartości, ale jest min i max
        private int maxColumnWidth = 300;
        private int maxDatagridWidth = 1500;
        private int dataGridColumnPadding = 15;          //wartość dobrana doświadczalnie, dodaję do szerokości datagridu obliczonej standardowo, inaczej pojawia się pasek przewijania na dole

        private List<DataGridViewColumn> columnsAdded = new List<DataGridViewColumn>();
        private int dataGridWidth = 0;
        private int defaultNrOfDatagridColumns = 4;


        public void formatDatagrid(DataGridView dataGrid, int numberOfHeaders, List<int> colWidths)
        {
            resetDatagrid(ref dataGrid);

            //datagrid ma przynajmniej tyle kolumn ile określa zmienna defaultNrOfDatagridColumns, więc po resecie datagridu kolumny mogę tylko dodać, jeżeli nagłówków jest więcej niż ta liczba
            addNewColumns(dataGrid, numberOfHeaders);
            resizeColumns(dataGrid, colWidths);

            //określam i ograniczam szerokość datagridu            
            dataGridWidth = dataGrid.Columns.GetColumnsWidth(DataGridViewElementStates.None) + dataGrid.RowHeadersWidth + dataGrid.Margin.Left + dataGrid.Margin.Right + dataGridColumnPadding;

            if (dataGridWidth > maxDatagridWidth)     //ograniczam max szerokość tworzonego datagrida 
            {
                dataGridWidth = maxDatagridWidth;
                dataGrid.Width = dataGridWidth;      //celowa redundancja, bo używam zmiennej dataGridWidth do ustawienia położenia buttonów i szerokości głównej formatki
            }
            else
            {
                dataGrid.Width = dataGridWidth;
            }
        }

        private void resizeColumns(DataGridView dataGrid, List<int> colWidths)
        {
            for (int i = 0; i < colWidths.Count; i++)
            {
                int calculatedColWidth = colWidths[i];
                if (calculatedColWidth < minColumnWidth)
                {
                    dataGrid.Columns[i].Width = minColumnWidth;
                }
                else if (calculatedColWidth > maxColumnWidth)
                {
                    dataGrid.Columns[i].Width = maxColumnWidth;
                }
                else
                {
                    dataGrid.Columns[i].Width = colWidths[i];
                }
            }
        }

        private void addNewColumns(DataGridView dataGrid, int numberOfHeaders)
        {
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
            }
        }

        //wraca  datagrid do jego pierwotnych rozmiarów, tj do tylu kolumn ile określa zmienna defaultNrOfDatagridColumns
        private void resetDatagrid(ref DataGridView dataGrid)
        {
            dataGridWidth = 0;
            if (columnsAdded.Count > 0)   //w czasie tej sesji dodane zostały columny
            {
                foreach (DataGridViewColumn col in columnsAdded)
                {
                    dataGrid.Columns.Remove(col);
                }
            }
            columnsAdded.Clear();
            //zmieniam nazwy kolumn na oryginalne, bo w razie gdyby obecna kwerenda miała mniej niż 4 kolumny, to zostaną w nich stare napisy
            for (int colNr = 0; colNr < defaultNrOfDatagridColumns; colNr++)
            {
                dataGrid.Columns[colNr].HeaderText = "Column " + colNr;
                dataGrid.Columns[colNr].Width = dafaultColWidth;
            }
        }
    }
}
