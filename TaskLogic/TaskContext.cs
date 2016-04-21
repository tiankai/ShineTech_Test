
namespace TaskLogic
{
    using System.Data.Entity;
    using System.Data.Common;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TaskContainer : DbContext
    {
        private string _tableStr;

        public TaskContainer(DbConnection con, string tableStr)
            : base(con, true)
        {
            _tableStr = tableStr;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (string.IsNullOrEmpty(_tableStr) == false)
            {
                // User
                modelBuilder.Entity<UserInfo>().ToTable(string.Format("{0}_UserInfo", _tableStr));
                modelBuilder.Entity<UserInfo>().HasKey(p => p.UserId);
                modelBuilder.Entity<UserInfo>().Property(c => c.UserId)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnName("uid");
                modelBuilder.Entity<UserInfo>().Property(c => c.UserName).HasColumnName("uname");
                modelBuilder.Entity<UserInfo>().Property(c => c.UserPass).HasColumnName("upass");
                modelBuilder.Entity<UserInfo>().Property(c => c.Status).HasColumnName("ustatus");
                modelBuilder.Entity<UserInfo>().Property(c => c.Memo).HasColumnName("memo");
                modelBuilder.Entity<UserInfo>().Property(c => c.LastLoginTime).HasColumnName("lastLoginTime");
                modelBuilder.Entity<UserInfo>().Property(c => c.LastLoginIp).HasColumnName("lastLoginIp");
                modelBuilder.Entity<UserInfo>().Property(c => c.IsWorking).HasColumnName("workstack");
                // Task
                modelBuilder.Entity<Mission>().ToTable(string.Format("{0}_TaskInfo", _tableStr));
                modelBuilder.Entity<Mission>().HasKey(p => p.TaskId);
                modelBuilder.Entity<Mission>().Property(c => c.TaskId)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None).HasColumnName("tid");
                modelBuilder.Entity<Mission>().Property(c => c.TaskTitle).HasColumnName("taskname");
                modelBuilder.Entity<Mission>().Property(c => c.TaskMemo).HasColumnName("taskDescription");
                modelBuilder.Entity<Mission>().Property(c => c.ParentTaskId).HasColumnName("ParentTask");
                modelBuilder.Entity<Mission>().Property(c => c.CreaterId).HasColumnName("Creater");
                modelBuilder.Entity<Mission>().Property(c => c.ExecuterId).HasColumnName("Executer");
                // 
                modelBuilder.Entity<Mission>().Ignore(c => c.UserActions).Ignore(c => c.Creater)
                    .Ignore(c => c.Executer).Ignore(c => c.ParentTask);
                // TaskFlow
                modelBuilder.Entity<TaskFlow>().ToTable(string.Format("{0}_TaskFlow", _tableStr));
                modelBuilder.Entity<TaskFlow>().HasKey(p => p.FlowId);
                modelBuilder.Entity<TaskFlow>().Property(c => c.FlowId)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity).HasColumnName("sid");
                modelBuilder.Entity<TaskFlow>().Property(c => c.TaskId).HasColumnName("tid");
                modelBuilder.Entity<TaskFlow>().Property(c => c.HappendTime).HasColumnName("worktime");
                modelBuilder.Entity<TaskFlow>().Property(c => c.OperatorId).HasColumnName("executer");
                // 
                modelBuilder.Entity<TaskFlow>().Ignore(c => c.Operator).Ignore(c => c.TaskInfo);
            }

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserInfo> AllUsers { get; set; }

        public DbSet<Mission> AllMissions { get; set; }

        public DbSet<TaskFlow> WorkLogs { get; set; }
    }
}
