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
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Collections;

namespace SilverSudoku
{
    public partial class Sudoku : UserControl
    {
        private static string textBoxStyle = "Standaard";
        private const int HorOffset = 5;
        private const int VertOffset = 5;
        private const int cellWidth = 50;
        private const int cellHeight = 50;
        //private const string GivenCellStyle = "TextBoxNoBorder";
        private int _rows;
        private int _columns;
        BackgroundWorker worker;
        public int CellSpacing=0;
        private const double LijnDikte = 0;
        private ValueSelector _selector;
        private TextBox _prevCell;
        Dictionary<String, Style> styles = new Dictionary<string, Style>();
        private Sequence zIndex=new Sequence();

        private TextBox[,] controls;
        public Sudoku()
        {
            InitializeComponent();
            //DrawLines(9, 9);    
            GetStyles();
        }

        public Sudoku(int numRows, int numColumns)
        {
            InitializeComponent();
            _rows = numRows;
            _columns = numColumns;
            GetStyles();
            //LoadNewPuzzle("puzzles/Sudoku varia p5.xml");
            LoadNewPuzzle("puzzles/Boekje50.xml");
            this.Loaded += new RoutedEventHandler(Sudoku_Loaded);    
            
        }

        public Sudoku(string path)
        {
            InitializeComponent();
            GetStyles();
            LoadNewPuzzle(path);
            this.Loaded += new RoutedEventHandler(Sudoku_Loaded);            
        }

        public Sudoku(TextReader reader)
        {
            InitializeComponent();
            GetStyles();
            LoadNewPuzzle(reader);
            this.Loaded += new RoutedEventHandler(Sudoku_Loaded);            
        }

        void Sudoku_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (TextBox tbx in controls)
            //{
            //    if (tbx.Text == string.Empty)
            //    {
            //        string text = "";
            //        RegistrationKey key = (RegistrationKey)tbx.Tag;
            //        foreach (int item in field.GetCell(key.ColumnIndex, key.RowIndex).Possibilities)
            //        {
            //            text += item.ToString() + " ";
            //        }
            //        if (text.EndsWith(" "))
            //        {
            //            text = text.Substring(0, text.Length - 1);
            //        }
            //        ToolTipService.SetToolTip(tbx, text);
            //    }
            //}                        
        }

        private void InitCanvas(int numRows, int numColumns)
        {
            controls = new TextBox[numRows, numColumns];

            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numColumns; c++)
                {
                    TextBox tbx = new SudokuTextBox();
                    tbx.SetValue(Canvas.LeftProperty, (double)(c * (cellWidth+CellSpacing)+CellSpacing));
                    tbx.SetValue(Canvas.TopProperty, (double)(r * (cellHeight+CellSpacing)+CellSpacing));

                    tbx.TextAlignment = TextAlignment.Center;
                    tbx.VerticalAlignment = VerticalAlignment.Center;
                    tbx.FontFamily = new FontFamily("Arial");
                    tbx.FontSize = 20;
                    tbx.MaxLength = 1;
       
                    tbx.Padding = new Thickness(12);

                    TransformGroup grp = new TransformGroup();
                    grp.Children.Add(new ScaleTransform());
                    grp.Children.Add(new TranslateTransform());
                    tbx.RenderTransform = grp;

                    tbx.Width = cellWidth;
                    tbx.Height = cellHeight;
                    tbx.Tag = new RegistrationKey(r, c);
                    tbx.SetValue(TextBox.NameProperty, string.Concat(r.ToString("D2"), c.ToString("D2")));
                    AddStoryboard(tbx);
                    
                    
                    //events
                    tbx.TextChanged += new TextChangedEventHandler(tbx_TextChanged);
                    tbx.MouseLeave += new MouseEventHandler(tbx_MouseLeave);
                    tbx.MouseEnter += new MouseEventHandler(tbx_MouseEnter);
                    tbx.KeyDown += new KeyEventHandler(tbx_KeyDown);
                    tbx.MouseLeftButtonDown += new MouseButtonEventHandler(tbx_MouseLeftButtonDown);

                    LayoutRoot.Children.Add(tbx);
                    LayoutRoot.UpdateLayout();

                    controls[r, c] = tbx;
                }
            }
            DrawLines(numRows, numColumns);

            
        }

        void tbx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (gebruikInvulhulp)
            {
                TextBox tbx = sender as TextBox;                
                if (_selector != null)
                {
                    //oude selector verwijderen indien deze bestaat
                    LayoutRoot.Children.Remove(_selector);
                    _selector = null;
                    //als het een andere cel betreft direct een  nieuwe selector maken
                    if (!tbx.Equals(_prevCell))
                    {
                        if (this.field.ContentType == ContentType.Decimal)
                        {
                            _selector = new ValueSelector(sender, PuzzleType.Sudoku9);
                        }
                        else
                        {
                            _selector = new ValueSelector(sender, PuzzleType.Sudoku16);
                        }
                        LayoutRoot.Children.Add(_selector);
                    }                    
                }
                else
                {
                    //selector bestaat nog niet: maak er een
                    if (this.field.ContentType == ContentType.Decimal)
                    {
                        _selector = new ValueSelector(sender, PuzzleType.Sudoku9);
                    }
                    else
                    {
                        _selector = new ValueSelector(sender, PuzzleType.Sudoku16);
                    }
                    LayoutRoot.Children.Add(_selector);
                }
                _prevCell = tbx;
            }
        }

        private void DrawLines(int numRows, int numColumns)
        {
            for (int b = 0; b <= (int)Math.Sqrt(numRows); b++)
            {
                //rijen
                Line line = new Line();
                line.StrokeThickness = LijnDikte;
                line.Stroke = new SolidColorBrush(Colors.Black);
                
               
                //linksboven
                line.X1 = CellSpacing / 2;
                line.Y1 = b * (cellHeight + CellSpacing) * (int)Math.Sqrt(numRows) + CellSpacing / 2;
                //rechtsonder
                line.X2 = line.X1 + numColumns * (cellHeight+ CellSpacing) ;
                line.Y2 = line.Y1;

                LayoutRoot.Children.Add(line);

                //kolommen
                line = new Line();
                line.StrokeThickness = LijnDikte;
                line.Stroke = new SolidColorBrush(Colors.Black);

                //linksboven
                line.X1 = b * (cellWidth +CellSpacing) * (int)Math.Sqrt(numColumns) +CellSpacing /2;
                line.Y1 = CellSpacing / 2;
                //rechtsonder
                line.X2 = line.X1;
                line.Y2 = line.Y1 + numRows * (cellWidth + CellSpacing) ;

                LayoutRoot.Children.Add(line);
                
            }
        }


        void tbx_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            RegistrationKey key = (RegistrationKey)tbx.Tag;
            if (!showFamily)
                return;
            SectionCollection sections = datasource[key.RowIndex, key.ColumnIndex].Parents;
            foreach (SilverSudokuSection section in sections)
            {
                foreach (SudokuCell cell in section.GetCells())
                {
                    RegistrationKey destKey = cell.Key;
                    controls[destKey.RowIndex, destKey.ColumnIndex].Background = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            SolveAsync();
        }

        void tbx_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            RegistrationKey key = (RegistrationKey)tbx.Tag;

            RegistrationKey destKey = null;
            switch (e.Key)
            {
                case Key.Left:
                    if (key.ColumnIndex > 0)
                        destKey = new RegistrationKey(key.RowIndex, key.ColumnIndex - 1);
                    break;
                case Key.Right:
                    if (key.ColumnIndex < field.NumColumns - 1)
                        destKey = new RegistrationKey(key.RowIndex, key.ColumnIndex + 1);
                    break;
                case Key.Down:
                    if (key.RowIndex < field.NumRows - 1)
                        destKey = new RegistrationKey(key.RowIndex + 1, key.ColumnIndex);
                    break;
                case Key.Up:
                    if (key.RowIndex > 0)
                        destKey = new RegistrationKey(key.RowIndex - 1, key.ColumnIndex);
                    break;
                case Key.Back:
                    break;
            }

            if (destKey != null)
            {
                TextBox target = controls[destKey.RowIndex, destKey.ColumnIndex];
                target.Select(0, target.Text.Length);
                target.Focus();
            }
        }

        void tbx_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!showFamily)
                return;
            TextBox tbx = (TextBox)sender;
            RegistrationKey key = (RegistrationKey)tbx.Tag;
            SectionCollection sections = datasource[key.RowIndex, key.ColumnIndex].Parents;
            foreach (SilverSudokuSection section in sections)
            {
                foreach (SudokuCell cell in section.GetCells())
                {
                    RegistrationKey destKey = cell.Key;
                    if (cell.Given)
                        controls[destKey.RowIndex, destKey.ColumnIndex].Background = new SolidColorBrush(Colors.Green);
                    else
                        controls[destKey.RowIndex, destKey.ColumnIndex].Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        void tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            RegistrationKey key = (RegistrationKey)tbx.Tag;
            datasource[key.RowIndex, key.ColumnIndex].Value = GetStoreValue(tbx.Text);
        }


        public void SetBackColor(RegistrationKey key, Color color)
        {
            controls[key.RowIndex, key.ColumnIndex].Background = new SolidColorBrush(color);
        }

        public void DataBind()
        {
            foreach (UIElement child in LayoutRoot.Children)
            {
                TextBox tbx = child as TextBox;
                if (tbx != null)
                    tbx.Text = string.Empty;
            }

            if (field == null)
                throw new InvalidOperationException("Field property must be set to a valid SudokuField before databinding.");
            datasource = field.GetData();
            if (datasource != null)
            {
                contentType = field.ContentType;
                field.Updated += new SudokuField.UpdateEvent(field_Updated);
                field.PuzzleSolved += new SudokuField.OnPuzzleSolved(field_PuzzleSolved);
                field.StatusChanged += new SudokuField.OnStatusChanged(field_StatusChanged);

                foreach (UIElement child in LayoutRoot.Children)
                {
                    TextBox tbx = child as TextBox;
                    if (tbx != null)
                    {
                        RegistrationKey key = (RegistrationKey)tbx.Tag;
                        tbx.Text = GetDisplayValue(datasource[key.RowIndex, key.ColumnIndex].Value);
                        if (datasource[key.RowIndex, key.ColumnIndex].Given)
                        {
                            tbx.Background = new SolidColorBrush(Colors.Green);
                            tbx.Style = styles[textBoxStyle];
                        }
                        else
                        {
                            
                            //LinearGradientBrush brush = new LinearGradientBrush();
                            //GradientStop stop = new GradientStop();
                            //stop.Color = Color.FromArgb(255, 66, 113, 131);
                            //stop.Offset = 0;
                            //brush.GradientStops.Add(stop);
                            //stop = new GradientStop();
                            //stop.Color = Color.FromArgb(255, 76, 138, 168);
                            //stop.Offset = 0;
                            //brush.GradientStops.Add(stop);
                            //tbx.Background = brush;
                            //tbx.Style = (Style)Application.Current.Resources[_cellStyle];
                            tbx.Style = styles[textBoxStyle];
                            
                            //tbx.Background = new SolidColorBrush(Colors.LightGray);
                        }
                    }
                }
            }
        }

        private Style GetStyle(String style)
        {
            string resource;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(style)))
            {
                Debug.WriteLine(textBoxStyle);
                resource = reader.ReadToEnd();
            }
            return XamlReader.Load(resource) as Style;       
        }

        private Storyboard GetStoryboard(string name)
        {
            string resource;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(name)))
            {
                resource = reader.ReadToEnd();
            }
            return XamlReader.Load(resource) as Storyboard;
        }

        private void GetStyles()
        {
            foreach (ResourceDictionary dict in Resources.MergedDictionaries)
            {
                foreach (DictionaryEntry entry in dict)
                {

                    try
                    {
                        String key = (String)entry.Key;
                        Style value = (Style)entry.Value;
                        Debug.WriteLine(key);
                        if (!styles.ContainsKey(key))
                        {
                            styles.Add(key, value);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }



        void field_StatusChanged(string newStatus)
        {
            if (StatusChanged != null)
                StatusChanged(newStatus);
        }

        public void ShowValues()
        {
            foreach (UIElement child in LayoutRoot.Children)
            {
                TextBox tbx = child as TextBox;
                if (tbx != null)
                {
                    RegistrationKey key = (RegistrationKey)tbx.Tag;

                    if (tbx.CheckAccess())
                    {
                        tbx.Text=GetDisplayValue(datasource[key.RowIndex, key.ColumnIndex].Value);
                    }
                    else
                    {
                        SetTextCallback callback = new SetTextCallback(this.SetText);
                        tbx.Dispatcher.BeginInvoke(callback, new object[] { tbx, GetDisplayValue(datasource[key.RowIndex, key.ColumnIndex].Value) });
                    }
                }
            }
        }


        void field_PuzzleSolved(TimeSpan timeTaken)
        {
            if (OnExit != null)
            {
                OnExit(this, new SudokuExitEventArgs());
            }
            isRunning = false;            
            //ShowValues();
        }



        public string GetDisplayValue(int value)
        {
            string result = string.Empty;
            if (ContentType == ContentType.HexaDecimal)
            {
                if (value == -1)
                {
                    result = string.Empty;
                }
                else
                {
                    result = value.ToString("X");
                }
            }
            else
            {
                if (value == -1)
                {
                    result = string.Empty;
                }
                else
                {
                    result = (value + 1).ToString().ToUpper();
                }
            }
            return result;
        }

        public int GetStoreValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return -1;

            if (ContentType == ContentType.HexaDecimal)
            {
                return int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return Convert.ToInt32(value)-1;
            }
        }




        public void CreateNewSudoku(int rows, int columns)
        {
            Status("Creating puzzle");
            field = SudokuField.CreateNewField(rows, columns);
            DataBind();
            Status("Puzzle created");
        }

        public void LoadNewPuzzle(string path)
        {
            field = new SudokuField();
            field.LoadNewPuzzle(path);
            HandlePuzzleLoading(field);
        }
        public void LoadNewPuzzle(TextReader reader)
        {
            field = new SudokuField();
            field.LoadNewPuzzle(reader);
            HandlePuzzleLoading(field);
        }

        public void HandlePuzzleLoading(SudokuField field)
        {
            Status("loading puzzle");
            this.ContentType = field.ContentType;
            this._rows = field.NumRows;
            this._columns = field.NumColumns;
            field.AutoOutput = true;
            LayoutRoot.Children.Clear();
            InitCanvas(field.NumRows, field.NumColumns);
            DataBind();
            ShowValues();
            field.Updated += new SudokuField.UpdateEvent(field_Updated);
            this.Height = field.NumRows * (cellHeight+CellSpacing)+CellSpacing;
            this.Width = field.NumColumns * (cellWidth+CellSpacing)+CellSpacing;
            Status("Puzzle loaded");            
        }

        public void LoadTemplate(string path)
        {
            field.LoadTemplate(path);
        }

        //public void SaveNewSudoku(string path)
        //{
        //    Status("Saving puzzle");
        //    field.IsNewPuzzle = true;
        //    field.SaveSudokuAs(path);
        //    Status("Puzzle saved");
        //}
        
        public void SaveNewSudoku(IsolatedStorageFileStream fileStream)
        {
            Status("Saving puzzle");
            field.IsNewPuzzle = true;
            field.SaveSudokuAs(fileStream);
            Status("Puzzle saved");

            CreatePreview();
        }

        private void CreatePreview()
        {
            Image img = new Image();
            
        }

        public void Load(TextReader reader)
        {
            Status("Saving puzzle");
            field = new SudokuField();
            field.LoadNewPuzzle(reader);
            Status("Puzzle saved");
        }

        //public void SaveSudoku(string path)
        //{

        //    field.IsNewPuzzle = false;
        //    field.SaveSudokuAs(fileStream);
        //}

        public void SolveAsync()
        {
            worker = new BackgroundWorker(); 
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork+=new DoWorkEventHandler(worker_DoWork);
            worker.WorkerSupportsCancellation = true;
            isRunning = true; 
            worker.RunWorkerAsync();            
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnExit != null)
            {
                SudokuExitEventArgs arg = new SudokuExitEventArgs();
                if (e.Cancelled)
                {                    
                    arg.Status = ExitStatus.Cancelled;
                }
                else if (e.Error!=null)
                {
                    arg.Status = ExitStatus.Error;
                }
                else
                {
                    arg.Status = ExitStatus.Completed;
                }
                OnExit(this, arg);
            }
        }

        public bool Validate()
        {
            List<RegistrationKey> keys = field.Validate();
            foreach (RegistrationKey key in keys)
            {
                TextBox tbx = controls[key.RowIndex, key.ColumnIndex];
                tbx.Background = new SolidColorBrush(Colors.Red);
            }
            return keys.Count == 0 ? true : false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Status("Solving puzzle....");
            BackgroundWorker worker = (BackgroundWorker)sender;
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                field.Solve(worker);
            }
        }

        public List<RegistrationKey> ValidateSolution()
        {
            List<RegistrationKey> result = field.Validate();
            foreach (RegistrationKey key in result)
            {
                controls[key.RowIndex, key.ColumnIndex].Background = new SolidColorBrush(Colors.Red);
            }
            return result;
        }

        public void Reset()
        {
            field.Reset();
            DataBind();
        }

        #region Public Properties

        public bool AutoOutput
        {
            get { return field.AutoOutput; }
            set { field.AutoOutput = value; }
        }

        private bool isSolved = false;
        public bool IsSolved
        {
            get { return isSolved; }
            set { isSolved = value; }
        }

        private ContentType contentType = ContentType.Decimal;
        public ContentType ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        private int rows;
        public int Rows
        {
            get { return rows; }
            set { rows = value; }
        }

        private int columns;
        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        private bool showFamily;
        public bool ShowFamily
        {
            get { return showFamily; }
            set { showFamily = value; }
        }

        private string _cellStyle = "SudokuTekenStyle";
        public string CellStyle
        {
            get { return _cellStyle; }
            set { _cellStyle = value; }
        }

        private Boolean gebruikInvulhulp;
        public Boolean GebruikInvulhulp
        {
            get { return gebruikInvulhulp; }
            set { gebruikInvulhulp = value; }        
        }
        
        private SudokuField field;

        private SudokuCell[,] datasource;

        #endregion

        #region Private Events

        public delegate void OnStatusChanged(string newStatus);
        public event OnStatusChanged StatusChanged;


        #endregion


        #region Event handlers
        void field_Updated(object sender)
        {
            SudokuCell cell = (SudokuCell)sender;
            RegistrationKey key = cell.Key;
            TextBox output = controls[key.RowIndex, key.ColumnIndex];
            int value = datasource[key.RowIndex, key.ColumnIndex].Value;
            string text = GetDisplayValue(value);
            SetTextCallback callback = new SetTextCallback(this.SetText);
            output.Dispatcher.BeginInvoke(callback, new object[] { output, text });            
        }

        private void SetText(TextBox output, string text)
        {
            //output.SetValue(Canvas.ZIndexProperty, zIndex.NextVal);
            output.Text = text;
            //output.SetValue(Canvas.ZIndexProperty, 100);
            Storyboard sb = (Storyboard)output.Resources["TextChanged"];
            sb.Begin();            
        }

        private void AddStoryboard(TextBox output)
        {
            TimeSpan begin = new TimeSpan(0, 0, 0);
            TimeSpan midden = new TimeSpan(0, 0, 0, 0, 250);
            TimeSpan eind = new TimeSpan(0, 0, 0, 0, 500);

            Storyboard sb = new Storyboard();

            ColorAnimation ca = new ColorAnimation();
            ca.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            ca.From = Colors.Red;
            ca.To = Colors.White;
            ca.SetValue(Storyboard.TargetNameProperty, output.Name);
            ca.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(TextBox.Background).(SolidColorBrush.Color)"));

            sb.Children.Add(ca);

            //DoubleAnimationUsingKeyFrames da = CreateDoubleAnimation(begin, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)", output.Name);
            //da.KeyFrames.Add(CreateKeyFrame(begin, 2));
            //da.KeyFrames.Add(CreateKeyFrame(midden, 1));
            //sb.Children.Add(da);

            //da = CreateDoubleAnimation(begin, "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)", output.Name);
            //da.KeyFrames.Add(CreateKeyFrame(begin, 2));
            //da.KeyFrames.Add(CreateKeyFrame(midden, 1));
            //sb.Children.Add(da);

            //da = CreateDoubleAnimation(begin, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.X)", output.Name);
            //da.KeyFrames.Add(CreateKeyFrame(begin, -15));
            //da.KeyFrames.Add(CreateKeyFrame(midden, 0));
            //sb.Children.Add(da);

            //da = CreateDoubleAnimation(begin, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(TranslateTransform.Y)", output.Name);
            //da.KeyFrames.Add(CreateKeyFrame(begin, -15));
            //da.KeyFrames.Add(CreateKeyFrame(midden, 0));
            //sb.Children.Add(da);

            //Storyboard sb2=(Storyboard)this.Resources["Storyboard2"];
            //Storyboard sb2 = GetStoryboard("SilverSudoku.Storyboard.Storyboard1.xaml");


            output.Resources.Add("TextChanged", sb);
        }

        private DoubleAnimationUsingKeyFrames CreateDoubleAnimation(TimeSpan begintime, string targetproperty, string targetName)
        {
            DoubleAnimationUsingKeyFrames da = new DoubleAnimationUsingKeyFrames();
            da.BeginTime = begintime;
            da.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(targetproperty, new Object[0]));
            da.SetValue(Storyboard.TargetNameProperty, targetName);
            return da;
        }

        private DoubleKeyFrame CreateKeyFrame(TimeSpan timespan, int value)
        {
            SplineDoubleKeyFrame sdkf = new SplineDoubleKeyFrame();
            sdkf.KeyTime = KeyTime.FromTimeSpan(timespan);
            sdkf.Value = value;

            return sdkf;
        }


        delegate void SetTextCallback(TextBox tbx, string value);

        #endregion

        public void CalculatePossibilities()
        {
            field.CalculatePossibilities();
        }

        private void Status(string message)
        {
            if (StatusChanged != null)
                StatusChanged(message);
        }


        /*
        public void Render(Control container)
        {
            if (SectionType == FieldSectionType.Block)
            {
                RenderAsBlock(container);
            }
            else
                RenderAsRow(container);
        }

        private void RenderAsBlock(Control container)
        {
            Panel block = new Panel();
            block.BorderStyle = BorderStyle.FixedSingle;
            block.Location = new Point(Key.ColumnIndex * CellSize * (int)Math.Sqrt(NumCells), Key.RowIndex * CellSize * (int)Math.Sqrt(NumCells));
            block.Size = new Size(CellSize * (int)Math.Sqrt(NumCells), CellSize * (int)Math.Sqrt(NumCells));
            container.Controls.Add(block);

            foreach (SudokuCell cell in GetCells())
            {
                cell.Render(block, cellSize);
                int rowFactor = (int)cell.Key.RowIndex % (int)Math.Sqrt(NumCells);
                int columnFactor = (int)cell.Key.ColumnIndex % (int)Math.Sqrt(NumCells);
                cell.Location = new Point((columnFactor) * CellSize, (rowFactor) * CellSize);
            }
        }

        private void RenderAsRow(Control container)
        {
            foreach (SudokuCell cell in GetCells())
            {
                cell.Render(container, CellSize);
            }
        }
        */

        public int NumRows
        {
            get { return _rows; }
        }

        public delegate void Exit(object sender, SudokuExitEventArgs e);
        public event Exit OnExit;

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        public void Stop()
        {
            worker.CancelAsync();
        }

        public void BeginStand()
        {
            field.Reset();
            ShowValues();
        }



        public void SetStyle(string newStyle)
        {
            for (int i = 0; i < LayoutRoot.Children.Count; i++)
            {
                TextBox tbx = LayoutRoot.Children[i] as TextBox;
                if (tbx != null)
                {
                    LayoutRoot.Children.RemoveAt(i);
                    i--;
                }
            }
            textBoxStyle = newStyle;
            InitCanvas(NumRows, NumRows);
            DataBind();
            this.LayoutRoot.UpdateLayout();
        }

        public String[] Styles
        {
            get
            {
                String[] keys = new String[styles.Keys.Count];
                int index = 0;
                foreach (String key in styles.Keys)
                {
                    keys[index] = key;
                    index++;
                }
                return keys;
            }
        }

        [DllImport("kernel32.dll")]        
        public static extern bool Beep(int BeepFreq, int BeepDuration);
    }
}
