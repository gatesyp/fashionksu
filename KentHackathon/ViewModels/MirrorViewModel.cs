﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KentHackathon.ViewModels
{
    class MirrorViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Mirror";
            }
        }
    }
}
