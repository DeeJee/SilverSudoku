using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Diagnostics;
using SilverSudoku;


namespace SilverSudoku.SudokuSolver
{

    /// <summary>
    /// 
    /// </summary>
    public enum FieldSectionType
    {
        NotSet,
        Block,
        Column,
        Row,
    }

    [DebuggerDisplay("{_sectionType}, Row: {_key.RowIndex}, Column:{_key.ColumnIndex}")]
    public class SilverSudokuSection
    {
        List<SudokuCell> cells= new List<SudokuCell>();
        
        public SudokuCell CreateCell()
        {
            SudokuCell cell=new SudokuCell();
            cells.Add(cell);
            return cell;
        }

        public SudokuCell CreateCell(RegistrationKey key)
        {
            SudokuCell cell = new SudokuCell(key);
            cells.Add(cell);
            return cell;
        }

        public static int numbersections = 0;
        public SilverSudokuSection(int NumCells)
        {
            _numCells = NumCells;
            numbersections++;
        }

        public SilverSudokuSection(int NumCells, RegistrationKey key)
            : this(NumCells)
        {
            _key = key;
        }

        /// <summary>
        /// The number of cells the section contains
        /// </summary>
        private int _numCells;
        public int NumCells
        {
            get { return _numCells; }
            set { _numCells = value; }
        }

        private RegistrationKey _key;
        /// <summary>
        /// A unique identifier
        /// </summary>
        public RegistrationKey Key
        {
            get { return _key; }
            set { _key = value; }
        }
        
        private FieldSectionType _sectionType;
        /// <summary>
        /// The type of section. Valid values are Row, Column, Block
        /// </summary>
        public FieldSectionType SectionType
        {
            get { return _sectionType; }
            set { _sectionType = value; }
        }

        IDictionary<RegistrationKey, SudokuCell> _cells = new Dictionary<RegistrationKey, SudokuCell>();

        /// <summary>
        /// Registers a cell as a member of this section
        /// </summary>
        /// <param name="cell"></param>
        public void RegisterCell(SudokuCell cell)
        {
            _cells.Add(cell.Key, cell);
        }

        #region IEnumerator Members

        public object Current
        {
            get
            {
                SudokuCell cell = null;
                _cells.TryGetValue(new RegistrationKey(_key.RowIndex, _key.ColumnIndex), out cell);
                return cell;
            }
        }

        public bool MoveNext()
        {
            switch (this._sectionType)
            {
                case FieldSectionType.Column:
                    if (_key.RowIndex < _numCells - 1)
                        _key.RowIndex++;
                    else
                        return false;
                    break;
                case FieldSectionType.Row:
                    if (_key.ColumnIndex < _numCells - 1)
                        _key.ColumnIndex++;
                    else
                        return false;
                    break;
                case FieldSectionType.Block:
                    if (_key.RowIndex < _numCells - 1)
                        _key.RowIndex++;
                    else
                        return false;
                    if (_key.ColumnIndex < _numCells - 1)
                        _key.ColumnIndex++;
                    else
                        return false;

                    break;
            }
            return true;
        }

        public void Reset()
        {
            switch (this._sectionType)
            {
                case FieldSectionType.Column:
                    _key.RowIndex = -1;
                    break;
                case FieldSectionType.Row:
                    _key.ColumnIndex = -1;
                    break;
            }
        }

        public ICollection<SudokuCell> GetCells()
        {
            return _cells.Values;
        }

        #endregion


        public override string ToString()
        {
            return String.Format("Section, Type:{0}, Key:{1}", _sectionType.ToString(), Key.ToString());
        }

        /// <summary>
        /// Calculates the possible values of the cells in this section
        /// </summary>
        /// <returns></returns>
        public bool CalculatePossibilities()
        {
            //TODO: optimaliseren. direct als retvalue=false gezet wordt de lus onderbreken
            bool retValue = true;
            foreach (SudokuCell cell in GetCells())
            {
                bool hasSolution=cell.CalculatePossibilities();
                if (!hasSolution)
                    retValue = false;                
            }
            return retValue;
        }

        /// <summary>
        /// Retrieves an array with the possibilities of the cell
        /// </summary>
        /// <param name="key">The key of the cell for which to retrieve the possibilities</param>
        /// <returns></returns>
        public List<int> GetPossibilities(RegistrationKey key)
        {
            SudokuCell cell=GetCellByKey(key);
            return cell.Possibilities;
        }

        /// <summary>
        /// Retrieves a cell by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SudokuCell GetCellByKey(RegistrationKey key)
        {
            SudokuCell cell = null;
            _cells.TryGetValue(key, out cell);
            return cell;
        }

        /// <summary>
        /// Validates the section
        /// </summary>
        /// <returns></returns>
        public List<RegistrationKey> Validate()
        {
            List<RegistrationKey> result = new List<RegistrationKey>();
            
            int oldValue = -1;
            foreach(SudokuCell cell in _cells.Values)
            {
                if (cell.Value < 0 | cell.Value > NumCells-1)
                    result.Add(cell.Key);

                if (cell.Value == oldValue)
                    result.Add(cell.Key);

                oldValue = cell.Value;
            }
            return result;
        }
    }


}
