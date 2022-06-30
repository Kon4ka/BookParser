using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;
using Parser.Core.BiblioGlobus;
using Parser.Core.BookHouseArbat;
using Parser.Core.Interfaces;
using Parser.Core.MBookWork;
using Parser.Core.YoungGuard;

namespace Parser.Core
{
    class ReadExelConfig
    {
        public Dictionary<string, List<string>> IsbnAndUrls;
        public List<IDataToFind> containers;
        private int _collumCount = 0;
        private int _rowCount = 0;
        private string path = "Входные_данные.csv";
        private TextFieldParser reader;
        private List<List<string>> _csvTable;

        public ReadExelConfig()
        {
            try
            {
                reader = new TextFieldParser("./Properties/" + path);
                reader.TextFieldType = FieldType.Delimited;
                reader.SetDelimiters(";");
                _csvTable = new List<List<string>>();

                IsbnAndUrls = new Dictionary<string, List<string>>();
                containers = new List<IDataToFind>();
                containers.Add(new MoscowBookData());
                containers.Add(new YoungGuardBookData());
                containers.Add(new BiblioGlobusData());
                containers.Add(new BookHouseArbatData());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ~ReadExelConfig()
        {
            reader.Dispose();
        }
        public void Initialisation()
        {

            while (!reader.EndOfData)
            {
                _csvTable.Add(new List<string>());
                _csvTable[_rowCount++] = reader.ReadFields().ToList();
            }
            _rowCount = _csvTable.Count;
            _collumCount = _csvTable[_rowCount-1].Count;

            if (_collumCount == 0 || _rowCount == 0)
                throw new ArgumentException("У вас пустая таблица.");

            Reading();
        }

        public void Reading()
        {
            for (int j = 0; j < _collumCount - 2; j++)
            {
                for (int k = 1; k < _rowCount; k++)
                {
                    containers[j].IsbnAndUrls[_csvTable[k][1]] = _csvTable[k][j+2];
                }
            }
        }

    }
}
