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
using System.Diagnostics;

using Telerik.Windows.Controls;
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Windows.Controls.Primitives;
using System.Resources;
using System.Collections;
using System.Threading;


namespace SilverSudoku
{
    public partial class MainMenu : UserControl
    {
        private Sudoku sudoku;
        


        public MainMenu()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainMenu_Loaded);
            Stijl.Loaded += new RoutedEventHandler(Stijl_Loaded);


            //new TextBlock().Margin;            
        }

        void Stijl_Loaded(object sender, RoutedEventArgs e)
        {
            RadMenuItem styles = sender as RadMenuItem;
            if (!styles.HasItems)
            {
                foreach (String name in sudoku.Styles)
                {
                    RadMenuItem itm = new RadMenuItem();
                    itm.Name = name;
                    itm.Header = name;
                    itm.Click += new RoutedEventHandler(StyleItem_Click);
                    styles.Items.Add(itm);
                }
            }
        }

        void MainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            RecentlyOpenedFilesHandler handler = new RecentlyOpenedFilesHandler();
            RecentFiles recentFiles = handler.GetRecentlyOpenedFiles();

            if (recentFiles != null)
            {
                foreach (RecentFile file in recentFiles)
                {
                    RadMenuItem qlItems = new RadMenuItem();
                    qlItems.Click += new RoutedEventHandler(qlItems_Click);
                    qlItems.Header = file.FileName;
                    qlItems.Tag = file;
                    Bestand.Items.Add(qlItems);
                }
            }

            foreach (RadMenuItem item in Stijl.Items)
            {
                string uri = "images/styles/" + item.Name + ".png";
                Rectangle rect = new Rectangle();
                rect.Width = 100;
                rect.Height = 100;

                ImageBrush br = new ImageBrush();
                br.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
                rect.Fill = br;

                ToolTipService.SetToolTip(item, rect);
            }
        }

        void qlItems_Click(object sender, RoutedEventArgs e)
        {
            RadMenuItem item = (RadMenuItem)sender;
            RecentFile file = (RecentFile)item.Tag;
            if (file.FileLocation == FileLocation.Assembly)
            {//load from filesystem
                LoadFromAssembly(file.FileName);
            }
            else
            {
                LoadFromIsolatedStorage((string)item.Header);
            }
        }

        private void LoadFromAssembly(string path)
        {
            //remove current puzzle
            if (sudoku != null)
            {
                LayoutRoot.Children.Remove(sudoku);
                sudoku = null;
            }
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(path);
            StreamReader rdr = new StreamReader(stream);
            sudoku = new Sudoku(rdr);
            sudoku.SetValue(Grid.ColumnProperty, 0);
            sudoku.SetValue(Grid.RowProperty, 1);
            sudoku.OnExit += new Sudoku.Exit(sudoku_OnExit);
            sudoku.ShowFamily = ToonFamilie.IsChecked;
            sudoku.StatusChanged += new Sudoku.OnStatusChanged(sudoku_StatusChanged);

            LayoutRoot.Children.Add(sudoku);
            LayoutRoot.ColumnDefinitions[0].Width = new GridLength(sudoku.Width);
            LayoutRoot.RowDefinitions[1].Height = new GridLength(sudoku.Height);

            SudokuMenu.IsEnabled = true;
            Visueel.IsEnabled = true;
            RecentlyOpenedFilesHandler handler = new RecentlyOpenedFilesHandler();
            handler.StoreRecentlyOpenedFile(path, FileLocation.Assembly);
        }

        private void Laden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Alle bestanden (*.*)|*.*";
            dialog.Multiselect = false;
            dialog.ShowDialog();
            FileInfo info = dialog.File;
            if (info != null)
            {
                LoadFromFileSystem(info);
            }
        }

        private void LoadFromFileSystem(FileInfo info)
        {
            //remove current puzzle
            if (sudoku != null)
            {
                LayoutRoot.Children.Remove(sudoku);
                sudoku = null;
            }
            //open new one
            StreamReader rdr = info.OpenText();
            sudoku = new Sudoku(rdr);
            sudoku.SetValue(Grid.ColumnProperty, 0);
            sudoku.SetValue(Grid.RowProperty, 1);
            sudoku.OnExit += new Sudoku.Exit(sudoku_OnExit);
            sudoku.ShowFamily = ToonFamilie.IsChecked;
            sudoku.StatusChanged += new Sudoku.OnStatusChanged(sudoku_StatusChanged);

            LayoutRoot.Children.Add(sudoku);
            LayoutRoot.ColumnDefinitions[0].Width = new GridLength(sudoku.Width);
            LayoutRoot.RowDefinitions[1].Height = new GridLength(sudoku.Height);

            SudokuMenu.IsEnabled = true;
            Visueel.IsEnabled = true;
            RecentlyOpenedFilesHandler handler = new RecentlyOpenedFilesHandler();
            handler.StoreRecentlyOpenedFile(info.Name, FileLocation.FileSystem);
        }

        void sudoku_StatusChanged(string newStatus)
        {
            StatusUpdateCallback callback = new StatusUpdateCallback(this.UpdateStatus);
            Status.Dispatcher.BeginInvoke(callback, new object[] { newStatus });
        }

        public delegate void StatusUpdateCallback(string newStatus);

        private void UpdateStatus(string newStatus)
        {
            Status.Text = newStatus;
        }

        void sudoku_OnExit(object sender, SudokuExitEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                if (e.Status == ExitStatus.Cancelled)
                {
                    Status.Text = "Operation cancelled";
                }
                Oplossen.IsEnabled = true;
                Stoppen.IsEnabled = false;
                sudoku.ShowValues();                
            });
        }

        private void Oplossen_Click(object sender, RoutedEventArgs e)
        {
            Oplossen.IsEnabled = false;
            Stoppen.IsEnabled = true;
            if (sudoku != null)
            {
                sudoku.SolveAsync();
            }
        }

        private void Stoppen_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.IsRunning)
            {
                sudoku.Stop();
            }
        }

        private void ToonFamilie_Click(object sender, RoutedEventArgs e)
        {
            sudoku.ShowFamily = ToonFamilie.IsChecked;
        }

        private void Valideren_Click(object sender, RoutedEventArgs e)
        {
            if (sudoku.Validate())
            {
                HtmlPage.Window.Alert("Valide");
            }
            else
            {
                HtmlPage.Window.Alert("Niet valide!");
            }
        }

        private void Afsluiten_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Animaties_Click(object sender, RoutedEventArgs e)
        {
            sudoku.AutoOutput = Animaties.IsChecked;
        }

        private void Beginstand_Click(object sender, RoutedEventArgs e)
        {
            sudoku.BeginStand();
        }

        private void StyleItem_Click(object sender, RoutedEventArgs e)
        {
            //LayoutRoot.Children.Remove(preview);
            RadMenuItem clickedButton = (RadMenuItem)sender;

            foreach (UIElement elem in Stijl.Items)
            {
                RadMenuItem rmi = (RadMenuItem)elem;
                rmi.IsChecked = false;
            }
            clickedButton.IsChecked = true;
            sudoku.SetStyle(clickedButton.Name);
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            IsoFileExplorerUI exp = new IsoFileExplorerUI(IsoFileExplorerMode.Save);
            exp.OnUnloaded += new IsoFileExplorerUI.Unload(exp_Unloaded);

            tip.Child = exp;
            tip.IsOpen = true;
        }

        void exp_Unloaded(object sender, IsoFileExplorerEventArgs e)
        {
            if (!e.Status.Equals(IsoFileExplorerStatus.Cancelled))
            {
                IsoFileExplorerUI exp = (IsoFileExplorerUI)sender;
                if (exp.Mode == IsoFileExplorerMode.Load)
                {
                    if (exp.FileLocation == FileLocation.IsolatedStorage)
                    {
                        LoadFromIsolatedStorage(exp.FileName);
                    }
                    else if (exp.FileLocation == FileLocation.Assembly)
                    {
                        LoadFromAssembly(exp.FileName);
                    }
                }
                else
                {
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                    IsolatedStorageFileStream fileStream = file.CreateFile(exp.FileName);
                    sudoku.SaveNewSudoku(fileStream);
                }
            }
            tip.Child = null;
            tip.IsOpen = false;


        }

        private void LoadFromIsolatedStorage(string fileName)
        {
            //close current puzzle
            if (sudoku != null)
            {
                LayoutRoot.Children.Remove(sudoku);
                sudoku = null;
            }

            //open new puzzle
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = file.OpenFile(fileName, FileMode.Open);
            TextReader reader = new StreamReader(fileStream);
            sudoku = new Sudoku(reader);

            sudoku.SetValue(Grid.ColumnProperty, 0);
            sudoku.SetValue(Grid.RowProperty, 1);
            sudoku.OnExit += new Sudoku.Exit(sudoku_OnExit);
            sudoku.ShowFamily = ToonFamilie.IsChecked;
            sudoku.StatusChanged += new Sudoku.OnStatusChanged(sudoku_StatusChanged);

            LayoutRoot.Children.Add(sudoku);
            LayoutRoot.ColumnDefinitions[0].Width = new GridLength(sudoku.Width);
            LayoutRoot.RowDefinitions[1].Height = new GridLength(sudoku.Height);

            SudokuMenu.IsEnabled = true;
            Visueel.IsEnabled = true;

            RecentlyOpenedFilesHandler handler = new RecentlyOpenedFilesHandler();
            handler.StoreRecentlyOpenedFile(fileName, FileLocation.IsolatedStorage);
        }

        private void IsoLaden_Click(object sender, RoutedEventArgs e)
        {
            IsoFileExplorerUI ife = new IsoFileExplorerUI(IsoFileExplorerMode.Load);
            ife.OnUnloaded += new IsoFileExplorerUI.Unload(exp_Unloaded);

            tip.Child = ife;
            tip.IsOpen = true;
        }

        private void Invulhulp_Click(Object sender, EventArgs e)
        {
            RadMenuItem item=sender as RadMenuItem;
            sudoku.GebruikInvulhulp = item.IsChecked;
        }
    }

}
