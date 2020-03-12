using MyCustomControlLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TicketCheckStation
{
    /// <summary>
    /// CashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CashWindow : Window
    {
        private WeighingBill mWeighingBill;
        public Action<Boolean> RefreshParent { get; set; }
        public CashWindow(WeighingBill bill)
        {
            InitializeComponent();
            this.mWeighingBill = bill;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mWeighingBill == null) {
                this.Close();
            }
            this.DataContext = mWeighingBill;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
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

        private void CashBtn_Click(object sender, RoutedEventArgs e)
        {
            MMessageBox.Result result = MMessageBox.GetInstance().ShowBox($"该车辆需要补交税费 {mWeighingBill.overtopMoney} 元，是否确认收取？", "提示", MMessageBox.ButtonType.YesNo, MMessageBox.IconType.Info, Orientation.Vertical, "是");
            if (result == MMessageBox.Result.Yes)
            {
                mWeighingBill.isReceiveMoney = (int)ReceiveMoneyType.Yes;
                mWeighingBill.lastUpdateTime = DateTime.Now;
                mWeighingBill.lastUpdateUserId = App.currentUser.id;
                mWeighingBill.lastUpdateUserName = App.currentUser.name;
                String updateSql = DatabaseOPtionHelper.GetInstance().getUpdateSql(mWeighingBill);

                BillTaxationMoneyRecord record = new BillTaxationMoneyRecord()
                {
                    id = Guid.NewGuid().ToString(),
                    stationId = App.mStation.id,
                    stationName = App.mStation.name,
                    addTime = DateTime.Now,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name,
                    number = CommonFunction.GetCashNumber(mWeighingBill.number),
                    money = mWeighingBill.overtopMoney,
                    overtopWeight = mWeighingBill.overtopWeight,
                    sendCompany = mWeighingBill.sendCompany,
                    receiveCompany =mWeighingBill.receiveCompany,
                    materialId = mWeighingBill.materialId,
                    materialName = mWeighingBill.materialName,
                    materialTaxation = mWeighingBill.materialTaxation,
                    carNumber =mWeighingBill.carNumber,
                    driver = mWeighingBill.driver,       
                    status = 1,
                };
                record.receiveType = this.CashTypeCB.SelectedIndex;
                record.remark = this.CashRemark.Text.Trim();
                String InsertSql = DatabaseOPtionHelper.GetInstance().getInsertSql(record);
                int res = DatabaseOPtionHelper.GetInstance().TransactionExecute(new string[] { updateSql,InsertSql});
                if (res > 0)
                {
                    MMessageBox.GetInstance().ShowSuccessAlert("收取成功！");
                    RefreshParent(true);
                    this.Close();
                }
                else
                {
                    MMessageBox.GetInstance().ShowErrorAlert("收取失败！");
                }
            }
        }
        
    }
}
