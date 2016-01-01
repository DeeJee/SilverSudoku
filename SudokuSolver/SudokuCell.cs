using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SilverSudoku.SudokuSolver;


namespace SilverSudoku
{
    //TODO: maak alle cellen 0-based. de functie getdisplayvalue maakt er dan de juiste waarde van

    [DebuggerDisplay("Cel: {Key.ToString()}, Value={Value}")]
    public class SudokuCell
    {
        private SectionCollection _parents = new SectionCollection(FieldSectionType.NotSet);

        public ContentType ContentType = ContentType.Decimal;

        //temporary variable
        public string UiId = string.Empty;


        public SudokuCell()
        {
        }

        public SudokuCell(RegistrationKey key)
            : this()
        {
            Key = key;
        }

        #region Public Properties
        /// <summary>
        /// The sections that have a reference to this cell.
        /// </summary>
        public SectionCollection Parents
        {
            get { return _parents; }
            set { _parents = value; }
        }

        /// <summary>
        /// The values the cell can have.
        /// </summary>
        private List<int> _possibilities;
        public List<int> Possibilities
        {
            get
            {
                if (_possibilities == null)
                {
                    CalculatePossibilities();
                }
                return _possibilities;
            }
        }


        private ValueCollection _p;
        public ValueCollection P
        {
            get
            {
                if (_p == null)
                {
                    CalculatePossibilities();
                }
                return _p;
            }
        }

        /// <summary>
        /// The current value
        /// </summary>
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// A unique identifier
        /// </summary>
        private RegistrationKey _key;
        public RegistrationKey Key
        {
            get { return _key; }
            set
            {
                _key = value;
                UiId = _key.RowIndex.ToString() + _key.ColumnIndex.ToString();
            }
        }

        /// <summary>
        /// When true, the value of this cell is part of the puzzle definition
        /// </summary>
        private bool _given;
        public bool Given
        {
            get { return _given; }
            set { _given = value; }
        }

        public bool IsEmpty
        {
            get
            {
                return (Value == -1 ? true : false);
            }
        }

        public void ClearValue()
        {
            _value = -1;
        }

        /// <summary>
        /// True if the puzzle can be solved.
        /// </summary>
        public bool HasSolution
        {
            get
            {
                if (IsEmpty)//possibilities are only relevant when the cell is empty.
                {
                    return Possibilities.Count > 0;
                }
                else
                    return true;
            }
        }

        #endregion

        /// <summary>
        /// Calculates the possible values the cell can have.
        /// </summary>
        /// <returns></returns>
        public bool CalculatePossibilities()
        {
            _possibilities = new List<int>();
            int capacity = ((SilverSudokuSection)Parents.GetSections()[0]).NumCells;
            List<int> impossible = new List<int>();
            foreach (SilverSudokuSection section in Parents)
            {
                foreach (SudokuCell sCell in section.GetCells())
                {
                    int number = sCell.Value;
                    //Debug.WriteLine(string.Format("rij: {0}, kolom: {1}, waarde: {2}",sCell.Key.RowIndex, sCell.Key.ColumnIndex, sCell.Value.ToString()));
                    //      if (number > 0)
                    {
                        if (!impossible.Contains(number))
                        {
                            impossible.Add(number);
                        }
                    }
                }
            }
            impossible.Sort();
            for (int i = 0; i < capacity; i++)
            {
                if (!impossible.Contains(i))
                {
                    _possibilities.Add(i);
                }
            }
            if (_p == null)
            {
                _p = new ValueCollection(_possibilities);
            }
            else
            {
                _p.cells = _possibilities;
            }
            return HasSolution;
        }


        public override string ToString()
        {
            return string.Concat(_key.ToString(), Value);
        }

        /// <summary>
        /// Resets the value to the starting value
        /// </summary>
        public void Reset()
        {
            if (!Given)
            {
                Value = -1;
            }
        }

    }

    /// <summary>
    /// The type of value the cell can have. Decimal means 1-9, Hexadecimal 0-9,A-F
    /// </summary>
    public enum ContentType
    {
        HexaDecimal,
        Decimal,
    }



}
