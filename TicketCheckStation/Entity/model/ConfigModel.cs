using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public class ConfigModel
    {
        public static List<Config> GetCurrStationConfigs()
        {
            string id = MyHelper.ConfigurationHelper.GetConfig(ConfigItemName.CurrStationId.ToString());
            string condition = ConfigColumns.station_id.ToString()+" = '"+id+"'" ;         
            String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.config.ToString(), null, condition);
            List<Config> list = DatabaseOPtionHelper.GetInstance().select<Config>(sql);
            return list;
        }
        
    }
}
