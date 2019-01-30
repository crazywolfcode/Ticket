using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    class DataAsyncHelper
    {
        private static String Url;
        private static String GetUrl()
        {
            if (String.IsNullOrEmpty(Url))
            {
                Url = MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.remoteUrl.ToString());
            }
            return Url;
        }
        private static String GetStationID()
        {
            if (App.mStation != null)
            {
                return App.mStation.id;
            }
            return "";
        }

        //开始同步数据 先下载，后上传 
        public static void Start()
        {
            //TestUpimageAsync();

            while (true)
            {
                System.Threading.Thread.Sleep(4000);
                List<TableSync> tablseList = TableSyncModel.GetList();
                for (int i = 0; i < tablseList.Count; i++)
                {
                    TableSync table = tablseList[i];
                    TableSync putTable = table;
                    if (table.noSync == 0)
                    {
                        try
                        {
                            UpdateData(table);

                            PutData(putTable);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("----" + e.Message);
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                System.Threading.Thread.Sleep(1500);
            }
        }

        private static void TestUpimageAsync()
        {
            Console.WriteLine($"上传图片==");
            String address = "capture/2019/1/24/DXYPZ201901240001_22.jpg";
            try
            {
                NetHelper.UpFile(GetUrl(), address);
            }
            catch (Exception e)
            {
                Console.WriteLine("------ " + e.Message);

            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="table"></param>
        public static void UpdateData(TableSync table)
        {
            switch (table.tableName)
            {
                case "bill_image":
                    UpdateBillImage(table);
                    break;
                case "bill_taxation_money_record":
                    Updatebill_taxation_money_record(table);
                    break;
                case "car_info":
                    Updatecar_info(table);
                    break;
                case "camera_info":
                    Updatecamera_info(table);
                    break;
                case "car_trae_recod":
                    Updatecar_trae_recod(table);
                    break;
                case "company":
                    UpdateCompany(table);
                    break;
                case "config":
                    Updateconfig(table);
                    break;
                case "material":
                    Updatematerial(table);
                    break;
                case "material_category":
                    Updatematerial_category(table);
                    break;
                case "material_taxation_recod":
                    Updatematerial_taxation_recod(table);
                    break;
                case "roles":
                    Updateroles(table);
                    break;
                case "station":
                    Updatestation(table);
                    break;
                case "user":
                    Updateuser(table);
                    break;
                case "weighing_bill":
                    Updateweighing_bill(table);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="table"></param>
        public static void PutData(TableSync table)
        {
            switch (table.tableName)
            {
                case "bill_image":
                    Put_bill_image(table);
                    break;
                case "bill_taxation_money_record":
                    Put_bill_taxation_money_record(table);
                    break;
                case "car_info":
                    Put_car_info(table);
                    break;
                case "camera_info":
                    Put_cameral_info(table);
                    break;
                case "car_trae_recod":
                    Put_car_trae_recod(table);
                    break;
                case "company":
                    Put_company(table);
                    break;
                case "config":
                    Put_config(table);
                    break;
                case "material":
                    Put_material(table);
                    break;
                case "material_category":
                    Put_material_category(table);
                    break;
                case "material_taxation_recod":
                    Put_material_taxation_recod(table);
                    break;
                case "roles":
                    Put_roles(table);
                    break;
                case "station":
                    Put_station(table);
                    break;
                case "user":
                    Put_user(table);
                    break;
                case "weighing_bill":
                    Put_weighing_bill(table);
                    break;
                default:
                    break;
            }
        }

        #region update 更新数据
        public static void UpdateBillImage(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<BillImage> datas = (List<BillImage>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<BillImage>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            UpdateBillImage(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatebill_taxation_money_record(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<BillTaxationMoneyRecord> datas = (List<BillTaxationMoneyRecord>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<BillTaxationMoneyRecord>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updatebill_taxation_money_record(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatecar_info(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<CarInfo> datas = (List<CarInfo>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<CarInfo>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updatecar_info(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatecamera_info(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<CameraInfo> datas = (List<CameraInfo>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<CameraInfo>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updatecamera_info(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatecar_trae_recod(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<CarTraeRecod> datas = (List<CarTraeRecod>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<CarTraeRecod>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updatecar_trae_recod(table);
                        }  
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void UpdateCompany(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<Company> datas = (List<Company>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Company>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }                            
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            UpdateCompany(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updateconfig(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<Config> datas = (List<Config>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Config>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updateconfig(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatematerial(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<Material> datas = (List<Material>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Material>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            Updatematerial(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatematerial_category(TableSync table) { }
        public static void Updatematerial_taxation_recod(TableSync table) { }
        public static void Updateroles(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<Roles> datas = (List<Roles>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Roles>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updateroles(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updatestation(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<Station> datas = (List<Station>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<Station>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updatestation(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updateuser(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<User> datas = (List<User>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<User>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updateuser(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }
        public static void Updateweighing_bill(TableSync table)
        {
            NetResult result = NetHelper.Get(GetUrl(), table.tableName, table.syncDownTime.ToString("yyyy-MM-dd HH:mm:ss"), GetStationID());
            if (result.ErrCode == 0)
            {
                if (!String.IsNullOrEmpty(result.Data.ToString()))
                {
                    try
                    {
                        List<WeighingBill> datas = (List<WeighingBill>)MyHelper.JsonHelper.JsonToObject(result.Data.ToString(), typeof(List<WeighingBill>));
                        foreach (var item in datas)
                        {
                            DatabaseOPtionHelper.GetInstance().insertOrUpdate(item);
                            if (item.lastUpdateTime > item.addTime)
                            {
                                table.syncDownTime = item.lastUpdateTime;
                            }
                            else
                            {
                                table.syncDownTime = item.addTime;
                            }
                        }
                        if (datas != null && datas.Count >= 10)
                        {
                            if (datas.Count > 0)
                            {
                                table.syncDownCount += datas.Count;
                                TableSyncModel.Update(table);
                            }
                            Updateweighing_bill(table);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"更新{table.tableName} 出错：" + e.Message);
                    }
                }
            }
        }

        #endregion 更新数据

        #region PutData 上传数据
        public static void Put_bill_image(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<BillImage> datas = DatabaseOPtionHelper.GetInstance().select<BillImage>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    item.address.Replace("\\", "\\\\");
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        table.syncUpCount += 1;
                        //上传图片文件
                        if (string.IsNullOrEmpty(item.remoteAddress))
                        {                                                     
                            NetResult netResult =  NetHelper.UpFile(GetUrl(), item.address);
                            Console.WriteLine("上传图片文件------ "+ netResult.Msg + "====path:" + netResult.Data);
                            
                            if (!String.IsNullOrEmpty(netResult.Data.ToString()))
                            {
                                item.remoteAddress = netResult.Data.ToString().Replace("\\", "\\\\");
                                item.lastUpdateTime = DateTime.Now;
                                DatabaseOPtionHelper.GetInstance().update(item);
                            }
                        }
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_bill_taxation_money_record(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<BillTaxationMoneyRecord> datas = DatabaseOPtionHelper.GetInstance().select<BillTaxationMoneyRecord>(sql);            
            if (datas == null)
            {
                return;
            }           
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }

        }
        public static void Put_car_info(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<CarInfo> datas = DatabaseOPtionHelper.GetInstance().select<CarInfo>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_cameral_info(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<CameraInfo> datas = DatabaseOPtionHelper.GetInstance().select<CameraInfo>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_car_trae_recod(TableSync table) { }
        public static void Put_company(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<Company> datas = DatabaseOPtionHelper.GetInstance().select<Company>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_config(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<Config> datas = DatabaseOPtionHelper.GetInstance().select<Config>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_material(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<Material> datas = DatabaseOPtionHelper.GetInstance().select<Material>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_material_category(TableSync table) { }
        public static void Put_material_taxation_recod(TableSync table) { }
        public static void Put_roles(TableSync table) { }
        public static void Put_station(TableSync table) { }
        public static void Put_user(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<User> datas = DatabaseOPtionHelper.GetInstance().select<User>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }
        public static void Put_weighing_bill(TableSync table)
        {
            Console.WriteLine($"上传=={table.tableName}==");
            String sql = BaseDataModel.GetSql(table);
            List<WeighingBill> datas = DatabaseOPtionHelper.GetInstance().select<WeighingBill>(sql);
            if (datas == null)
            {
                return;
            }
            int i = 0;
            try
            {
                for (; i < datas.Count; i++)
                {
                    var item = datas[i];
                    NetResult result = NetHelper.Post(GetUrl(), item, table.tableName);
                    if (result == null)
                    {
                        return;
                    }
                    if (result.ErrCode == 0)
                    {
                        Console.WriteLine($"上传=={table.tableName}={i}：" + result.Msg);
                        item.isUp = 1;
                        item.upDatetime = DateTime.Now;
                        DatabaseOPtionHelper.GetInstance().update(item);
                        table.syncUpCount += 1;
                        if (item.addTime > item.lastUpdateTime)
                        {
                            table.syncUpTime = item.addTime;
                        }
                        else
                        {
                            table.syncUpTime = item.lastUpdateTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"上传=={table.tableName}==出错：" + e.Message);
            }
            finally
            {
                //说明有成功的
                if (i > 0)
                {
                    TableSyncModel.Update(table);
                }
            }
        }

        #endregion 上传数据

  
    }
}
