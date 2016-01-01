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
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace SilverSudoku
{
    public class RecentlyOpenedFilesHandler:IDisposable
    {
        IsolatedStorageFile isoFile;
        RecentFiles files;
        private const string RecentFilesStorage = "RecentPuzzles.xml";

        public RecentlyOpenedFilesHandler()
        {
            isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            
            if (isoFile.FileExists(RecentFilesStorage))
            {
                using (IsolatedStorageFileStream stream = isoFile.OpenFile(RecentFilesStorage, FileMode.Open))
                {
                    if (stream.Length != 0)
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(RecentFiles));
                        files = (RecentFiles)ser.Deserialize(stream);                     
                    }
                    stream.Close();
                }
            }
            else
            {
                files = new RecentFiles();
            }
        }




        public RecentFiles GetRecentlyOpenedFiles()
        {
            if(!isoFile.FileExists(RecentFilesStorage))
                return null;

            using (IsolatedStorageFileStream stream = isoFile.OpenFile(RecentFilesStorage, FileMode.Open))
            {
                if (stream.Length > 0)
                {
                    XmlSerializer ser = new XmlSerializer(typeof(RecentFiles));
                    files = (RecentFiles)ser.Deserialize(stream);
                }
                stream.Close();
            }


            return  files;
        }

        public void StoreRecentlyOpenedFile(string filename, FileLocation location)
        {
            if (files == null)
            {
                files = new RecentFiles();
            }

            files.Add(filename, location);

            //the storagefile is deleted..
            if (isoFile.FileExists(RecentFilesStorage))
            {
                isoFile.DeleteFile(RecentFilesStorage);
            }

            //..and recreated
            using (IsolatedStorageFileStream stream = isoFile.OpenFile(RecentFilesStorage, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(RecentFiles));
                ser.Serialize(stream, files);
                stream.Close();
            }
        }



        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (isoFile != null)
            {
                isoFile.Dispose();
            }
        }

        #endregion
    }

    public class RecentFiles:IEnumerable<RecentFile>
    {
        private Queue<RecentFile> list = new Queue<RecentFile>();


        public void Add(object o)
        {
            RecentFile file = (RecentFile)o;
            if (!list.Contains(file))
            {
                if (list.Count == 4)
                { //eerst de laatste wissen
                    list.Dequeue();
                }
                list.Enqueue(file);
            }
        }

        public void Add(string filename, FileLocation location)
        {
            RecentFile file = new RecentFile(filename, location);
            if (!list.Contains(file))
            {
                if (list.Count == 4)
                { //eerst de laatste wissen
                    list.Dequeue();
                }
                list.Enqueue(file);
            }
        }

        public List<string> GetList()
        {
            List<string> newlist = new List<string>();
            foreach (RecentFile  file in list)
            {
                newlist.Add(file.FileName);
            }
            return newlist;
        }

        public RecentFile[] Files
        {
            get
            {
                RecentFile[] newlist = new RecentFile[list.Count];
                int index = 0;
                foreach (RecentFile file in list)
                {
                    newlist[index]=file;
                    index++;
                }
                return newlist;
            }
            set
            {
                foreach (RecentFile file in value)
                {
                    list.Enqueue(file);
                }
            }
        }

     
        #region IEnumerable<RecentFile> Members

        public IEnumerator<RecentFile> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator)list;
        }

        #endregion
    }


    public class RecentFile
    {

        public RecentFile()
        {         
        }

        public RecentFile(string fileName, FileLocation location)
        {
            _filename = fileName;
            _location = location;
        }
        private string _filename;
        private FileLocation _location;

        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public FileLocation FileLocation
        {
            get{return _location;}
            set { _location = value; }            
        }

        public override bool Equals(object obj)
        {
            RecentFile file = obj as RecentFile;
            if (file == null)
            {
                return false;
            }

            if (file.FileLocation.Equals(this.FileLocation) && file.FileName.Equals(this.FileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum FileLocation
    {
        FileSystem,
        IsolatedStorage,
        Assembly
    }
}
