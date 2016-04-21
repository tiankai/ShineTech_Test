using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLogic
{
    /// <summary>
    /// 用户执行动作类型
    /// </summary>
    public enum  UserActionType : uint
    {
        /// <summary>
        /// 新建任务
        /// </summary>
        CreateNewMission = 0,
        /// <summary>
        /// 分配任务
        /// </summary>
        AssignMission = 1,
        /// <summary>
        /// 接受任务
        /// </summary>
        AcceptMission = 2,
        /// <summary>
        /// 拒绝任务
        /// </summary>
        DenyMission = 3,
        /// <summary>
        /// 完结任务
        /// </summary>
        MissionDone = 4,
        /// <summary>
        /// 编辑任务
        /// </summary>
        EditMission = 5
    }

    public class UnifiedTaskJson
    {
        public int result { get; set; }

        public string message { get; set; }
    }

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
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }
        /// <summary>
        /// 任务说明
        /// </summary>
        public string TaskMemo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime DueTime { get; set; }
    }
    /// <summary>
    /// 新建任务结构
    /// </summary>
    public class NewMission
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
        /// 任务说明
        /// </summary>
        public string TaskMemo { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime DueTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public int Creater { get; set; }
        /// <summary>
        /// 上一级任务编号
        /// </summary>
        public int ParentTaskId { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int TaskStatus { get; set; }
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
        /// 任务说明
        /// </summary>
        public string TaskMemo { get; set; }
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
        public Nullable<DateTime> AssignTime { get; set; }
        /// <summary>
        /// 受理时间
        /// </summary>
        public Nullable<DateTime> AcceptTime { get; set; }
        /// <summary>
        /// 完结时间
        /// </summary>
        public Nullable<DateTime> DoneTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public int CreaterId { get; set; }
        /// <summary>
        /// 执行者
        /// </summary>
        public Nullable<int> ExecuterId { get; set; }
        /// <summary>
        /// 上一级任务编号
        /// </summary>
        public int ParentTaskId { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public byte TaskStatus { get; set; }
        /// <summary>
        /// 用户行为动作
        /// </summary>
        public IEnumerable<UserActionType> UserActions { get; set; }

        public virtual UserInfo Creater { get; set; }

        public virtual UserInfo Executer { get; set; }

        public virtual Mission ParentTask { get; set; }
    }

    public class TaskFlow
    {
        public Int64 FlowId { get; set; }

        public int TaskId { get; set; }

        public byte OperType { get; set; }

        public string WorkMemo { get; set; }

        public DateTime HappendTime { get; set; }

        public int OperatorId { get; set; }

        /// <summary>
        /// 关联任务信息
        /// </summary>
        public virtual Mission TaskInfo { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public virtual UserInfo Operator { get; set; }
    }
    /// <summary>
    /// 任务业务逻辑
    /// </summary>
    public interface ITaskLogic
    {
        /// <summary>
        /// 检查任务名称是否存在
        /// </summary>
        /// <param name="missionName"></param>
        /// <returns></returns>
        bool MissionNameIsExists(string missionName);
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        UnifiedTaskJson CreateMission(NewMission task);
        /// <summary>
        /// 编辑任务属性
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        UnifiedTaskJson EditMissionProperty(EditMission task);
        /// <summary>
        /// 分配任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        UnifiedTaskJson AssignMission(int taskId, int userId);
        /// <summary>
        /// 受理任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        UnifiedTaskJson AcceptMission(int taskId);
        /// <summary>
        /// 拒绝任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        UnifiedTaskJson RejectMission(int taskId);
        /// <summary>
        /// 任务完结
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        UnifiedTaskJson MissionDone(int taskId);
        /// <summary>
        /// 获取要编辑的任务信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        EditMission GetEditMission(int taskId);
        /// <summary>
        /// 查询某个用户创建的任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<Mission> QueryUserMission(int userId, int pageIndex, int pageSize);
        /// <summary>
        /// 查询某个用户的任务数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int QueryUserMissionCount(int userId);
        /// <summary>
        /// 释放资源
        /// </summary>
        void Release();
    }
}
