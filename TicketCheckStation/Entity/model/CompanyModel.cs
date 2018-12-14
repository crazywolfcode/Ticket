using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public  class CompanyModel
    {
        public static List<Company> IndistinctSearchByNameOrNameFirstCase(String nameOrCase)
        {
            if (String.IsNullOrEmpty(nameOrCase))
            {
                return null;
            }
            else
            {
                string condition =CompanyColumns.name.ToString() + " like '%" + nameOrCase + "%' " + " OR " + CompanyColumns.name_first_case.ToString() + " like '%" + nameOrCase.ToUpper() + "%'";
                String sql = DatabaseOPtionHelper.GetInstance().getSelectSql(TableName.company.ToString(), null, condition);
                List<Company> list = DatabaseOPtionHelper.GetInstance().select<Company>(sql);
                return list;
            }
        }
    }
}
