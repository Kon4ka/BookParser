using Interfaces;
using Parser.Core;
using Parser.Core.BiblioGlobus;
using Parser.Core.BookHouseArbat;
using Parser.Core.MBookWork;
using Parser.Core.YoungGuard;
using System;
using System.Collections.Generic;
using System.IO;
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
                textBox1.Text = Path.GetFullPath(sourseFilePath);
            }
            else
            {
                sourseFilePath = "./Properties/Входные_данные.csv";
                textBox1.Text = Path.GetFullPath("./Properties/Входные_данные.csv");
            }
            customParsers = new List<IParser>();
            customParsers.Add(new MoscowParser());
            customParsers.Add(new YoungGuardParser());
            customParsers.Add(new BiblioGlobusParser());
            customParsers.Add(new BookHouseArbatParser());

            simpleData = new ReadExelConfig(sourseFilePath);
            simpleData.Initialisation();
            all = simpleData.containers[0].IsbnAndUrls.Count * simpleData.containers.Count;

            customParserSettings = new List<IParserSettings>();
            customParserSettings.Add(new MoscowParserSettings(simpleData.containers[0].IsbnAndUrls));
            customParserSettings.Add(new YoungGuardParserSettings(simpleData.containers[1].IsbnAndUrls));
            customParserSettings.Add(new BiblioGlobusParserSettings(simpleData.containers[2].IsbnAndUrls));
            customParserSettings.Add(new BookHouseArbatParserSettings(simpleData.containers[3].IsbnAndUrls));

            parser = new ParserWorker<string[]>(
                    new BookHouseArbatParser()
                );
            ParserWorker<string[]>.onCount += PrograssInBar;
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
            label8.Text = "Complete";
            if (!isAborted) 
                simpleData.Writing();
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
            }
            else
            {
                isSourceEnable = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string outputPath = Path.GetFullPath($"Результат {DateTime.Today.ToShortDateString()}.csv");

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
                System.Diagnostics.Process.Start(sourseFilePath);
        }
    }
}
