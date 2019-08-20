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
            // rozważam bazodanowe typy danych:  bit, int, bigint, (char, varchar), w grupach: (float, decimal, numeric), (datetime), (geometry)
            string typeName = cell.DataTypeName;
            object cellValue = cell.getCellValue(cellValueTypes.newValue);
            if (cellValue != null)
            {
                if (typeName.Contains("bigint"))
                {
                    return handleBigint(cellValue);
                }
                else if (typeName.Contains("bit"))
                {
                    return handleBit(cellValue);
                }
                else if (typeName.Contains("int"))
                {
                    return handleInt(cellValue);
                }
                else if (typeName.Contains("char"))
                {
                    return true;
                }
                else if (typeName.Contains("float") || typeName.Contains("decimal") || typeName.Contains("numeric"))
                {
                    return handleDouble(cellValue);
                }
                else if (typeName.Contains("datetime"))
                {
                    return handleDatetime(cellValue);
                }
                else if (typeName.Contains("geometry"))
                {
                    MyMessageBox.display("Nie można edytować pól typu geometry", MessageBoxType.Error);
                }
                return false;
            }
            return true;
        }

        private bool handleInt(object cellValue)
        {
            try
            {
                int value = int.Parse(cellValue.ToString());    //zwykły parse zapisany jako  (int)cellValue  nie działa
                return true;
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą", MessageBoxType.Error);
            }
            catch (OverflowException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą od -32,768 do 32,767 ", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleBit(object cellValue)
        {
            try
            {
                int value = int.Parse(cellValue.ToString());
                if (value == 1 || value == 0)
                {
                    return true;
                }
                else
                {
                    MyMessageBox.display("\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
                    return false;
                }
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
            }
            catch (OverflowException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę 0 lub 1", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleBigint(object cellValue)
        {
            try
            {
                long value = long.Parse(cellValue.ToString());
                return true;
            }
            catch (FormatException e)
            {
                MyMessageBox.display(e.Message + "\r\nNależy wprowadzić liczbę całkowitą", MessageBoxType.Error);
            }
            return false;
        }

        private bool handleDatetime(object cellValue)
        {
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

        private bool handleDouble(object cellValue)
        {
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
        
        //skoro wartość przeszła pierwsze sprawdzenie funkcją verifyCellDataType to ta funkcja musi rozważyć tylko trzy przypadki:
        //czy zwrócona wartość nie jest nullowa (użytkownik skasował zawartość celki)
        //zwrócić string w apostrofach i zamienić przecinek na kropkę w double
        public string getConvertedValue(ref DataGridCell cell)
        {
            if (!verifyCellNull(cell))
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
            return null;
        }

        private bool verifyCellNull(DataGridCell cell)
        {
            return cell.getCellValue(cellValueTypes.newValue) == null;
        }
    }
}
