using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTest_Web.Models
{
    public enum ParamType : uint
    {
        TableStr = 0,

        SqlConStr = 1

    }

    /// <summary>
    /// 辅助工具
    /// </summary>
    public class Utility
    {
        public static string GetParamStr(ParamType type)
        {
            string paramStr = string.Empty;
            // 
            switch(type)
            {
                case ParamType.TableStr:
                    paramStr = GetXmlValue("/Config/TableStr");
                    break;
                case ParamType.SqlConStr:
                    paramStr = GetXmlValue("");
                    break;
            }

            return paramStr;
        }

        private static string GetXmlValue(string keyPath)
        {
            return "";
        }
    }
}