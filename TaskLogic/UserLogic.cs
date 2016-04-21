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
                }
            }

            return new UserInfo() { UserId = userId, UserName = userName };            
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

        public bool RemoveUser(string userName)
        {
            throw new NotImplementedException();
        }
    }

    public class EF_UserLogic : IUser, IDisposable
    {
        private readonly TaskContainer _taskContext;

        public EF_UserLogic(string sqlConStr, string tableStr)
        {
            var conSql = new SqlConnection(sqlConStr);
            conSql.Open();
            //
            _taskContext = new TaskContainer(conSql, tableStr);
        }

        public UserInfo UserLogin(string userName, string userPass)
        {
            var model = new UserInfo() { UserId = -1, UserName = userName, UserPass = string.Empty };

            var user = _taskContext.AllUsers.Where(u => u.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                model.UserId = user.UserId;
                //
                user.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _taskContext.SaveChanges();
            }

            return model;
        }

        public void UserLogout(string userName)
        {
            throw new NotImplementedException();
        }

        public bool RegisterUser(UserInfo user)
        {
            bool flag = false;
            int UID = _taskContext.AllUsers.Max(s => s.UserId) + 1;
            // user.Memo 数据库中有长度限制 若是超长了 报错 "string or binary data would be truncated"
            var u = new UserInfo() { UserId = UID, UserPass = user.UserPass, UserName = user.UserName, IsWorking = false, LastLoginIp = "125.0.2.3", LastLoginTime = DateTime.Now.ToShortDateString(), Status = 0, Memo = user.Memo };
            _taskContext.AllUsers.Add(u);
            _taskContext.SaveChanges();
            flag = true;

            return flag;
        }

        public bool ModifyUserPass(string userName, string oldPass, string newPass)
        {
            bool flag = false;
            var user = _taskContext.AllUsers.Where(s => s.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                user.UserPass = newPass;
                user.LastLoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            _taskContext.SaveChanges();
            flag = true;

            return flag;
        }

        public bool RemoveUser(string userName)
        {
            bool flag = false;
            var user = _taskContext.AllUsers.Where(s => s.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                _taskContext.AllUsers.Remove(user);
            }
            _taskContext.SaveChanges();
            flag = true;

            return flag;
        }

        public void Dispose()
        {
            if(_taskContext != null)
            {
                _taskContext.Dispose();
            }
        }
    }
}
