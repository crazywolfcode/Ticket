using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TicketCheckStation
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        public static Station mStation;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mStation = new Station() { id = "047ee76b-314c-45cd-9216-f1238235d86c",name="验票一站" };

            new MainWindow().Show();
        }
    }
}
