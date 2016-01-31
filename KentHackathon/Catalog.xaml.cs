using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KentHackathon
{
    /// <summary>
    /// Interaction logic for Catalog.xaml
    /// </summary>
    public partial class Catalog : UserControl
    {
        private IList<Article> ArticleList;

        public Catalog(IList<Article> ArticleArray)
        {
            InitializeComponent();
            ArticleList = ArticleArray;
            wrapPanel.Children.Clear();

            foreach(Article a in ArticleList)
            {
                KinectTileButton but = new KinectTileButton();
                but.Name = "Ktb" + a.id;
                but.Background = new ImageBrush(a.bitmapImage);
                wrapPanel.Children.Add(but);

            }

        }



        
    }
}
