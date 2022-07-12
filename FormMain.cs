using Interfaces;
using Parser.Core;
using Parser.Core.BiblioGlobus;
using Parser.Core.BookHouseArbat;
using Parser.Core.MBookWork;
using Parser.Core.YoungGuard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial class FormMain : Form
    {
        private bool isSourceEnable = false;
        private bool isAborted = false;
        private string sourseFilePath = "";
        private ParserWorker<string[]> parser;
        private List<IParser> customParsers;
        private List<IParserSettings> customParserSettings;
        private ReadExelConfig simpleData;
        private int progress = 0;
        private int all = 0;

        private bool IsSourceEnable
        {
            get { return isSourceEnable; }
            set
            {
                if (value)
                {
                    tabControl1.Visible = true;
                }
                else
                    tabControl1.Visible = false;
                isSourceEnable = value;
            }
        }

        public FormMain()
        {
            InitializeComponent();
            IsSourceEnable = true;
            if (!Directory.Exists("./Properties") || !File.Exists("./Properties/Входные_данные.csv"))
            {
                MessageBox.Show("Выберите файл со входными данными.");
                загрузитьФайлToolStripMenuItem_Click(this, new EventArgs());
                if (sourseFilePath != "")
                    textBox1.Text = Path.GetFullPath(sourseFilePath);
                else
                    textBox1.Text = "Cannot find Source file! Use File -> Load";
            }
            else
            {
                sourseFilePath = "./Properties/Входные_данные.csv";
                textBox1.Text = Path.GetFullPath("./Properties/Входные_данные.csv");
                CollectInfo();
            }
            customParsers = new List<IParser>();
            customParsers.Add(new MoscowParser());
            customParsers.Add(new YoungGuardParser());
            customParsers.Add(new BiblioGlobusParser());
            customParsers.Add(new BookHouseArbatParser());


            parser = new ParserWorker<string[]>(
                    new BookHouseArbatParser()
                );
            ParserWorker<string[]>.onCount += PrograssInBar;
        }

        public void CollectInfo()
        {
            simpleData = new ReadExelConfig(sourseFilePath);
            try
            {
                simpleData.Initialisation();
            }
            catch(ArgumentOutOfRangeException ex)
            {
                textBox1.Text = "Ошибка: Неверное форматирование таблицы. Попробуйте выделить белые поля в таблице исходных данных и нажать Delete на клавиатуре.";
                IsSourceEnable = false;
                return;
            }
            var validRes = simpleData.Validation();
            if (validRes != "ОК")
            {
                MessageBox.Show(validRes);
                IsSourceEnable = false;
                return;
            }
            all = simpleData.containers[0].IsbnAndUrls.Count * simpleData.containers.Count;
            if (dataGridView1.Columns.Count == 0)
            {
                var headers = simpleData.GetHeaders();
                for (int i = 0; i < headers.Count; i++)
                {
                    dataGridView1.Columns.Add($"column{i}", headers[i]);
                }
            }
            if (dataGridView1.Rows.Count == 0)
                dataGridView1.Rows.Add();

            customParserSettings = new List<IParserSettings>();
            customParserSettings.Add(new MoscowParserSettings(simpleData.containers[0].IsbnAndUrls));
            customParserSettings.Add(new YoungGuardParserSettings(simpleData.containers[1].IsbnAndUrls));
            customParserSettings.Add(new BiblioGlobusParserSettings(simpleData.containers[2].IsbnAndUrls));
            customParserSettings.Add(new BookHouseArbatParserSettings(simpleData.containers[3].IsbnAndUrls));
        }

        public void PrograssInBar(long i)
        {
            if (progress == 0)
            {
                progressBar1.Maximum = all;
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
                progressBar1.Step = 1;
            }
            else
            {
                if (progress + 1 == all)
                {
                    progressBar1.Value = 0;
                    progress = 0;
                    /*progressBar1.Visible = false;*/
                }
                //progressBar1.PerformStep();
                progressBar1.Value = progress;
            }
            progress++;
        }

        private async void ButtonStart_ClickAsync(object sender, EventArgs e)
        {
            if (customParserSettings is null || customParserSettings.Count == 0)
            {
                MessageBox.Show("Вы не загрузили таблицу входных данных");
                return;
            }
            progressBar1.Visible = true;
            isAborted = false;
/*            List<Task> tasks = new List<Task>();

            for (int i = 0; i < customParsers.Count; i++)
                tasks.Add(Task.Run(async () => await CollectInformationAsync(i)));*/
            //var res = Parallel.For(0, customParsers.Count, async i => await CollectInformationAsync(i));
            for (int i = 0; i < customParsers.Count; i++)
            {
                parser.ChangeParser(customParsers[i]);
                parser.Settings = customParserSettings[i];
                if (!isAborted) 
                    simpleData.containers[i].IsbnAndCost = await parser.StartAsync();
            }
            /*await Task.WhenAll(tasks.ToArray());*/
            progressBar1.Visible = false;
            if (!isAborted)
            {
                label8.Text = "Complete";
                label8.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label8.Text = "Stoped";
                label8.ForeColor = System.Drawing.Color.Red;
            }
            if (!isAborted)
            {
                try
                {
                    simpleData.Writing();
                }
                catch(IOException ex)
                {
                    MessageBox.Show($"Невозможно получить доступ к файлу для вывода: \n\"Результат {DateTime.Today.ToShortDateString()}.csv\" \nЗакройте файл и повторите попытку.");
                    label8.Text = "Error";
                    label8.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private async Task CollectInformationAsync(int i)
        {
            parser.ChangeParser(customParsers[i]);
            parser.Settings = customParserSettings[i];
            simpleData.containers[i].IsbnAndCost = await parser.StartAsync();
        }

        private void ButtonAbort_Click(object sender, EventArgs e)
        {
            parser.Abort();
            isAborted = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label6.Visible = !label6.Visible;
            textBox4.Visible = !textBox4.Visible;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Text = "";
            label7.Text = "";
            label8.Text = "";
        }

        private void загрузитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                sourseFilePath = openFileDialog1.FileName;
                textBox1.Text = Path.GetFullPath(sourseFilePath);
                IsSourceEnable = true;
                CollectInfo();
            }
            else
            {
                IsSourceEnable = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string outputPath = Path.GetFullPath($"./Results/Результат {DateTime.Today.ToShortDateString()}.csv");

            if (!File.Exists(outputPath))
            {
                MessageBox.Show("Файл не найден, возможно вы ещё не сгенерировали его за сегодня.");
                return;
            }
            else
            System.Diagnostics.Process.Start(outputPath);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(sourseFilePath))
            {
                MessageBox.Show("Файл данных не найден.");
                return;
            }
            else
                System.Diagnostics.Process.Start(Path.GetFullPath(sourseFilePath));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if ((string)dataGridView1.Rows[0].Cells[0].Value is null|| 
                    (string)dataGridView1.Rows[0].Cells[1].Value  is null ||
                    (string)dataGridView1.Rows[0].Cells[0].Value == "" ||
                    (string)dataGridView1.Rows[0].Cells[1].Value == "")
                {
                    MessageBox.Show("Вы забыли ввести ISBN или наименование книги.");
                    return;
                }
                StringBuilder s = new StringBuilder();
                for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
                {
                    if ((string)dataGridView1.Rows[0].Cells[i].Value != "") s.Append(dataGridView1.Rows[0].Cells[i].Value);
                    if (i != dataGridView1.Rows[0].Cells.Count - 1) 
                        s.Append(";");
                }
                var addResult = simpleData.WriteLineToSource((string)dataGridView1.Rows[0].Cells[1].Value, s.ToString());
                if (!addResult)
                {
                    MessageBox.Show("Книга с таким ISBN уже существует.\nДобавление невозможно");
                    label3.Text = "Error";
                    label3.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                label3.Text = "Added";
                label3.ForeColor = System.Drawing.Color.Green;
                for (int i = 0; i < dataGridView1.Rows[0].Cells.Count; i++)
                {
                    dataGridView1.Rows[0].Cells[i].Value = "";
                }
                CollectInfo();
            }
            catch (IOException ex)
            {
                MessageBox.Show("У вас открыт файл, который используется \nпрограммой как входные данные. \nЗакройте файл и повторите попытку.");
                label3.Text = "Error";
                label3.ForeColor = System.Drawing.Color.Red;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка - " + ex.Message);
                label3.Text = "Error";
                label3.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string isbn = textBox3.Text;

            if (checkBox1.Checked)
            {

            }
            else
            {
                try
                {

                    string tempFile = Path.GetTempFileName();

                    using (var sr = new StreamReader(sourseFilePath, Encoding.Default))
                    {
                        using (var sw = new StreamWriter(tempFile, false, Encoding.Default))
                        {
                            string line;

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (!line.Contains(isbn))
                                    sw.WriteLine(line);
                            }
                        }
                    }
                    File.Delete(sourseFilePath);
                    File.Move(tempFile, sourseFilePath);
                    File.Delete(tempFile);
                    label7.Text = "Deleted";
                }
                catch (IOException ex)
                {
                    MessageBox.Show("У вас открыт файл, который используется \nпрограммой как входные данные. \nЗакройте файл и повторите попытку.");
                    label7.Text = "Error";
                }
            }
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 about = new Form1();
            about.Show();
        }
    }
}
