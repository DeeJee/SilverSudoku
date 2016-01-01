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

namespace SilverSudoku
{
    public class SudokuExitEventArgs:EventArgs
    {
        private ExitStatus _status;
        public ExitStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

    }


    public enum ExitStatus
    {
        Completed,
        Cancelled,
        Error
    }
}
