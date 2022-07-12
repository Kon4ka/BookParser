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
        private string outputPath = "./Results/Результат {Дата}.csv";
        private TextFieldParser reader;
        private List<List<string>> _csvTable;

        public ReadExelConfig(string filepath = "./Properties/Входные_данные.csv")
        {
            inputPath = filepath;
            try
            {
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
            try
            {
                reader = new TextFieldParser(inputPath, Encoding.Default);
                reader.TextFieldType = FieldType.Delimited;
                reader.SetDelimiters(";");
                while (!reader.EndOfData)
                {
                    _csvTable.Add(new List<string>());
                    _csvTable[_rowCount++] = reader.ReadFields().ToList();
                }
                _rowCount = _csvTable.Count;
                _collumCount = _csvTable[_rowCount - 1].Count;

                if (_collumCount == 0 || _rowCount == 0)
                    throw new ArgumentException("У вас пустая таблица.");

                Reading();
                reader.Close();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw ex;
            }
        }

        public string Validation()
        {
/*            Dictionary<string, int> check = new Dictionary<string, int>();
            for (int k = 1; k < _rowCount; k++)
            {
                if (!check.ContainsKey(_csvTable[k][0]))
                    check.Add(_csvTable[k][0], 0);
                check[_csvTable[k][0]]++;
                if (check[_csvTable[k][0]] > 1)
                    return $"Ошибка: В названии книг встречаются повторы. \nПример: {_csvTable[k][0]}";
            }*/
/*            if (check.Count < _rowCount - 1)
                return "Ошибка: В названии книг встречаются повторы.";*/

            for (int i = 0; i < _collumCount - 2; i++)
            {
                for (int j = 1; j < _rowCount; j++)
                {
                    if (_csvTable[j][i + 2] == "") continue;
                    if (_csvTable[j][i + 2].Length < 4||
                        _csvTable[j][i + 2][0] != 'h' ||
                        _csvTable[j][i + 2][1] != 't' ||
                        _csvTable[j][i + 2][2] != 't' ||
                        _csvTable[j][i + 2][3] != 'p' )
                        return $"Ошибка: В ячейке [{j+1}][{i+1}] стоит не ссылка, а \"{_csvTable[j][i + 2]}\".";
                }
            }

            return "ОК";
        }

        public void Reading()
        {
            try
            {
                for (int j = 0; j < _collumCount - 2; j++) //не оставлять наименования без ссылок
                {
                    for (int k = 1; k < _rowCount; k++) // Не должно быть повторяющихся разделителей
                    {
                        containers[j].IsbnAndUrls[_csvTable[k][1]] = _csvTable[k][j + 2];
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw ex;
            }

        }

        public List<string> GetHeaders()
        {
            List<string> s = new List<string>();
            s.Add("Name");
            s.Add("ISBN");
            for (int j = 2; j < _collumCount; j++)
                s.Add(_csvTable[0][j]);
            return s;
        }

        public bool WriteLineToSource(string isbn,string s)
        {
            if (containers[0].IsbnAndUrls.ContainsKey(isbn))
                return false;
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(inputPath, true, Encoding.Default))
                {
                    streamWriter.WriteLine(s);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Writing()
        {
            if (!Directory.Exists("./Results/"))
                Directory.CreateDirectory("./Results/");

            var data = DateTime.Today.ToShortDateString();
            outputPath = outputPath.Replace("{Дата}", data);
            using (StreamWriter streamWriter = new StreamWriter(outputPath, false, Encoding.Default))
            {
                //streamReader.WriteLine(data);
                StringBuilder s = new StringBuilder();
                s.Append(data+";"+"ISBN;");
                for (int j = 2; j < _collumCount; j++)
                    s.Append(_csvTable[0][j]+";");
                streamWriter.WriteLine(s.ToString());
                s.Clear();
                for (int i = 1; i < containers[0].IsbnAndCost.Count + 1; i++)
                {
                    s.Append(_csvTable[i][0] + ";");
                    s.Append(_csvTable[i][1] + ";");
                    foreach (var container in containers)
                    {
                        s.Append(container.IsbnAndCost[_csvTable[i][1]] + ";");
                    }
                    streamWriter.WriteLine(s.ToString());
                    s.Clear();
                }
            }
        }

    }
}
