using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using Interfaces;

namespace Parser.Core.YoungGuard
{
    class IsbnToCurrentId
    {
/*        public async Task<List<string>> IsbnToCurrentIdAsync(List<string> isbn, IParserSettings settings)
        {
            HtmlSearchLoader loader = new HtmlSearchLoader(settings);
            SearchParserYG searchParser = new SearchParserYG();
            string result = new string[6];
            foreach (var cur in isbn)
            {
                var source = await loader.GetSourceByPageId(cur);

                var domParser = new HtmlParser();

                var document = await domParser.ParseAsync(source);

                result = searchParser.Parse(document);
                return result.ToList();
            }
            return result.ToList();

        }*/
    }
}
