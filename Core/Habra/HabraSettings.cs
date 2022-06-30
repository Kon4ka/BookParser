
using System.Collections.Generic;

namespace Parser.Core.Habra
{
    class HabraSettings : IParserSettings
    {
        public HabraSettings(List<string> isbns)
        {
            Isbns = isbns;
        }

        public string BaseUrl { get; set; } = "https://habrahabr.ru";

        public string Prefix { get; set; } = "page{CurrentId}";

        public List<string> Isbns { get; set; }
    }
}
