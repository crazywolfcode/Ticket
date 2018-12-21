using MyHelper;
using ScaleDataInterpreter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
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
        DispatcherTimer ReaderDataDispatcherTimer;
        protected IScaleDataInterpreter mScaleDataInterpreter;
        private System.IO.Ports.SerialPort mSerialPort;
        private int NoCashCount = 0;
        private int NomalDataCount = 0;
        private int NoUpDataCount = 0;
        #endregion


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartClock();

            this.CurrUserBtn.Content = App.currentUser.name;
            this.RoleNameTb.Text = App.currentUser.roleName;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ReaderWeight();

            Showcameral();

            LoadData();

        }
        #region 读取磅称数据
        /// <summary>
        /// 读取磅称数据
        /// </summary>
        private void ReaderWeight()
        {
            int ScaleBrandType = Convert.ToInt32(ConfigurationHelper.GetConfig(ConfigItemName.ScaleBrandType.ToString()));
            mSerialPort = new System.IO.Ports.SerialPort()
            {
                PortName = ConfigurationHelper.GetConfig(ConfigItemName.Com.ToString()),
                BaudRate = Convert.ToInt32(ConfigurationHelper.GetConfig(ConfigItemName.BaudRate.ToString())),
                Parity = System.IO.Ports.Parity.None,
                DataBits = Convert.ToInt32(ConfigurationHelper.GetConfig(ConfigItemName.DataBits.ToString())),
                Encoding = Encoding.UTF8
            };
            mScaleDataInterpreter = DataInterpreter.GetDataInterpreter(ScaleBrandType, mSerialPort);
            ReaderDataDispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            ReaderDataDispatcherTimer.Tick += ReaderDataDispatcherTimer_Tick;
            ReaderDataDispatcherTimer.Start();
        }

        private void ReaderDataDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ScaleDataResult result = mScaleDataInterpreter.ReadValue();
            if (result.ErrCode == 0)
            {
                this.OnePointLoading.Visibility = Visibility.Visible;
                Information(result.Msg);
                String value = Properties.Settings.Default.WeihgingValue;
                if (value.Equals(result.Value.ToString()))
                {
                    return;
                }
                else
                {
                    Properties.Settings.Default.WeihgingValue = result.Value.ToString();
                    this.WeighingDataTb.Text = result.Value.ToString();
                }
            }
            else
            {
                if (result.Value < 0)
                {
                    Warning(result.Msg);
                }
                else {
                    Alert(result.Msg);
                }                
            }
        }
        #endregion

        public void LoadData()
        {
            NoCashCount = 0;
            NoUpDataCount = 0;
            NomalDataCount = 0;
            List<WeighingBill> list = WeighingBillModel.GetTodayData();
            this.TodayDataGrid.ItemsSource = list;
        }

        #region 时钟
        private void StartClock()
        {
            mDispatcherTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1),
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
        private List<int> CameraIds;
        private List<CHCNetSDK.NET_DVR_DEVICEINFO_V30> mDeviceInfors;
        private List<System.Windows.Forms.PictureBox> mPictureBoxs;
        private void Showcameral()
        {
            cameraInfoList = new CameralInfoModel().GetList(App.mStation.id);
            double width = this.ActualWidth;
            double singleWidth = Math.Floor(width / cameraInfoList.Count);
            int camerialWidth = Convert.ToInt32(singleWidth);
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
            else
            {
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
                try
                {
                    CameraInfo camera = cameraInfoList[i];
                    WindowsFormsHost formsHost = new WindowsFormsHost();
                    System.Windows.Forms.PictureBox pictureBox;
                    pictureBox = new System.Windows.Forms.PictureBox()
                    {
                        Name = $"pictureBox{i}",
                        Width = camerialWidth,
                        Height = camerialHeight
                    };
                    mPictureBoxs.Add(pictureBox);
                    CHCNetSDK.NET_DVR_DEVICEINFO_V30 devieInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                    //login
                    int cameraid = CameraHelper.loginCamera(camera.ip, camera.port, camera.userName, camera.password, ref devieInfo);
                    if (cameraid <= -1)
                    {
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
                }
                catch (Exception e)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Width = camerialWidth,
                        Height = camerialHeight,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "视频加载失败: " + e.Message,
                        FontSize = 16,
                        Foreground = Brushes.Red
                    };
                    VideoPenal.Children.Add(textBlock);
                }
            }
            this.Cursor = Cursors.Arrow;
        }

        private void StorpPreviewCamera()
        {
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
        private void LogoutCamera()
        {
            if (CameraIds != null && CameraIds.Count > 0)
            {
                for (int i = 0; i < CameraIds.Count; i++)
                {
                    try
                    {
                        CameraHelper.LoginOutCamera(CameraIds[i]);
                    }
                    catch (Exception e)
                    {
                        ConsoleHelper.writeLine("退出摄像头" + i + "失败: " + e.Message);
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
                bool res = CameraHelper.CaptureJpeg(fileNamePath, CameraIds[i], mDeviceInfors[i].byChanNum);
                if (res)
                {
                    BillImage im = new BillImage()
                    {
                        id = Guid.NewGuid().ToString(),
                        stationId = App.mStation.id,
                        stationName = App.mStation.name,
                        addUserId = App.currentUser.id,
                        addUserName = App.currentUser.name,
                        addTime = DateTime.Now,
                        billNumber = currBillNumber,
                        address = filePath + "/" + fileName,
                        positon = i,
                    };
                    DatabaseOPtionHelper.GetInstance().insert(im);
                }
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mDispatcherTimer != null)
            {
                mDispatcherTimer.Stop();
            }
            if (ReaderDataDispatcherTimer != null)
            {
                ReaderDataDispatcherTimer.Stop();
            }
            try
            {
                if (mSerialPort != null)
                {
                    mSerialPort.Close();
                    mSerialPort.Dispose();
                }
            }
            catch { }
            try { CHCNetSDK.NET_DVR_Cleanup(); } catch { }
        }
        /// <summary>
        /// 点击手动验票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.currentUser.roleLevel == (int)RoleLevelType.YPY || App.currentUser.roleLevel == (int)RoleLevelType.SHY)
            {
                new InputWindow() { captureImg = new Action<string>(CaptureJpeg), Owner = this }.ShowDialog();
            }
            else
            {
                MyCustomControlLibrary.MMessageBox.GetInstance().ShowBox("无权限操作！", "提示", MyCustomControlLibrary.MMessageBox.ButtonType.Yes, MyCustomControlLibrary.MMessageBox.IconType.warring, Orientation.Horizontal, "好");
                return;
            }
        }
        #region datagrid
        private void setDataCount()
        {
            this.NoCashSBI.Content = NoCashCount.ToString();
            this.NomalSBI.Content = NomalDataCount.ToString();
            this.NoUpSBI.Content = NoUpDataCount.ToString();
        }
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            WeighingBill bill = (WeighingBill)e.Row.DataContext;
            if (bill.isUp == 0) {
                NoUpDataCount ++;
            }            
            if (bill.isReceiveMoney == 0 && bill.overtopMoney > 0)
            {
                e.Row.Foreground = Brushes.Red;
                NoCashCount++;
            }
            else {
                NomalDataCount++;
                if (bill.isReceiveMoney == 1)
                {
                    e.Row.Foreground = Brushes.Green;
                }
                else
                {
                    e.Row.Foreground = Brushes.Black;
                }
            }
            if(e.Row.GetIndex ().Equals( TodayDataGrid.Items.Count-1))
            {
                setDataCount();
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }
        private void cashBtn_Click(object sender, RoutedEventArgs e)
        {
            MyCustomControlLibrary.IconButton button = sender as MyCustomControlLibrary.IconButton;
            WeighingBill bill = button.Tag as WeighingBill;
            MessageBox.Show(bill.carNumber + bill.overtopMoney.ToString());
        }
        #endregion
        private void RefreshDataBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded == false)
            {
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
            WeighingBill bill = (WeighingBill)this.TodayDataGrid.SelectedItem;
            if (bill == null)
            {
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
            new ReportWindow() { Owner = this }.ShowDialog();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow() { IsChangeAccount = true }.Show();
            this.Close();
        }
        /// <summary>
        ///  MenuItem_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item == null)
            {
                return;
            }
            switch (item.Name)
            {
                case "BaseSettingMI":
                   
                    break;
                case "HeithtSettintMI":

                    break;
                case "SystemSettintMI":

                    break;
                case "UserManagerMI":

                    break;
                case "AboutMI":
                    new AboutW().ShowDialog();
                    break;
                case "ConnMeMI":
                    new ConnectionWindow().ShowDialog();
                    break;
                    
            }
        }

        #region status Bar
        /// <summary>
        /// 警告信息提示（一直提示）
        /// </summary>
        /// <param name="message">提示信息</param>
        private void Alert(string message)
        {
            // #FF68217A
            this.StatusBarSb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x21, 0x2A));
            AlertBarItemTb.Text = message;
        }

        /// <summary>
        /// 普通状态信息提示
        /// </summary>
        /// <param name="message">提示信息</param>
        private void Information(string message)
        {
            this.StatusBarSb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x7a, 0xcc));
            AlertBarItemTb.Text = message;
        }

        /// <summary>
        /// 警告状态信息提示
        /// </summary>
        /// <param name="message">提示信息</param>
        private void Warning(string message)
        {
            // #FFCA5100 警告
            this.StatusBarSb.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCA, 0x51, 0x00));
            AlertBarItemTb.Text = message;
        }

        #endregion



      
    }
}
