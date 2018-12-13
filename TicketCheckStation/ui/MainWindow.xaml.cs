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
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Showcameral();
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
            int camerialWidth = 300;
            int camerialHeight = 300;
            cameraInfoList = new CameralInfoModel().GetList(App.mStation.id);
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
         }

        private void StorpPreviewCamera() { }
        private void LogoutCamera() { }
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

        }
    }
}
