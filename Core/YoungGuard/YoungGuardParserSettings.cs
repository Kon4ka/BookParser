using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Parser.Core.YoungGuard
{
    class YoungGuardParserSettings: IParserSettings
    {
        public YoungGuardParserSettings(Dictionary<string, string> urls)
        {
            BaseUrl = urls;
        }

        public Dictionary<string, string> BaseUrl { get; set; }
    }
}
