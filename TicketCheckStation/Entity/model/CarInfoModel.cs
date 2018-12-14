using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public class CarInfoModel
    {
        public static List<CarInfo> FuzzySearch(String palteNumberPart)
        {
            string condition = CarInfoColumns.car_number.ToString() + " like '%" + palteNumberPart + "%' ";
            String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.car_info.ToString(), null, condition);
            List<CarInfo> list = DatabaseOPtionHelper.GetInstance().select<CarInfo>(sql);
            return list;
        }
    }
}
