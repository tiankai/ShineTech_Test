using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using System.Data.SqlClient;
//
using System.Data.Entity;

namespace TaskLogic
{

    public class SimpleTaskLogic : ITaskLogic
    {
        private const string TableTaskName = "TaskInfo";

        private string _tableStr;

        private DBHelper.IDBHelper _dbHelper;

        private readonly TaskLoger.ITaskLoger _loger;

        private DBHelper.DB_Type _dbType;

        private string[] _errMessage;

        public SimpleTaskLogic(DBHelper.IDBHelper dbHelper, TaskLoger.ITaskLoger loger, string tableStr)
        {
            _tableStr = tableStr;
            _dbHelper = dbHelper;
            _loger = loger;
            _dbType = dbHelper.GetDataBaseType();
            //
            _errMessage = new string[9] { "", // 0
                "Task not exists!",  // 1
                "Task already accepted by someone", // 2
                "you have rejected task", // 3
                "Task has been done", // 4
                "task has not been accepted yet", // 5
                "task has not been assigned yet", // 6
                "task has already existed",       // 7
                "database error",   // 8
            };
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

        public UnifiedTaskJson CreateMission(NewMission task)
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
            //
            bool isExisted = MissionNameIsExists(task.TaskTitle);
            if(isExisted == true)
            {
                return new UnifiedTaskJson() { result = 7, message = _errMessage[7] };
            }
            else
            {
                bool flag = _dbHelper.ExecuteSql(sqlInsert, taskTitleParameter, taskDueTimeParameter, taskTitleDesParameter) > 0;

                return new UnifiedTaskJson() { result = flag == true ? 0 : 8, message = _errMessage[flag == true ? 0 : 8] };
            }
        }

        public UnifiedTaskJson EditMissionProperty(EditMission task)
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
            // 
            bool flag =_dbHelper.ExecuteSql(sqlEdit, taskDueTimeParameter, taskTitleParameter, taskMemoParameter) > 0;

            return new UnifiedTaskJson() { result = flag ? 0 : 8, message = _errMessage[flag ? 0 : 8] };
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
                        CreaterId = dr.IsDBNull(6) ? 0 : dr.GetInt32(6),
                        TaskStatus = dr.GetByte(7),
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
                        TaskStatus = dr.GetByte(7),
                        TaskMemo = dr[8].ToString(),
                        ExecuterId = dr.IsDBNull(6) ? 0 : dr.GetInt32(6),
                        CreaterId =  dr.GetInt32(11)
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
                r.Creater = new UserInfo() { UserName = GetUserName(r.CreaterId) };
                // 
                if(r.ExecuterId == 0)
                {
                    continue;
                }
                r.Executer = new UserInfo() { UserName = GetUserName((int)r.ExecuterId) };               
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

        public void Release()
        {

        }
       
    }

    public class EF_TaskLogic : ITaskLogic, IDisposable
    {
        private int _OperatorId;

        private TaskContainer _taskContext;

        private string[] _errMessage;

        public EF_TaskLogic(int operatorId, string sqlConStr, string tableStr)
        {
            _OperatorId = operatorId;
            _errMessage = new string[16] { "", // 0
                "Task not exists!",  // 1
                "Task already accepted by someone", // 2
                "you have rejected task", // 3
                "Task has been done", // 4
                "task has not been accepted yet", // 5
                "task has not been assigned yet", // 6
                "task has already existed",       // 7
                "task was not created by you",  // 8
                "delegated user to do task is not exists",  // 9
                "delegated user has already done a task",  // 10
                "you can't accept the task which you are not executer",  // 11
                "you are doing a task now, can not accept a new task until you finish the current task",  // 12
                "you can't reject the task which you are not executer",  // 13
                "you can't finish the task which you are neither creater nor executer", // 14
                "task status is changed, can not edit it"  // 15
            };
            //
            var sqlCon = new SqlConnection(sqlConStr);
            sqlCon.Open();
            _taskContext = new TaskContainer(sqlCon, tableStr);
        }

        public bool MissionNameIsExists(string missionName)
        {
            bool flag = true;
            var task = _taskContext.AllMissions.Where(s => s.TaskTitle == missionName).FirstOrDefault();
            if (task == null)
            {
                flag = false;
            }

            return flag;
        }

        public UnifiedTaskJson CreateMission(NewMission task)
        {
            var result = new UnifiedTaskJson() { result = 0, message = _errMessage[0] };
            var existTask = _taskContext.AllMissions.Where(s => s.TaskTitle.Equals(task.TaskTitle)).FirstOrDefault();
            if (existTask == null)
            {
                //
                int taskId = _taskContext.AllMissions.Max(s => s.TaskId) + 1;
                var newTask = new Mission()
                {
                    TaskId = taskId,
                    TaskTitle = task.TaskTitle,
                    AcceptTime = null,
                    AssignTime = null,
                    CreaterId = task.Creater,
                    CreateTime = DateTime.Now,
                    DoneTime = null,
                    DueTime = task.DueTime,
                    ExecuterId = null,
                    ParentTaskId = task.ParentTaskId,
                    Priority = task.Priority,
                    TaskMemo = task.TaskMemo,
                    TaskStatus = (byte)task.TaskStatus
                };
                _taskContext.AllMissions.Add(newTask);
                //
                AddTaskFlow(_taskContext, string.Format("User[{0}] create a new task[name:{1}, priority:{2}, dueTime:{3}, Memo:{4}], parentId:{5}", _OperatorId, task.TaskTitle, task.Priority, task.DueTime, task.TaskMemo, task.ParentTaskId), taskId, 0);
                _taskContext.SaveChanges();
            }
            else
            {
                result.result = 7; result.message = _errMessage[result.result];
            }

            return result;
        }

        public UnifiedTaskJson EditMissionProperty(EditMission task)
        {
            var result = new UnifiedTaskJson() { result = 0, message = _errMessage[0] };
            var editTask = _taskContext.AllMissions.Where(s => s.TaskId == task.TaskId).FirstOrDefault();
            if (editTask == null) // 任务不存在
            {
                result.result = 1; result.message = _errMessage[result.result];
            }
            else // 任务存在
            {
                if (editTask.CreaterId == _OperatorId) // 有权编辑
                {
                    if (editTask.TaskStatus == 0) // 允许编辑
                    {
                        if (task.TaskTitle.Equals(editTask.TaskTitle) == false) // 任务名称变化了，需要判断唯一性
                        {
                            var existsTask = _taskContext.AllMissions.Where(s => s.TaskTitle == task.TaskTitle).FirstOrDefault();
                            if (existsTask == null)
                            {
                                AddTaskFlow(_taskContext, string.Format("edit task[{0}], Name:{7} -> {8}, priority:{1} -> {2}, dueTime:{3} -> {4}, taskMemo:{5} -> {6}", task.TaskId, editTask.Priority, task.Priority, editTask.DueTime, task.DueTime, editTask.TaskMemo, task.TaskMemo, editTask.TaskTitle, task.TaskTitle), task.TaskId, 5);
                                // 
                                editTask.TaskTitle = task.TaskTitle;
                                editTask.Priority = task.Priority;
                                editTask.TaskMemo = task.TaskMemo;
                                editTask.DueTime = task.DueTime;
                                //
                                _taskContext.SaveChanges();
                            }
                            else
                            {
                                result.result = 7; result.message = _errMessage[result.result];
                            }
                        }
                        else // 任务名称没有变化
                        {
                            AddTaskFlow(_taskContext, string.Format("edit task[{0}],priority:{1} -> {2}, dueTime:{3} -> {4}, taskMemo:{5} -> {6}", task.TaskId, editTask.Priority, task.Priority, editTask.DueTime, task.DueTime, editTask.TaskMemo, task.TaskMemo), task.TaskId, 5);
                            editTask.Priority = task.Priority;
                            editTask.TaskMemo = task.TaskMemo;
                            editTask.DueTime = task.DueTime;
                            //
                            _taskContext.SaveChanges();
                        }
                    }
                    else // 不允许编辑
                    {
                        result.result = 15; result.message = _errMessage[result.result];
                    }
                }
                else  // 无权编辑任务信息
                {
                    result.result = 8; result.message = _errMessage[result.result];
                }
            }

            return result;
        }

        public UnifiedTaskJson AssignMission(int taskId, int userId)
        {
            var result = new UnifiedTaskJson() { result = 0, message = _errMessage[0] };
            //
            var assignedTask = _taskContext.AllMissions.Where(s => s.TaskId == taskId).FirstOrDefault();
            if (assignedTask == null)
            {
                result.result = 1; result.message = _errMessage[result.result];
            }
            else
            {
                if (assignedTask.CreaterId == _OperatorId) // 有权分配任务
                {
                    if (assignedTask.TaskStatus == 2)
                    {
                        result.result = 2; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 4)
                    {
                        result.result = 4; result.message = _errMessage[result.result];
                    }
                    else
                    {
                        var assignedUser = _taskContext.AllUsers.Find(userId);
                        if (assignedUser == null) // 被委派任务的人不存在
                        {
                            result.result = 9; result.message = _errMessage[result.result];
                        }
                        else // 判断被委派任务的人的现在状态
                        {
                            if (assignedUser.IsWorking == false) // 无任务
                            {
                                assignedTask.AssignTime = DateTime.Now;
                                assignedTask.ExecuterId = userId;
                                assignedTask.TaskStatus = 1;
                                // 流水记录
                                AddTaskFlow(_taskContext, string.Format("User[{0}] assign task[{1}] to user[{2}]", _OperatorId, taskId, userId), taskId, 1);
                                // 
                                _taskContext.SaveChanges();
                            }
                            else // 被委派任务的人 已经有任务了
                            {
                                result.result = 10; result.message = _errMessage[result.result];
                            }
                        }
                    }
                }
                else  // 无权分配任务
                {
                    result.result = 8; result.message = _errMessage[result.result];
                }
            }

            return result;
        }

        public UnifiedTaskJson AcceptMission(int taskId)
        {
            var result = new UnifiedTaskJson() { result = 0, message = null };
            //
            var assignedTask = _taskContext.AllMissions.Where(s => s.TaskId == taskId).FirstOrDefault();
            if (assignedTask == null)
            {
                result.result = 1; result.message = _errMessage[result.result];
            }
            else // 任务存在
            {
                if (assignedTask.ExecuterId == _OperatorId) // 是分配给当前用户的任务
                {
                    if (assignedTask.TaskStatus == 2)
                    {
                        result.result = 2; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 4)
                    {
                        result.result = 4; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 1)
                    {
                        var you = _taskContext.AllUsers.Find(_OperatorId);
                        if (you.IsWorking == false) // 没有任务
                        {
                            you.IsWorking = true;
                            _taskContext.Entry<UserInfo>(you).State = System.Data.Entity.EntityState.Modified;
                            // 
                            assignedTask.AcceptTime = DateTime.Now;
                            assignedTask.TaskStatus = 2;
                            // 流水记录
                            AddTaskFlow(_taskContext, string.Format("User[{0}] accept task[{1}]", _OperatorId, taskId), taskId, 2);
                            // 
                            _taskContext.SaveChanges();
                        }
                        else  // 已经有任务了
                        {
                            result.result = 12; result.message = _errMessage[result.result];
                        }
                    }
                    else if (assignedTask.TaskStatus == 3)
                    {
                        result.result = 3; result.message = _errMessage[result.result];
                    }
                }
                else // 无权接受任务
                {
                    result.result = 11; result.message = _errMessage[result.result];
                }
            }

            return result;
        }

        public UnifiedTaskJson RejectMission(int taskId)
        {
            var result = new UnifiedTaskJson() { result = 0, message = _errMessage[0] };
            //
            var assignedTask = _taskContext.AllMissions.Where(s => s.TaskId == taskId).FirstOrDefault();
            if (assignedTask == null)
            {
                result.result = 1; result.message = _errMessage[result.result];
            }
            else // 任务存在
            {
                if (assignedTask.ExecuterId == _OperatorId)  // 是分配给当前用户的任务
                {
                    if (assignedTask.TaskStatus == 2)
                    {
                        result.result = 2; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 4)
                    {
                        result.result = 4; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 1)
                    {
                        var you = _taskContext.AllUsers.Where(w => w.UserId == _OperatorId).FirstOrDefault();
                        you.IsWorking = false;
                        assignedTask.TaskStatus = 3;
                        // 流水记录
                        AddTaskFlow(_taskContext, string.Format("User[{0}] reject task[{1}]", _OperatorId, taskId), taskId, 3);
                        // 
                        _taskContext.SaveChanges();
                    }
                    else if (assignedTask.TaskStatus == 3)
                    {
                        result.result = 3; result.message = _errMessage[result.result];
                    }
                }
                else  // 不是分配给当前用户的
                {
                    result.result = 13; result.message = _errMessage[result.result];
                }
            }

            return result;
        }

        public UnifiedTaskJson MissionDone(int taskId)
        {
            var result = new UnifiedTaskJson() { result = 0, message = _errMessage[0] };
            //
            var assignedTask = _taskContext.AllMissions.Where(s => s.TaskId == taskId).FirstOrDefault();
            if (assignedTask == null)
            {
                result.result = 1; result.message = _errMessage[result.result];
            }
            else // 任务存在
            {
                if (assignedTask.ExecuterId == _OperatorId || assignedTask.CreaterId == _OperatorId) // 任务执行者或任务创建者
                {
                    if (assignedTask.TaskStatus == 4)
                    {
                        result.result = 4; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 2)
                    {
                        var executer = _taskContext.AllUsers.Where(w => w.UserId == assignedTask.ExecuterId).FirstOrDefault();
                        executer.IsWorking = false;
                        // 
                        assignedTask.DoneTime = DateTime.Now;
                        assignedTask.TaskStatus = 4;
                        // 流水记录
                        AddTaskFlow(_taskContext, string.Format("User[{0}] finish the task[{1}]", _OperatorId, taskId), taskId, 4);
                        // 
                        _taskContext.SaveChanges();
                    }
                    else if (assignedTask.TaskStatus == 3)
                    {
                        result.result = 3; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 1)
                    {
                        result.result = 5; result.message = _errMessage[result.result];
                    }
                    else if (assignedTask.TaskStatus == 0)
                    {
                        result.result = 6; result.message = _errMessage[result.result];
                    }
                }
                else  // 用户无权完结任务
                {
                    result.result = 14; result.message = _errMessage[result.result];
                }
            }

            return result;
        }

        public EditMission GetEditMission(int taskId)
        {
            EditMission task;
            var mission = _taskContext.AllMissions.Where(s => s.TaskId == taskId).FirstOrDefault();
            if (mission != null)
            {
                if(mission.CreaterId == _OperatorId) // 是执行者创建 才可以编辑
                {
                    if (mission.TaskStatus == 0) // 允许编辑
                    {
                        task = new EditMission(taskId);
                        task.TaskTitle = mission.TaskTitle;
                        task.Priority = mission.Priority;
                        task.DueTime = mission.DueTime;
                        task.TaskMemo = mission.TaskMemo;
                    }
                    else // 不允许编辑
                    {
                        task = new EditMission(-3) { TaskTitle = "", Priority = -1, DueTime = DateTime.MinValue, TaskMemo = "" };
                    }
                }
                else  // 无权编辑
                {
                    task = new EditMission(-2) { TaskTitle = "", Priority = -1, DueTime = DateTime.MinValue, TaskMemo = "" };
                }
            }
            else  // 任务不存在
            {
                task = new EditMission(-1) { TaskTitle = "", Priority = -1, DueTime = DateTime.MinValue, TaskMemo = "" };
            }

            return task;
        }

        public IEnumerable<Mission> QueryUserMission(int userId, int pageIndex, int pageSize)
        {
            var queryResult = new List<Mission>();

            var taskList = _taskContext.AllMissions.Where(s => (s.ExecuterId == userId && s.TaskStatus == 1) || s.CreaterId == userId)
                                    .OrderByDescending(o => o.CreateTime).OrderBy(o => o.TaskStatus)
                                    .Skip(pageIndex * pageSize).Take(pageSize)
                                    .ToList();
            if (taskList != null && taskList.Count > 0)
            {
                foreach (var t in taskList)
                {
                    var model = new Mission()
                    {
                        TaskId = t.TaskId,
                        TaskTitle = t.TaskTitle,
                        Priority = t.Priority,
                        DueTime = t.DueTime,
                        AcceptTime = t.AcceptTime,
                        AssignTime = t.AssignTime,
                        CreaterId = t.CreaterId,
                        ExecuterId = t.ExecuterId,
                        CreateTime = t.CreateTime,
                        DoneTime = t.DoneTime,
                        ParentTaskId = t.ParentTaskId,
                        TaskMemo = t.TaskMemo,
                        TaskStatus = t.TaskStatus,
                        UserActions = GetUserActions(t.TaskStatus, t.CreaterId == userId)
                    };
                    queryResult.Add(model);
                }
            }

            return queryResult;
        }

        public int QueryUserMissionCount(int userId)
        {
            int count = -1;
            try
            {
                count = _taskContext.AllMissions.Where(s => (s.ExecuterId == userId && s.TaskStatus == 1) || s.CreaterId == userId).Count();

            }
            catch (Exception ex)
            {
                count = 0;
            }
            
            return count;
        }

        private IEnumerable<UserActionType> GetUserActions(int taskStatus, bool isOwner = true)
        {
            var actions = new List<UserActionType>();
            if(isOwner == false) // 不是任务创建者 是执行者
            {
                switch (taskStatus)
                {
                    case 0: // 新任务未指派
                        break;
                    case 1: // 已分配的任务
                        actions.Add(UserActionType.DenyMission);
                        actions.Add(UserActionType.AcceptMission);
                        break;
                    case 2: // 已受理的任务
                        actions.Add(UserActionType.MissionDone);
                        break;
                    case 3: // 被拒绝的任务
                        actions.Add(UserActionType.AcceptMission);
                        break;
                    case 4: // 已完结的任务
                        break;
                    default:
                        break;
                }
            }
            else // 任务创建者
            {
                switch(taskStatus)
                {
                    case 0: // 新任务未指派
                        actions.Add(UserActionType.AssignMission);
                        actions.Add(UserActionType.EditMission);
                        break;
                    case 1: // 已分配的任务
                        actions.Add(UserActionType.AssignMission);
                        actions.Add(UserActionType.AcceptMission);
                        actions.Add(UserActionType.MissionDone);
                        break;
                    case 2: // 已受理的任务
                        actions.Add(UserActionType.MissionDone);
                        break;
                    case 3: // 被拒绝的任务
                        actions.Add(UserActionType.AssignMission);
                        actions.Add(UserActionType.EditMission);
                        break;
                    case 4: // 已完结的任务
                        break;
                    default:
                        break;
                }
            }

            return actions;
        }

        private void AddTaskFlow(TaskContainer context, string workMemo, int taskId, byte operType)
        {
            // 
            var model = new TaskFlow()
            {
                OperatorId = _OperatorId,
                WorkMemo = workMemo,
                TaskId = taskId,
                HappendTime = DateTime.Now,
                OperType = operType
            };

            context.WorkLogs.Add(model);
        }

        public void Release()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (_taskContext != null)
            {
                _taskContext.Dispose();
            }
        }
    }
}
