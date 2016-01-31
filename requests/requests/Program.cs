using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace requests
{
    public class Product
    {
        public string suggestions { get; set; }
        //public bool Active { get; set; }
        //public IList<string> Roles { get; set; }
    }
class Program
    {
        static void Main(string[] args)
        {
            Product product = new Product();
            string sURL;
            sURL = "http://stoh.io/recScript.php";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }
            Console.WriteLine(sLine);
            //Product deserializedProduct = JsonConvert.DeserializeObject<Product>(sLine);
            //Console.WriteLine(product.suggestions);
            Console.ReadLine();
        }
    }
}

