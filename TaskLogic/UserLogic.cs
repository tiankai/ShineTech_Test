using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using System.Data.SqlClient;

namespace TaskLogic
{
    public class SimpleUserLogic : IUser
    {
        private const string TableUserName = "UserInfo";

        private string _tableStr;

        private DBHelper.IDBHelper _dbHelper;

        private readonly TaskLoger.ITaskLoger _loger;

        public SimpleUserLogic(DBHelper.IDBHelper dbHelper, TaskLoger.ITaskLoger loger, string tableStr)
        {
            _tableStr = tableStr;
            _dbHelper = dbHelper;
            _loger = loger;
        }

        private string GetTableName(string tableStr, string tableName)
        {
            return string.IsNullOrEmpty(tableStr) ? tableName : string.Concat(tableStr, "_", tableName);
        }

        public UserInfo UserLogin(string userName, string userPass)
        {
            string dbPass = string.Empty;
            int userId = -1; uint actor = 0;
            const string userNameParaName = "@P_UserName_In";
            string sql = string.Format("select Uid, UPass, UserRole from {0} where UName = {1}", GetTableName(_tableStr, TableUserName), userNameParaName);
            // 
            var userNameParameter = new SqlParameter(userNameParaName, SqlDbType.VarChar, 10);
            userNameParameter.Value = userName;
            userNameParameter.Direction = ParameterDirection.Input;
            // 
            using (var dr = _dbHelper.ExecuteDataReader(sql, userNameParameter))
            {
                while (dr.Read())
                {
                    userId = dr.GetInt32(0);
                    dbPass = dr.GetString(1);
                    actor =  Convert.ToUInt32(dr[2]);
                }
            }

            return new UserInfo() { UserId = userId, UserName = userName, Actor = (UserRole)actor };            
        }

        public void UserLogout(string userName)
        {
            throw new NotImplementedException();
        }

        public bool RegisterUser(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public bool ModifyUserPass(string userName, string oldPass, string newPass)
        {
            throw new NotImplementedException();
        }
    }
}
