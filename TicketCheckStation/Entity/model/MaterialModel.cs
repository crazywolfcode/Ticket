using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public  class MaterialModel
    {
        public static List<Material> IndistinctSearchByNameOrNameFirstCase(String nameOrCase)
        {
            if (String.IsNullOrEmpty(nameOrCase))
            {
                return null;
            }
            else
            {
                string condition =MaterialColumns.name.ToString() + " like '%" + nameOrCase + "%' " + " OR " + MaterialColumns.name_first_case.ToString() + " like '%" + nameOrCase.ToUpper() + "%'";
                String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.material.ToString(), null, condition);
                List<Material> list = DatabaseOPtionHelper.GetInstance().select<Material>(sql);
                return list;
            }
        }   
    }
}
