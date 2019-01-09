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
using System.Windows.Shapes;

namespace TicketCheckStation
{
    /// <summary>
    /// SetStationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetStationWindow : Window
    {
        private bool IsReSet;

        public SetStationWindow(bool isReSet =false)
        {
            InitializeComponent();
            this.IsReSet = isReSet;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Station> list;         
            NetResult result = NetHelper.Get(MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.remoteUrl.ToString()),TableName.station.ToString(),"","");
            if (result.errCode == 0)
            {
                list = (List<Station>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Station>));
                if (list.Count > 0)
                {
                    this.StationCb.ItemsSource = list;
                }
                else {
                    CommonFunction.ShowErrorAlert("系统未能与监管中心连接");
                }               
            }
            else {
                CommonFunction.ShowErrorAlert("系统未能与监管中心连接");
            }
        }

        #region Window Default Event
        /// <summary>
        /// window move event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void headerBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        #endregion

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StationCb.SelectedItem is Station station)
            {
                MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.CurrStationId.ToString(), station.id);
                MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.InitStep.ToString(), "3");
                   if (IsReSet)
                    {
                        CommonFunction.ShowSuccessAlert("保存成功！");
                    MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.SoftSrartDate.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        CommonFunction.ShowSuccessAlert("保存成功！,需要重先启动");
                    }  
                this.Close();     
            }
        }
    }
}
