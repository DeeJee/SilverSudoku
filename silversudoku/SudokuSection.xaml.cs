using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverSudoku.SudokuSolver;
using System.Xml.Linq;

namespace SilverSudoku
{
    public partial class SudokuSection : UserControl
    {
        List<SudokuCell> cells= new List<SudokuCell>();
        

        public SudokuSection(RegistrationKey key)
        {
            InitializeComponent();         
        }

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
    }
}
