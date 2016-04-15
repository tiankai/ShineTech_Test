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
            // 
            _iTaskLogic = new TaskLogic.SimpleTaskLogic(tableStr, conStr);

            base.Initialize(requestContext);
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

        private Int32 GetUserId(string userName)
        {
            return 1;
        }

        /// <summary>
        /// 新建任务
        /// </summary>
        /// <param name="model">任务信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewTask(TaskTest_Web.Models.NewTaskModel model)
        {
            string userKey = GetUserKeyName("UName");
            var currentUser = this.Session[userKey] as TaskTest_Web.Models.LogOnModel;

            if(currentUser == null)
            {
                return RedirectToAction("Logon", "Account");
            }
            else
            {
                int userId = GetUserId(currentUser.UserName);
                var mission = new TaskLogic.NewMission()
                {
                    TaskId = 0,
                    TaskTitle = model.TaskTitle,
                    TaskMemo = model.TaskDescription,
                    TaskStatus = 0,
                    Priority = (int)model.Priority,
                    DueTime = model.DueTime,
                    ParentTaskId = model.ParentTaskId,
                    Creater = userId
                };

                bool flag = _iTaskLogic.CreateMission(mission);

                return RedirectToAction("Index");
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

            if(string.IsNullOrEmpty(model.TaskTitle) == true) // 任务信息不存在
            {
                return RedirectToAction("Index");
            }
            else // 任务信息存在
            {
                var viewModel = new TaskTest_Web.Models.EditTaskModel();
                viewModel.TaskId = id;
                viewModel.TaskTitle = model.TaskTitle;
                viewModel.TaskDescription = model.TaskMemo;
                viewModel.DueTime = model.DueTime;
                viewModel.Priority = (Models.TaskPriority)model.Priority;

                return View((object)viewModel);
            }
        }
        
        /// <summary>
        /// 将任务信息更新
        /// </summary>
        /// <param name="model">新的任务信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTask(TaskTest_Web.Models.EditTaskModel model)
        {
            // 模型转换
            var editModel = new TaskLogic.EditMission(model.TaskId);
            editModel.TaskTitle = model.TaskTitle;
            editModel.TaskMemo = model.TaskDescription;
            editModel.Priority = (int)model.Priority;
            editModel.DueTime = model.DueTime;
            // 更新任务信息
            bool flag = _iTaskLogic.EditMissionProperty(editModel);
            if(flag == true)
            {

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "更新任务信息失败!");
                return View((object)model);
            }
        }

        private TaskTest_Web.Models.EachTaskModel GetNodeModel(int taskId, string taskTitle, TaskTest_Web.Models.TaskPriority taskPriority, DateTime dt, int parentTaskId)
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
    }
}
