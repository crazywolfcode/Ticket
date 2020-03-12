﻿using MyCustomControlLibrary;
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
    public partial class CashReportWindow : Window
    {
        private double totalWeight = 0;
        private double totalMoney = 0;
        public CashReportWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(-1);
            this.StratDatePicker.Text = d1.ToString("yyyy年MM月dd日");
            this.EndDatePicker.Text = now.ToString("yyyy年MM月dd日");
            this.EndDatePicker.DisplayDateEnd = now;
            this.StratDatePicker.DisplayDateEnd = now;
            this.PrintTitleTb.Text = ConfigurationHelper.GetConfig(ConfigItemName.CashReportTitle.ToString());
            this.StationNametb.Text = "(" + App.mStation.name + "-补税记录)";
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.StationCb.ItemsSource = new List<Station>() { App.mStation };
            this.StationCb.SelectedIndex = 0;
            LoadData();
        }
        public void LoadData()
        {
            String condition = GetSeachCondition();
            var data = BillTaxationMoneyRecordModel.searchData(condition);
            if (data.Count == 0)
            {
                MMessageBox.Result reault = MMessageBox.GetInstance().ShowBox(
                  "没有查询到任何数据！",
                  "提示",
                  MMessageBox.ButtonType.Yes,
                  MMessageBox.IconType.Info,
                   Orientation.Horizontal,
                   "知道了"
                  );
            }
            this.ReportDataGrid.ItemsSource = data;
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


        #region Search Tab Button
        private void SearchTabBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.searchAreaPanel.Height == 0)
            {
                SearAreaAnimationToShow();
            }
            else
            {
                SearAreaAnimationToHidden();
            }
        }
        private void SearAreaAnimationToShow()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 60;
            animation.Duration = TimeSpan.FromSeconds(0.8);
            this.searchAreaPanel.BeginAnimation(HeightProperty, animation);
        }
        private void SearAreaAnimationToHidden()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 60;
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.5);
            this.searchAreaPanel.BeginAnimation(HeightProperty, animation);
        }
        #endregion

        #region EXport EXCL，
        private void ExportExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReportDataGrid.Items.Count <= 0)
            {
               MMessageBox.GetInstance().ShowBox(
                   "没有要导出的数据！",
                   "错误",
                   MMessageBox.ButtonType.Yes,
                   MMessageBox.IconType.error,
                   Orientation.Vertical,
                   "知道了");
                return;
            }

            this.IsEnabled = false;
            MMessageBox.GetInstance().ShowLoading(
                MMessageBox.LoadType.One,
                "导出中。。。",
                new Point(),
                new Size(100, 100),
                "&#xe752;",
                Orientation.Vertical,
                "#ffffff",
                5);
            ConsoleHelper.writeLine("110000");
            ExclHelper.ExclExprotToExcelWitchStatisticInfo(
                            this.ReportDataGrid,
                            DateTimeHelper.getCurrentDateTime(DateTimeHelper.DateFormat),
                            this.PrintTitleTb.Text,
                            this.StationNametb.Text,
                            this.StationNametb.Text + "导出时间:" + DateTimeHelper.getCurrentDateTime(),
                            this.SummaryTextBlock.Text,
                            GetListStatisticToListString(this.InStatisticListBox)
                    );
            this.IsEnabled = true;
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

        #region Supply
        private Company sendCompany;
        private void SupplyCb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.SupplyCb.SelectedIndex >= 0)
            {
                return;
            }
            String tempStr = this.SupplyCb.Text.Trim();
            if (String.IsNullOrEmpty(tempStr))
            {
                this.SupplyCb.SelectedIndex = -1;
            }
            //最少要输入两位字符
            if (tempStr.Length < 2)
            {
                this.SupplyCb.ItemsSource = App.tempSupplyCompanys.Values.ToList();
                return;
            }
            List<Company> tempList = CompanyModel.IndistinctSearchByNameOrNameFirstCase(tempStr);
            this.SupplyCb.ItemsSource = tempList;
            if (this.SupplyCb.ItemsSource != null)
            {
                this.SupplyCb.IsDropDownOpen = true;
            }
        }

        private void SupplyCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SupplyCb.SelectedIndex < 0)
            {
                sendCompany = null;
            }
            else
            {
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
            if (this.ReceiverCompanyCb.SelectedIndex > -1)
            {
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
                this.ReceiverCompanyCb.ItemsSource = App.tempCustomerCompanys.Values.ToList();
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
            if (this.MaterialNameCb.SelectedIndex >= 0)
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

            }
            else
            {
                carInfo = this.CarNumberCb.SelectedItem as CarInfo;
            }
        }
        #endregion
        private string GetSeachCondition()
        {
            string condition = string.Empty;
            condition = BillTaxationMoneyRecordColumuns.add_time.ToString() + ">='" + GetStartDate() + "'";
            condition = condition + " and " + BillTaxationMoneyRecordColumuns.add_time.ToString() + "<='" + GetEndDate() + "'";
            condition = condition + " and " + BillTaxationMoneyRecordColumuns.station_id.ToString() + "='" + mStation.id + "'";
            if (sendCompany != null)
            {
                if (String.IsNullOrEmpty(condition))
                {
                    condition += BillTaxationMoneyRecordColumuns.send_company.ToString() + " = " + Constract.valueSplit + sendCompany.name + Constract.valueSplit;
                }
                else
                {
                    condition += " and " + BillTaxationMoneyRecordColumuns.send_company.ToString() + " = " + Constract.valueSplit + sendCompany.name + Constract.valueSplit;
                }
            }
            if (receiverCompany != null)
            {
                if (String.IsNullOrEmpty(condition))
                {
                    condition += BillTaxationMoneyRecordColumuns.receive_company.ToString() + " = " + Constract.valueSplit + receiverCompany.name + Constract.valueSplit;
                }
                else
                {
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_company.ToString() + " = " + Constract.valueSplit + receiverCompany.name + Constract.valueSplit;
                }
            }
            if (carInfo != null)
            {
                if (String.IsNullOrEmpty(condition))
                {
                    condition += BillTaxationMoneyRecordColumuns.car_number.ToString() + " = " + Constract.valueSplit + carInfo.carNumber + Constract.valueSplit;
                }
                else
                {
                    condition += " and " + BillTaxationMoneyRecordColumuns.car_number.ToString() + " = " + Constract.valueSplit + carInfo.carNumber + Constract.valueSplit;
                }
            }
            if (mMaterial != null)
            {
                if (String.IsNullOrEmpty(condition))
                {
                    condition += BillTaxationMoneyRecordColumuns.material_name.ToString() + " = " + Constract.valueSplit + mMaterial.name + Constract.valueSplit;
                }
                else
                {
                    condition += " and " + BillTaxationMoneyRecordColumuns.material_name.ToString() + " = " + Constract.valueSplit + mMaterial.name + Constract.valueSplit;
                }
            }
            switch (this.StatusCb.SelectedIndex)
            {
                case 0:
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_type.ToString() + " = " + 0;
                    break;
                case 1:
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_type.ToString() + " = " + 1;
                    break;
                case 2:
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_type.ToString() + " = " + 2;
                    break;
                case 3:
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_type.ToString() + " = " + 3;
                    break;
                case 4:
                    condition += " and " + BillTaxationMoneyRecordColumuns.receive_type.ToString() + " = " + 4;
                    break;
            }
            return condition;
        }
        private string GetStartDate()
        {
            if (StratDatePicker.SelectedDate == null)
            {
                return null;
            }
            DateTime tempdate = (DateTime)StratDatePicker.SelectedDate;
            if (tempdate == null)
            {
                return null;
            }
            string res = tempdate.ToString(Constract.defaultDateTimeFormat);
            return res;
        }
        private string GetEndDate()
        {
            if (EndDatePicker.SelectedDate == null)
            {
                return null;
            }
            DateTime tempdate = (DateTime)EndDatePicker.SelectedDate;
            if (tempdate == null)
            {
                return null;
            }
            string endtime = " 23:59:59";
            string res = tempdate.ToString(Constract.DateFormat) + endtime;
            return res;
        }
        #region Search btn click
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            this.SearchBtn.Cursor = Cursors.Wait;
            Object oldContent = this.SearchBtn.Content;
            this.SearchBtn.Content = new ThreePoingLoading();
            LoadData();

            this.SearchBtn.Cursor = Cursors.Hand;
            this.SearchBtn.Content = oldContent;
        }

        #endregion

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            BillTaxationMoneyRecord bill = (BillTaxationMoneyRecord)e.Row.DataContext;
            totalMoney += bill.money;
            totalWeight += bill.overtopWeight;
            if (e.Row.GetIndex().Equals(ReportDataGrid.Items.Count - 1))
            {
                Statistic();
            }
        }

        private void Statistic()
        {
            this.SummaryTextBlock.Text = $"合计：{ReportDataGrid.Items.Count} 次，超出 {totalWeight} 吨,补税：￥{totalMoney} 元";
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO
        }

        private Station mStation;
        private void StationCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mStation = (Station)StationCb.SelectedItem;
        }
    }
}
