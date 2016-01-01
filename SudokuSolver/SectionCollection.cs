using System;
using System.Collections.Generic;
using System.Collections;


namespace SilverSudoku.SudokuSolver
{
    /// <summary>
    /// Summary description for SectionCollection
    /// </summary>
    public class SectionCollection : IEnumerable
    {
        List<SilverSudokuSection> sections = new List<SilverSudokuSection>(3);

        public SectionCollection(FieldSectionType sectionCollectionType)
        {
//            _sectionCollectionType = sectionCollectionType;
        }

        public void Add(SilverSudokuSection section)
        {
            sections.Add(section);
        }

        /// <summary>
        /// Retrieves a section by a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SilverSudokuSection GetSectionByKey(RegistrationKey key)
        {
            SilverSudokuSection retValue = null;
            foreach (SilverSudokuSection section in sections)
            {
                if (section.Key.Equals(key))
                {
                    retValue = section;
                }
            }
            return retValue;
        }

        /// <summary>
        /// Returns true if the sectioncollection contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(RegistrationKey key)
        {
            bool retValue = false;
            foreach (SilverSudokuSection section in sections)
            {
                if (section.Key == key)
                    retValue = true;
            }
            return retValue;
        }

        /// <summary>
        /// Calculates the possible values of the cells in this sectioncollection
        /// </summary>
        /// <returns></returns>
        public bool CalculatePossibilities()
        {
            bool result = true;
            foreach (SilverSudokuSection section in sections)
            {
                bool hasSolution = section.CalculatePossibilities();
                if (!hasSolution)
                    result = false;
            }
            return result;
        }

        //private FieldSectionType _sectionCollectionType;

        //public FieldSectionType SectionCollectionType
        //{
        //    get { return _sectionCollectionType; }
        //}

        public List<SilverSudokuSection> GetSections()
        {
            return sections;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SectionCollectionEnumerator(sections);
        }

        #endregion
    }

    public class SectionCollectionEnumerator : IEnumerator
    {
        private List<SilverSudokuSection> _section = null;

        private int position = -1;

        public SectionCollectionEnumerator(List<SilverSudokuSection> sections)
        {
            _section = sections;

        }

        #region IEnumerator Members

        public object Current
        {
            get { return _section[position]; }
        }

        public void Reset()
        {
            position = -1;
        }

        public bool MoveNext()
        {
            bool retValue = false;
            if (position < _section.Count - 1)
            {
                position += 1;
                retValue = true;
            }
            return retValue;
        }

        #endregion

    }



}