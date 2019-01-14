using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public class BillFactory
    {

        public static SendBill CreateSendbill(String[] strValues, string snr)
        {
            SendBill bill = new SendBill(strValues, snr);
            return bill;
        }

        public static WeighingBill CreateWeightBill(SendBill sendBill)
        {
            WeighingBill weighingBill = new WeighingBill()
            {
                id = Guid.NewGuid().ToString(),
                stationId = App.mStation.id,
                stationName = App.mStation.name,
                sendGrossWeight = sendBill.grossWeight,
                sendTraeWeight = sendBill.traeWeight,
                sendNetWeight = sendBill.netWeight,
                operatorId = App.currentUser.id,
                operatorName = App.currentUser.name,
                addTime = DateTime.Now,
                addUserId = App.currentUser.id,
                addUserName = App.currentUser.name,
                number = CommonFunction.GetWeighingNumber(),
                status = 1,
            };
            Double taxationPrice = 0.0;
            Double limitTone = 0;
            Double overStopWeight = 0.0;
            Double totalMoney = 0.0;

            //car 是否存在
            //if (!MyHelper.RegexHelper.IsVehicleNumber(sendBill.CarNumber))
            //{
            //    weighingBill.status = 0;
            //    weighingBill.remark = "不是有效的车牌号";
            //    return weighingBill;
            //}

            bool CarExistence = CarInfoModel.Existence(sendBill.CarNumber);
            if (CarExistence == false)
            {
                CarInfo carInfo = new CarInfo()
                {
                    id = Guid.NewGuid().ToString(),
                    addTime = DateTime.Now,
                    driver = sendBill.driver,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name,
                    carNumber = sendBill.CarNumber,
                    traeWeight = sendBill.carTraeWeight,
                    icNumber = sendBill.ICNumber,
                    affiliatedProvince = sendBill.carArea,
                    remark = "读卡验票时系统自动添加"
                };
                if (carInfo.carNumber != null)
                    CarInfoModel.Create(carInfo);
            }
            weighingBill.carNumber = sendBill.CarNumber;
            weighingBill.driver = sendBill.driver;
            weighingBill.carTraeWeight = sendBill.carTraeWeight;

            //煤种信息
            Material material = MaterialModel.GetByName(sendBill.materialName);
            if (material == null)
            {
                material = new Material()
                {
                    id = Guid.NewGuid().ToString(),
                    addTime = DateTime.Now,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name,
                    name = sendBill.materialName,
                    nameFirstCase = StringHelper.GetFirstPinyin(sendBill.materialName),
                    currTaxation = 0,
                    limitTone = 0,
                };
                if (material.name != null)
                    MaterialModel.Create(material);
            }
            weighingBill.materialId = material.id;
            weighingBill.materialName = material.name;
            weighingBill.materialFirstCase = material.nameFirstCase;
            taxationPrice = material.currTaxation;
            weighingBill.materialTaxation = taxationPrice;

            Company sendCompany = CompanyModel.GetByName(sendBill.sendCompany);
            if (sendCompany == null)
            {
                sendCompany = new Company()
                {
                    id = Guid.NewGuid().ToString(),
                    addTime = DateTime.Now,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name,
                    name = sendBill.sendCompany,
                    nameFirstCase = StringHelper.GetFirstPinyin(sendBill.sendCompany).ToUpper(),
                };
                if (sendCompany.name != null)
                {
                    CompanyModel.Create(sendCompany);
                }
            }
            weighingBill.sendCompany = sendCompany.name;
            weighingBill.sendCompanyCase = sendCompany.nameFirstCase;

            Company receiveCompany = CompanyModel.GetByName(sendBill.receiveCompany);
            if (receiveCompany == null)
            {
                receiveCompany = new Company()
                {
                    id = Guid.NewGuid().ToString(),
                    addTime = DateTime.Now,
                    addUserId = App.currentUser.id,
                    addUserName = App.currentUser.name,
                    name = sendBill.receiveCompany,
                    nameFirstCase = StringHelper.GetFirstPinyin(sendBill.receiveCompany).ToUpper(),
                };
                if (receiveCompany.name != null)
                {
                    CompanyModel.Create(receiveCompany);
                }

            }
            weighingBill.receiveCompany = receiveCompany.name;
            weighingBill.receiveCompanyCase = receiveCompany.nameFirstCase;



            if (ConfigurationHelper.GetConfig(ConfigItemName.IsUnifeidLimitTone.ToString()).Equals("1"))
            {
                limitTone = Convert.ToDouble(ConfigurationHelper.GetConfig(ConfigItemName.limitTone.ToString()));
            }
            else
            {
                limitTone = material.limitTone;
            }
            weighingBill.limitWeight = limitTone;
            Double nowGrossWeight = 0.0;
            try
            {
                nowGrossWeight = double.Parse(Properties.Settings.Default.WeihgingValue);
                weighingBill.grossWeight = nowGrossWeight;
                weighingBill.netWeight = weighingBill.grossWeight - weighingBill.carTraeWeight;
                overStopWeight = Math.Round(weighingBill.netWeight - (weighingBill.sendNetWeight + limitTone), 2);
                if (overStopWeight > 0)
                {
                    totalMoney = Math.Round(weighingBill.netWeight * taxationPrice);
                    weighingBill.overtopWeight = overStopWeight;
                    weighingBill.overtopMoney = Math.Round((weighingBill.netWeight - weighingBill.sendNetWeight) * taxationPrice, 2);
                }
                else
                {
                    totalMoney = Math.Round(sendBill.netWeight * taxationPrice);
                }
            }
            catch
            {
                return null;
            }
            weighingBill.totalTaxstionMoney = totalMoney;
            if (weighingBill.overtopMoney > 0)
            {
                weighingBill.isReceiveMoney = (int)ReceiveMoneyType.No;
            }
            else
            {
                weighingBill.isReceiveMoney = (int)ReceiveMoneyType.NotNeed;
            }

            return weighingBill;
        }
    }
}
