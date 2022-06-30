using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IParserSettings
    {
        Dictionary<string, string> BaseUrl { get; set; }

        //string Prefix { get; set; }

        //List<string> Isbns { get; set; }
    }
}
