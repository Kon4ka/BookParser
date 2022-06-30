using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Interfaces;

namespace Parser.Core
{
    class HtmlLoader: IDisposable
    {
        readonly HttpClient client;

        public HtmlLoader(IParserSettings settings)
        {
            client = new HttpClient();
        }
        ~HtmlLoader()
        {
            client.Dispose();
        }

        public async Task<string> GetSourceByPageId(string id)
        {
            //var currentUrl = url.Replace("{CurrentId}", id);
            var response = await client.GetAsync(id);
            string source = null;

            if(response != null && response.StatusCode == HttpStatusCode.OK)
            {
                source = await response.Content.ReadAsStringAsync();
            }

            return source;
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(client);
        }
    }
}
