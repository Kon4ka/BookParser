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
        public Dictionary<string, string> IsbnAndNames;
        public List<IDataToFind> containers;
        private int _collumCount = 0;
        private int _rowCount = 0;
        private string inputPath;
        private string outputPath = "Результат {Дата}.csv";
        private TextFieldParser reader;
        private List<List<string>> _csvTable;

        public ReadExelConfig(string filepath = "./Properties/Входные_данные.csv")
        {
            inputPath = filepath;
            try
            {
                reader = new TextFieldParser( inputPath);
                reader.TextFieldType = FieldType.Delimited;
                reader.SetDelimiters(";");
                _csvTable = new List<List<string>>();

                IsbnAndNames = new Dictionary<string, string>();
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
/*            for (int k = 1; k < _rowCount; k++)
            {
                IsbnAndNames[_csvTable[k][1]] = _csvTable[k][0];
            }*/
            for (int j = 0; j < _collumCount - 2; j++)
            {
                for (int k = 1; k < _rowCount; k++)
                {
                    containers[j].IsbnAndUrls[_csvTable[k][1]] = _csvTable[k][j+2];
                }
            }
        }

        public void Writing()
        {
            var data = DateTime.Today.ToShortDateString();
            outputPath = outputPath.Replace("{Дата}", data);
            using (StreamWriter streamReader = new StreamWriter(outputPath, false, Encoding.Default))
            {
                //streamReader.WriteLine(data);
                StringBuilder s = new StringBuilder();
                s.Append(data+";"+"ISBN;");
                for (int j = 2; j < _collumCount; j++)
                    s.Append(_csvTable[0][j]+";");
                streamReader.WriteLine(s.ToString());
                s.Clear();
                for (int i = 1; i < containers[0].IsbnAndCost.Count + 1; i++)
                {
                    s.Append(_csvTable[i][0] + ";");
                    s.Append(_csvTable[i][1] + ";");
                    foreach (var container in containers)
                    {
                        s.Append(container.IsbnAndCost[_csvTable[i][1]] + ";");
                    }
                    streamReader.WriteLine(s.ToString());
                    s.Clear();
                }
            }
        }

    }
}
