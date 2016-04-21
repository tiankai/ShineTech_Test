using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskTest_Web.Controllers
{
    public class TaskController : Controller
    {
        private TaskLogic.ITaskLogic _iTaskLogic;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            string tableStr = Models.Utility.GetParamStr(Models.ParamType.TableStr);
            string conStr = Models.Utility.GetParamStr(Models.ParamType.SqlConStr);
            // 
            int runMode = Convert.ToInt32(Models.Utility.GetParamStr(Models.ParamType.ProgramMode));
            if (runMode == 0)
            {
                _iTaskLogic = new TaskLogic.SimpleTaskLogic(tableStr, conStr);
            }
            else if(runMode == 1)
            {
                var user = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));
                _iTaskLogic = new TaskLogic.EF_TaskLogic(user.UserId, conStr, tableStr);
            }
            //
            base.Initialize(requestContext);
        }
        
        private Models.LogOnModel GetUser(string keyName)
        {
            string userKey = GetUserKeyName(keyName);
            if (string.IsNullOrEmpty(userKey) == false)
            {
                var currentUser = this.Session[userKey] as Models.LogOnModel;
                if (currentUser == null)
                {
                    return new Models.LogOnModel() { UserId = -1 };
                }
                return currentUser;
            }

            return new Models.LogOnModel() { UserId = -1 };
        }

        private string GetUserKeyName(string keyName)
        {
            var cookie = this.Request.Cookies[keyName];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return cookie.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public string ValidateName(string name)
        {
            bool flag = _iTaskLogic.MissionNameIsExists(name);

            return flag == false ? "0" : "1";
        }

        public string Test(int? pageIndex, int? pageSize)
        {
            int index = pageIndex.HasValue == true ? (int)pageIndex : 1;
            int size = pageSize.HasValue == true ? (int)pageSize : 10;

            var sbText = new System.Text.StringBuilder("{ \"PagePart\": { \"TotoalPages\": 5, \"PageSize\": 10, \"PageIndex\": ");
            //
            sbText.Append(index).Append(" }, ").Append("\"Tasks\": [{ \"id\": 2, \"title\":\"John\", \"priority\":6, \"dueTime\":\"2015-06-05\", \"executer\":\"John Doe\", \"memo\": \"hello world\",  \"doActions\" : [ {\"doAction\": 2, \"actionMemo\" : \"Accept\" }, {\"doAction\": 3, \"actionMemo\" : \"Reject\" }, {\"doAction\": 4, \"actionMemo\" : \"Done\" } ] }, { \"id\": 9, \"title\":\"Stars Plan\", \"priority\":4, \"dueTime\":\"2015-06-15\", \"executer\":\"Wesley Gibson\", \"memo\": \"what's your plan?\", \"doActions\" : [ { \"doAction\": 0, \"actionMemo\" : \"Create New\" }, { \"doAction\": 5, \"actionMemo\" : \"Edit Task\" }, { \"doAction\": 1, \"actionMemo\" : \"Assign\" } ] }]}");

            return sbText.ToString(); 
        }
        
        public string MyTaskToDo(int? pageIndex, int? pageSize)
        {
            int index = pageIndex.HasValue == true ? (int)pageIndex : 1;
            int size = pageSize.HasValue == true ? (int)pageSize : 10;
            // 
            var currentUser = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));

            if (currentUser.UserId == -1)
            {
                return "";
            }
            else
            {
                var sbText = new System.Text.StringBuilder("{ \"PagePart\": { \"TotoalPages\": 5, \"PageSize\": 10, \"PageIndex\": ");
                //
                sbText.Append(index).Append(" }, ").Append("\"Tasks\": [{ \"id\": 2, \"title\":\"John\", \"priority\":6, \"dueTime\":\"2015-06-05\", \"creater\":\"").Append(currentUser.UserName).Append("\", \"memo\": \"hello world\",  \"doActions\" : [ {\"doAction\": 2, \"actionMemo\" : \"Accept\" }, {\"doAction\": 3, \"actionMemo\" : \"Reject\" } ] }, { \"id\": 9, \"title\":\"Stars Plan\", \"priority\":4, \"dueTime\":\"2015-06-15\", \"creater\":\"Wesley Gibson\", \"memo\": \"what's your plan?\", \"doActions\" : [ { \"doAction\": 2, \"actionMemo\" : \"Accept\" }, { \"doAction\": 3, \"actionMemo\" : \"Reject\" } ] }]}");

                return sbText.ToString();
            } 
        }

        public string GetTaskList(int? pageIndex, int? pageSize)
        {
            string [] actionTitles = new string[6] { "Create", "Assign", "Accept", "Reject", "Done", "Edit" };
            int index = pageIndex.HasValue == true ? (int)pageIndex : 1;
            int size = pageSize.HasValue == true ? (int)pageSize : 10;
            var viewModel = new Models.TaskListViewModel() { TaskPageCount = size, CurrentPageIndex = index, TaskTotalPages = 0 };
            // 
            string userKey = GetUserKeyName("UName");
            var currentUser = this.Session[userKey] as TaskTest_Web.Models.LogOnModel;

            if (currentUser == null)
            {
                viewModel.result = 1;
            }
            else
            {                
                var result = _iTaskLogic.QueryUserMission(currentUser.UserId, index, size);
                int totoalCount = _iTaskLogic.QueryUserMissionCount(currentUser.UserId);
                if(totoalCount > 0)
                {
                    if(totoalCount % size == 0)
                    {
                        viewModel.TaskTotalPages = totoalCount / size;
                    }
                    else
                    {
                        viewModel.TaskTotalPages = totoalCount / size + 1;
                    }
                    var taskList = new List<Models.EachTaskModel>();
                    foreach(var r in result)
                    {
                        var task = new Models.EachTaskModel()
                        {
                            AcceptTime = r.AcceptTime,
                            AssignTime = r.AssignTime,
                            Creater = r.Creater.UserName,
                            CreateTime = r.CreateTime,
                            DoneTime = r.DoneTime,
                            DueTime = (long)(r.DueTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds),
                            Executer = r.Executer.UserName,
                            ParentTaskId = r.ParentTaskId,
                            Priority = (int)r.Priority,
                            Status = (int)r.TaskStatus,
                            TaskDescription = r.TaskMemo,
                            TaskId = r.TaskId,
                            TaskTitle = r.TaskTitle
                        };
                        if(r.UserActions != null)
                        {
                            var actionList = new List<Models.UserBehaivour>();
                            foreach(var act in r.UserActions)
                            {
                                actionList.Add(new Models.UserBehaivour() { ActionType = (uint)act, ActionTitle = actionTitles[(uint)act] });
                            }
                            task.UserActions = actionList;
                        }
                        taskList.Add(task);
                    }
                    //
                    viewModel.result = 0;
                    viewModel.TaskList = taskList;
                }
                else
                {
                    viewModel.result = 2;
                }
            }

          

            return LitJson.JsonMapper.ToJson(viewModel);
        }

        private string GetUserNotLoginString(int errCode)
        {
            var sbText = new System.Text.StringBuilder("{ \"result\" : ");
            sbText.Append(errCode).Append(", \"message\" : \"")
                .Append("user not login").Append("\" }");

            return sbText.ToString();
        }

        public string AssignTask(int id, int userId)
        {
            var user = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (user.UserId == -1)
            {
                return GetUserNotLoginString(-1);
            }
            else
            {
                var result = _iTaskLogic.AssignMission(id, userId);
                return LitJson.JsonMapper.ToJson(result);
            }
        }

        public string AcceptTask(int id)
        {
            var user = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (user.UserId == -1)
            {
                return GetUserNotLoginString(-1);
            }
            else
            {
                var result = _iTaskLogic.AcceptMission(id);
                return LitJson.JsonMapper.ToJson(result);
            }
        }

        public string RejectTask(int id)
        {
            var user = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (user.UserId == -1)
            {
                return GetUserNotLoginString(-1);
            }
            else
            {
                var result = _iTaskLogic.RejectMission(id);
                return LitJson.JsonMapper.ToJson(result);
            }
        }

        public string TaskDone(int id)
        {
            var user = GetUser(Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (user.UserId == -1)
            {
                return GetUserNotLoginString(-1);
            }
            else
            {
                var result = _iTaskLogic.MissionDone(id);
                return LitJson.JsonMapper.ToJson(result);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(_iTaskLogic != null)
            {
                _iTaskLogic.Release();
            }
            base.Dispose(disposing);
        }
    }
}