using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyCustomControlLibrary;

namespace TicketCheckStation
{
    /// <summary>
    /// CoMmon Function Library
    /// </summary>
    public class CommonFunction
    {

        public static BitmapImage getImageSource(String uri)
        {

            BitmapImage bmp = new BitmapImage();
            try
            {
                bmp.BeginInit();//初始化
                bmp.UriSource = new Uri(uri);//设置图片路径
                bmp.EndInit();//结束初始化
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }
        /// <summary>
        /// Update Used Base Data run in the app exiting
        /// </summary>
        public static void UpdateUsedBaseData()
        {
            if (App.tempSupplyCompanys != null)
            {
                String xml = MyHelper.XmlHelper.Serialize(typeof(List<Company>), App.tempSupplyCompanys.Values.ToList());
                MyHelper.FileHelper.Write(System.IO.Path.Combine(Constract.tempPath, Constract.tempSupplyFileName), xml);
            }
            if (App.tempCustomerCompanys != null)
            {
                String xml = MyHelper.XmlHelper.Serialize(typeof(List<Company>), App.tempCustomerCompanys.Values.ToList());
                MyHelper.FileHelper.Write(System.IO.Path.Combine(Constract.tempPath, Constract.tempCustomerFileName), xml);
            }

            if (App.tempMaterials != null)
            {
                String xml = MyHelper.XmlHelper.Serialize(typeof(List<Material>), App.tempMaterials.Values.ToList());
                MyHelper.FileHelper.Write(System.IO.Path.Combine(Constract.tempPath, Constract.tempMatreialFileName), xml);
            }

            if (App.tempCars != null)
            {
                String xml = MyHelper.XmlHelper.Serialize(typeof(List<CarInfo>), App.tempCars.Values.ToList());
                MyHelper.FileHelper.Write(System.IO.Path.Combine(Constract.tempPath, Constract.tempCarFileName), xml);
            }
        }

        internal static void UpdateInputReamak(object remark)
        {
            if (remark != null && !String.IsNullOrEmpty(remark.ToString()))
            {
                if (!App.inputRemarkList.Contains(remark.ToString()))
                {
                    App.inputRemarkList.Add(remark.ToString());
                }
            }
        }

        /// <summary>
        /// Update Used Base Data run in the save success
        /// </summary>
        public static void TempUpdateUsedBase(object baseData)
        {
            BaseDataClassV baseDataClassV = (BaseDataClassV)baseData;
            Company supply = baseDataClassV.send;
            Company Customer = baseDataClassV.receive;
            Material material = baseDataClassV.material;
            CarInfo carInfo = baseDataClassV.carInfo;
            if (supply != null)
            {
                supply.lastUpdateTime = DateTime.Now;
                if (App.tempSupplyCompanys.ContainsKey(supply.id))
                {
                    App.tempSupplyCompanys.Remove(supply.id);
                }
                App.tempSupplyCompanys.Add(supply.id, supply);
                App.tempSupplyCompanys.OrderBy(O => O.Value.lastUpdateTime);
            }
            if (Customer != null)
            {
                Customer.lastUpdateTime = DateTime.Now;
                if (App.tempCustomerCompanys.ContainsKey(Customer.id))
                {
                    App.tempCustomerCompanys.Remove(Customer.id);
                }
                App.tempCustomerCompanys.Add(Customer.id, Customer);
                App.tempCustomerCompanys.OrderBy(O => O.Value.lastUpdateTime);
            }
            if (material != null)
            {
                material.lastUpdateTime = DateTime.Now;
                if (App.tempMaterials.ContainsKey(material.id))
                {
                    App.tempMaterials.Remove(material.id);
                }
                App.tempMaterials.Add(material.id, material);
                App.tempMaterials.OrderBy(O => O.Value.lastUpdateTime);
            }
            if (carInfo != null)
            {
                carInfo.lastUpdateTime = DateTime.Now;
                if (App.tempCars.ContainsKey(carInfo.id))
                {
                    App.tempCars.Remove(carInfo.id);
                }
                App.tempCars.Add(carInfo.id, carInfo);
                App.tempCars.OrderBy(O => O.Value.lastUpdateTime);
            }
        }

        /// <summary>
        /// Get Weighing Number 获取磅单
        /// </summary>
        /// <param name="WeightingBillType"> CK OR RK</param>
        /// <param name="ExtStr"> BL OR WL</param>
        /// <param name="noaml">true 正常过磅 false WeightingBillType + CurrentDateTime</param>
        /// <returns></returns>
        public static String GetWeighingNumber(bool noaml = true, String ExtStr = null)
        {
            String date = MyHelper.DateTimeHelper.getCurrentDateTime(MyHelper.DateTimeHelper.DateFormat);
            String oldDate = MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.BillNumberDate.ToString());
            if (!date.Equals(oldDate))
            {
                MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.BillNumberDate.ToString(), date);
                MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.BillNumberSort.ToString(), "1");
            }
            string oldSort = MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.BillNumberSort.ToString());
            switch (oldSort.Length)
            {
                case 1:
                    oldSort = "000" + oldSort;
                    break;
                case 2:
                    oldSort = "00" + oldSort;
                    break;
                case 3:
                    oldSort = "0" + oldSort;
                    break;
            }
            return App.mStation.nameFirstCase.ToUpper() + date.Replace("-", "") + oldSort;
        }


        /// <summary>
        /// Get Cash Number 
        /// </summary>
        /// <param name="BillNmber"> 磅单编号</param>     
        /// <returns></returns>
        public static String GetCashNumber(String BillNmber)
        {
            String date = MyHelper.DateTimeHelper.getCurrentDateTime(MyHelper.DateTimeHelper.DateFormat);
            return "BJXK" + date.Replace("-", "") + BillNmber.Substring(BillNmber.Length - 4, 4);
        }

        public static void AddBillNumberSort()
        {
            int Sort = Convert.ToInt32(MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.BillNumberSort.ToString()));
            MyHelper.ConfigurationHelper.SetConfig(ConfigItemName.BillNumberSort.ToString(), (Sort + 1).ToString());
        }

        #region 显示提示窗口

        /// <summary>
        /// 显示提示窗口
        /// </summary>
        /// <param name="content">提示内容</param>
        /// <param name="Title">标题</param>
        ///   /// <param name="orientation">方向</param>
        public static void ShowAlert(String content, String Title = "提示", Orientation orientation = Orientation.Horizontal)
        {
            MMessageBox.GetInstance().ShowBox(content, Title, MMessageBox.ButtonType.Yes, MMessageBox.IconType.Info, orientation, "好");
        }

        public static void ShowSuccessAlert(String content, String Title = "提示", Orientation orientation = Orientation.Horizontal)
        {
            MMessageBox.GetInstance().ShowBox(content, Title, MMessageBox.ButtonType.Yes, MMessageBox.IconType.success, orientation, "好");
        }

        public static void ShowErrorAlert(String content, String Title = "错误", Orientation orientation = Orientation.Horizontal)
        {
            MMessageBox.GetInstance().ShowBox(content, Title, MMessageBox.ButtonType.No, MMessageBox.IconType.success, orientation);
        }
        public static MMessageBox.Result ShowAlertResult()
        {
            return MMessageBox.GetInstance().ShowBox("保存成功 ! 要继续过磅吗？", "恭喜", MMessageBox.ButtonType.YesNo, MMessageBox.IconType.success, Orientation.Vertical, "是");
        }

        #endregion

        //角色级别 0 验票员 1 审核员 2 监管员 3系统作者
        public static string GetRoleName(RoleLevelType type)
        {
            int temp = (int)type;
            String res = "";
            switch (temp)
            {
                case 0:
                    res = "验票员";
                    break;
                case 1:
                    res = "审核员";
                    break;
                case 2:
                    res = "监管员";
                    break;
                case 3:
                    res = "系统作者";
                    break;
            }
            return res;
        }
        public static readonly string connectionStringTemplate = "Database={0};Data Source={1};User Id={2};Password={3};pooling=false;CharSet=utf8;port={4};";


        public static string BuildMyqlconn(String db, string ip, String user, String pwd, String port)
        {
            return string.Format(connectionStringTemplate, db, ip, user, pwd, port);
        }
    }
}
