using MyCustomControlLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
        private System.Windows.Forms.NotifyIcon notifyIcon;

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

            CreateNotifyIcon();

          Window loginWindow =  new LoginWindow();
            Current.MainWindow = loginWindow;
            Current.MainWindow.Show();
           // currentUser = new User() { id = "ea2cd14c-35f0-450894cb-7f126ed8e5a1", name = "陈龙飞" };
        }

        private void GetCurrStatin(String stationId) {

            mStation = StationModel.SelectById(stationId);

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            String assimblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
 
            bool createNew=true;
            EventWaitHandle ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, assimblyName,out createNew);


            if (!createNew)
            {
                MMessageBox.GetInstance().ShowBox("该程序已经在运行中，不能重复创建！", "提示", MMessageBox.ButtonType.Yes, MMessageBox.IconType.warring,System.Windows.Controls.Orientation.Vertical, "好");
                App.Current.Shutdown();
                Environment.Exit(0);
            }
  
            base.OnStartup(e);
        }


        #region  Notify icon

        /// <summary>
        /// 创建Notification
        /// </summary>
        private void CreateNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipTitle = "BalloonTipTitle",
                BalloonTipText = "BalloonTipText intel connectation weighing" ,
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                //Icon = new System.Drawing.Icon("aislogo_48.ico"),
                Visible = true,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
            };

            notifyIcon.ShowBalloonTip(1000);
            notifyIcon.Text = "煤炭运煤监管系统";
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.ContextMenu = GetNotifyMenu();
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Current.MainWindow.ShowActivated = true;
        }

        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //ShowCurrentWindow();
        }



        /// <summary>
        /// 创建Notify icon Menu
        /// </summary>
        /// <returns></returns>
        public System.Windows.Forms.ContextMenu GetNotifyMenu()
        {

            System.Windows.Forms.MenuItem[] notifyMenu;
            System.Windows.Forms.MenuItem quitMenuItem = new System.Windows.Forms.MenuItem();
            quitMenuItem.Text ="退出";
            quitMenuItem.DefaultItem = true;
            quitMenuItem.Click += QuitMenuItem_Click;
            notifyMenu = new System.Windows.Forms.MenuItem[] { quitMenuItem };
            return new System.Windows.Forms.ContextMenu(notifyMenu);
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            //currWindow.Activate();

            MMessageBox.Result result = MMessageBox.GetInstance().ShowBox("你确定退出程系吗", "提示", MMessageBox.ButtonType.YesNo, MMessageBox.IconType.Info);
            if (result == MMessageBox.Result.Yes)
            {
              Current.Shutdown();
            }
            else
            {
                Current.MainWindow.ShowActivated = true;
            }
        }
        #endregion

    }
}
