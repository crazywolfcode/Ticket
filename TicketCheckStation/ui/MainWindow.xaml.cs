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
using System.Windows.Forms.Integration;
using MyHelper;
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
            System.Windows.Controls.Primitives.CalendarItem calendar = new System.Windows.Controls.Primitives.CalendarItem();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Showcameral();

            LoadData();
        }

      public void  LoadData(){
            List<WeighingBill> list = WeighingBillModel.GetTodayData();
            this.TodayDataGrid.ItemsSource = list;
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

        #region cameral info
        private List<CameraInfo> cameraInfoList;
        private  List<int> CameraIds;
        private List<CHCNetSDK.NET_DVR_DEVICEINFO_V30> mDeviceInfors;
        private List<System.Windows.Forms.PictureBox> mPictureBoxs;
        private void Showcameral() {
            cameraInfoList = new CameralInfoModel().GetList(App.mStation.id);
            double width = this.ActualWidth;
            double singleWidth = Math.Floor(width / cameraInfoList.Count);
            int camerialWidth =Convert.ToInt32( singleWidth);
            int camerialHeight = 300;          
            VideoPenal.Children.Clear();
            if (cameraInfoList.Count <= 0)
            {
                TextBlock textBlock = new TextBlock
                {
                    Name = "videotAlertTb",
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Text = "没有添加任何摄像头",
                    FontSize = 16,
                    Foreground = Brushes.Red
                };
                VideoPenal.Children.Add(textBlock);
                return;
            }
            if (!CameraHelper.InitSDK())
            {
                if (VideoPenal.FindName("videotAlertTb") is TextBlock textBlock)
                {
                    textBlock.Text = "摄像头SDK初始化失败!";
                }
                return;
            }
            VideoPenal.Children.Clear();
            if (CameraIds == null)
            {
                CameraIds = new List<int>();
            }
            else {
                StorpPreviewCamera();
                LogoutCamera();
                CameraIds.Clear();
            }
            if (mPictureBoxs == null)
            {
                mPictureBoxs = new List<System.Windows.Forms.PictureBox>();
            }
            else
            {
                mPictureBoxs.Clear();
            }
            if (mDeviceInfors == null)
            {
                mDeviceInfors = new List<CHCNetSDK.NET_DVR_DEVICEINFO_V30>();
            }
            else
            {
                mDeviceInfors.Clear();
            }
            this.Cursor = Cursors.Wait;
            for (int i = 0; i < cameraInfoList.Count; i++)
            {
                try {
                    CameraInfo camera = cameraInfoList[i];
                    WindowsFormsHost formsHost = new WindowsFormsHost();
                    System.Windows.Forms.PictureBox pictureBox;
                     pictureBox = new System.Windows.Forms.PictureBox() {
                        Name = $"pictureBox{i}",
                        Width = camerialWidth,
                        Height =camerialHeight
                    };
                    mPictureBoxs.Add(pictureBox);
                    CHCNetSDK.NET_DVR_DEVICEINFO_V30 devieInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                    //login
                    int cameraid = CameraHelper.loginCamera(camera.ip, camera.port, camera.userName, camera.password,ref devieInfo);
                    if (cameraid <= -1) {
                        throw new Exception("登录摄像头失败");
                    }
                    CameraIds.Add(cameraid);
                    mDeviceInfors.Add(devieInfo);
                    //preview
                    int ChanNum = devieInfo.byChanNum;
                    ////码流类型：0-主码流，1-子码流，2-码流3，3-码流4
                    int streamType = 0;
                    bool res = CameraHelper.Preview(ref pictureBox, ChanNum, cameraid, streamType);
                    formsHost.Child = pictureBox;
                    VideoPenal.Children.Add(formsHost);
                } catch (Exception e) {
                    TextBlock textBlock = new TextBlock
                    {
                        Width = camerialWidth,
                        Height = camerialHeight,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "视频加载失败: "+e.Message,
                        FontSize = 16,
                        Foreground = Brushes.Red
                    };
                    VideoPenal.Children.Add(textBlock);                   
                }                
            }
            this.Cursor = Cursors.Arrow;
         }

        private void StorpPreviewCamera() {
            if (CameraIds != null && CameraIds.Count > 0)
            {
                for (int i = 0; i < CameraIds.Count; i++)
                {
                    try
                    {
                        CameraHelper.stopPreview(CameraIds[i]);
                    }
                    catch (Exception e)
                    {
                        MyHelper.ConsoleHelper.writeLine("停止预览 " + i + "失败: " + e.Message);
                    }
                }
            }
        }
        private void LogoutCamera() {
            if (CameraIds != null && CameraIds.Count > 0)
            {
                for (int i = 0; i < CameraIds.Count; i++)
                {
                    try
                    {
                        CameraHelper.LoginOutCamera(CameraIds[i]);
                    }
                    catch(Exception e)
                    {
                       ConsoleHelper.writeLine("退出摄像头"+i+"失败: " +e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 通道截图
        /// </summary>
        protected void CaptureJpeg(String currBillNumber)
        {
            string filePath = ConfigurationHelper.GetConfig(ConfigItemName.CameraCaptureFilePath.ToString());
            if (String.IsNullOrEmpty(filePath))
            {
                filePath = System.IO.Path.Combine(FileHelper.GetRunTimeRootPath(), "capture");
            }
            String fileName = String.Empty;
            //根据登陆成功的通过截图
            for (int i = 0; i < CameraIds.Count; i++)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    fileName = Guid.NewGuid() + Constract.CaputureSuffix;
                }
                else
                {
                    fileName = currBillNumber + "_" + i + "_" + Constract.CaputureSuffix;
                }
                String fileNamePath = System.IO.Path.Combine(filePath, fileName);
             bool res =   CameraHelper.CaptureJpeg(fileNamePath, CameraIds[i], mDeviceInfors[i].byChanNum);
                if (res) {                  
                    BillImage im = new BillImage() {
                        id = Guid.NewGuid().ToString(),
                        stationId = App.mStation.id,
                        stationName = App.mStation.name,
                        addUserId = App.currentUser.id,
                        addUserName = App.currentUser.name,
                        addTime = DateTime.Now,
                        billNumber = currBillNumber,
                        address = filePath+"/"+fileName,
                        positon = i,
                    };
                    DatabaseOPtionHelper.GetInstance().insert(im);
                }
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mDispatcherTimer != null) {
                mDispatcherTimer.Stop();
            }
            try { CHCNetSDK.NET_DVR_Cleanup(); } catch { }
        }
        /// <summary>
        /// 点击手动验票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            new InputWindow() { captureImg = new  Action<string>(CaptureJpeg) ,Owner = this}.ShowDialog();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            WeighingBill bill =(WeighingBill) e.Row.DataContext;

            if (bill.isReceiveMoney == 0 && bill.overtopMoney >0)
            {
                e.Row.Foreground = Brushes.Red;
            }
            else if (bill.isReceiveMoney == 1)
            {
                e.Row.Foreground = Brushes.Green;
            }
            else {
                e.Row.Foreground = Brushes.Black;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }

        private void RefreshDataBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded == false) {
                return;
            }
           double width = this.ActualWidth;
            double singleWidth = Math.Floor(width / mPictureBoxs.Count);
            for (int i = 0; i < mPictureBoxs.Count; i++)
            {
                mPictureBoxs[i].Width = Convert.ToInt32(singleWidth); 
            }
        }
        /// <summary>
        /// 打印选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
         WeighingBill bill =(WeighingBill)  this.TodayDataGrid.SelectedItem;
            if (bill == null) {
                return;
            }
            new PrintBillW(bill).ShowDialog();
        }
        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            new ReportWindow() {Owner = this }.ShowDialog();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
           //TODO login
            this.Close();
        }
    }
}
