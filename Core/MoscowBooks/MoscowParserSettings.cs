using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Core.MoscowBooks
{
    class MoscowParserSettings: IParserSettings
    {
        public MoscowParserSettings(List<string> isbns)
        {
            Isbns = isbns;
        }

        public string BaseUrl { get; set; } = "https://www.moscowbooks.ru";

        public string Prefix { get; set; } = "search/?r46_search_query={CurrentId}";

        public List<string> Isbns { get; set; }
    }
}
