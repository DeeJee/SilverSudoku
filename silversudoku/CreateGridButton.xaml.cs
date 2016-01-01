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

namespace SilverSudoku
{
    public partial class CreateGridButton : UserControl
    {
        private Point MouseDownLocation;
        private bool mouseIsDown;
        private Rectangle selection = new Rectangle();
        private const double threshHold = 10;
        private const int gridSize = 20;
        private List<Line> grid = new List<Line>();
        private Brush outLine;
        private double left;
        private double top;
        private TextBlock label;

        /// <summary>
        /// Number of selected rows
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            set { _rows = (int)Math.Abs(value); }
        }
        private int _rows;

        /// <summary>
        /// Number of selected rows
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set { _columns = (int)Math.Abs( value); }
        }
        private int _columns;

        public CreateGridButton()
        {
            InitializeComponent();

            LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            LayoutRoot.MouseMove += new MouseEventHandler(LayoutRoot_MouseMove);
            LayoutRoot.Children.Add(selection);

            outLine = new SolidColorBrush(Colors.Blue);
            this.Height = 30;
            this.Width = 30;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            Sudoku pg = new Sudoku();
            pg.Width = 30;
            pg.Height = 30;
            pg.SetValue(Canvas.TopProperty, (double)30);
            pg.SetValue(Canvas.LeftProperty, (double)30);
            pg.SetValue(Canvas.ZIndexProperty, 100);
            this.LayoutRoot.Children.Add(pg);
        }


        void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas layoutRoot = (Canvas)sender;

            if (mouseIsDown)
            {
                selection.SetValue(Canvas.LeftProperty, (double)(e.GetPosition(this).X < MouseDownLocation.X ? e.GetPosition(this).X : MouseDownLocation.X));
                selection.Width = (e.GetPosition(this).X < MouseDownLocation.X ? MouseDownLocation.X - e.GetPosition(this).X : e.GetPosition(this).X - MouseDownLocation.X);
                selection.SetValue(Canvas.TopProperty, (double)(e.GetPosition(this).Y < MouseDownLocation.Y ? e.GetPosition(this).Y : MouseDownLocation.Y));
                selection.Height = (e.GetPosition(this).Y < MouseDownLocation.Y ? MouseDownLocation.Y - e.GetPosition(this).Y : e.GetPosition(this).Y - MouseDownLocation.Y);

                selection.Fill = new SolidColorBrush(Color.FromArgb(200, 85, 160, 255));
                selection.Stroke = outLine;
                selection.StrokeDashArray = new DoubleCollection() { 3, 1 };
                selection.StrokeThickness = 1;

                //Resizing the canvas
                left = (double)selection.GetValue(Canvas.LeftProperty);
                top = (double)selection.GetValue(Canvas.TopProperty);
                this.Width = (this.Width < selection.Width + left + threshHold ? selection.Width + left + threshHold : this.Width);
                this.Height = (this.Height < selection.Height + top + threshHold ? selection.Height + top + threshHold : this.Height);

                DrawGrid(e);
                SetRowsAndColumns(e.GetPosition(this));
                DrawLabel(e.GetPosition(this).X,e.GetPosition(this).Y);
                

            }
        }

        private void DrawLabel(double x, double y)
        {
            LayoutRoot.Children.Remove(label);
            label = new TextBlock();
            label.Text = Rows.ToString() + "," + Columns.ToString();
            label.SetValue(Canvas.LeftProperty, x);
            label.SetValue(Canvas.TopProperty, y);
            LayoutRoot.Children.Add(label);
        }

        private void DrawGrid(MouseEventArgs e)
        {
            ClearGrid();
            //verticale lijnen
            for (int w = 0; w < selection.Width; w += gridSize)
            {
                Line line = new Line();
                line.X1 = left + w;
                line.Y1 = top;
                line.X2 = left + w;
                line.Y2 = top + selection.Height;
                line.StrokeThickness = 1;
                line.Stroke = outLine;
                LayoutRoot.Children.Add(line);
                grid.Add(line);
            }

            //horizontale lijnen
            for (int h = 0; h < selection.Height; h += gridSize)
            {
                Line line = new Line();
                line.X1 = left;
                line.Y1 = top + h;
                line.X2 = left + selection.Width;
                line.Y2 = top + h;
                line.StrokeThickness = 1;
                line.Stroke = outLine;
                LayoutRoot.Children.Add(line);
                grid.Add(line);
            }
        }

        private void ClearGrid()
        {
            foreach (Line line in grid)
            {
                LayoutRoot.Children.Remove(line);
            }
            grid.Clear();
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = false;
            LayoutRoot.Children.Remove(selection);
            selection = new Rectangle();
            LayoutRoot.Children.Add(selection);
            ClearGrid();

            SetRowsAndColumns(e.GetPosition(this));

        }

        private void SetRowsAndColumns(Point mouse)
        {
            Rows = (int)(mouse.Y - MouseDownLocation.Y) / gridSize;
            Columns = (int)(mouse.X  - MouseDownLocation.X) / gridSize;
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownLocation = e.GetPosition(this);
            mouseIsDown = true;
        }
    }
}
