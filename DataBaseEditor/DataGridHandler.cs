using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseEditor
{

    //tworzy powiązanie danych uzyskanych z kwerendy z danymi wyświetlonymi w datagridzie
    //co pozwala na jednoznaczne stwierdzenie jaki wynik kwerendy gdzie jest wyświetlony
    //a przez to rejestrowanie zmian
    public class DataGridHandler: IDisposable
    {
        private Dictionary<int, object> dataGridRowDict;   //kluczem jest index wiersza, wartością ID (lub inny klucz główny) identyfikujący jednoznacznie wiersz wyników kwerendy zasilającej datagrid
        private Dictionary<int, DataGridCell> changedCellsDict; //przechowuje zrobione przez użytkownika zmiany w celkach datagridu;kluczem jest kolejny numer zmiany, wartością zmieniona celka
        private int changeNumber = 0;
       
        public DataGridHandler()
        {
            dataGridRowDict = new Dictionary<int, object>();
            changedCellsDict = new Dictionary<int, DataGridCell>();
        }

        public bool addChangedCell(DataGridCell cell)
        {
            CellConverter cellConverter = new CellConverter();
            if (cellConverter.verifyCellDataType(ref cell))
            {
                changedCellsDict.Add(changeNumber, cell);
                changeNumber++;
                return true;
            }
            return false;
        }        

        public void addDataGridIndex(int rowIndex, object primaryKey)
        {
            dataGridRowDict.Add(rowIndex, primaryKey);
        }

        public Dictionary<int, object> getDataGridIndex()
        {
            return dataGridRowDict;
        }

        public Dictionary<int, DataGridCell> getCellsChanged()
        {
            return changedCellsDict;
        }

        public DataGridCell getLastCellChangedAndUndoChanges()
        {
            if (changeNumber > 0)
            {
                DataGridCell cell;
                changedCellsDict.TryGetValue(changeNumber-1, out cell);
                changedCellsDict.Remove(changeNumber - 1);
                changeNumber--;
                return cell;
            }
            else
            {
                return null;
            }            
        }

        public bool checkChangesExist()
        {
            return changedCellsDict.Values.Count > 0;
        }

        public object getCellPrimaryKey(DataGridCell cell)
        {
            int rowIndex = cell.getCellIndex(cellIndexTypes.rowIndex);
            object primaryKey;
            dataGridRowDict.TryGetValue(rowIndex, out primaryKey);
            return primaryKey;
        }


        #region IDisposable Support
        private bool disposedValue = false; // Aby wykryć nadmiarowe wywołania

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: wyczyść stan zarządzany (obiekty zarządzane).
                }

                // TODO: Zwolnić niezarządzane zasoby (niezarządzane obiekty) i przesłonić poniższy finalizator.
                // TODO: ustaw wartość null dla dużych pól.

                disposedValue = true;
            }
        }

        // TODO: Przesłonić finalizator tylko w sytuacji, gdy powyższa metoda Dispose(bool disposing) zawiera kod służący do zwalniania niezarządzanych zasobów.
        // ~DataGridHandler()
        // {
        //   // Nie zmieniaj tego kodu. Umieść kod czyszczący w powyższej metodzie Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Ten kod został dodany w celu prawidłowego zaimplementowania wzorca rozporządzającego.
        public void Dispose()
        {
            // Nie zmieniaj tego kodu. Umieść kod czyszczący w powyższej metodzie Dispose(bool disposing).
            Dispose(true);
            // TODO: Usunąć komentarz z poniższego wiersza, jeśli finalizator został przesłonięty powyżej.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
