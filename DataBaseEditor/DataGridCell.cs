using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseEditor
{
   public enum cellValueTypes { oldValue, newValue}
    public enum cellIndexTypes { rowIndex, columnIndex}
   
    public class DataGridCell
    {

        private object[] cellValue;
        private int[] cellIndex;

        public string DataTypeName { get; set; }


        public DataGridCell()
        {
            cellIndex = new int[2];
            cellValue = new object[2];
            DataTypeName = "";
        }

        public void setCellValue(cellValueTypes valueType,  object value)
        {
            switch (valueType)
            {
                case cellValueTypes.oldValue:
                    cellValue[0] = value;
                    break;
                case cellValueTypes.newValue:
                    cellValue[1] = value;
                    break;
            }
        }

        public void setCellIndex(cellIndexTypes indexType, int index)
        {
            switch(indexType)
            {
                case cellIndexTypes.rowIndex:
                    cellIndex[0] = index;
                    break;
                case cellIndexTypes.columnIndex:
                    cellIndex[1] = index;
                    break;
            }            
        }

        public object getCellValue(cellValueTypes valueType)
        {
            object value = null; ;
            switch (valueType)
            {
                case cellValueTypes.oldValue:
                    value = cellValue[0];
                    break;
                case cellValueTypes.newValue:
                    value = cellValue[1];
                    break;
            }
            return value;
        }

        public int getCellIndex(cellIndexTypes indexType)
        {
            int index = 0;
            switch(indexType)
            {
                case cellIndexTypes.rowIndex:
                    index = cellIndex[0];
                    break;
                case cellIndexTypes.columnIndex:
                    index = cellIndex[1];
                    break;
            }
            return index;
        }

    }


}
