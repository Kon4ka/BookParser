using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Parser.Core.YoungGuard
{
    public class HtmlSearchLoader
    {
        readonly HttpClient client;
        readonly string url;

        public HtmlSearchLoader(IParserSettings settings)
        {
            client = new HttpClient();
            url = $"{settings.BaseUrl}";
        }

        public async Task<string> GetSourceByPageId(string id)
        {
            var currentUrl = url.Replace("{CurrentId}", id);
            var response = await client.GetAsync(currentUrl);
            string source = null;

            if(response != null && response.StatusCode == HttpStatusCode.OK)
            {
                source = await response.Content.ReadAsStringAsync();
            }

            return source;
        }

    }
}
