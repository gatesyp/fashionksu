using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KentHackathon
{
    public class Article
    {
        public string id { get; set; }
        public string url { get; set; }
        public Bitmap bitmap { get; set; }
        public BitmapImage bitmapImage { get; set; }

        public Article(String id, String url)
        {
            Debug.Print("Article " + id + " Is Loading......");
            this.id = id;
            this.url = url;

            BitmapLoader loader = new BitmapLoader(url);
            bitmap = loader.loadImageToBitmap();
            bitmapImage = loader.loadImageToBitmapImage();
        }



    }
}
