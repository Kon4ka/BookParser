using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Interfaces;

namespace Parser.Core.YoungGuard
{
    public class SearchParserYG: IParser
    {
        public string Parse(IHtmlDocument document, string ISBN = null)
        {
            var list = new List<string>();
            var items = document.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("book_author"));

            foreach (var item in items)
            {
                list.Add(item.TextContent);
                /*                var urlToBook = item.GetAttribute("href").Split('/');
                                list.Add(urlToBook[urlToBook.Length - 1]);*/
                break;
            }

            return list[0];
        }
    }
}
