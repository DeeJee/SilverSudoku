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
using System.Diagnostics;

namespace SilverSudoku
{
    public class Sequence
    {
        private int teller = 0;
        public int NextVal
        {
            get 
            {
                Debug.WriteLine(teller);
                return teller++; } 
        }

    }
}
