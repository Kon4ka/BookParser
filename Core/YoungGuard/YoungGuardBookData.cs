using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Core.Interfaces;

namespace Parser.Core.YoungGuard
{
    class YoungGuardBookData: IDataToFind
    {
        public Dictionary<string, string> IsbnAndUrls { get; set; }
        public Dictionary<string, string> IsbnAndCost { get; set; }

        public YoungGuardBookData()
        {
            IsbnAndUrls = new Dictionary<string, string>();
            IsbnAndCost = new Dictionary<string, string>();
        }

    }
}
