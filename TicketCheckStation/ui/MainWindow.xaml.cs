using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;
using System.Windows.Threading;
namespace TicketCheckStation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variable area
        DispatcherTimer mDispatcherTimer;
        #endregion


        public MainWindow()
        {
            InitializeComponent();         
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartClock();
        }

        #region 时钟
        private void StartClock() {
            mDispatcherTimer = new DispatcherTimer() {
                Interval = new TimeSpan(0,0,1),                
            };
            mDispatcherTimer.Tick += MDispatcherTimer_Tick;
            mDispatcherTimer.Start();
        }

        private void MDispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string timeStr = string.Format("{0}年{1}月{2}日 {3}:{4}:{5}",
                now.Year,
                now.Month.ToString("00"),
                now.Day.ToString("00"),
                now.Hour.ToString("00"),
                now.Minute.ToString("00"),
                now.Second.ToString("00"));
            this.currTimeTb.Text = timeStr;
        }
        #endregion
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mDispatcherTimer != null) {
                mDispatcherTimer.Stop();
            }
        }
    }
}
