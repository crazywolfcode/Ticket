using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MyHelper;
namespace TicketCheckStation
{
    /// <summary>
    /// CameraAddW.xaml 的交互逻辑
    ///  CameraAddW.xaml's interactive logical 
    /// </summary>
    public partial class CameraAddW : Window
    {
        private string ip = string.Empty;
        private string port = string.Empty;
        private string userName = string.Empty;
        private string pwd = string.Empty;
        private String mId;
        private CameraInfo mCameraInfo;
        #region Camera   
        public Int32 currCameraId = -1;
        public bool isInitSDK;
        public bool isLogin = false;
        public bool isPreviewSuccess = false;        
        #endregion
        public CameraAddW(String cameraId = null)
        {
            InitializeComponent();
            this.mId = cameraId;
        }
        public delegate void MyDebugInfo(string str);
        public void DebugInfo(string str)
        {
            ConsoleHelper.writeLine(str);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(mId)) {
                bindingCurrrCamera();
            }                        
            setPreviewImageHeight();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
          
        }

        private void bindingCurrrCamera() {
            String condition = CameraInfoColumuns.id.ToString() + "=" + Constract.valueSplit + mId + Constract.valueSplit;
            String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.camera_info.ToString(), null, condition);

            mCameraInfo = DatabaseOPtionHelper.GetInstance().select<CameraInfo>(sql)[0];
                  
            this.IpTB.Text = mCameraInfo.ip;
            this.portTB.Text = mCameraInfo.port;
            this.userNameTB.Text = mCameraInfo.userName;
            this.pwdTB.Text = mCameraInfo.password;
        }
        private void setPreviewImageHeight()
        {
            this.previewFormsHost.Height = this.previewStackPanel.ActualHeight;
            this.previewFormsHost.Width = this.previewStackPanel.ActualWidth;
        }
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
       
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) {
                CommonFunction.ShowErrorAlert("摄像头初始化失败！");
                return;
            }

            if (!isPreviewSuccess) {
                CommonFunction.ShowAlert("保存摄像头之前必须先预览成功！");
                return;
            }

            if (checkInput())
            {
                String condition = String.Empty;
                String sql = string.Empty;
                if (!String.IsNullOrEmpty(mId)) {
                    //update
                    mCameraInfo.lastUpdateTime =DateTime.Now;
                    mCameraInfo.ip = this.IpTB.Text.Trim();
                    mCameraInfo.port = this.portTB.Text.Trim();
                    mCameraInfo.userName = this.userNameTB.Text.Trim();
                    mCameraInfo.password = this.pwdTB.Text.Trim();
                    int res = DatabaseOPtionHelper.GetInstance().update(mCameraInfo);
                    if (res > 0)
                    {
                        CommonFunction.ShowSuccessAlert("修改成功！");
                        this.Close();
                    }
                    else
                    {
                        CommonFunction.ShowErrorAlert("修改失败！");
                        return;
                    }
                } else {
                    //insert
                    CameraInfo camera = new CameraInfo();
                    string cid = ConfigurationHelper.GetConfig(ConfigItemName.CurrStationId.ToString());                    
                    condition = CameraInfoColumuns.station_id.ToString() + "=" + Constract.valueSplit + cid + Constract.valueSplit + " and " +
                        CameraInfoColumuns.ip.ToString() + "=" + Constract.valueSplit + IpTB.Text.ToString() + Constract.valueSplit +
                        " and " + CameraInfoColumuns.port.ToString() + "=" + Constract.valueSplit + this.portTB.Text.ToString() + Constract.valueSplit;
                    sql = DatabaseOPtionHelper.GetInstance().getSelectSqlNoSoftDeleteCondition(DatabaseOPtionHelper.GetInstance().getTableName(camera), null, condition, null, null, null, 1, 0);
                    SqlDao.DbHelper optionHelper =  DatabaseOPtionHelper.GetInstance();
                   List<CameraInfo> list = optionHelper.select<CameraInfo>(sql);
                    if (list.Count > 0)
                    {
                        CommonFunction.ShowAlert("该摄像头已经存在，不要再添加拉！");
                        return;
                    }
                    else
                    {
                        camera.stationId = cid;
                        camera.companyId = ConfigurationHelper.GetConfig(ConfigItemName.CurrStationId.ToString());
                        camera.status = 0;
                        camera.isDelete = 0;
                        camera.lastUpdateTime =DateTime.Now;
                        camera.ip = this.IpTB.Text.Trim();
                        camera.port = this.portTB.Text.Trim();
                        camera.userName = this.userNameTB.Text.Trim();
                        camera.password = this.pwdTB.Text.Trim();
                        camera.id = Guid.NewGuid().ToString();
                        camera.stationName = App.mStation.name;
                        int res = optionHelper.insert(camera);
                        if (res > 0)
                        {
                            CommonFunction.ShowSuccessAlert("保存成功！");
                            this.Close();
                        }
                        else
                        {
                            CommonFunction.ShowErrorAlert("保存失败！");
                            return;
                        }
                    }
                }

               
            }           
        }
        /// <summary>
        /// preview camera
        /// 1.inint sdk 
        /// 2.login cameera
        /// 3.preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lookBtn_Click(object sender, RoutedEventArgs e)
        {

            isInitSDK = CameraHelper.InitSDK();
            if (isInitSDK)
            {
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 info = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                currCameraId = CameraHelper.loginCamera(this.IpTB.Text.Trim(),this.portTB.Text.Trim(),this.userNameTB.Text.Trim(),this.pwdTB.Text.Trim() ,ref info);
                if (currCameraId >=0)
                {
                   System.Windows.Forms.PictureBox pb = this.previewFormsHost.Child as System.Windows.Forms.PictureBox;
                    int chanlnum = (int)info.byChanNum;
                    isPreviewSuccess= CameraHelper.Preview( ref pb, chanlnum, currCameraId);
                    if (isPreviewSuccess)
                    {
                        CommonFunction.ShowSuccessAlert("预览成功！");
                    }
                    else {
                        CommonFunction.ShowErrorAlert("预览失败！");
                    }
                }
            }
            else
            {
                CommonFunction.ShowAlert("init failure");
            }          
            switchBtnVisbility();
        }

        private void nulookBtn_Click(object sender, RoutedEventArgs e)
        {
           CameraHelper.stopPreview(currCameraId);
            CameraHelper.LoginOutCamera(currCameraId);
            System.Windows.Forms.PictureBox pb = this.previewFormsHost.Child as System.Windows.Forms.PictureBox;
            pb.Image = null;
            isPreviewSuccess = false;
            switchBtnVisbility();
        }

        private bool checkInput()
        {
            ip = this.IpTB.Text.Trim();
            port = this.portTB.Text.Trim();
            userName = this.userNameTB.Text.Trim();
            pwd = this.pwdTB.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                CommonFunction.ShowAlert("摄像头的地址不能为空");
                this.IpTB.Focusable = true;
                return false;
            }
            if (!RegexHelper.IsIPv4(ip)) {
                CommonFunction.ShowAlert("摄像头的地址不是正确的IPV4地址");
                this.IpTB.Focusable = true;
                return false;
            }
            if (string.IsNullOrEmpty(port))
            {
                CommonFunction.ShowAlert("端口不能为空");
                this.portTB.Focusable = true;
                return false;
            }
            if (!RegexHelper.IsNumber(port)) {
                CommonFunction.ShowAlert("端口只是是数字");
                this.portTB.Focusable = true;
                return false;
            }
            if (string.IsNullOrEmpty(userName))
            {
                CommonFunction.ShowAlert("摄像头的登录账号不能为空");
                this.IpTB.Focusable = true;
                return false;
            }
            if (string.IsNullOrEmpty(port))
            {
                CommonFunction.ShowAlert("摄像头的登录密码不能为空");
                this.portTB.Focusable = true;
                return false;
            }
            return true;
        }

        private void switchBtnVisbility()
        {
            if (isPreviewSuccess)
            {
                lookBtn.Visibility = Visibility.Collapsed;
                this.nulookBtn.Visibility = Visibility.Visible;
            }
            else
            {
                lookBtn.Visibility = Visibility.Visible;
                this.nulookBtn.Visibility = Visibility.Collapsed;
            }
        }

 

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isInitSDK)
            {
                CHCNetSDK.NET_DVR_Cleanup();
            }        
        }

        private void previewFormsHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CommonFunction.ShowAlert("System.Windows.Forms.PictureBoxSystem.Windows.Forms.PictureBoxSystem.Windows.Forms.PictureBox");
        }
    }
}
