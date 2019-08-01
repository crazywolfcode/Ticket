using MyHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDao;
namespace TicketCheckStation
{
    public class DatabaseOPtionHelper
    {

        private static DbHelper Instance;


        public static DbHelper GetInstance()
        {
            string conn = ConfigurationHelper.GetConnectionConfig(ConfigItemName.mysqlConn.ToString());
            Instance = new MySqlHelper(conn);
            App.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                if (Instance == null)
                {
                    Instance = new MySqlHelper(conn);
                }
            }));
            return Instance;
        }

    }
}
