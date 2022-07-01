using Interfaces;
using Parser.Core;
using Parser.Core.BiblioGlobus;
using Parser.Core.BookHouseArbat;
using Parser.Core.MBookWork;
using Parser.Core.YoungGuard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parser
{
    public partial class FormMain : Form
    {
        private ParserWorker<string[]> parser;
        private List<IParser> customParsers;
        private List<IParserSettings> customParserSettings;
        private ReadExelConfig simpleData;
        private int progress = 0;
        private int all = 0;

        public FormMain()
        {
            InitializeComponent();
            customParsers = new List<IParser>();
            customParsers.Add(new MoscowParser());
            customParsers.Add(new YoungGuardParser());
            customParsers.Add(new BiblioGlobusParser());
            customParsers.Add(new BookHouseArbatParser());

            simpleData = new ReadExelConfig();
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
/*            List<Task> tasks = new List<Task>();

            for (int i = 0; i < customParsers.Count; i++)
                tasks.Add(Task.Run(async () => await CollectInformationAsync(i)));*/
            //var res = Parallel.For(0, customParsers.Count, async i => await CollectInformationAsync(i));
            for (int i = 0; i < customParsers.Count; i++)
            {
                parser.ChangeParser(customParsers[i]);
                parser.Settings = customParserSettings[i];
                simpleData.containers[i].IsbnAndCost = await parser.StartAsync();
            }
            /*await Task.WhenAll(tasks.ToArray());*/
            progressBar1.Visible = false;
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
        }
    }
}
