using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KentHackathon
{
    class SearchResult
    {
        public string id { get; set; }
        public string url { get; set; }
        public SearchResult(string id, string url)
        {
            this.id = id;
            this.url = url;

        }


    }
}
