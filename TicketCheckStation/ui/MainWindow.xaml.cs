using IcReaderSdk;
using MyCustomControlLibrary;
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
        DispatcherTimer ICReaderDispatcherTimer;
        protected IScaleDataInterpreter mScaleDataInterpreter;
        private System.IO.Ports.SerialPort mSerialPort;
        private int NoCashCount = 0;
        private int NomalDataCount = 0;
        private int NoUpDataCount = 0;
        #endregion

        public MainWindow()
        {
            App.Current.MainWindow = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartClock();
            App.Current.MainWindow = this;
            this.CurrUserBtn.Content = App.currentUser.name;
            this.RoleNameTb.Text = App.currentUser.roleName;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ReaderWeight();

            Showcameral();

            LoadData();

            ReaderIcCard();
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
                else
                {
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

        #region Reader Ic card
        public int icdev; // 通讯设备标识符
        public Int16 st;
        public int sec;
        public uint snr;
        private void ReaderIcCard()
        {
            string IcCom = ConfigurationHelper.GetConfig(ConfigItemName.IcCom.ToString());
            int IcBaudRate = Convert.ToInt32(ConfigurationHelper.GetConfig(ConfigItemName.IcBaudRate.ToString()));
            byte[] ver = new byte[30];
            st = common.lib_ver(ver);
            string c = IcCom.Substring(IcCom.Length - 1, 1);
            int com = Convert.ToInt32(c);
            if (com > 0)
            {
                com = com - 1;
            }
            icdev = common.rf_init((short)com, IcBaudRate);
            if (icdev > 0)
            {
                byte[] status = new byte[30];
                st = common.rf_get_status(icdev, status);
                ICReaderTb.Text = "打开读写器成功！硬件版本：" + Encoding.ASCII.GetString(status).Substring(5, 10);
                common.rf_beep(icdev, 5);
                ICReaderDispatcherTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(3000) };
                ICReaderDispatcherTimer.Tick += ICReaderDispatcherTimer_Tick;
                ICReaderDispatcherTimer.Start();
            }
            else
            {
                ICReaderTb.Text = "打开读写器失败！";
            }
        }
        //是否在验票中
        private bool isChecking = false;
        private void ICReaderDispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (isChecking == true)
            {
                ICReaderDispatcherTimer.Stop();
                return;
            }
            Mifareone.rf_reset(icdev, 3);
            st = common.rf_card(icdev, 1, out snr);
            if (st == 0)
            {
                foreach (Window item in Application.Current.Windows)
                {
                    if (item.Title == "") continue;
                    if (item.Title != this.Title)
                        item.Close();
                }
                if (this.WindowState == WindowState.Minimized)
                {
                    this.WindowState = WindowState.Maximized;
                    this.ShowActivated = true;
                }
                isChecking = true;
                ICReaderDispatcherTimer.Stop();
                common.rf_beep(icdev, 2);
                common.rf_beep(icdev, 2);
                this.IsEnabled = false;
                readerCard();
                isChecking = false;
            }
            else
            {
                ConsoleHelper.writeLine("寻卡失败！:" + st);
            }

        }
        private void readerCard()
        {
            Size size = new Size(MainBodyGrid.ActualWidth, MainBodyGrid.ActualHeight);
            Point point = MainBodyGrid.PointToScreen(new Point());
            MMessageBox.GetInstance().ShowLoading(MMessageBox.LoadType.Three, "验票中...卡号:" + snr, point, size, "&#xe752;", Orientation.Vertical, "#ffffff", 12);

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(
                delegate
                {
                    Mifareone.rf_reset(icdev, 3);
                    common.rf_card(icdev, 1, out snr);
                    Console.WriteLine("卡的序列号：" + snr);
                    byte[] key1 = Encoding.ASCII.GetBytes("FFFFFFFFFFFF");
                    byte[] key2 = new byte[6];
                    common.a_hex(key1, key2, 12);
                    String[] HexValues = new string[16];
                    for (int i = 0; i < 16; i++)
                    {
                        common.rf_load_key(icdev, 0, i, key2);
                        Mifareone.rf_authentication(icdev, 1, i);
                        common.rf_beep(icdev, 3);
                        int j = 0;
                        if (i == 0)
                        {
                            j = 1;
                        }
                        for (; j < 3; j++)
                        {
                            byte[] data = new byte[16];
                            byte[] buff = new byte[32];
                            st = Mifareone.rf_read(icdev, i * 4 + j, data);
                            if (st == 0)
                            {
                                common.hex_a(data, buff, 16);
                                string vastr = Encoding.ASCII.GetString(buff);
                                if (!vastr.StartsWith("0000"))
                                {
                                    if (vastr.Contains("0000"))
                                    {
                                        vastr = vastr.Substring(0, vastr.IndexOf("0000"));
                                        if (vastr.Length % 2 == 0)
                                        {
                                            HexValues[i] += vastr + "20";
                                        }
                                        else
                                        {
                                            HexValues[i] += vastr + "020";
                                        }
                                    }
                                    else
                                    {
                                        HexValues[i] += vastr;
                                    }
                                }
                            }
                        }
                    }
                    String[] strValues = new string[HexValues.Length];
                    for (int k = 0; k < HexValues.Length; k++)
                    {
                        String res = ICReaderHelper.HexToStr(HexValues[k]);
                        strValues[k] = res;
                        Console.WriteLine("=====" + k + " 区：" + res);
                    }
                    SendBill sendBill = BillFactory.CreateSendbill(strValues, snr.ToString());
                    WeighingBill weighingBill = BillFactory.CreateWeightBill(sendBill);
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        MMessageBox.GetInstance().Close();
                        this.IsEnabled = true;
                        if (String.IsNullOrEmpty(sendBill.numeber))
                        {
                            MMessageBox.GetInstance().ShowLoading(MMessageBox.LoadType.Two, "数据没有读完", new Point(0, 0), new Size(0, 0), null, Orientation.Vertical, Brushes.Red, 3);
                            ICReaderDispatcherTimer.Start();
                            return;
                        }
                        if (double.Parse(Properties.Settings.Default.WeihgingValue) <= 0)
                        {
                            MMessageBox.GetInstance().ShowBox("磅称可能没有读取到数据,不能验票！", "错误", MMessageBox.ButtonType.Yes, MMessageBox.IconType.error, Orientation.Vertical, "好");
                        }
                        else
                        {
                            new InputWindow(weighingBill, true, true,
                                new Action<bool>(RefreshData),
                                new Action<string>(CaptureJpeg));
                        }
                        ICReaderDispatcherTimer.Start();
                    }));
                }
                ));

            thread.Start();
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
            String title = "煤炭运煤监管系统 ";
            String text = "最小化在到这里";
            App.ShowBalloonTip(title, text);
            return;
            if (mDispatcherTimer != null)
            {
                mDispatcherTimer.Stop();
            }
            if (ReaderDataDispatcherTimer != null)
            {
                ReaderDataDispatcherTimer.Stop();
            }
            if (ICReaderDispatcherTimer != null)
            {
                ICReaderDispatcherTimer.Stop();
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
            isChecking = true;
            if (App.currentUser.roleLevel == (int)RoleLevelType.YPY || App.currentUser.roleLevel == (int)RoleLevelType.SHY)
            {
                new InputWindow() { CaptureImg = new Action<string>(CaptureJpeg), Owner = this }.ShowDialog();
                isChecking = false;
                try
                {
                    this.ICReaderDispatcherTimer.Start();
                }
                catch { }
            }
            else
            {
                MMessageBox.GetInstance().ShowBox("无权限操作！", "提示", MMessageBox.ButtonType.Yes, MMessageBox.IconType.warring, Orientation.Horizontal, "好");
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
            if (bill.isUp == 0)
            {
                NoUpDataCount++;
            }
            if (bill.isReceiveMoney == 0 && bill.overtopMoney > 0)
            {
                e.Row.Foreground = Brushes.Red;
                NoCashCount++;
            }
            else
            {
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
            if (e.Row.GetIndex().Equals(TodayDataGrid.Items.Count - 1))
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
            IconButton button = sender as IconButton;
            if (button.Tag is WeighingBill bill)
            {
                isChecking = true;
                new CashWindow(bill) { RefreshParent = new Action<bool>(RefreshData) }.ShowDialog();
                isChecking = false;
                try
                {
                    this.ICReaderDispatcherTimer.Start();
                }
                catch { }
            }
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
                MMessageBox.Result result = MMessageBox.GetInstance().ShowBox("未选择要打印的数据", "提示", MMessageBox.ButtonType.YesNo, MMessageBox.IconType.Info, Orientation.Vertical, "是");
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
            //CarManageMI AddMaterialMI MaterialManageMI CompanyManageMI
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
                case "CarManageMI":
                    new CarManageWindow().ShowDialog();
                    break;
                case "AddMaterialMI":

                    break;
                case "MaterialManageMI":

                    break;
                case "CompanyManageMI":
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


        private void RefreshData(bool obj)
        {
            if (obj == true)
            {
                LoadData();
            }
        }

        private void TaxationBtn_Click(object sender, RoutedEventArgs e)
        {
            new CashReportWindow().Show();
        }
    }
}
