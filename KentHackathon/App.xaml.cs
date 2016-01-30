using KentHackathon.ViewModels;
using KentHackathon.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KentHackathon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppView app = new AppView();
            AppViewModel context = new AppViewModel();
            app.DataContext = context;
            app.Show();
        }
    }
}
