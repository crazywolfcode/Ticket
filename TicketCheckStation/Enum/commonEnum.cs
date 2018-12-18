using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public enum ConfigItemName
    {
        //connectionStrings
        appSettings,
        connectionStrings,
        mysqlConn,
        //appSettings
        programVersion,
        coryRight,
        dbType,
        CameraCaptureFilePath,
        BillNumberDate,
        BillNumberSort,
        defaultPrintFrequency,
        BillDescription,//磅单联的介绍
        PrintTitle,//磅单打印的标题
        autoPrintSecend,//自动开始打印的秒数
        autoPrint,//是否自动开始打印
    }


    public enum ReceiveMoneyType {
        No,Yes,NotNeed            
    }
}
