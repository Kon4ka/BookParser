using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace Parser.Core
{
    class ParserWorker<T> where T : class
    {

        IParser<T> parser;
        IParserSettings parserSettings;

        HtmlLoader loader;

        bool isActive;

        #region Properties

        public IParser<T> Parser
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

        public event Action<object, T> OnNewData;
        public event Action<object> OnCompleted;

        public ParserWorker(IParser<T> parser)
        {
            this.parser = parser;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public ParserWorker(IParser<T> parser, IParserSettings parserSettings) : this(parser)
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
            List<string> isbns = new List<string> { "978-5-17-100294-7", "978-5-17-090436-5" };

            foreach (var i in isbns)
            {
                if (!isActive)
                {
                    OnCompleted?.Invoke(this);
                    return;
                }

                var source = await loader.GetSourceByPageId(i);
                using (FileStream fstream = new FileStream(Directory.GetCurrentDirectory()+"/aaa.txt", FileMode.OpenOrCreate))
                {
                    // преобразуем строку в байты
                    byte[] buffer = Encoding.Default.GetBytes(source);
                    // запись массива байтов в файл
                    await fstream.WriteAsync(buffer, 0, buffer.Length);
                }

                var domParser = new HtmlParser();

                var document = await domParser.ParseAsync(source);

                var result = parser.Parse(document);

                OnNewData?.Invoke(this, result);
            }

            OnCompleted?.Invoke(this);
            isActive = false;
        }


    }
}
