using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KentHackathon
{
    public class BitmapLoader
    {
        private String URL;

        public BitmapLoader(String url)
        {
            URL = url;

        }

        public Bitmap loadImageToBitmap()
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(URL);
            return bitmap;
        }

        public Bitmap loadImageToBitmapWeb()
        {
            System.Net.WebRequest request =
       System.Net.WebRequest.Create(URL);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream = response.GetResponseStream();
            Bitmap bitmap = new Bitmap(responseStream);


            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                return bitmap;
            }

        }
        public BitmapImage loadImageToBitmapImage()
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(URL);


            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }

        }
    }
}
