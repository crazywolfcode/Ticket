using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public   class BaseDataModel
    {
        private static String DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static String GetSql(TableSync table)
        {
            string condition = PublicColumns.add_time.ToString() + "> '" + table.syncTime.ToString(DateTimeFormat) + "' OR " + PublicColumns.last_update_time.ToString() + " > '" + table.syncTime.ToString(DateTimeFormat) + "'";
            string order = PublicColumns.add_time.ToString() + " asc ," + PublicColumns.last_update_time.ToString() + " asc";
            int limit = table.limitCount;
            string sql = DatabaseOPtionHelper.GetInstance().getSelectSqlNoSoftDeleteCondition(table.tableName, null, condition, null, null, order, limit);
            return sql;
        }

    }
}
