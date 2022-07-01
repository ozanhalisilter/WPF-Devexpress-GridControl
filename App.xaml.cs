using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF_Devexpress_GridControl.ViewModel;

namespace WPF_Devexpress_GridControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Views.MainView window = new Views.MainView();
            MainViewModel VM = new MainViewModel();
            window.DataContext = VM;
            window.Show();
        }
    }
}
