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
using System.IO.IsolatedStorage;

namespace SilverSudoku
{
    public partial class IsoFileExplorerUI : UserControl
    {

        public delegate void Unload(object sender, IsoFileExplorerEventArgs e);
        public event Unload OnUnloaded;

        private IsolatedStorageFile isoFile;
        private IsoFileExplorerMode _mode;
        private string _fileName;
        private FileLocation _fileLocation;

        public IsoFileExplorerUI()
        {
            InitializeComponent();
            this.Loaded += IsoFileExplorer_Loaded;
            LayoutRoot.KeyDown += new KeyEventHandler(IsoFileExplorerUI_KeyDown);            
        }

        void IsoFileExplorerUI_KeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        void IsoFileExplorerUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (OnUnloaded != null)
                {
                    OnUnloaded(this, new IsoFileExplorerEventArgs(IsoFileExplorerStatus.Cancelled));
                }
            }
        }
        
        public IsoFileExplorerUI(IsoFileExplorerMode mode)
        {
            InitializeComponent();
            _mode = mode;
            this.Loaded += IsoFileExplorer_Loaded;
        }

        void IsoFileExplorer_Loaded(object sender, RoutedEventArgs e)
        {
            isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            string[] directories=isoFile.GetDirectoryNames();
            string[] files = isoFile.GetFileNames();

            ListBoxItem itm;
            FileList.Items.Clear();
            foreach (string file in files)
            {
                if (file != "RecentPuzzles.xml")
                {
                    itm = new ListBoxItem();
                    itm.Tag = FileLocation.IsolatedStorage;
                    itm.Content = file;
                    FileList.Items.Add(itm);
                }
            }
            string[] embeddedResources = this.GetType().Assembly.GetManifestResourceNames();
            foreach (string item in embeddedResources)
            {
                string puzzlePrefix="SilverSudoku.Puzzles";
                if (item.Contains(puzzlePrefix))
                {
                    itm = new ListBoxItem();
                    itm.Tag = FileLocation.Assembly;
                    itm.Content = item;
                    FileList.Items.Add(itm);
                }
            }
        }

        private void Annuleer_Click(object sender, RoutedEventArgs e)
        {
            if (OnUnloaded != null)
            {
                OnUnloaded(this, new IsoFileExplorerEventArgs(IsoFileExplorerStatus.Cancelled));
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem == null)
                return;

            _fileName = ((ListBoxItem)FileList.SelectedItem).Content.ToString();
            foreach (ListBoxItem item in FileList.Items)
            {
                if (item.IsSelected)
                {
                    _fileLocation = (FileLocation)item.Tag;
                }
            }

            if (OnUnloaded != null)
            {
                OnUnloaded(this, new IsoFileExplorerEventArgs(IsoFileExplorerStatus.Selected));
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public FileLocation FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        public IsoFileExplorerMode Mode
        {
            get { return _mode; }
        }

        private void FileList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ListBox lbx = (ListBox)sender;
            //foreach (ListBoxItem itm in lbx.Items)
            //{
            //    if (itm.IsSelected)
            //    {
            //        Selection.Text = itm.Content.ToString();
            //    }
            //}
        }
    }

    public enum IsoFileExplorerMode
    {
        Load,
        Save
    }
}
