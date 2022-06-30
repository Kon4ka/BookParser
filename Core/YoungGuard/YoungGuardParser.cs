using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Interfaces;

namespace Parser.Core.YoungGuard
{
    class YoungGuardParser: IParser
    {
        public string Parse(IHtmlDocument document, string ISBN)
        {
            var list = new List<string>();
            var items1 = document.QuerySelectorAll("div p");
            var items = document.QuerySelectorAll("*").Where(item => item.ClassName != null && item.ClassName.Contains("basket_prise"));

            foreach (var item in items)
            {
                //if 
                list.Add(ClearStr(item.TextContent));
                break;
            }

            return list[0];
        }

        public string ClearStr(string input)
        {
            int dotCount = 0;
            StringBuilder s = new StringBuilder();
            foreach (var letter in input)
                if (Char.IsNumber(letter) || (letter == '.' && dotCount < 1))
                {
                    s.Append(letter);
                    if (letter == '.')
                        dotCount++;
                }
            float res = -1;
            if (!float.TryParse(s.ToString(), out res))
            {
                throw new ArgumentException("У сайта сменился формат... На месте цены не число");
            }
            return ((int)res).ToString();
        }
    }
}
