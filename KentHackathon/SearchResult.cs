using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KentHackathon
{
    public class SearchResult
    {
        public string id { get; set; }
        public string url { get; set; }
        public string response { get; set; }

        public SearchResult(string id, string url)
        {
            this.id = id;
            this.url = url;

        }
        public SearchResult(string response)
        {
            this.response = response;
        }


    }
}
