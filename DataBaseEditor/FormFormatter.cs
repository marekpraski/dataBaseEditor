
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DataBaseEditor
{
    //zmienia układ przekazanego formularza, raczej nie jest uniwersalna tylko chodziło mi o oddzielenie tej funkcjonalności
    public class FormFormatter
    {
        private int originalButtonXLocation = 464;       //położenie buttonów przy 4 kolumnach
        private int originalDatagridWidth = 444;        //szerokość datagridu mającego 4 kolumny to 444
        private int originalFormWidth = 565;            //szerokość formularza dla datagridu mającego 4 kolumny
        private int displayButtonYLocation = 147;
        private int undoButtonYLocation = 193;
        private int saveButtonYLocation = 243;
        private int loadNextButtonYLocation = 606;
        private int remainingRowsLabelYLocation = 590;

        private DataGridView dataGrid;
        private int dataGridWidth => this.dataGrid.Width;


        public void formatDatagrid(ref DataGridView dataGrid, int numberOfHeaders, List<int> colWidths)
        {
            this.dataGrid = dataGrid;
            DatagridFormatter dgvFormatter = new DatagridFormatter();
            dgvFormatter.formatDatagrid(dataGrid, numberOfHeaders, colWidths);
        }

        public void changeUndoButtonLocation(ref Button button)
        {
            Point newUndoButtonLocation = new Point(dataGridWidth + (originalButtonXLocation - originalDatagridWidth), undoButtonYLocation);
            button.Location = newUndoButtonLocation;
        }

        public void changeLoadNextButtonLocation(Button button)
        {
            Point newULoadNexButtonLocation = new Point(dataGridWidth + (originalButtonXLocation - originalDatagridWidth),loadNextButtonYLocation);
            button.Location = newULoadNexButtonLocation;
        }

        public void changeRemainingRowsLabelLocation(Label label)
        {
            Point newremainingRowsLabelLocation = new Point(dataGridWidth + (originalButtonXLocation - originalDatagridWidth), remainingRowsLabelYLocation);
            label.Location = newremainingRowsLabelLocation;
        }

        public void changeSaveButtonLocation(ref Button button)
        {
            Point newSaveButtonLocation = new Point(dataGridWidth + (originalButtonXLocation - originalDatagridWidth), saveButtonYLocation);
            button.Location = newSaveButtonLocation;
        }

        public void changeDisplayButtonLocation(ref Button button)
        {
            Point newDisplayButtonLocation = new Point(dataGridWidth + (originalButtonXLocation - originalDatagridWidth), displayButtonYLocation);
            button.Location = newDisplayButtonLocation;
        }

        public int calculateFormWidth()
        {
            return dataGridWidth + (originalFormWidth - originalDatagridWidth);
        }

        public void setTextboxSize(ref RichTextBox textbox)
        {
            textbox.Width = dataGridWidth;
        }

    }
}
