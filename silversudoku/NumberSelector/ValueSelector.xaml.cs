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
using System.Windows.Markup;

namespace SilverSudoku
{
    public partial class ValueSelector : UserControl
    {
        private const double spacing = 2;
        TextBox tbx;
        private double cellHeight = 30;
        private double cellWidth = 30;
        private Point punt1;
        private Point punt2;
        private double offSet = 10;
        private TextBox blaat;
        private string selectorStyle = "SilverSudoku.Styles.ValueSelector.xaml";

        public ValueSelector(object textbox, PuzzleType type)
        {
            InitializeComponent();
            tbx = textbox as SudokuTextBox;

            if(type.Equals(PuzzleType.Sudoku9))
            {
            DrawSudoku9Selector();
            }
            else{
                DrawSudoku16Selector();
            }
            //Ellipse boundary = new Ellipse();
            //boundary.SetValue(Canvas.LeftProperty, tbx.GetValue(Canvas.LeftProperty));
            //boundary.SetValue(Canvas.TopProperty, tbx.GetValue(Canvas.TopProperty));
            //boundary.Width = tbx.Width;
            //boundary.Height =tbx.Height;
            //boundary.Stroke = new SolidColorBrush(Colors.Black);
            //boundary.StrokeThickness = 1;
            //LayoutRoot.Children.Add(boundary);

          

            //buitendiam
            // Ellipse buiten= new Ellipse();
            // buiten.Stroke = new SolidColorBrush(Colors.Black);
            // buiten.StrokeThickness = 1;
            // buiten.Height = tbx.Height*3;
            // buiten.Width = tbx.Width*3;
            //// LayoutRoot.Children.Add(buiten);
            // buiten.SetValue(Canvas.LeftProperty, (double)tbx.GetValue(Canvas.LeftProperty) - tbx.Width);
            // buiten.SetValue(Canvas.TopProperty, (double)tbx.GetValue(Canvas.TopProperty) -tbx.Height);
        }

        private void DrawSudoku9Selector()
        {
            double numberOfValues=9;

            Point middelpunt = new Point((double)tbx.GetValue(Canvas.LeftProperty) + tbx.Width / 2,
                                          (double)tbx.GetValue(Canvas.TopProperty) + tbx.Height / 2);
            double centerLeft=0;
            double centerTop=0;
            for (int i = 0; i < numberOfValues; i++)
            {
                TextBox waarde = new SudokuTextBox();
                waarde.MouseLeftButtonDown += new MouseButtonEventHandler(waarde_MouseLeftButtonDown);
                waarde.Height = cellHeight + offSet;
                waarde.Width = cellWidth + offSet;
                waarde.FontSize = tbx.FontSize;
                waarde.Text= (i + 1).ToString();
                
                waarde.Style = GetStyle();
                //waarde.Style=(Style)App.Current.Resources["ButtonStyle1"];

                waarde.MouseEnter += new MouseEventHandler(waarde_MouseEnter);
                waarde.MouseLeave += new MouseEventHandler(waarde_MouseLeave);

                double unitAngle = (double)numberOfValues;
                double straal = cellWidth / (2*Math.Sin((Math.PI / unitAngle)));
                centerLeft = middelpunt.X - Math.Cos(i / unitAngle * Math.PI * 2) * straal;
                centerTop = middelpunt.Y - Math.Sin(i / unitAngle * Math.PI * 2) * straal;

                waarde.SetValue(Canvas.LeftProperty, centerLeft - cellWidth / 2);
                waarde.SetValue(Canvas.TopProperty, centerTop - cellHeight / 2);

                if (i == 0)
                {
                    punt1=new Point(centerLeft, centerTop);
                }
                if (i == 1)
                {
                    punt2= new Point(centerLeft, centerTop);;

                }

                LayoutRoot.Children.Add(waarde);
                if(i==0)
                {
                    blaat = waarde;
                }
            }
            //Line lijn = new Line();
            //lijn.X1 = punt1.X + offSet / 2;
            //lijn.Y1 = punt1.Y + offSet / 2;
            //lijn.X2 = punt2.X + offSet / 2;
            //lijn.Y2 = punt2.Y + offSet / 2;
            //lijn.Stroke = new SolidColorBrush(Colors.Black);
            //lijn.StrokeThickness = 1;
            ////LayoutRoot.Children.Add(lijn);


            //Ellipse middelpuntSnijlijn = new Ellipse();
            //middelpuntSnijlijn.StrokeThickness = 3;
            //middelpuntSnijlijn.Stroke = new SolidColorBrush(Colors.Red);
            //middelpuntSnijlijn.Height = 1;
            //middelpuntSnijlijn.Width = 1;
            //middelpuntSnijlijn.SetValue(Canvas.LeftProperty, (double)(lijn.X1 + lijn.X2) / 2);
            //middelpuntSnijlijn.SetValue(Canvas.TopProperty, (double)(lijn.Y1 + lijn.Y2) / 2);
            //LayoutRoot.Children.Add(middelpuntSnijlijn);

            //double r = cellHeight / 2;
            //double a = offSet;
            //double rminakwadraat = (r - a) * (r - a);
            //double gedeelddoorrkwadraat = rminakwadraat / (r * r);

            //double lengte = r * Math.Sqrt(1 - gedeelddoorrkwadraat);

            //double nweHelling = -((lijn.Y2-lijn.Y1)/(lijn.X2-lijn.X1));
            //Line snijLijn = new Line();
            //snijLijn.StrokeThickness = 1;
            //snijLijn.Stroke = new SolidColorBrush(Colors.Red);
            //snijLijn.X1 = (double)middelpuntSnijlijn.GetValue(Canvas.LeftProperty) + lengte;
            //snijLijn.X2 = (double)middelpuntSnijlijn.GetValue(Canvas.LeftProperty) - lengte;
            //snijLijn.Y1 = (double)middelpuntSnijlijn.GetValue(Canvas.TopProperty) + lengte / nweHelling;
            //snijLijn.Y2 = (double)middelpuntSnijlijn.GetValue(Canvas.TopProperty) - lengte / nweHelling;

            //BezierSegment bez = new BezierSegment();
            //bez.Point1 = new Point(snijLijn.X1, snijLijn.Y1);
            //bez.Point2 = new Point(2 * punt2.X - (double)middelpuntSnijlijn.GetValue(Canvas.LeftProperty), 2 * punt2.Y - (double)middelpuntSnijlijn.GetValue(Canvas.TopProperty));
            //bez.Point3 = new Point(snijLijn.X2, snijLijn.Y2);

            //ArcSegment arc1 = new ArcSegment();
            //arc1.Point = new Point(snijLijn.X1, snijLijn.Y1);
            //arc1.Size = new Size(1, 1);
            
            //ArcSegment arc2 = new ArcSegment();
            //arc2.Point = new Point(snijLijn.X2, snijLijn.Y2);
            //arc2.Size = new Size(cellHeight, cellHeight);
            //arc2.IsLargeArc = false;         

            //Ellipse el3= new Ellipse();
            //el3.StrokeThickness = 3;
            //el3.Stroke = new SolidColorBrush(Colors.Purple);
            //el3.Height = 1;
            //el3.Width = 1;
            //el3.SetValue(Canvas.LeftProperty, snijLijn.X1);
            //el3.SetValue(Canvas.TopProperty, snijLijn.Y1);
            //LayoutRoot.Children.Add(el3);

            //el3 = new Ellipse();
            //el3.StrokeThickness = 3;
            //el3.Stroke = new SolidColorBrush(Colors.Purple);
            //el3.Height = 1;
            //el3.Width = 1;
            //el3.SetValue(Canvas.LeftProperty, snijLijn.X2);
            //el3.SetValue(Canvas.TopProperty, snijLijn.Y2);
            //LayoutRoot.Children.Add(el3);

            //PathFigure figure1 = new PathFigure();
            //figure1.StartPoint=new Point(snijLijn.X2, snijLijn.Y2);
            //figure1.Segments.Add(arc1);
            
            //PathFigure figure2 = new PathFigure();
            //figure2.StartPoint = new Point(snijLijn.X1, snijLijn.Y1);
            //figure2.Segments.Add(arc2);

            //EllipseGeometry cirkel = new EllipseGeometry();
            //cirkel.Center = punt2;
            //cirkel.RadiusX = cellHeight;
            //cirkel.RadiusY = cellHeight; 

            //PathGeometry geom = new PathGeometry();
            //geom.Figures.Add(figure1);
            //geom.Figures.Add(figure2);
            
            //Path pad = new Path();
            //pad.Data = geom;
            //pad.Fill = new SolidColorBrush(Colors.Green);
            //LayoutRoot.Children.Add(pad);
            
            //LayoutRoot.Children.Add(snijLijn);
        }

        void waarde_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox tbx = sender as TextBox;

            tbx.SetValue(Canvas.ZIndexProperty, (int)0);
        }

        void waarde_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox tbx = sender as TextBox;

            tbx.SetValue(Canvas.ZIndexProperty, (int)10);
        }
        
        private void DrawSudoku16Selector()
        {
            int numberOfValues=16;
            
            Point middelpunt = new Point((double)tbx.GetValue(Canvas.LeftProperty) + tbx.Width / 2,
                                          (double)tbx.GetValue(Canvas.TopProperty) + tbx.Height / 2);

            int numberOfCirclesInnerRing = 6;
            int numberOfCirclesOuterring = 10;
            double straalBinnen = cellWidth / (2 * Math.Sin((Math.PI / numberOfCirclesInnerRing)));
            double straalBuiten = straalBinnen + cellWidth;

            for (int i = numberOfValues; i >= numberOfCirclesOuterring; i--)
            {
                SudokuTextBox waarde = new SudokuTextBox();
                waarde.MouseLeftButtonDown += new MouseButtonEventHandler(waarde_MouseLeftButtonDown);
                waarde.Height = cellHeight;
                waarde.Width = cellWidth;
                waarde.FontSize = tbx.FontSize;
                waarde.Text = i.ToString("X");
                waarde.Style = GetStyle();
                double centerLeft = middelpunt.X - Math.Cos(i / (double)numberOfCirclesInnerRing * Math.PI * 2) * straalBinnen;
                double centerTop = middelpunt.Y - Math.Sin(i / (double)numberOfCirclesInnerRing * Math.PI * 2) * straalBinnen;

                waarde.SetValue(Canvas.LeftProperty, centerLeft - waarde.Width / 2);
                waarde.SetValue(Canvas.TopProperty, centerTop - waarde.Height / 2);

                LayoutRoot.Children.Add(waarde);
            }

            for (int i = 9; i >= 0; i--)
            {
                SudokuTextBox waarde = new SudokuTextBox();
                waarde.MouseLeftButtonDown += new MouseButtonEventHandler(waarde_MouseLeftButtonDown);
                waarde.Height = cellHeight;
                waarde.Width = cellWidth;
                waarde.FontSize = tbx.FontSize;
                waarde.Text = i.ToString("X");
                waarde.Style = GetStyle();
                double centerLeft = middelpunt.X - Math.Cos(i / (double)numberOfCirclesOuterring * Math.PI * 2) * straalBuiten;
                double centerTop = middelpunt.Y - Math.Sin(i / (double)numberOfCirclesOuterring * Math.PI * 2) * straalBuiten;

                waarde.SetValue(Canvas.LeftProperty, centerLeft - waarde.Width / 2);
                waarde.SetValue(Canvas.TopProperty, centerTop - waarde.Height / 2);

                LayoutRoot.Children.Add(waarde);
            }
        }

        public Style GetStyle()
        {
            string resource;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(selectorStyle)))
            {
                resource = reader.ReadToEnd();
            }
            return XamlReader.Load(resource) as Style;            
        }

        void waarde_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Canvas)this.Parent).Children.Remove(this);
            tbx.Text = ((SudokuTextBox)sender).Text;
        }

        public double CellHeight
        {
            get { return cellHeight; }
            set { cellHeight = value; }
        }

        public double CellWidth
        {
            get { return cellWidth; }
            set { cellWidth = value; }
        }
    }

    public enum PuzzleType
    {
        Sudoku9,
        Sudoku16
    }

  
}
