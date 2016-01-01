using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
using SilverSudoku;
using System.Linq;
using System.Xml.Linq;

using System.Windows.Controls;
using System.ComponentModel;
using System.IO.IsolatedStorage;

//TODO: fixxen dat de sudokusolver doorgaat als het programma wordt afgesloten

namespace SilverSudoku.SudokuSolver
{
    public class SudokuField
    {

        private List<SudokuCell> EmptyCells = new List<SudokuCell>();

        public delegate void UpdateEvent(object sender);
        public event UpdateEvent Updated;

        public delegate void OnPuzzleSolved(TimeSpan calculationTime);
        public event OnPuzzleSolved PuzzleSolved;

        public delegate void OnStatusChanged(string newStatus);
        public event OnStatusChanged StatusChanged;

        private List<SudokuCell> cellen = new List<SudokuCell>();

        SectionCollection _rows = null;
        SectionCollection _columns = null;
        SectionCollection _blocks = null;

        public static int test = 0;

        public int NumRows = 0;
        public int NumColumns = 0;

        public bool Opgelost = false;

        public string FileName = string.Empty;
        public bool IsNewPuzzle;

        public int CellsFilled = 0;
        public ContentType ContentType = ContentType.Decimal;
        SudokuCell[,] speelveld;

        private bool autoOutput;
        /// <summary>
        /// When set to true, the SudokuField throws an updated event whenever the solver updates the cell value
        /// </summary>
        public bool AutoOutput
        {
            get { return autoOutput; }
            set { autoOutput = value; }
        }

        private void Init()
        {
            _rows = new SectionCollection(FieldSectionType.Row);
            _columns = new SectionCollection(FieldSectionType.Column);
            _blocks = new SectionCollection(FieldSectionType.Block);
        }

        public SudokuField()
        {
            test++;
            Init();
        }

        public SudokuField(int rows, int columns)
            : this()
        {
            if (rows > 9)
                ContentType = ContentType.HexaDecimal;
        }

        /// <summary>
        /// Creates a new, empty SudokuField with the specified numer of rows and columns
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public void CreateNew(int rows, int columns)
        {
            if (StatusChanged != null)
                StatusChanged("Creating new puzzle");

            IsNewPuzzle = true;
            NumRows = rows;
            NumColumns = columns;
            Init();
            if (NumRows > 9)
                ContentType = ContentType.HexaDecimal;

            //this is an aid to help registering the cells with their section
            SudokuCell[,] speelveld = new SudokuCell[rows, columns];
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    SudokuCell sudokuCell = new SudokuCell(new RegistrationKey(rowIndex, columnIndex));
                    speelveld[rowIndex, columnIndex] = sudokuCell;
                }
            }

            RegisterField(speelveld);
            if (StatusChanged != null)
                StatusChanged("Finished creating new puzzle");
        }

        /// <summary>
        /// Creates a new, empty SudokuField with the specified numer of rows and columns
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static SudokuField CreateNewField(int rows, int columns)
        {
            SudokuField newField = new SudokuField(rows, columns);
            newField.IsNewPuzzle = true;
            newField.NumRows = rows;
            newField.NumColumns = columns;


            if (newField.NumRows > 9)
                newField.ContentType = ContentType.HexaDecimal;

            //this is an aid to help registering the cells with their section
            newField.speelveld = new SudokuCell[rows, columns];
            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                {
                    SudokuCell sudokuCell = new SudokuCell(new RegistrationKey(rowIndex, columnIndex));
                    newField.speelveld[rowIndex, columnIndex] = sudokuCell;
                }
            }

            newField.RegisterField(newField.speelveld);

            return newField;
        }

        #region Solving
        /// <summary>
        /// Calculates the possible values for each cell in the field
        /// </summary>
        /// <returns></returns>
        public bool CalculatePossibilities()
        {
            bool retValue = true;
            foreach (SilverSudokuSection section in _rows.GetSections())
            {
                bool temp = section.CalculatePossibilities();
                if (!temp)
                    retValue = false;
            }
            return retValue;
        }

        /// <summary>
        /// Solves the puzzle
        /// </summary>
        public void Solve(BackgroundWorker worker)
        {
            if (StatusChanged != null)
                StatusChanged("Solving puzzle");

            DateTime startTime = DateTime.Now;
            SolveRecursively(0, worker);

            if (StatusChanged != null)
                StatusChanged(string.Format("Puzzle solved in {0}", DateTime.Now - startTime));

            if (PuzzleSolved != null)
                PuzzleSolved(DateTime.Now - startTime);
        }

        public void SolveRecursively(int nesting, BackgroundWorker worker)
        {
            bool bContinue = true;
            CellsFilled = 0;
            nesting++;
            int delay = 00;

            if (!Opgelost && !worker.CancellationPending)
            {
                //lus door elke rij
                IEnumerator en = cellen.GetEnumerator();
                while (en.MoveNext() && bContinue)
                {
                    SudokuCell cell = (SudokuCell)en.Current;
                    if (!cell.IsEmpty) //als de cel leeg is waarde proberen
                    {
                        CellsFilled++;
                        if (CellsFilled == NumColumns * NumRows)
                            Opgelost = true;
                    }
                    else //als de cel leeg is waarde proberen                        
                    {
                        //only the possibilities of the cells affected by the change are calculated
                        bool test = cell.Parents.CalculatePossibilities();
                        for (int number = 0; number < NumColumns; number++)
                        {   //maak een mogelijkheden object dat de volgende aanlevert
                            if (cell.Possibilities.Contains(number))
                            {
                                cell.Value = number;
                                if (Updated != null && AutoOutput)
                                {
                                    Updated(cell);
                                    Thread.Sleep(delay);
                                }

                                SolveRecursively(nesting, worker);
                                if (!Opgelost && !worker.CancellationPending)
                                {
                                    cell.ClearValue();
                                    if (Updated != null && AutoOutput)
                                    {
                                        Updated(cell);
                                        Thread.Sleep(delay);
                                    }
                                    //only the possibilities of the cells affected by the change are calculated
                                    bool testje = cell.Parents.CalculatePossibilities();
                                }
                            }//if possibility contains number
                            if (number == NumColumns - 1 && cell.IsEmpty)
                            {//als alle mogelijkheden geprobeerd zijn en de puzzle is niet opgelost
                                bContinue = false;
                            }
                        }//waarde proberen
                    }//cell is leeg
                }//cell                
            }//opgelost
            return;
        }//solveRecursively

        //public void SolveRecursivelyOptimized(int startIndex)
        //{
        //    bool bContinue = true;
        //    if (!Opgelost)
        //    {
        //        //lus door elke rij
        //        int index = startIndex;
        //        while (index < EmptyCells.Count && bContinue)
        //        {
        //            SudokuCell cell = (SudokuCell)EmptyCells[index];

        //            //only the possibilities of the cells affected by the change are calculated
        //            cell.Parents.CalculatePossibilities();
        //            for (int number = 1; number <= NumColumns; number++)
        //            {   //maak een mogelijkheden objct dat de volgende aanlevert
        //                if (cell.Possibilities.Contains(number))
        //                {
        //                    cell.Value = number;
        //                    if (index == EmptyCells.Count)
        //                    {
        //                        Opgelost = true;
        //                        return;
        //                    }
        //                    if (Updated != null && AutoOutput)
        //                        Updated(cell);
        //                    SolveRecursivelyOptimized(startIndex + 1);
        //                    if (!Opgelost)
        //                    {
        //                        cell.Value = 0;
        //                        if (Updated != null && AutoOutput)
        //                            Updated(cell);
        //                        //only the possibilities of the cells affected by the change are calculated
        //                        cell.Parents.CalculatePossibilities();
        //                    }
        //                }//if possibility contains number      
        //                if (cell.Value == 0 && number == NumColumns)
        //                {
        //                    bContinue = false;
        //                }
        //            }//for                
        //            index++;
        //        }//while
        //    }//opgelost
        //    return;
        //}//solveRecursively
        #endregion

        #region Saving and loading
        //public void SaveSudokuAs(string filename)
        //{
        //    if (string.IsNullOrEmpty(filename))
        //        throw new ArgumentException("A valid filename must be entered.");
        //    FileName = filename;
        //    SaveSudoku();            
        //}

        public void SaveSudokuAs(IsolatedStorageFileStream fileStream)
        {
            SaveSudoku(fileStream);
        }

        public void SaveSudoku(IsolatedStorageFileStream stream)
        {
            int rowNumber = 0;

            object[] newRows = new object[NumRows];
            foreach (SilverSudokuSection row in _rows)
            {

                //puzzleValues.AppendChild(newRow);

                int columnNumber = 0;


                object[] newCells = new object[NumRows];
                foreach (SudokuCell cell in row.GetCells())
                {
                    XElement newCell = new XElement("Cell");
                    newCell.SetAttributeValue("Row", cell.Key.RowIndex.ToString());
                    newCell.SetAttributeValue("Column", cell.Key.ColumnIndex.ToString());
                    if (IsNewPuzzle && cell.Value != -1)
                    {
                        newCell.SetAttributeValue("Given", "true");
                    }
                    else
                    {
                        newCell.SetAttributeValue("Given", cell.Given.ToString());
                    }
                    newCell.SetAttributeValue("Value", cell.Value.ToString()); ;

                    newCells[columnNumber] = newCell;


                    columnNumber++;
                }

                XElement newRow = new XElement("Row", newCells);
                newRow.SetAttributeValue("Number", rowNumber.ToString());

                newRows[rowNumber] = newRow;

                rowNumber++;
            }

            XElement newValues = new XElement("puzzleValues", newRows);

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

        //private void SerializePuzzle(string filename)
        //{
        //    XmlSerializer ser=new XmlSerializer(typeof(SudokuField));
        //    FileStream stream=new FileStream(filename,FileMode.OpenOrCreate);
        //    ser.Serialize(stream, this);
        //}

        //public void LoadPuzzle(string path)
        //{
        //    Init();
        //    Reset();

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(path);

        //    //create cells
        //    XmlNodeList rows = doc.SelectNodes("puzzleValues/Row");
        //    NumRows = rows.Count;
        //    NumColumns = rows.Count;
        //    speelveld = new SudokuCell[NumRows, NumColumns];
        //    NumRows = rows.Count;
        //    foreach (XmlNode row in rows)
        //    {
        //        XmlNodeList cells = row.SelectNodes("Cel");
        //        NumColumns = cells.Count;
        //        foreach (XmlNode cell in cells)
        //        {
        //            int rowIndex = Convert.ToInt32(cell.Attributes["Row"].Value);
        //            int columnIndex = Convert.ToInt32(cell.Attributes["Column"].Value);
        //            SudokuCell sudokuCell = new SudokuCell(new RegistrationKey(rowIndex, columnIndex));
        //            sudokuCell.Value = Convert.ToInt32(cell.Attributes["Value"].Value);
        //            sudokuCell.Given = Convert.ToBoolean(cell.Attributes["Given"].Value);
        //            speelveld[rowIndex, columnIndex] = sudokuCell;
        //            cellen.Add(speelveld[rowIndex, columnIndex]);
        //        }
        //    }

        //    RegisterField(speelveld);
        //}




        /// <summary>
        /// Registers cells with the standard sections (row, column, block).
        /// </summary>
        /// <param name="field"></param>
        private void RegisterField(SudokuCell[,] field)
        {
            //register cells with sections
            foreach (SudokuCell cell in field)
            {
                RegisterCellWithSection(_rows, FieldSectionType.Row, cell);
                RegisterCellWithSection(_columns, FieldSectionType.Column, cell);
                RegisterCellWithSection(_blocks, FieldSectionType.Block, cell);

                cell.ContentType = this.ContentType;
            }
        }

        /// <summary>
        /// This event fires after the cell value has been updated by the solver
        /// </summary>
        /// <param name="sender"></param>
        void cell_Updated(object sender)
        {
            if (Updated != null)
                Updated(sender);
        }


        #endregion

        public void Reset()
        {
            Opgelost = false;

            foreach (SilverSudokuSection row in _rows.GetSections())
            {
                foreach (SudokuCell cell in row.GetCells())
                {
                    cell.Reset();
                }
            }
        }

        public List<RegistrationKey> Validate()
        {
            List<RegistrationKey> result = new List<RegistrationKey>();
            foreach (SilverSudokuSection row in _rows)
            {
                result.AddRange(row.Validate());
            }
            return result;
        }

        public SudokuCell[,] GetData()
        {
            return speelveld;
        }


        #region Private helper methods

        private RegistrationKey CreateRowKey(SudokuCell cell)
        {
            return new RegistrationKey(cell.Key.RowIndex, 0);

        }

        private RegistrationKey CreateColumnKey(SudokuCell cell)
        {
            return new RegistrationKey(0, cell.Key.ColumnIndex);
        }

        private RegistrationKey CreateBlockKey(SudokuCell cell)
        {
            int rowIndex = (int)(cell.Key.RowIndex / Math.Sqrt(NumRows));
            int columnindex = (int)(cell.Key.ColumnIndex / Math.Sqrt(NumColumns));
            return new RegistrationKey(rowIndex, columnindex);
        }

        private void RegisterCellWithSection(SectionCollection sectionCollection, FieldSectionType sectionType, SudokuCell cell)
        {
            RegistrationKey sectionKey = null;
            switch (sectionType)
            {
                case FieldSectionType.Row:
                    sectionKey = CreateRowKey(cell);
                    break;
                case FieldSectionType.Column:
                    sectionKey = CreateColumnKey(cell);
                    break;
                case FieldSectionType.Block:
                    sectionKey = CreateBlockKey(cell);
                    break;
                default:
                    throw new Exception("No fieldtype is set");
            }

            //retrieve the section from the collection
            SilverSudokuSection theSection = sectionCollection.GetSectionByKey(sectionKey);
            if (theSection == null)
            {   //the section doesn't exist: create it and add it to the collection
                theSection = new SilverSudokuSection(NumColumns, sectionKey);
                theSection.SectionType = sectionType;
                sectionCollection.Add(theSection);
            }
            theSection.RegisterCell(cell);
            cell.Parents.Add(theSection);
        }
        #endregion

        public void LoadNewPuzzle(string path)
        {
            XDocument doc = XDocument.Load(path);
            HandleLoadedPuzzleData(doc);
        }

        public void LoadNewPuzzle(TextReader textReader)
        {
            XDocument doc = XDocument.Load(textReader);
            HandleLoadedPuzzleData(doc);
        }


        //private List<SudokuSection> section = new List<SudokuSection>();


        private void HandleLoadedPuzzleData(XDocument puzzleData)
        {
            speelveld = new SudokuCell[9, 9];
            var query = from p in puzzleData.Elements("puzzleValues").Elements("Row").Elements("Cell")
                        select p;

            foreach (XElement cell in query)
            {
                //int sectionIndex = Convert.ToInt32(cell.Attribute("Section").Value);
                int rowIndex = Convert.ToInt32(cell.Attribute("Row").Value);
                int columnIndex = Convert.ToInt32(cell.Attribute("Column").Value);



                SudokuCell sudokuCell = new SudokuCell(new RegistrationKey(rowIndex, columnIndex));
                sudokuCell.Value = Convert.ToInt32(cell.Attribute("Value").Value);
                sudokuCell.Given = Convert.ToBoolean(cell.Attribute("Given").Value);
                if (!sudokuCell.Given)
                {
                    EmptyCells.Add(sudokuCell);
                }
                speelveld[rowIndex, columnIndex] = sudokuCell;
                cellen.Add(speelveld[rowIndex, columnIndex]);

            }
            //if (rows.Count() > 9)
            //{
            //    this.ContentType = ContentType.HexaDecimal;
            //}
            NumColumns = puzzleData.Elements("puzzleValues").Elements("Row").First().Elements("Cell").Count();
            NumRows = puzzleData.Elements("puzzleValues").Elements("Row").Count();
            ////dit veranderen . speelveld moet automatisch geinitialiseerd worden
            //speelveld = new SudokuCell[NumRows, NumColumns];


            RegisterField(speelveld);
            //this.IsNewPuzzle = true;
        }

        public SudokuCell GetCell(RegistrationKey key)
        {
            return speelveld[key.RowIndex, key.ColumnIndex];
        }

        public SudokuCell GetCell(int row, int column)
        {
            return speelveld[row, column];
        }

        public void LoadTemplate(string path)
        {
            //Init();
            //Reset();

            //XmlDocument doc = new XmlDocument();
            //doc.Load(path);

            ////create cells
            //XmlNodeList cells = doc.SelectNodes("Template/Cells/Cell");
            ////NumRows = rows.Count;
            ////NumColumns = rows.Count;
            ////speelveld = new SudokuCell[NumRows, NumColumns];
            ////NumRows = rows.Count;
            //foreach (XmlElement cell in cells)
            //{
            //    int rowIndex = Convert.ToInt32(cell.Attributes["Row"].Value);
            //    int columnIndex = Convert.ToInt32(cell.Attributes["Column"].Value);
            //    SudokuCell sudokuCell = new SudokuCell(new RegistrationKey(rowIndex, columnIndex));
            //    sudokuCell.Value = 0;
            //    sudokuCell.Given = false;
            //    speelveld[rowIndex, columnIndex] = sudokuCell;
            //    cellen.Add(speelveld[rowIndex, columnIndex]);
            //    //TODO: maak alle secties handmatig definieerbaar
            //    //TODO: maak het mogelijk om een rij, kolom of blok met x cellen in een keer 
            //    //als sectie in te stellen
            //}

            //RegisterField(speelveld);
        }
    }//end class
}//end namespace



