
using System;
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


        public void formatDatagrid(DataGridView dataGrid, List<string> columnHeaders, List<int> colWidths, string[] hiddenColumns)
        {
            resetDatagrid(dataGrid);
            addNewColumns(dataGrid, columnHeaders.Count);
            resizeColumns(dataGrid, colWidths);
            nameColumnHeaders(dataGrid, columnHeaders);
            hideColumns(dataGrid, hiddenColumns);
            resizeDatagrid(dataGrid, hiddenColumns);
        }

        /// <summary>
        /// wraca  datagrid do jego pierwotnych rozmiarów, tj do tylu kolumn ile określa zmienna defaultNrOfDatagridColumns
        /// </summary>
        private void resetDatagrid(DataGridView dataGrid)
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

        /// <summary>
        /// datagrid ma przynajmniej tyle kolumn ile określa zmienna defaultNrOfDatagridColumns, więc po resecie datagridu kolumny mogę tylko dodać, jeżeli nagłówków jest więcej niż ta liczba
        /// </summary>
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

        private void nameColumnHeaders(DataGridView dataGrid, List<string> columnHeaders)
        {
            //nazywam nagłówki
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                dataGrid.Columns[i].HeaderText = columnHeaders[i];
                dataGrid.Columns[i].Name = columnHeaders[i];
            }
        }

        private void hideColumns(DataGridView dataGrid, string[] hiddenColumns)
        {
            if (hiddenColumns == null)
                return;
            for (int i = 0; i < hiddenColumns.Length; i++)
            {
                dataGrid.Columns[hiddenColumns[i]].Visible = false;
            }
        }

        /// <summary>
        /// określam i ograniczam szerokość datagridu    
        /// </summary>
        private void resizeDatagrid(DataGridView dataGrid, string[] hiddenColumns)
        {
            int paddings = dataGrid.RowHeadersWidth + dataGrid.Margin.Left + dataGrid.Margin.Right + dataGridColumnPadding;
            int hiddenColumnsWidth = getHiddenColumnsTotalWidth(dataGrid, hiddenColumns);
            dataGridWidth = dataGrid.Columns.GetColumnsWidth(DataGridViewElementStates.None) + paddings - hiddenColumnsWidth;

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

        private int getHiddenColumnsTotalWidth(DataGridView dataGrid, string[] hiddenColumns)
        {
            int totalWidth = 0;
            if (hiddenColumns == null)
                return 0;
            for (int i = 0; i < hiddenColumns.Length; i++)
            {
                totalWidth += dataGrid.Columns[hiddenColumns[i]].Width;
            }
            return totalWidth;
        }
    }
}
