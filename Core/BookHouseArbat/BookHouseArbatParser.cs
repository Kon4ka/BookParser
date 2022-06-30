﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Interfaces;

namespace Parser.Core.BookHouseArbat
{
    class BookHouseArbatParser: IParser
    {
        public string Parse(IHtmlDocument document, string ISBN)
        {
            var list = new List<string>();
            var items1 = document.QuerySelectorAll("div span");
            var items = document.QuerySelectorAll("div span").Where(item => item.ClassName != null && item.ClassName.Contains("itempage-price_inet"));

            foreach (var item in items)
            {
                list.Add(item.TextContent.Replace("₽", ""));
                break;
            }

            return list[0];
        }
    }
}
