using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseEditor
{
    //podczas edycji datagridu - sprawdza czy wartość wpisywana przez użytkownika odpowiada typowi bazodanowemu tej celki
    //podczas tworzenia kwerendy zapisującej wartość celki do bazy konwertuje tę wartość na taki ciąg znaków, który można bezpośrednio
    //wpisać do kwerendy
    public class CellConverter
    {       
        public bool verifyCellDataType(ref DataGridCell cell)
        {
            // rozważam bazodanowe typy danych, w grupach: (bit, int, bigint), (char, varchar), (float, decimal, numeric), (datetime), (geometry)
            string typeName = cell.DataTypeName;
            if (typeName.Contains("bit") || typeName.Contains("int"))
            {
                return handleBigint(cell);
            }
            else if (typeName.Contains("char"))
            {
                return true;
            }
            else if (typeName.Contains("float") || typeName.Contains("decimal") || typeName.Contains("numeric"))
            {
                return handleDouble(cell);
            }
            else if (typeName.Contains("datetime"))
            {
                return handleDatetime(cell);
            }
            else if (typeName.Contains("geometry"))
            {
                MyMessageBox.display("Nie można edytować pól typu geometry", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleDatetime(DataGridCell cell)
        {
            object cellValue = cell.getCellValue(cellValueTypes.newValue);
            try
            {
                DateTime value = (DateTime)cellValue;
                return true;
            }
            catch (InvalidCastException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić datę", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleDouble(DataGridCell cell)
        {
            object cellValue = cell.getCellValue(cellValueTypes.newValue);
            string stringCellValue = cellValue.ToString(); 
            try
            {
                double value = Double.Parse(stringCellValue);
                return true;
            }
            catch (InvalidCastException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę", MessageBoxType.Error);
            }
            catch (System.FormatException ex)
            {
                MyMessageBox.display(ex.Message + "\r\nZnak separatora dziesiętnego musi być taki jaki jest ustawiony w systemie");
            }
            return false;
        }

        private bool handleBigint(DataGridCell cell)
        {
            object cellValue = cell.getCellValue(cellValueTypes.newValue);
            try
            {
                long value = (long)cellValue;
                return true;
            }
            catch (InvalidCastException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą", MessageBoxType.Error);
            }
            return false;
        }

        //skoro wartość przeszła pierwsze sprawdzenie funkcją verifyCellDataType to ta funkcja musi rozważyć tylko dwa przypadki:
        //zwrócić string w apostrofach i zamienić przecinek na kropkę w double
        public string getConvertedValue(ref DataGridCell cell)
        {
            string typeName = cell.DataTypeName;
            string convertedValue = cell.getCellValue(cellValueTypes.newValue).ToString();
            if (typeName.Contains("float") || typeName.Contains("decimal") || typeName.Contains("numeric"))
            {
                return convertedValue.Replace(",", ".");    
            }
            else if (typeName.Contains("char"))
            {
                return "'" + convertedValue + "'";
            }
                return convertedValue;
        }
    }
}
