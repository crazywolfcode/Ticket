﻿using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MyCustomControlLibrary;
using System.Text;

namespace TicketCheckStation
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public Action<string> captureImg { get; set; }

        private WeighingBill mWeighingBill;
        private String currBillNumber;
        private bool IsInsert = true;
        private bool IsReadCard = true;
        public InputWindow(WeighingBill bill = null, bool isInsert = true, bool isReadCard = false)
        {
            InitializeComponent();
            mWeighingBill = bill;
            this.IsInsert = isInsert;
            this.IsReadCard = isReadCard;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsReadCard) {
                if (mWeighingBill == null) {
                    this.Close();
                    return;
                }
                //构建磅单信息 并转到打印                
                mWeighingBill.id = new Guid().ToString();
                mWeighingBill.addTime = DateTime.Now;
                mWeighingBill.addUserId = App.currentUser.id;
                mWeighingBill.addUserName = App.currentUser.name;
                mWeighingBill.lastUpdateTime = mWeighingBill.addTime;
                mWeighingBill.lastUpdateUserId = mWeighingBill.addUserId;
                mWeighingBill.lastUpdateUserName = mWeighingBill.addUserName;
                mWeighingBill.operatorId = App.currentUser.id;
                mWeighingBill.operatorName = App.currentUser.name;
            } else {
                //将本机使用的基础数据设置默认数据源 下5个方法
                SetSupplyCompanyDefaultSource(this.SupplyCb);
                SetCustomerCompanyDefaultSource(this.ReceiverCompanyCb);
                SetMaterialDefaultSource(this.MaterialNameCb);
                SetCarDefaultSource(this.CarNumberCb);
                SetRemarkDefaultSource(this.RemardCombox);
                //mWeighing bill is null is Insert New else  is modenfy bill 

                BuildCurrWeighingBill();

            }
        }

        /// <summary>
        /// 构建当前的磅单
        /// </summary>
        private void BuildCurrWeighingBill()
        {
            if (mWeighingBill == null)
            {
                //Add new 
                mWeighingBill = new WeighingBill()
                {
                    id = new Guid().ToString(),
                    addTime = DateTime.Now,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name
                };
                mWeighingBill.lastUpdateTime = mWeighingBill.addTime;
                mWeighingBill.lastUpdateUserId = mWeighingBill.addUserId;
                mWeighingBill.lastUpdateUserName = mWeighingBill.addUserName;
                mWeighingBill.operatorId = App.currentUser.id;
                mWeighingBill.operatorName = App.currentUser.name;
            }
            else
            {
                //Update
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //  List<String> list = new List<string>() {"富源县天鑫煤业有限公司","富源县老厂镇宏发煤矿","富源县十八连山镇雄达煤矿","兴义市云黔工贸有限责任公司","黔西南州兴义兴富煤矿","富源县老厂乡恒达煤矿","贵州黔能机电设备有限公司" };
            //  this.SupplyCb.ItemsSource = list;
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void ZhuaTu() {
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(delegate {
                if (this.captureImg != null) {
                    captureImg(currBillNumber);
                }
            });
            new Thread(threadStart).Start((object)currBillNumber);
        }

        #region 将本机使用的基础数据设置默认数据源 最新使用的排在最前面
        protected void SetSupplyCompanyDefaultSource(ComboBox SupplyCb)
        {
            if (App.tempSupplyCompanys.Count <= 0)
            {
                String path = System.IO.Path.Combine(Constract.tempPath, Constract.tempSupplyFileName);
                String xml = String.Empty;
                if (FileHelper.Exists(path))
                {
                    xml = FileHelper.Reader(path, Encoding.UTF8);
                    if (String.IsNullOrEmpty(xml))
                    {
                        SupplyCb.ItemsSource = null;
                        return;
                    }

                    if (App.tempSupplyCompanys.Count <= 0)
                    {
                        List<Company> list = (List<Company>)XmlHelper.Deserialize(typeof(List<Company>), xml);
                        foreach (Company cp in list)
                        {
                            App.tempSupplyCompanys.Add(cp.id, cp);
                        }
                        App.tempSupplyCompanys = App.tempSupplyCompanys.OrderByDescending(O => O.Value.lastUpdateTime).ToDictionary(p => p.Key, O => O.Value);
                    }
                    SupplyCb.ItemsSource = App.tempSupplyCompanys.Values.ToList();
                }
                else
                {
                    FileHelper.CreateFile(path);
                }
            }
        }
        protected void SetCustomerCompanyDefaultSource(ComboBox ReceiverCompanyCb)
        {
            if (App.tempCustomerCompanys.Count <= 0)
            {
                String path = System.IO.Path.Combine(Constract.tempPath, Constract.tempCustomerFileName);
                String xml = String.Empty;
                if (FileHelper.Exists(path))
                {
                    xml = FileHelper.Reader(path, Encoding.UTF8);
                    if (String.IsNullOrEmpty(xml))
                    {
                        ReceiverCompanyCb.ItemsSource = null;
                        return;
                    }
                    if (App.tempCustomerCompanys.Count <= 0)
                    {
                        List<Company> list = (List<Company>)XmlHelper.Deserialize(typeof(List<Company>), xml);
                        foreach (Company cp in list)
                        {
                            App.tempCustomerCompanys.Add(cp.id, cp);
                        }
                        App.tempCustomerCompanys = App.tempCustomerCompanys.OrderByDescending(O => O.Value.lastUpdateTime).ToDictionary(p => p.Key, O => O.Value);
                    }
                    ReceiverCompanyCb.ItemsSource = App.tempCustomerCompanys.Values.ToList();
                }
                else
                {
                    FileHelper.CreateFile(path);
                }
            }
        }
        protected void SetMaterialDefaultSource(ComboBox MaterialNameCb)
        {
            if (App.tempMaterials.Count <= 0)
            {
                String path = System.IO.Path.Combine(Constract.tempPath, Constract.tempMatreialFileName);
                String xml = String.Empty;
                if (FileHelper.Exists(path))
                {
                    xml = FileHelper.Reader(path, Encoding.UTF8);
                    if (String.IsNullOrEmpty(xml))
                    {
                        MaterialNameCb.ItemsSource = null;
                        return;
                    }

                    if (App.tempMaterials.Count <= 0)
                    {
                        List<Material> list = (List<Material>)XmlHelper.Deserialize(typeof(List<Material>), xml);
                        foreach (Material material in list)
                        {
                            App.tempMaterials.Add(material.id, material);
                        }
                        App.tempMaterials = App.tempMaterials.OrderByDescending(O => O.Value.lastUpdateTime).ToDictionary(p => p.Key, O => O.Value);
                    }
                    MaterialNameCb.ItemsSource = App.tempMaterials.Values.ToList();
                }
                else
                {
                    FileHelper.CreateFile(path);
                }
            }
        }
        protected void SetCarDefaultSource(ComboBox CarNumberCb)
        {

            if (App.tempCars.Count <= 0)
            {
                String path = System.IO.Path.Combine(Constract.tempPath, Constract.tempCarFileName);
                String xml = String.Empty;
                if (FileHelper.Exists(path))
                {
                    xml = FileHelper.Reader(path, Encoding.UTF8);
                    if (String.IsNullOrEmpty(xml))
                    {
                        CarNumberCb.ItemsSource = null;
                        return;
                    }

                    if (App.tempCars.Count <= 0)
                    {
                        List<CarInfo> list = (List<CarInfo>)XmlHelper.Deserialize(typeof(List<CarInfo>), xml);
                        foreach (CarInfo car in list)
                        {
                            App.tempCars.Add(car.id, car);
                        }
                        App.tempCars = App.tempCars.OrderByDescending(O => O.Value.lastUpdateTime).ToDictionary(p => p.Key, O => O.Value);
                    }
                    CarNumberCb.ItemsSource = App.tempCars.Values.ToList();
                }
                else
                {
                    FileHelper.CreateFile(path);
                }
            }
        }
        protected void SetRemarkDefaultSource(ComboBox RemardCombox)
        {
            RemardCombox.ItemsSource = null;
            RemardCombox.ItemsSource = App.inputRemarkList;
        }
        #endregion

        /// <summary>
        /// 显示提示窗口
        /// </summary>
        /// <param name="content">提示内容</param>
        /// <param name="Title">标题</param>
        ///   /// <param name="orientation">方向</param>
        protected void ShowAlert(String content, String Title = "提示", Orientation orientation = Orientation.Horizontal)
        {
            MMessageBox.GetInstance().ShowBox(content, Title, MMessageBox.ButtonType.No, MMessageBox.IconType.error, orientation);
        }

        protected MMessageBox.Result ShowAlertResult()
        {
            return MMessageBox.GetInstance().ShowBox("保存成功 ! 要继续过磅吗？", "恭喜", MMessageBox.ButtonType.YesNo, MMessageBox.IconType.success, Orientation.Vertical, "是");
        }


        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            String remark = this.RemardCombox.Text.Trim();
            if (!String.IsNullOrEmpty(remark)) {
                mWeighingBill.remark = remark;
                UpdateRemark();
            }

            UpdateUsedBaseData();
        }


        #region Supply
        private Company sendCompany;
        private void SupplyCb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.SupplyCb.SelectedIndex > 0) {
                return;
            }
            String tempStr = this.SupplyCb.Text.Trim();
            if (String.IsNullOrEmpty(tempStr)) {
                this.SupplyCb.SelectedIndex = -1;
            }
            //最少要输入两位字符
            if (tempStr.Length < 2) {
                this.SupplyCb.ItemsSource = App.tempSupplyCompanys.Values.ToList();
                return;
            }
            List<Company> tempList = CompanyModel.IndistinctSearchByNameOrNameFirstCase(tempStr);
            this.SupplyCb.ItemsSource = tempList;
            if (this.SupplyCb.ItemsSource != null) {
                this.SupplyCb.IsDropDownOpen = true;
            }
        }

        private void SupplyCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SupplyCb.SelectedIndex < 0)
            {
                sendCompany = null;
            }
            else {
                sendCompany = this.SupplyCb.SelectedItem as Company;
                if (checkSupplyCustomer() == false)
                {
                    MMessageBox.GetInstance().ShowBox("发货公司和收货公司不能是同一个！", "提示", MMessageBox.ButtonType.Yes, MMessageBox.IconType.Info, Orientation.Vertical);
                    //this.SupplyCb.Text = null;
                    this.SupplyCb.SelectedIndex = -1;
                    sendCompany = null;
                    return;
                }
            }
        }

        /// <summary>
        /// 供应商和客户是不能是同一个公司
        /// </summary>
        /// <returns></returns>
        private bool checkSupplyCustomer()
        {
            if (receiverCompany != null && sendCompany != null)
            {
                if (receiverCompany.id == sendCompany.id)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion supply


        #region Receiver Company
        private Company receiverCompany;
        private void ReceiverCompanyCb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ReceiverCompanyCb.SelectedIndex > 0)
            {
                this.ReceiverCompanyCb.ItemsSource = App.tempCustomerCompanys.Values.ToList();
                return;
            }
            String tempStr = this.ReceiverCompanyCb.Text.Trim();
            if (String.IsNullOrEmpty(tempStr))
            {
                this.ReceiverCompanyCb.SelectedIndex = -1;
            }
            //最少要输入两位字符
            if (tempStr.Length < 2)
            {
                return;
            }
            List<Company> tempList = CompanyModel.IndistinctSearchByNameOrNameFirstCase(tempStr);
            this.ReceiverCompanyCb.ItemsSource = tempList;
            if (this.ReceiverCompanyCb.ItemsSource != null)
            {
                this.ReceiverCompanyCb.IsDropDownOpen = true;
            }
        }

        private void ReceiverCompanyCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ReceiverCompanyCb.SelectedIndex == -1)
            {
                receiverCompany = null;
            }
            else
            {
                receiverCompany = this.ReceiverCompanyCb.SelectedItem as Company;
                if (checkSupplyCustomer() == false)
                {
                    MMessageBox.GetInstance().ShowBox("发货公司和收货公司不能是同一个！", "提示", MMessageBox.ButtonType.Yes, MMessageBox.IconType.Info, Orientation.Vertical);
                    //this.SupplyCb.Text = null;
                    this.ReceiverCompanyCb.SelectedIndex = -1;
                    receiverCompany = null;
                    return;
                }
            }
        }
        #endregion

        #region material
        private Material mMaterial;
        private void MaterialNameCb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.MaterialNameCb.SelectedIndex > 0)
            {
                return;
            }
            String tempStr = this.MaterialNameCb.Text.Trim();
            if (String.IsNullOrEmpty(tempStr))
            {
                this.SupplyCb.SelectedIndex = -1;
            }
            //最少要输入一位字符
            if (tempStr.Length < 1)
            {
                this.MaterialNameCb.ItemsSource = App.tempMaterials.Values.ToList();
                return;
            }
            List<Material> tempList = MaterialModel.IndistinctSearchByNameOrNameFirstCase(tempStr);
            this.MaterialNameCb.ItemsSource = tempList;
            if (this.MaterialNameCb.ItemsSource != null)
            {
                this.MaterialNameCb.IsDropDownOpen = true;
            }
        }

        private void MaterialNameCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MaterialNameCb.SelectedIndex < 0)
            {
                mMaterial = null;
            }
            else
            {
                mMaterial = this.MaterialNameCb.SelectedItem as Material;
            }
        }
        #endregion

        #region Car info
        private CarInfo carInfo;
        private void CarNumberCb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.CarNumberCb.SelectedIndex > -1)
            {
                return;
            }
            String tempStr = this.CarNumberCb.Text.Trim();
            if (String.IsNullOrEmpty(tempStr))
            {
                this.CarNumberCb.SelectedIndex = -1;
            }
            //最少要输入2位字符
            if (tempStr.Length < 2)
            {
                this.CarNumberCb.ItemsSource = App.tempCars.Values.ToList();
                return;
            }
            List<CarInfo> tempList = CarInfoModel.FuzzySearch(tempStr);
            this.CarNumberCb.ItemsSource = tempList;
            if (this.CarNumberCb.ItemsSource != null)
            {
                this.CarNumberCb.IsDropDownOpen = true;
            }
        }

        private void CarNumberCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CarNumberCb.SelectedIndex == -1)
            {
                carInfo = null;
                this.DriverTbox.Text = String.Empty;
                this.PhoneTbox.Text = String.Empty;
                this.CarTraeWeightTbox.Text = "0.0";
            }
            else
            {
                carInfo = this.CarNumberCb.SelectedItem as CarInfo;
                this.DriverTbox.Text = carInfo.driver;
                this.PhoneTbox.Text = carInfo.driverMobile;
                this.CarTraeWeightTbox.Text = carInfo.traeWeight.ToString();
            }
        }
        #endregion
        protected void UpdateUsedBaseData()
        {
            // success to do TempUpdateUsedBase
            Thread thread = new Thread(new ParameterizedThreadStart(CommonFunction.TempUpdateUsedBase));
            thread.Start(new BaseDataClassV() { send = sendCompany, receive = receiverCompany, material = mMaterial, carInfo = carInfo });
            SetCustomerCompanyDefaultSource(ReceiverCompanyCb);
            SetSupplyCompanyDefaultSource(SupplyCb);
            SetMaterialDefaultSource(MaterialNameCb);
            SetCarDefaultSource(CarNumberCb);
        }
        /// <summary>
        /// update decuation description and remark list
        /// </summary>
        private void UpdateRemark()
        {
            Thread thread = new Thread(new ParameterizedThreadStart(CommonFunction.UpdateInputReamak));
            thread.Start(mWeighingBill.remark);
            SetRemarkDefaultSource(this.RemardCombox);
        }


        #region Calc Weight and overstop monet
     
        /// <summary>
        /// 重量吨位的计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Weight_text_changed(object sender, TextChangedEventArgs e)
        {
            if (!this.IsLoaded) {
                return;
            }
            TextBox tb = sender as TextBox;
            if (tb.Text == String.Empty || tb.Text.Length <= 0)
            {
                tb.Text = "0.0";
            }
            else {
                try
                {
                    Convert.ToDouble(tb.Text.Trim());
                   
                }
                catch (Exception exc) {
                    MMessageBox.GetInstance().ShowBox("只能输入数字："+tb.Name, Title, MMessageBox.ButtonType.No, MMessageBox.IconType.error);
                    Console.WriteLine("===========只能输入数字:" + exc.Message);
                }
                CalcWeight();
            }
        }
        private void CalcWeight()
        {
            if (carInfo == null || mMaterial ==null) { return; }
            Double sendGross = 0.0;
            Double sendTrae = 0.0;
            Double sendNet = 0.0;
            Double gross = 0.0;
            Double carTrae = 0.0;
            Double taxationPrice =mMaterial.currTaxation;
            Double netWeight = 0.0;
            Double overStopWeight = 0.0;
            Double limitTone = Convert.ToDouble(MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.limitTone.ToString()));
            sendGross = Convert.ToDouble(this.SendGrossWeightTbox.Text.Trim());
            sendTrae = Convert.ToDouble(this.SendTraeWeightTbox.Text.Trim());
            gross = Convert.ToDouble(this.GrossWeightTbox.Text.Trim());           
            carTrae = carInfo.traeWeight;

            sendNet = System.Math.Round(sendGross - sendTrae, 2);
            this.SendNetWeightTbox.Text = sendNet.ToString();
            netWeight = System.Math.Round(gross - carTrae, 2);
            this.NetWeightTbox.Text = netWeight.ToString();

            overStopWeight = System.Math.Round(netWeight - (sendNet+limitTone), 2);
            this.differenceWeightTbox.Text = overStopWeight.ToString();
            this.LimitTbox.Text = limitTone.ToString();
            this.TaxationMoneyTbox.Text = System.Math.Round(limitTone * overStopWeight, 2).ToString();
        }
        #endregion
    }
}
