using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TaskTest_Web.Controllers
{
    public class AdminController : Controller
    {
        private TaskLogic.ITaskLogic _iTaskLogic;

        protected override void Initialize(RequestContext requestContext)
        {
            string tableStr = Models.Utility.GetParamStr(Models.ParamType.TableStr);
            string conStr = Models.Utility.GetParamStr(Models.ParamType.SqlConStr);
            int mode = Convert.ToInt32(Models.Utility.GetParamStr(Models.ParamType.ProgramMode));
            // 
            if(mode == 1) // Entity Framework Mode
            {
                var user = GetUser(requestContext.HttpContext, Models.Utility.GetParamStr(Models.ParamType.CookieName));
                _iTaskLogic = new TaskLogic.EF_TaskLogic(user.UserId, conStr, tableStr);
            }
            else if(mode == 0) // ADO.NET-DAL Mode
            {
                _iTaskLogic = new TaskLogic.SimpleTaskLogic(tableStr, conStr);
            }
            //
            base.Initialize(requestContext);
        }

        private string GetUserKeyName(HttpContextBase httpContext, string keyName)
        {
            var cookie = httpContext.Request.Cookies[keyName];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return cookie.Value;
            }
            else
            {
                return string.Empty;
            }
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据任务编号获取下级任务信息
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        public ActionResult GetTaskTree(int? id)
        {
            const int unValidTaskId = -1;
            int taskId = id.HasValue == true ? (int)id : unValidTaskId;
            if (taskId == unValidTaskId)
            {
                var taskTreeModel = GetTaskTree(taskId, 0, 3);

                return PartialView("TaskTree", taskTreeModel);
            }
            else
            {
                //TODO: 依据任务编号查询出所有的任务信息
                var taskTreeModel = GetTaskTree(taskId, 0, 3);

                return PartialView("TaskTree", taskTreeModel);
            }                       
        }
        /// <summary>
        /// 新建任务
        /// </summary>
        /// <param name="id">0：顶级任务, >0:父级任务编号 </param>
        /// <returns></returns>
        public ActionResult NewTask(int? id)
        {
            const int DueDays = 30;
            int taskId = id.HasValue == true ? (int)id : 0;
            // 
            var viewModel = new TaskTest_Web.Models.NewTaskModel();
            viewModel.DueTime = DateTime.Now.AddDays(DueDays);
            viewModel.ParentTaskId = taskId;
            viewModel.IsTopTask = taskId == 0 ? true : false;

            return View((object)viewModel);
        }
        
        private Models.LogOnModel GetUser(HttpContextBase httpContext, string keyName)
        {
            string userKey = GetUserKeyName(httpContext, keyName);
            if(string.IsNullOrEmpty(userKey) == false)
            {
                var currentUser = httpContext.Session[userKey] as Models.LogOnModel;
                if(currentUser == null)
                {
                    return new Models.LogOnModel() { UserId = -1 };
                }
                return currentUser;
            }

            return new Models.LogOnModel() { UserId = -1 };
        }

        /// <summary>
        /// 新建任务
        /// </summary>
        /// <param name="model">任务信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewTask(Models.NewTaskModel model)
        {
            var currentUser = GetUser(this.HttpContext, Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (currentUser.UserId == -1)
            {
                return RedirectToAction("Logon", "Account");
            }
            else
            {
                var mission = new TaskLogic.NewMission()
                {
                    TaskId = 0,
                    TaskTitle = model.TaskTitle,
                    TaskMemo = model.TaskDescription,
                    TaskStatus = 0,
                    Priority = (int)model.Priority,
                    DueTime = model.DueTime,
                    ParentTaskId = model.ParentTaskId,
                    Creater = currentUser.UserId
                };

                var res = _iTaskLogic.CreateMission(mission);

                if (res.result == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", string.Format("创建任务失败, errCode = {0}, 失败原因: {1}", res.result, res.message));
                    return View(model);
                }
            }
        }
        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="id">任务编号</param>
        /// <returns></returns>
        public ActionResult EditTask(int id)
        {
            var model = _iTaskLogic.GetEditMission(id);
            var viewModel = new TaskTest_Web.Models.EditTaskModel();
            viewModel.TaskId = model.TaskId;
            viewModel.TaskTitle = model.TaskTitle;
            viewModel.TaskDescription = model.TaskMemo;
            viewModel.DueTime = model.DueTime;
            if(model.TaskId == -1) // 任务信息不存在
            {
                ModelState.AddModelError("", string.Format("任务[{0}]信息不存在", id));
            }
            else if(model.TaskId == -2) // 无权编辑任务
            {
                ModelState.AddModelError("", string.Format("无权编辑任务[{0}]", id));
            }
            else if(model.TaskId == -3)
            {
                ModelState.AddModelError("", string.Format("任务[{0}]状态已经改变了，不能进行修改", id));
            }
            else // 任务信息存在
            {                
                viewModel.Priority = (Models.TaskPriority)model.Priority;               
            }

            return View((object)viewModel);
        }
        
        /// <summary>
        /// 将任务信息更新
        /// </summary>
        /// <param name="model">新的任务信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTask(TaskTest_Web.Models.EditTaskModel model)
        {
            var currentUser = GetUser(this.HttpContext, Models.Utility.GetParamStr(Models.ParamType.CookieName));
            if (currentUser.UserId == -1)
            {
                return RedirectToAction("Logon", "Account");
            }
            else
            {
                // 模型转换
                var editModel = new TaskLogic.EditMission(model.TaskId);
                editModel.TaskTitle = model.TaskTitle;
                editModel.TaskMemo = model.TaskDescription;
                editModel.Priority = (int)model.Priority;
                editModel.DueTime = model.DueTime;
                // 更新任务信息
                var res = _iTaskLogic.EditMissionProperty(editModel);
                if (res.result == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", string.Format("更新任务信息失败, errCode = {1}, 失败原因:{0}", res.message, res.result));
                    return View((object)model);
                }
            }
        }

        private Models.EachTaskModel GetNodeModel(int taskId, string taskTitle, Models.TaskPriority taskPriority, DateTime dt, int parentTaskId)
        {
            var model = new TaskTest_Web.Models.EachTaskModel()
            {
                TaskId = taskId,
                TaskTitle = taskTitle,
                Executer = "Wesley Gibson",
                Priority = (int)taskPriority,
                Creater = "Asmon Kobe",
                CreateTime = dt,
                AcceptTime = dt.AddHours(3),
                AssignTime = dt.AddMinutes(30),
                DueTime = dt.AddDays(60).Ticks,
                ParentTaskId = parentTaskId,
                Status = (int)Models.TaskStatus.Assigned
            };

            return model;
        }        

        private TaskTest_Web.Models.TaskTreeNode GetTaskTree(int taskId, byte nodeLevel, int childrenNodeCount)
        {
            var rootNode = new TaskTest_Web.Models.TaskTreeNode() { RootNode = GetNodeModel(taskId, "Stars Plan", Models.TaskPriority.Senior, DateTime.Now, 0), NodeLevel = nodeLevel, ChildrenNodes = new List<TaskTest_Web.Models.TaskTreeNode>() };
            nodeLevel++;
            // 
            for (int i = 0; i < childrenNodeCount; i++)
            {
                // 创建子节点
                var subNodeModel = GetNodeModel(taskId + i + 1, string.Format("Plan {0}", i + 1), Models.TaskPriority.Junior, DateTime.Now, taskId);
                var subNode = new TaskTest_Web.Models.TaskTreeNode() { RootNode = subNodeModel, NodeLevel = nodeLevel, ChildrenNodes = new List<TaskTest_Web.Models.TaskTreeNode>() };
                // 添加子节点信息
                rootNode.ChildrenNodes.Add(subNode);
            }

            return rootNode;
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
