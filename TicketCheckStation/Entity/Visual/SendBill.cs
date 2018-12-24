using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
  public  class SendBill
    {
        public SendBill() { }
        public SendBill(String[] values,String snr) {
            try {
                driver = values[0];
                String[] cartemp = values[1].Split(' ');
                CarNumber = IcReaderSdk.ICReaderHelper.ToDBC(cartemp[0].Trim());
                carTraeWeight = double.Parse(cartemp[1].Trim());
                sendCompany = values[2].Trim();
                sendCompanyId = int.Parse(values[3].Trim());
                receiveCompany = values[4].Trim();
                receiveCompanyId = int.Parse(values[5]);
                carArea = values[6].Trim();
                String[] m = values[7].Split(' ');
                materialName = m[0];
                materialId = int.Parse(m[1]);
                String[] w = values[8].Split(' ');
                netWeight = double.Parse(w[0]);
                traeWeight = Double.Parse(w[2]);
                grossWeight = netWeight + traeWeight;
                addTime = DateTime.Parse(values[9]);
                money = values[13].Trim();
                status = int.Parse(values[14]);
                numeber = values[15].Trim();
                ICNumber = snr;
            } catch { }
            
        }
        public String CarNumber { get; set; }
        public String ICNumber { get; set; }
        public String driver { get; set; }
        public double carTraeWeight { get; set; }
        public String sendCompany { get; set; }
        public int sendCompanyId { get; set; }
        public String receiveCompany { get; set; }
        public int receiveCompanyId { get; set; }
        public string carArea { get; set; }
        public string materialName { get; set; }
        public int materialId { get; set; }
        public DateTime addTime { get; set; }
        public double netWeight { get; set; }
        public double traeWeight { get; set; }
        public double grossWeight { get; set; }

       // 以下为未知道的字段
        public string money { get; set; }
        public int status { get; set; }
        public string numeber { get; set; }
    }
}
