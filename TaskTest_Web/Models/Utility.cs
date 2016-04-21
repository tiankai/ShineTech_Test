using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTest_Web.Models
{
    public enum ParamType : uint
    {
        TableStr = 0,

        SqlConStr = 1,

        OracleConStr = 2,

        MySqlConStr = 3,
        /// <summary>
        /// 程序运行方式
        /// </summary>
        ProgramMode = 11,

        CookieName = 12
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
                    paramStr = "K"; // GetXmlValue("/Config/TableStr");
                    break;
                case ParamType.SqlConStr:
                    paramStr = "Data Source=.;Initial Catalog=ShineTech_Test;User ID=sa;Password=sa"; // GetXmlValue("");
                    break;
                case ParamType.ProgramMode:
                    paramStr = "1"; // 0: ADO.NET-DAL Mode,  1: Entity Framework Mode
                    break;
                case ParamType.CookieName:
                    paramStr = "UName";
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