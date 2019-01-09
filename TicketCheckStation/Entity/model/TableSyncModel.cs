using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public  class TableSyncModel
    {

        public static List<TableSync> GetList()
        {
            List<TableSync> list = new List<TableSync>();           
            String sql = DatabaseOPtionHelper.GetInstance().getSelectSqlNoSoftDeleteCondition(TableName.table_sync.ToString());
            list = DatabaseOPtionHelper.GetInstance().select<TableSync>(sql);
            return list;
        }

        public static int Update(TableSync table)
        {          
            return DatabaseOPtionHelper.GetInstance().update(table);
        }
    }
}
