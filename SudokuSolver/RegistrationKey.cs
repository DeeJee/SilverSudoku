using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SilverSudoku.SudokuSolver
{
    [DebuggerDisplay("RowIndex={_rowIndex}, ColumnIndex={_columnIndex}")]
    public class RegistrationKey
    {
        public RegistrationKey()
        {
        }

        public RegistrationKey(int rowIndex, int columnIndex)
        {
            _rowIndex = rowIndex;
            _columnIndex = columnIndex;
        }

        private int _rowIndex;
        public int RowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }

        private int _columnIndex;
        public int ColumnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }

        public override bool Equals(object obj)
        {
            RegistrationKey otherKey=obj as RegistrationKey;
            if (otherKey == null) return false;
            if (otherKey._columnIndex == this._columnIndex &&
                    otherKey._rowIndex == this._rowIndex)
                {
                    return true;
                }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (_rowIndex + _columnIndex).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Row:{0:D2}, Column:{1:D2}",_rowIndex,_columnIndex);
        }

    }
}

