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
            string conStr = "Data Source=.;Initial Catalog=ShineTech_Test;User ID=sa;Password=sa";
            var loger = new TaskLoger.TaskLoger();
            // 
            _dbHelper = new DBHelper.SimpleDBHelper(DBHelper.DB_Type.SqlServer, conStr, loger);

            _taskLogic = new TaskLogic.SimpleTaskLogic(_dbHelper, loger, "K");
        }
        
        [Test]
        public void Test_CreateMission()
        {
            var model = new TaskLogic.NewMission() { TaskTitle = "Hello HG star", ParentTaskId = 0, Priority = 3, DueTime = DateTime.Now.AddDays(30), Creater = 1, TaskMemo = "" };

            bool flag = _taskLogic.CreateMission(model);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Test_EditMission()
        {
            const int taskId = 2;
            var model = new TaskLogic.EditMission(taskId);
            model.TaskTitle = "Saving Stars";
            model.TaskMemo = "";
            model.Priority = 4;
            model.DueTime = DateTime.Now.AddMonths(1);

            bool flag = _taskLogic.EditMissionProperty(model);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Test_AssignMission()
        {
            const int uid = 1;
            const int taskId = 2;
            const int expectedValue = 0;

            var result = _taskLogic.AssignMission(taskId, uid);

            Console.WriteLine("result = {0}, message = {1}", result.result, result.message);
            Assert.AreEqual(expectedValue, result.result);
        }
        
        [Test]
        public void Test_QueryMissionList()
        {
            const int uid = 1;
            const int pageIndex = 1;
            const int pageSize = 5;

            const int expectedCount = 4;

            var queryResult = _taskLogic.QueryUserMission(uid, pageIndex, pageSize);

            Assert.AreEqual(expectedCount, queryResult.Count());
        }

        [Test]
        public void Test_QueryMissionCount()
        {
            const int uid = 1;
            const int expectedCount = 3;

            int count = _taskLogic.QueryUserMissionCount(uid);

            Assert.AreEqual(expectedCount, count);
        }

        [Test]
        public void Test_CheckTaskName()
        {
            const string name = "Saving Stars!";
            const bool expectedValue = false;

            bool flag = _taskLogic.MissionNameIsExists(name);

            Assert.AreEqual(expectedValue, flag);
        }
    }
}
