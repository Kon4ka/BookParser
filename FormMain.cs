using Parser.Core;
using Parser.Core.BiblioGlobus;
using Parser.Core.BookHouseArbat;
using Parser.Core.MBookWork;
using Parser.Core.YoungGuard;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Parser
{
    public partial class FormMain : Form
    {
        ParserWorker<string[]> parser;

        public FormMain()
        {
            InitializeComponent();
/*
            parser = new ParserWorker<string[]>(
                    new HabraParser()
                );*/

            /*parser = new ParserWorker<string[]>(
                    new MoscowParser()
                );*/
            /*parser = new ParserWorker<string[]>(
                    new YoungGuardParser()
                );*/
/*            parser = new ParserWorker<string[]>(
                    new BiblioGlobusParser()
                );*/
            parser = new ParserWorker<string[]>(
                    new BookHouseArbatParser()
                );


            parser.OnCompleted += Parser_OnCompleted;
            parser.OnNewData += Parser_OnNewData;
        }

        private void Parser_OnNewData(object arg1, string arg2)
        {
            ListTitles.Items.Add(arg2);
        }

        private void Parser_OnCompleted(object obj)
        {
            MessageBox.Show("All works done!");
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            ReadExelConfig simpleData = new ReadExelConfig();
            simpleData.Initialisation();
            //simpleData.containers[0].IsbnAndUrls.Values;
            parser.Settings = new BookHouseArbatParserSettings(simpleData.containers[3].IsbnAndUrls);
            parser.Start();
        }

        private void ButtonAbort_Click(object sender, EventArgs e)
        {
            parser.Abort();
        }
    }
}
