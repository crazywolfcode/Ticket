using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketCheckStation
{
   public class IcError
    {
        public static string GetErrorMsg(int ErrorCode) {
            string res = "未知道错误";
            switch (ErrorCode) {
                case 0:
                    res = "正确";
                    break;
                case 1:
                    res = "无卡";               
                    break;
                case 2:
                    res = "CRC校验错";                 
                    break;
                case 3:
                    res = "值溢出";
                    break;
                case 4:
                    res = "未验证密码";
                    break;
                case 5:
                    res = "奇偶校验错";
                    break;
                case 6:
                    res = "通讯出错";                    
                    break;
                case 8:
                    res = "错误的序列号";
                    break;
                case 10:
                    res = "验证密码失败";
                    break;
                case 11:
                    res = "接收的数据位错误";
                    break;
                case 12:
                    res = "接收的数据字节错误";
                    break;
                case 14:
                    res = "Transfer错误";
                    break;
                case 15:
                    res = "写失败";
                    break;
                case 16:
                    res = "加值失败";
                    break;
                case 17:
                    res = "减值失败";
                    break;
                case 18:
                    res = "读失败";
                    break;
                case -0x10:
                    res = "PC与读写器通讯错误";
                    break;
                case -0x11:
                    res = "通讯超时";
                    break;
                case -0x20:
                    res = "打开通信口失败";
                    break;
                case -0x24:
                    res = "串口已被占用";
                    break;
                case -0x30:
                    res = "地址格式错误";
                    break;
                case -0x31:
                    res = "该块数据不是值格式";
                    break;
                case -0x32:
                    res = "长度错误";
                    break;
                case -0x40:
                    res = "值操作失败";
                    break;
                case -0x50:
                    res = "卡中的值不够减";
                    break;
            }
            return res;
        }
    }
}
