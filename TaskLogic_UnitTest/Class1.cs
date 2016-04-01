using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using NUnit.Framework;

namespace TaskLogic_UnitTest
{
    [TestFixture]
    public class SimpleTaskLogic_UnitTest
    {
        private DBHelper.IDBHelper _dbHelper;

        private TaskLogic.ITaskLogic _taskLogic;

        [SetUp]
        protected void SetUp()
        {
            string conStr = "Data Source=.;Initial Catalog=JLYLY;User ID=sa;Password=5678lgysa";
            var loger = new TaskLoger.TaskLoger();
            // 
            _dbHelper = new DBHelper.SimpleDBHelper(DBHelper.DB_Type.SqlServer, conStr, loger);

            _taskLogic = new TaskLogic.SimpleTaskLogic(_dbHelper, loger, "K13");
        }

        [Test]
        public void Test_CreateMission()
        {
            var model = new TaskLogic.Mission() { TaskTitle = "Project Server", ParentTaskId = 0, Priority = 8, DueTime = DateTime.Now.AddDays(30), Creater = "Admin" };

            bool flag = _taskLogic.CreateMission(model);

            Assert.IsTrue(flag);
        }
    }
}
