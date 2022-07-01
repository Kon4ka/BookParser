using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Interfaces;

namespace Parser.Core
{
    public class ParserWorker<T> where T : class
    {

        private IParser parser;
        private IParserSettings parserSettings;

        private HtmlLoader loader;

        private bool isActive;

        ~ParserWorker()
        {
            loader.Dispose();
        }

        #region Properties

        public IParser Parser
        {
            get
            {
                return parser;
            }
            set
            {
                parser = value;
            }
        }

        public IParserSettings Settings
        {
            get
            {
                return parserSettings;
            }
            set
            {
                parserSettings = value;
                loader = new HtmlLoader(value);
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }

        #endregion

        public delegate void PrograssInBar(long i);
        public static event PrograssInBar onCount;

        public ParserWorker(IParser parser)
        {
            this.parser = parser;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public void ChangeParser(IParser parser)
        {
            this.parser = parser;
        }

        public ParserWorker(IParser parser, IParserSettings parserSettings) : this(parser)
        {
            this.parserSettings = parserSettings;
        }

        public async Task<Dictionary<string, string>> StartAsync()
        {
            isActive = true;
            return await Worker();
        }

        public void Abort()
        {
            isActive = false;
        }

        private async Task<Dictionary<string, string>> Worker()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var i in parserSettings.BaseUrl)
            {
                if (!isActive)
                {
                    //OnCompleted?.Invoke(this);
                    return new Dictionary<string, string>();
                }

                if (i.Value == "")
                {
                    result[i.Key] = "Не отслеж.";
                    onCount(1);
                    continue;
                }

                var source = await loader.GetSourceByPageId(i.Value);
/*                using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory()+"/aaa.txt", FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] buffer = Encoding.Default.GetBytes(source);
                    // запись массива байтов в файл
                    await fstream.WriteAsync(buffer, 0, buffer.Length);
                }*/

                var domParser = new HtmlParser();

                var document = await domParser.ParseAsync(source);

                try
                {
                    result[i.Key] = parser.Parse(document);
                    onCount(1);
                }
                catch (ArgumentException ex)
                {
                    result[i.Key] = "Цены нет на сайте, возможно товара нет в наличии или сайт изменил место вывода цены";
                }

                //OnNewData?.Invoke(this, result[i.Key]);
            }

            //OnCompleted?.Invoke(this);
            isActive = false;
            return result;
        }



    }
}
