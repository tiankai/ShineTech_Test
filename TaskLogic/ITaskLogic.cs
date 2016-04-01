using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLogic
{

    /// <summary>
    /// 任务编辑属性结构
    /// </summary>
    public class EditMission
    {
        private readonly int _taskId;

        public EditMission(int taskId)
        {
            _taskId = taskId;
        }
        /// <summary>
        /// 任务编号
        /// </summary>
        public int TaskId { get { return _taskId; } }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime DueTime { get; set; }
        /// <summary>
        /// 上一级任务编号
        /// </summary>
        public int ParentTaskId { get; set; }
    }

    /// <summary>
    /// 任务结构
    /// </summary>
    public class Mission
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime DueTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 任务分配时间
        /// </summary>
        public DateTime AssignTime { get; set; }
        /// <summary>
        /// 受理时间
        /// </summary>
        public DateTime AcceptTime { get; set; }
        /// <summary>
        /// 完结时间
        /// </summary>
        public DateTime DoneTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 上一级任务编号
        /// </summary>
        public int ParentTaskId { get; set; }
    }

    /// <summary>
    /// 任务业务逻辑
    /// </summary>
    public interface ITaskLogic
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        bool CreateMission(Mission task);
        /// <summary>
        /// 编辑任务属性
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        bool EditMissionProperty(EditMission task);
        /// <summary>
        /// 分配任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool AssignMission(int taskId, int userId);
        /// <summary>
        /// 受理任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        bool AcceptMission(int taskId);
        /// <summary>
        /// 任务完结
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        bool MissionDone(int taskId);

        /// <summary>
        /// 查询某个用户的任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        IEnumerable<Mission> QueryUserMission(int userId, int pageIndex, int pageCount);
        /// <summary>
        /// 查询某个用户的任务数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int QueryUserMissionCount(int userId);
    }
}
