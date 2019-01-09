using MyCustomControlLibrary;
using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TicketCheckStation
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class CameralManageWindow : Window
    {       
        public CameralManageWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {          
            LoadData();
        }
        public void LoadData()
        {            
            List<CameraInfo> list = CameralInfoModel.GetList(App.mStation.id);
            this.ReportDataGrid.ItemsSource = list;
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



        #region EXport EXCL，
        private void ExportExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExclHelper.ExclExprotToExcelWitchStatisticInfo(this.ReportDataGrid, "摄像头信息" + DateTimeHelper.getCurrentDateTime(DateTimeHelper.DateFormat), "摄像头信息", "","","",null);             
        }

        private List<String> GetListStatisticToListString(ListBox listBox)
        {
            if (listBox == null || listBox.Items.Count <= 0)
            {
                return null;
            }
            List<string> list = new List<string>();

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                TextBlock tb = (TextBlock)listBox.Items[i];
                if (tb != null)
                {
                    list.Add(tb.Text);
                }
                else
                {
                    list.Add(" ");
                }
            }
            return list;
        }
        #endregion
        
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;           
        }


        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }        

        private void AddCameralTabBtn_Click(object sender, RoutedEventArgs e)
        {
            new CameraAddW().ShowDialog();
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            IconButton button = sender as IconButton;
            CameraInfo camera = button.Tag as CameraInfo;
            new CameraAddW(camera.id).ShowDialog();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            IconButton button = sender as IconButton;
            CameraInfo camera = button.Tag as CameraInfo;
            if (camera != null) {
             int res =   DatabaseOPtionHelper.GetInstance().delete(camera);
                if (res > 0) {
                    CommonFunction.ShowSuccessAlert("删除成功！");
                    LoadData();
                } else
                {
                    CommonFunction.ShowErrorAlert("删除失败！");
                }
            }
        }
    }
}
