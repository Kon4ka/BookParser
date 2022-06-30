using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Core.Interfaces
{
    public interface IDataToFind
    {
        Dictionary<string, string> IsbnAndUrls { get; set; }
        Dictionary<string, string> IsbnAndCost { get; set; }

    }
}
