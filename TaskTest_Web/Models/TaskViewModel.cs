using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTest_Web.Models
{
    /// <summary>
    /// 新建任务
    /// </summary>
    public class NewTaskModel
    {
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
        public DateTime DueTime { get; set; }
        /// <summary>
        /// 是否是顶级任务
        /// </summary>
        public bool IsTopTask { get; set; }
        /// <summary>
        /// 父级任务Id
        /// </summary>
        public int ParentTaskId { get; set; }
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
        public int Status {get;set;}
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
        public IEnumerable<TaskTreeNode> ChildrenNodes { get; set; }
    }

    public class TaskListViewModel
    {
        public IEnumerable<EachTaskModel> TaskList { get; set; }
        /// <summary>
        /// 任务数量
        /// </summary>
        public int TaskCounts { get; set; }
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