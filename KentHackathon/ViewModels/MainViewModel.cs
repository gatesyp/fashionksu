using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KentHackathon.ViewModels
{
    public class MainViewModel : ObservableObject, IPageViewModel
    {

        public MainViewModel()
        {

        }
        public string Name
        {
            get
            {
                return "Main Window";
            }
        }
    }
}
