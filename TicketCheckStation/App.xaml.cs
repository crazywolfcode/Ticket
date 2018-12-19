﻿using System;
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

            String stationId = MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.CurrStationId.ToString());
            if (string.IsNullOrEmpty(stationId)) {
                //没有初始化
            } else {
                GetCurrStatin(stationId);
            }

            if (mStation == null) {
                //没有初始化
                return;
            }
            new LoginWindow().Show();
           // currentUser = new User() { id = "ea2cd14c-35f0-450894cb-7f126ed8e5a1", name = "陈龙飞" };
        }

        private void GetCurrStatin(String stationId) {

            mStation = StationModel.SelectById(stationId);

        }
    }
}
