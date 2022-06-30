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
    class ParserWorker<T> where T : class
    {

        IParser parser;
        IParserSettings parserSettings;

        HtmlLoader loader;

        bool isActive;

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

        public event Action<object, string> OnNewData;
        public event Action<object> OnCompleted;

        public ParserWorker(IParser parser)
        {
            this.parser = parser;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public ParserWorker(IParser parser, IParserSettings parserSettings) : this(parser)
        {
            this.parserSettings = parserSettings;
        }

        public void Start()
        {
            isActive = true;
            Worker();
        }

        public void Abort()
        {
            isActive = false;
        }

        private async void Worker()
        {
            List<string> pages = new List<string> { "1", "2", "3" };    //Mock
            List<string> isbns = new List<string> { "/book/917460/?recommended_by=instant_search&r46_search_query=978-5-17-100294-7",
                                                    "/book/794389/?recommended_by=instant_search&r46_search_query=978-5-17-090436-5" };
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var i in parserSettings.BaseUrl)
            {
                if (!isActive)
                {
                    OnCompleted?.Invoke(this);
                    return;
                }

                var source = await loader.GetSourceByPageId(i.Value);
                using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory()+"/aaa.txt", FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] buffer = Encoding.Default.GetBytes(source);
                    // запись массива байтов в файл
                    await fstream.WriteAsync(buffer, 0, buffer.Length);
                }

                var domParser = new HtmlParser();

                var document = await domParser.ParseAsync(source);

                result[i.Key] = parser.Parse(document);

                OnNewData?.Invoke(this, result[i.Key]);
            }

            OnCompleted?.Invoke(this);
            isActive = false;
        }



    }
}
