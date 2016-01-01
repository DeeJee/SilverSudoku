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
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;


namespace SilverSudoku
{
    public partial class LayoutScreen : UserControl
    {

        string currSection;

        double width = 30;
        int height = 30;

        int numberOfCells = 16;
        private List<Line> grid = new List<Line>();
        private List<Rectangle> prevSelection = new List<Rectangle>();
        private Rectangle selection = new Rectangle();
        private Point MouseDownLocation;
        private bool mouseIsDown;
        private Brush outLine = new SolidColorBrush(Colors.Black);
        private Brush background = new SolidColorBrush(Colors.White);
        private Brush selectedBackground = new SolidColorBrush(Colors.Orange);
        private bool ctrlPressed = false;
        private Brush selectionColor = new SolidColorBrush(Color.FromArgb(200, 85, 160, 255));
        private bool mouseWasMoved = false;
        private List<Rectangle>[] sections = new List<Rectangle>[16];
        Color[] colors = new Color[16];

        


        protected Brush SelectedbackGround
        {
            get { return selectedBackground; }
            set { selectedBackground = value; }
        }
        
        public LayoutScreen()
        {
            InitializeComponent();
            DrawGrid();

            this.MouseMove += new MouseEventHandler(LayoutScreen_MouseMove);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            this.KeyDown += new KeyEventHandler(LayoutScreen_KeyDown);
            this.KeyUp += new KeyEventHandler(LayoutScreen_KeyUp);

            AddSection.Click += new RoutedEventHandler(AddSection_Click);
            InitSection();
            
        }

        void tbx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LayoutRoot_MouseLeftButtonDown(sender, e);
            e.Handled = true;
        }

        void tbx_MouseMove(object sender, MouseEventArgs e)
        {
            LayoutScreen_MouseMove(sender, e);
        }

        void tbx_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LayoutRoot_MouseLeftButtonUp(sender, e);

            if (!mouseWasMoved)
            {
                ((TextBox)sender).Text = ((ListBoxItem)Sections.SelectedItem).Tag.ToString();
            }
            mouseWasMoved = false;
        }

        void AddSection_Click(object sender, RoutedEventArgs e)
        {
       
            SaveTemplate();
            LoadTemplate();
        }

        void LayoutScreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl)
                ctrlPressed = false;


        }

        void LayoutScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl)
                ctrlPressed = true;
        }

        void LayoutScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                mouseWasMoved = true;
                foreach (UIElement element in Field.Children)
                {
                    Rectangle rect = element as Rectangle;
                    if (rect != null)
                    {
                        if (SelectionContains(rect))
                        {
                            if (ctrlPressed)
                            {
                                if (prevSelection.Contains(rect))
                                {
                                    //rect.Background = null;
                                    prevSelection.Remove(rect);
                                }
                            }
                            else//add to selection
                            {
                                rect.Fill = new SolidColorBrush(colors[Sections.SelectedIndex]);
                                //rect.Text = currSection.ToString(); ;

                                if (!prevSelection.Contains(rect))
                                {
                                    prevSelection.Add(rect);
                                }
                            } 

                            //hier stond het
                           
                            
                        }
                        else if (prevSelection.Contains(rect))
                        {
                            rect.Fill= null;
                            prevSelection.Remove(rect);
                        }
                    }
                }

                selection.SetValue(Canvas.LeftProperty, (double)(e.GetPosition(Field).X < MouseDownLocation.X ? e.GetPosition(Field).X : MouseDownLocation.X));
                selection.Width = (e.GetPosition(Field).X < MouseDownLocation.X ? MouseDownLocation.X - e.GetPosition(Field).X : e.GetPosition(Field).X - MouseDownLocation.X);
                selection.SetValue(Canvas.TopProperty, (double)(e.GetPosition(Field).Y < MouseDownLocation.Y ? e.GetPosition(Field).Y : MouseDownLocation.Y));
                selection.Height = (e.GetPosition(Field).Y < MouseDownLocation.Y ? MouseDownLocation.Y - e.GetPosition(Field).Y : e.GetPosition(Field).Y - MouseDownLocation.Y);

                selection.Fill = selectionColor;
                selection.Stroke = outLine;
                selection.StrokeDashArray = new DoubleCollection() { 3, 1 };
                selection.StrokeThickness = 1;
                if (!Field.Children.Contains(selection))
                {
                    Field.Children.Add(selection);
                }
            }
        }

        private bool SelectionContains(Rectangle rect)
        {
            double rectLeft = (double)rect.GetValue(Canvas.LeftProperty);
            double rectTop = (double)rect.GetValue(Canvas.TopProperty);
            double selectionLeft = (double)selection.GetValue(Canvas.LeftProperty);
            double selectionTop = (double)selection.GetValue(Canvas.TopProperty);

            if (rectLeft > selectionLeft &&
                 rectLeft + rect.Width < selectionLeft + selection.Width &&
                rectTop > selectionTop &&
                rectTop + rect.Height < selectionTop + selection.Height)
            {
                return true;
            }

            return false;
        }


        private void DrawGrid()
        {
            //verticale lijnen
            for (int r = 0; r < numberOfCells; r++)
            {
                for (int c = 0; c < numberOfCells; c++)
                {
                    Rectangle rect = new Rectangle();
                    //zorgt ervoor dat de clickevents doorbubbelen naar boven
                    rect.IsHitTestVisible = false;
                    rect.Width = width;
                    rect.Height = height;
                    rect.SetValue(Canvas.LeftProperty, c * width);
                    rect.SetValue(Canvas.TopProperty, (double)r * height);

                    rect.StrokeThickness = 1;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Tag = new RegistrationKey(r, c);
                    Debug.WriteLine(new RegistrationKey(r, c).ToString());
                    rect.MouseLeftButtonDown += new MouseButtonEventHandler(tbx_MouseLeftButtonDown);
                    rect.MouseLeftButtonUp += new MouseButtonEventHandler(tbx_MouseLeftButtonUp);
                    rect.MouseMove += new MouseEventHandler(tbx_MouseMove);
                    Field.Children.Add(rect);
                }
            }

            FieldColumn.Width = new GridLength(numberOfCells * width);
            FieldRow.Height = new GridLength(numberOfCells * height);
            SectionColumn.Width = new GridLength(Sections.Width);
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = false;
            Field.Children.Remove(selection);
            
            sections[Sections.SelectedIndex] = prevSelection;
            //sets the text of the selected cells
            foreach (Rectangle tbx in prevSelection)
            {
                //tbx.Text = ((ListBoxItem)Sections.SelectedItem).Tag.ToString();
                tbx.Fill= ((ListBoxItem)Sections.SelectedItem).Background;
            }

            prevSelection = new List<Rectangle>();



        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownLocation = e.GetPosition(Field);
            mouseIsDown = true;            
        }

        //private void ClearGrid()
        //{
        //    //foreach (Line line in grid)
        //    //{
        //    //    LayoutRoot.Children.Remove(line);
        //    //}
        //    foreach (Rectangle rect in rects)
        //    {
        //        rect.Fill = background; 
        //    }
        //    rects.Clear();
        //    grid.Clear();
        //}
        private void InitSection()
        {
            ColorSwitcher cs = new ColorSwitcher();
               

            for (int i = 0; i < numberOfCells; i++)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = "Section" + i.ToString();
                itm.Tag = i;
                colors[i] = cs.NextColor();
                itm.Background=new SolidColorBrush(colors[i]);
                itm.MouseLeftButtonUp += new MouseButtonEventHandler(itm_MouseLeftButtonUp);
                Sections.Items.Add(itm);
                
            }
            Sections.SelectedIndex = 0;
        }

        void itm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedbackGround = ((ListBoxItem)sender).Background;
            currSection = ((ListBoxItem)sender).Tag.ToString();
        }

        private void SaveTemplate()
        {
            IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().CreateFile("template");

            int rowNumber = 0;

            object[] newRows = new object[16];

            for (int i = 0; i < 16; i++)
            {
                int columnNumber = 0;
                object[] newCells = new object[16];
                if (sections[i] != null)
                {
                    foreach (Rectangle cell in sections[i])
                    {
                        RegistrationKey key = (RegistrationKey)cell.Tag;
                        XElement newCell = new XElement("Cell");
                        newCell.SetAttributeValue("Row", key.RowIndex.ToString());
                        newCell.SetAttributeValue("Column", key.ColumnIndex.ToString());
                        //newCell.SetAttributeValue("Section", cell.Text);
                        newCells[columnNumber] = newCell;

                        columnNumber++;
                    }
                }
                XElement newRow = new XElement("Row", newCells);
                newRow.SetAttributeValue("Number", rowNumber.ToString());
                newRows[rowNumber] = newRow;

                rowNumber++;
            }


            XElement newValues = new XElement("cellDefinitions", newRows);
            using (StreamWriter wr = new StreamWriter(stream))
            {
                try
                {
                    wr.WriteLine(newValues.ToString());
                    wr.Close();
                }
                catch (IOException)
                {
                    //TODO: errorhandling implementeren
                    //TODO: methods van commentaar voorzien
                }
                finally
                {
                    wr.Close();
                }
            }
        }

        private void LoadTemplate()
        {
            IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("template",FileMode.Open);
            StreamReader rdr = new StreamReader(stream);
            Debug.WriteLine(rdr.ReadToEnd());
        }
    }
}
