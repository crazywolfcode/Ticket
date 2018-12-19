using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
    public enum ConfigItemName
    {
        CurrStationId,
        ScaleBrandType,
        ScaleSeries,
        Com,
        BaudRate,
        DataBits,
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
        IsUnifeidLimitTone,//统限定的吨位
        limitTone,
    }


    public enum ReceiveMoneyType {
        No,Yes,NotNeed            
    }

    //角色级别 0 验票员 1 审核员 2 监管员 3系统作者
    public enum RoleLevelType
    {
        YPY, SHY, JGY,XTZZ
    }
}
