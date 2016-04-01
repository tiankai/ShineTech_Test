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

        public SimpleTaskLogic(DBHelper.IDBHelper dbHelper, TaskLoger.ITaskLoger loger, string tableStr)
        {
            _tableStr = tableStr;
            _dbHelper = dbHelper;
            _loger = loger;
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

        public bool CreateMission(Mission task)
        {
            string tableName = GetTableName(_tableStr, TableTaskName);
            // 
            string sqlMaxId = string.Format("select isnull(max(skbh), 0) from {0}", tableName);
            task.TaskId = GetMaxId(sqlMaxId) + 1;
            //
            const string taskTitleParaName = "@P_TaskTitle_In";
            const string taskDueTimeParaName = "@P_DueTime_In";
            string sqlInsert = string.Format("Insert into {0}({1}) values({2}, {3}, {4}, {5}, {6}, {7}, '{8}')", tableName, GetFieldNames("Tid", "TaskName", "Priority", "ParentTask", "DueTime", "CreateTime", "Creater"), task.TaskId, taskTitleParaName, task.Priority, task.ParentTaskId, taskDueTimeParaName, "getdate()", task.Creater);
            // 
            var taskTitleParameter = new SqlParameter(taskTitleParaName, SqlDbType.VarChar, 40);
            taskTitleParameter.Value = task.TaskTitle;
            taskTitleParameter.Direction = ParameterDirection.Input;
            // 
            var taskDueTimeParameter = new SqlParameter(taskDueTimeParaName, SqlDbType.DateTime);
            taskDueTimeParameter.Value = task.DueTime;
            taskDueTimeParameter.Direction = ParameterDirection.Input;
            
            return _dbHelper.ExecuteSql(sqlInsert, taskTitleParameter, taskDueTimeParameter) > 0;
        }

        public bool EditMissionProperty(EditMission task)
        {
            const string taskDueTimeParaName = "@P_DueTime_In";
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Format("update {0} set Priority = {1}, DueTime = {1}, ParentTask = {2} where Tid = {3}", tableName, task.Priority, taskDueTimeParaName, task.ParentTaskId, task.TaskId);
            //
            var taskDueTimeParameter = new SqlParameter(taskDueTimeParaName, SqlDbType.DateTime);
            taskDueTimeParameter.Value = task.DueTime;
            taskDueTimeParameter.Direction = ParameterDirection.Input;

            return _dbHelper.ExecuteSql(sqlEdit, taskDueTimeParameter) > 0;
        }


        public bool AssignMission(int taskId, int userId)
        {
            int status = 1;
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Format("update {0} set TaskStatus = {1}, Executer = {2} where Tid = {3}", tableName, status, userId, taskId);

            return _dbHelper.ExecuteSql(sqlEdit) > 0;
        }

        public bool AcceptMission(int taskId)
        {
            int status = 2;
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Format("update {0} set TaskStatus = {1} where Tid = {2}", tableName, status, taskId);

            return _dbHelper.ExecuteSql(sqlEdit) > 0;
        }

        public bool MissionDone(int taskId)
        {
            int status = 3;
            string tableName = GetTableName(_tableStr, TableTaskName);
            string sqlEdit = string.Format("update {0} set TaskStatus = {1} where Tid = {2}", tableName, status, taskId);

            return _dbHelper.ExecuteSql(sqlEdit) > 0;
        }


        public IEnumerable<Mission> QueryUserMission(int userId, int pageIndex, int pageCount)
        {
            throw new NotImplementedException();
        }

        public int QueryUserMissionCount(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
