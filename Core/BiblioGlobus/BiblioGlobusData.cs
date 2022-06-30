using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Core.Interfaces;

namespace Parser.Core.BiblioGlobus
{
    class BiblioGlobusData: IDataToFind
    {
        public Dictionary<string, string> IsbnAndUrls { get; set; }
        public Dictionary<string, string> IsbnAndCost { get; set; }

        public BiblioGlobusData()
        {
            IsbnAndUrls = new Dictionary<string, string>();
            IsbnAndCost = new Dictionary<string, string>();
        }

    }
}
