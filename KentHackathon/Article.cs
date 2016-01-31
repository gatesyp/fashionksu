using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KentHackathon
{
    class Article
    {
        String id { get; } 
        String url { get; }

        public Article(String id, String url)
        {
            this.id = id;
            this.url = url;
        }



    }
}
