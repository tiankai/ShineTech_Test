using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using System.Data.SqlClient;
namespace TaskLogic
{
    public class SimpleTaskLogic : ITaskLogic
    {
        private const string TableTaskName = "TaskInfo";

        private string _tableStr;

        private DBHelper.IDBHelper _dbHelper;

        private readonly TaskLoger.ITaskLoger _loger;

        private DBHelper.DB_Type _dbType;

        public SimpleTaskLogic(DBHelper.IDBHelper dbHelper, TaskLoger.ITaskLoger loger, string tableStr)
        {
            _tableStr = tableStr;
            _dbHelper = dbHelper;
            _loger = loger;
            _dbType = dbHelper.GetDataBaseType();
        }

        public SimpleTaskLogic(string tableStr, string conStr)
        {
            _tableStr = tableStr;
            _loger = new TaskLoger.TaskLoger();
            _dbType = DBHelper.DB_Type.SqlServer;
            _dbHelper = new DBHelper.SimpleDBHelper(_dbType, conStr, _loger);
        }
        /// <summary>
        /// 组合字段列表
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        private string GetFieldNames(params string[] fieldNames)
        {
            StringBuilder sb_Fields = new StringBuilder();
            foreach (var fieldName in fieldNames)
            {
                if (string.IsNullOrEmpty(fieldName) == false) // 字段不为空 有对应的字段
                {
                    sb_Fields.Append(fieldName);
                    sb_Fields.Append(",");
                }
            }
            // 
            string sqlFields = sb_Fields.ToString();

            return sqlFields.Substring(0, sqlFields.Length - 1);
        }

        private string GetTableName(string tableStr, string tableName)
        {
            return string.IsNullOrEmpty(tableStr) ? tableName : string.Concat(tableStr, "_", tableName);
        }

        private int GetMaxId(string sql)
        {
            int id = _dbHelper.ExecuteScalar<int>(sql);

            return id;
        }

        private SqlDbType GetSqlDbType(System.Data.DbType paraType)
        {
            SqlDbType returnDbType = SqlDbType.Char;
            switch(paraType)
            {
                case DbType.AnsiString:
                case DbType.String:
                    returnDbType = SqlDbType.VarChar;
                    break;
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    returnDbType = SqlDbType.Char;
                    break;
                case DbType.DateTime:
                case DbType.DateTime2:
                    returnDbType = SqlDbType.DateTime;
                    break;
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                    returnDbType = SqlDbType.Decimal;
                    break;
                case DbType.Int16:
                case DbType.Int32:
                case DbType.UInt16:
                    returnDbType = SqlDbType.Int;
                    break;
                case DbType.Int64:
                case DbType.UInt32:
                case DbType.UInt64:
                    returnDbType = SqlDbType.BigInt;
                    break;
                case DbType.Binary:
                    returnDbType = SqlDbType.Binary;
                    break;
            }

            return returnDbType;
        }

        private IDataParameter CreateIDataParameter(string paraName, System.Data.DbType paraType, int size, bool isInParameter = true)
        {
            switch (_dbType)
            {
                case DBHelper.DB_Type.SqlServer:
                    var sqlP = size == 0 ? new SqlParameter(paraName, GetSqlDbType(paraType)) : new SqlParameter(paraName, GetSqlDbType(paraType), size);
                    sqlP.Direction = isInParameter ? ParameterDirection.Input : ParameterDirection.Output;
                    return sqlP;
                case DBHelper.DB_Type.Oracle:
                    return null;
                default:
                    var defaultP = size == 0 ? new SqlParameter(paraName, GetSqlDbType(paraType)) : new SqlParameter(paraName, GetSqlDbType(paraType), size);
                    defaultP.Direction = isInParameter ? ParameterDirection.Input : ParameterDirection.Output;
                    return defaultP;
            }
        }

        private string GetParameterName(string nameStr, bool isIn = true)
        {
            string baseStr = string.Concat(nameStr, isIn ? "_In" : "_Out");
            // 
            switch(_dbType)
            {
                case DBHelper.DB_Type.SqlServer:
                case DBHelper.DB_Type.MySql:
                    return string.Concat("@P_", baseStr);
                case DBHelper.DB_Type.Oracle:
                    return string.Concat(":P_", baseStr);
                default:
                    return string.Concat("@P_", baseStr);
            }
        }

        public bool MissionNameIsExists(string missionName)
        {
            //
            string taskTitleParaName = GetParameterName("TaskTitle");
            var taskTitleParameter = CreateIDataParameter(taskTitleParaName, DbType.AnsiString, 40);
            taskTitleParameter.Value = missionName;
            string sql = string.Format("select count(*) from {0} where taskName = {1}", GetTableName(_tableStr, TableTaskName), taskTitleParaName);
            //
            int value = _dbHelper.ExecuteScalar<int>(sql, taskTitleParameter);

            return value == 0 ? false : true;
        }

        public bool CreateMission(NewMission task)
        {
            string tableName = GetTableName(_tableStr, TableTaskName);
            // 
            string sqlMaxId = string.Empty;
            switch(_dbType)
            {
                case DBHelper.DB_Type.SqlServer:
                    sqlMaxId = string.Format("select isnull(max(tid), 0) from {0}", tableName);
                    break;
                case DBHelper.DB_Type.Oracle:
                    sqlMaxId = string.Format("select nvl(max(tid), 0) from {0}", tableName);
                    break;
            }
            // 
            task.TaskId = GetMaxId(sqlMaxId) + 1;
            //
            string taskTitleParaName = GetParameterName("TaskTitle");
            string taskDueTimeParaName = GetParameterName("DueTime");
            string taskTitleDesParaName = GetParameterName("TaskDescription");
            string sqlInsert = string.Format("Insert into {0}({1}) values({2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})", tableName, GetFieldNames("Tid", "TaskName", "Priority", "ParentTask", "DueTime", "CreateTime", "Creater", "TaskDescription"), task.TaskId, taskTitleParaName, task.Priority, task.ParentTaskId, taskDueTimeParaName, "getdate()", task.Creater, taskTitleDesParaName);
            // 
            var taskTitleParameter = CreateIDataParameter(taskTitleParaName, DbType.AnsiString, 40);
            taskTitleParameter.Value = task.TaskTitle;
            // 
            var taskDueTimeParameter = CreateIDataParameter(taskDueTimeParaName, DbType.DateTime, 0);
            taskDueTimeParameter.Value = task.DueTime;
            // 
            var taskTitleDesParameter = CreateIDataParameter(taskTitleDesParaName, DbType.AnsiString, 512);
            taskTitleDesParameter.Value = task.TaskMemo;
            
            return _dbHelper.ExecuteSql(sqlInsert, taskTitleParameter, taskDueTimeParameter, taskTitleDesParameter) > 0;
        }

        public bool EditMissionProperty(EditMission task)
        {
            string taskTitleParaName = GetParameterName("TaskTitle");
            string taskMemoParaName = GetParameterName("TaskMemo");
            string taskDueTimeParaName = GetParameterName("DueTime");
            //
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Format("update {0} set Priority = {1}, DueTime = {2}, TaskName = {3}, TaskDescription = {5} where Tid = {4}", tableName, task.Priority, taskDueTimeParaName, taskTitleParaName, task.TaskId, taskMemoParaName);
            //
            var taskDueTimeParameter = CreateIDataParameter(taskDueTimeParaName, DbType.DateTime, 0);
            taskDueTimeParameter.Value = task.DueTime;
            //
            var taskTitleParameter = CreateIDataParameter(taskTitleParaName, DbType.AnsiString, 40);
            taskTitleParameter.Value = task.TaskTitle;
            //
            var taskMemoParameter = CreateIDataParameter(taskMemoParaName, DbType.AnsiString, 512);
            taskMemoParameter.Value = task.TaskMemo;                 

            return _dbHelper.ExecuteSql(sqlEdit, taskDueTimeParameter, taskTitleParameter, taskMemoParameter) > 0;
        }
        
        private string MissionStatusSqlText(int status, int taskId)
        {
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Empty;
            //
            switch(status)
            {
                case 2:
                    sqlEdit = string.Format("update {0} set AcceptTime = getdate(), TaskStatus = {1} where Tid = {2}", tableName, status, taskId);
                    break;
                case 3:
                    sqlEdit = string.Format("update {0} set TaskStatus = {1} where Tid = {2}", tableName, status, taskId);
                    break;
                case 4:
                    sqlEdit = string.Format("update {0} set DoneTime = getdate(), TaskStatus = {1} where Tid = {2}", tableName, status, taskId);
                    break;
            }           

            return sqlEdit;
        }

        public UnifiedTaskJson AssignMission(int taskId, int userId)
        {
            int value = 0; string message = string.Empty;
            string sqlIsWork = string.Format("select workStack from {0} where uid = {1}", GetTableName(_tableStr, "UserInfo"), userId);
            bool isWorking = _dbHelper.ExecuteScalar<bool>(sqlIsWork);
            if (isWorking == false) // 没有任务状态
            {
                const int status = 1;
                string tableName = GetTableName(_tableStr, TableTaskName);
                string sqlEdit = string.Format("update {0} set AssignTime = getdate(), TaskStatus = {1}, Executer = {2} where Tid = {3}", tableName, status, userId, taskId);
                // 
                var cmdEdit = new DBHelper.CmdSqlStruct() { CmdSql = sqlEdit, Parameters = null };
                var cmdUser = new DBHelper.CmdSqlStruct() { CmdSql = GetUserWorkSql(userId, true), Parameters = null };
                //
                value = _dbHelper.ExecuteCmdParamsSql(cmdEdit, cmdUser) ? 0: 1;
                if(value == 1)
                {
                    message = "Assign Task failed!";
                }
            }
            else  // 有任务状态
            {
                value = 2;
                message = "User already has work to do";
            }

            return new UnifiedTaskJson() { result = value, message = message };
        }

        public UnifiedTaskJson AcceptMission(int taskId)
        {
            int value = 0; string message = string.Empty;
            const int status = 2;

            value = _dbHelper.ExecuteSql(MissionStatusSqlText(status, taskId));
            if(value <= 0)
            {
                message = "accept task failed";
            }

            return new UnifiedTaskJson() { result = value, message = message };
        }

        public UnifiedTaskJson RejectMission(int taskId)
        {
            int value = 0; string message = string.Empty;
            const int status = 3;
            // 
            string sqlGetUserId = string.Format("select executer from {0} where tid = {1}", GetTableName(_tableStr, TableTaskName), taskId);
            int userId = _dbHelper.ExecuteScalar<int>(sqlGetUserId);

            var cmdReject = new DBHelper.CmdSqlStruct() { CmdSql = MissionStatusSqlText(status, taskId), Parameters = null };
            var cmdUser = new DBHelper.CmdSqlStruct() { CmdSql = GetUserWorkSql(userId, false), Parameters = null };

            bool flag = _dbHelper.ExecuteCmdParamsSql(cmdReject, cmdUser);
            if(flag == false)
            {
                value = 1; message = "Reject Task failed!";
            }

            return new UnifiedTaskJson() { result = value, message = message };
        }

        private string GetUserWorkSql(int userId, bool isWorking)
        {
            int workStack = isWorking ? 1 : 0;

            return string.Format("update {0} set workStack = {2} where uid = {1}", GetTableName(_tableStr, "UserInfo"), userId, workStack);
        }

        public UnifiedTaskJson MissionDone(int taskId)
        {
            int value = 0; string message = string.Empty;
            const int status = 4;
            // 
            string sqlGetUserId = string.Format("select executer from {0} where tid = {1}", GetTableName(_tableStr, TableTaskName), taskId);
            int userId = _dbHelper.ExecuteScalar<int>(sqlGetUserId);

            var cmdDone = new DBHelper.CmdSqlStruct() { CmdSql = MissionStatusSqlText(status, taskId), Parameters = null };
            var cmdUser = new DBHelper.CmdSqlStruct() { CmdSql = GetUserWorkSql(userId, false), Parameters = null };

            //return _dbHelper.ExecuteSql() > 0;
            return new UnifiedTaskJson() { result = value, message = message };
        }

        public EditMission GetEditMission(int taskId)
        {
            var model = new EditMission(taskId);
            // 
            string sql = string.Format("select taskName, priority, dueTime, taskDescription from {0} where tid = {1}", GetTableName(_tableStr, TableTaskName), taskId);
            //
            using(var dr = _dbHelper.ExecuteDataReader(sql))
            {
                while(dr.Read())
                {
                    model.TaskTitle = dr.GetString(0);
                    model.Priority = dr.GetInt32(1);
                    model.DueTime = dr.GetDateTime(2);
                    model.TaskMemo = dr[3].ToString();
                }
            }

            return model;
        }

        public IEnumerable<Mission> MyTask_ToDo(int userId, int pageIndex, int pageSize)
        {
            var resultQuery = new List<Mission>();
            // 
            string sql = string.Empty;
            switch(_dbType) // 
            {
                case DBHelper.DB_Type.SqlServer:
                    sql = string.Format(@"select tid, taskName, priority, dueTime, createTime, assignTime, 
                                            creater, taskStatus, taskDescription, rowNum, (rowNum / {3}) + 1 as pageNum
                                          from (
                                            select t.tid, t.taskName, t.priority, t.dueTime, t.createTime, t.assignTime,
                                                u.uName as creater, t.taskStatus, t.TaskDescription, 
                                                Row_Number() over (order by t.createTime desc, t.taskStatus asc) as rowNum
                                            from {0} t inner join {1} u on u.uid = t.Executer 
                                            where t.Executer = {2}
                                               ) x
                                          where (rowNum / {3}) + 1 = {4}", GetTableName(_tableStr, TableTaskName), GetTableName(_tableStr, "UserInfo"), userId, pageSize, pageIndex);
                    break;
                case DBHelper.DB_Type.Oracle:
                    break;
                case DBHelper.DB_Type.MySql:
                    break;
            }            
            // 
            using(var dr = _dbHelper.ExecuteDataReader(sql))
            {
                while(dr.Read())
                {
                    
                    var mission = new Mission()
                    {
                        TaskId = dr.GetInt32(0),
                        TaskTitle = dr.GetString(1),
                        Priority = dr.GetInt32(2),
                        DueTime = dr.GetDateTime(3),
                        CreateTime = dr.GetDateTime(4),
                        Creater = dr.GetString(6),
                        TaskStatus = Convert.ToInt32(dr[7]),
                        TaskMemo = dr[8].ToString()
                    };
                    if(dr.IsDBNull(5) == true)
                    {
                        mission.AssignTime = null;
                    }
                    else
                    {
                        mission.AssignTime = dr.GetDateTime(5);
                    }
                    mission.AcceptTime = null;
                    mission.DoneTime = null;
                    // 
                    resultQuery.Add(mission);
                }
            }

            return resultQuery;
        }

        public int MyTask_ToDo_Count(int userId)
        {
            string sql = string.Format("select count(*) from {0} where Executer = {1}", GetTableName(_tableStr, TableTaskName), userId);

            return _dbHelper.ExecuteScalar<int>(sql);
        }

        private bool IsSuperAdmininstrator(int userId)
        {
            return false;
        }

        private string UnionMissionSqlText(bool isAdmin, int userId, int pageIndex, int pageSize)
        {
            var sqlBuilder = new StringBuilder();
            switch (_dbType) // 
            {
                case DBHelper.DB_Type.SqlServer:
                    sqlBuilder.AppendFormat(@"select tid, taskName, priority, dueTime, createTime, assignTime, 
                                            executer, taskStatus, taskDescription, acceptTime, doneTime, creater, rowNum, (rowNum / {1}) + 1 as pageNum
                                          from (
                                            select t.tid, t.taskName, t.priority, t.dueTime, t.createTime, t.assignTime,
                                                t.Executer, t.taskStatus, t.TaskDescription, t.acceptTime, t.doneTime, t.creater,
                                                Row_Number() over (order by t.createTime desc, t.taskStatus asc) as rowNum
                                            from {0} t ", GetTableName(_tableStr, TableTaskName), pageSize);
                    if(isAdmin == true)
                    {
                        sqlBuilder.AppendFormat(@"
                                               ) x
                                          where (rowNum / {0}) + 1 = {1}", pageSize, pageIndex);
                    }
                    else
                    {
                        sqlBuilder.AppendFormat(@"
                                            where t.creater = {2} or t.executer = {2}
                                               ) x
                                          where (rowNum / {0}) + 1 = {1}", pageSize, pageIndex, userId);
                    }
                            
                    break;
                case DBHelper.DB_Type.Oracle:
                    break;
                case DBHelper.DB_Type.MySql:
                    break;
            }

            return sqlBuilder.ToString();
        }

        private string GetUserName(int userId)
        {
            string sql = string.Format("select uName from {0} where uid = {1}", GetTableName(_tableStr, "UserInfo"), userId);

            return _dbHelper.ExecuteScalar<string>(sql);
        }

        public IEnumerable<Mission> QueryUserMission(int userId, int pageIndex, int pageSize)
        {
            var resultQuery = new List<Mission>();
            bool isAdmin = IsSuperAdmininstrator(userId);
            // 
            string sql =  UnionMissionSqlText(isAdmin, userId, pageIndex, pageSize);
            // 
            using (var dr = _dbHelper.ExecuteDataReader(sql))
            {
                while (dr.Read())
                {

                    var mission = new Mission()
                    {
                        TaskId = dr.GetInt32(0),
                        TaskTitle = dr.GetString(1),
                        Priority = dr.GetInt32(2),
                        DueTime = dr.GetDateTime(3),
                        CreateTime = dr.GetDateTime(4),
                        TaskStatus = Convert.ToInt32(dr[7]),
                        TaskMemo = dr[8].ToString(),
                        Executer = dr[6].ToString(),
                        Creater = dr[11].ToString()
                    };
                    if (dr.IsDBNull(5) == true)
                    {
                        mission.AssignTime = null;
                    }
                    else
                    {
                        mission.AssignTime = dr.GetDateTime(5);
                    }
                    if(dr.IsDBNull(9) == true)
                    {
                        mission.AcceptTime = null;
                    }
                    else
                    {
                        mission.AcceptTime = dr.GetDateTime(9);
                    }
                    if(dr.IsDBNull(10) == true)
                    {
                        mission.DoneTime = null;
                    }
                    else
                    {
                        mission.DoneTime = dr.GetDateTime(10);
                    }
                    // 本人创建的任务
                    if(userId == Convert.ToInt32(mission.Creater))
                    {
                        if (mission.TaskStatus == (int)UserActionType.CreateNewMission)  // 新建的任务
                        {
                            mission.UserActions = new List<UserActionType>() {  UserActionType.CreateNewMission, UserActionType.AssignMission, UserActionType.EditMission };
                        }
                        else if (mission.TaskStatus == (int)UserActionType.AssignMission) // 已经分配
                        {
                            mission.UserActions = new List<UserActionType>() { UserActionType.AssignMission };
                        }
                        else if(mission.TaskStatus == (int)UserActionType.AcceptMission) // 已受理
                        {
                            mission.UserActions = new List<UserActionType>() { UserActionType.MissionDone };
                        }
                        else if(mission.TaskStatus == (int)UserActionType.DenyMission) // 拒绝任务
                        {
                            mission.UserActions = new List<UserActionType>() { UserActionType.AssignMission };
                        }
                        else
                        {
                            mission.UserActions = null;
                        }
                    }
                    // 本人是执行者
                    if(mission.Executer.Equals(userId.ToString()))
                    {
                        if (mission.TaskStatus == (int)UserActionType.AssignMission) // 已经分配
                        {
                            mission.UserActions = new List<UserActionType>() { UserActionType.AcceptMission, UserActionType.DenyMission };
                        }
                        else if (mission.TaskStatus == (int)UserActionType.AcceptMission) // 已受理
                        {
                            mission.UserActions = new List<UserActionType>() { UserActionType.MissionDone };
                        }
                        else
                        {
                            mission.UserActions = null;
                        }
                    }
                    // 
                    resultQuery.Add(mission);
                }
            }
            //
            foreach(var r in resultQuery)
            {
                int createUserId = Convert.ToInt32(r.Creater);
                r.Creater = GetUserName(createUserId);
                // 
                if(string.IsNullOrEmpty(r.Executer))
                {
                    continue;
                }
                int execUserId = Convert.ToInt32(r.Executer);
                r.Executer = GetUserName(execUserId);               
            }

            return resultQuery;
        }

        public int QueryUserMissionCount(int userId)
        {
            string sql = string.Format("select count(*) from {0}", GetTableName(_tableStr, TableTaskName));
            string sqlWhere = string.Format("where Creater = {0} or executer = {0}", userId);
            bool isAdmin = IsSuperAdmininstrator(userId);
            if(isAdmin == true)
            {
                return _dbHelper.ExecuteScalar<int>(sql);
            }
            else
            {
                return _dbHelper.ExecuteScalar<int>(string.Concat(sql, " ", sqlWhere));
            }            
        }
       
    }
}
