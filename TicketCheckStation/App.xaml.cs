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
        internal static User currentUser;
        #region 本机使用的临时基础数据
        public static Dictionary<String, Company> tempSupplyCompanys = new Dictionary<string, Company>();
        public static Dictionary<String, Company> tempCustomerCompanys = new Dictionary<string, Company>();
        public static Dictionary<String, Material> tempMaterials = new Dictionary<string, Material>();
        public static Dictionary<String, CarInfo> tempCars = new Dictionary<string, CarInfo>();
        public static List<String> inputRemarkList = new List<string>() { };

        #endregion
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mStation = new Station() { id = "047ee76b-314c-45cd-9216-f1238235d86c", name = "验票一站" };

            currentUser = new User() { id = "ea2cd14c-35f0-450894cb-7f126ed8e5a1", name = "陈龙飞" };
            new MainWindow().Show();
        }
    }
}
