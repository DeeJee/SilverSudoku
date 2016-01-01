using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverSudoku;
using System.Collections.Generic;

namespace SilverSudoku
{
    /// <summary>
    /// This class holds the possible values the cell can have
    /// </summary>
    public class ValueCollection
    {
        public List<int> cells = new List<int>();
        int _current = -1;

        public ValueCollection(List<int> values)
        {
            cells = values;
        }


        #region IEnumerator<int> Members

        public int Current
        {
            get { return cells[_current]; }
        }

        public bool MoveNext()
        {
            if (_current != cells.Count-1)
            {
                _current++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _current = -1;
        }

        #endregion


        public void Remove(int possibility)
        {
            cells.Remove(possibility);
        }
        
    }
}
