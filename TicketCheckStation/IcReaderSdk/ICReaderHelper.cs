using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IcReaderSdk
{
    public class ICReaderHelper
    {
        /// <summary>
        /// 将字符转化成十六进制的字符串
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="hasSpace">是否含有空格</param>
        /// <returns></returns>
        public static string STringToHex(string strValue,bool hasSpace=true)
        {         
            return BitConverter.ToString(ASCIIEncoding.GetEncoding("GB2312").GetBytes(strValue)).Replace("-", hasSpace== true?" ":"");
        }
        /// <summary>
        /// 返回十六进制代表的字符串 空格有和没都 可以
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns></returns>
        public static string HexToStr(string mHex)
        {
            if (string.IsNullOrEmpty(mHex)) {
                return "";
            }
            if (mHex.Contains(" "))
            {
                mHex = mHex.Replace(" ", "");
            }
            if (mHex.Length <= 0) return "";           
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                {
                    vBytes[i / 2] = 0;
                }
            return Encoding.GetEncoding("GB2312").GetString(vBytes);
        }
        public static string HexToStr(byte[] mHex)
        {
            return Encoding.GetEncoding("GB2312").GetString(mHex);
        }


        /// <summary>
        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ToSBC(String input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }



        /// <summary>
        ///转半角的函数(DBC case)       
        ///任意字符串
        /// 半角字符串        
        /// 全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>        
        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
    }
    }
