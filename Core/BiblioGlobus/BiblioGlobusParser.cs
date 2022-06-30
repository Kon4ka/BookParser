using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Interfaces;

namespace Parser.Core.BiblioGlobus
{
    class BiblioGlobusParser: IParser
    {
        public string Parse(IHtmlDocument document, string ISBN)
        {
            var list = new List<string>();
            var items1 = document.QuerySelectorAll("div span");
            var items = document.QuerySelectorAll("div span").Where(item => item.Id != null && item.Id.Contains("price"));

            foreach (var item in items)
            {
                list.Add(ClearStr(item.TextContent));
                break;
            }

            return list[0];
        }

        public string ClearStr(string input)
        {
            float tmp;
            string num = input.Replace(" руб.", "");
            num = num.Replace(",", ".");
            if (!float.TryParse(num, out tmp))
            {
                throw new ArgumentException("У сайта сменился формат... На месте цены не число");
            }
            return ((int)tmp).ToString();
        }
    }
}
