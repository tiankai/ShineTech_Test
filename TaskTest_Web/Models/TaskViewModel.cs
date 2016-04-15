using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// 
using System.ComponentModel.DataAnnotations;

namespace TaskTest_Web.Models
{

    /// <summary>
    /// 任务优先级
    /// </summary>
    public enum TaskPriority : uint
    {
        /// <summary>
        /// 最高级别
        /// </summary>
        Top = 0,
        /// <summary>
        /// 高级别
        /// </summary>
        High = 1,
        /// <summary>
        /// 主要级别
        /// </summary>
        Senior = 2,
        /// <summary>
        /// 次要级别
        /// </summary>
        Junior = 3,
        /// <summary>
        /// 低级别
        /// </summary>
        Low = 4,
        /// <summary>
        /// 无关紧要
        /// </summary>
        None = 5
    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus : uint
    {
        /// <summary>
        /// 新任务未分配
        /// </summary>
        NewUnassigned  = 0,
        /// <summary>
        /// 已分配
        /// </summary>
        Assigned = 1,
        /// <summary>
        /// 已受理
        /// </summary>
        Accepted = 2,
        /// <summary>
        /// 被拒绝
        /// </summary>
        Rejected = 3,
        /// <summary>
        /// 已完结
        /// </summary>
        Done = 4
    }

    public class UserBehaivour
    {
        public uint ActionType { get; set; }

        public string ActionTitle { get; set; }
    }

    /// <summary>
    /// 新建任务
    /// </summary>
    public class NewTaskModel
    {
        [Required]
        [Display(Name = "Task Title")]
        [StringLength(40)]
        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }

        [Display(Name = "Task Description")]
        [StringLength(512)]
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }

        [Required]
        [EnumDataType(typeof(TaskTest_Web.Models.TaskPriority))]
        [Display(Name = "Task Priority")]
        /// <summary>
        /// 优先级
        /// </summary>
        public TaskPriority Priority { get; set; }        
        /// <summary>
        /// 到期时间
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ConvertEmptyStringToNull = true, NullDisplayText = "", ApplyFormatInEditMode = true)]
        [Display(Name = "Task DueTime")]
        public DateTime DueTime { get; set; }
        /// <summary>
        /// 是否是顶级任务, 任务树的根节点
        /// </summary>
        public bool IsTopTask { get; set; }
        /// <summary>
        /// 父级任务Id, 若是任务树根节点, 此项为0
        /// </summary>
        public int ParentTaskId { get; set; }
    }

    /// <summary>
    /// 编辑任务
    /// </summary>
    public class EditTaskModel
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public int TaskId { get; set; }

        [Required]
        [Display(Name = "Task Title")]
        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }

        [Display(Name = "Task Description")]
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }

        [Required]
        [EnumDataType(typeof(TaskTest_Web.Models.TaskPriority))]
        [Display(Name = "Task Priority")]
        /// <summary>
        /// 优先级
        /// </summary>
        public TaskPriority Priority { get; set; }

        [Required]
        [Display(Name = "Task DueTime")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime DueTime { get; set; }
    }

    public class EachTaskModel
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
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public long DueTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 任务分配时间
        /// </summary>
        public DateTime? AssignTime { get; set; }
        /// <summary>
        /// 受理时间
        /// </summary>
        public DateTime? AcceptTime { get; set; }
        /// <summary>
        /// 完结时间
        /// </summary>
        public DateTime? DoneTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 执行者
        /// </summary>
        public string Executer {get;set;}
        /// <summary>
        /// 上一级任务编号
        /// </summary>
        public int ParentTaskId { get; set; }
        /// <summary>
        /// 任务的状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 用户动作
        /// </summary>
        public IEnumerable<UserBehaivour> UserActions { get; set; }
    }


    /// <summary>
    /// 任务树结构
    /// </summary>
    public class TaskTreeNode
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public EachTaskModel RootNode { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public IList<TaskTreeNode> ChildrenNodes { get; set; }
        /// <summary>
        /// 当前节点层级 0:根节点
        /// </summary>
        public byte NodeLevel { get; set; }
    }

    public class TaskListViewModel
    {
        /// <summary>
        /// 结果
        /// </summary>
        public int result { get; set; }
        public IEnumerable<EachTaskModel> TaskList { get; set; }
        /// <summary>
        /// 任务数量
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int TaskPageCount { get; set; }
        /// <summary>
        /// 总计页数
        /// </summary>
        public int TaskTotalPages { get; set; }
    }
}