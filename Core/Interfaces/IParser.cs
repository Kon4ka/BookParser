using AngleSharp.Dom.Html;

namespace Interfaces
{
    public interface IParser
    {
        string Parse(IHtmlDocument document, string ISBN = null);
    }
}
