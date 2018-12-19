using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public  class StationModel
    {
        public static Station SelectById(String id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return null;
            }
            else
            {
                string condition = StationColumns.id.ToString() + " = '" + id + "' and status = 1";
                String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.station.ToString(), null, condition,null,null,null,1);
                List<Station> list = DatabaseOPtionHelper.GetInstance().select<Station>(sql);
                if (list.Count > 0) {
                    return list[0];
                }
                return null;
            }
        }
    }
}
