using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using NUnit.Framework;
using System.Data.SqlClient;

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

            var result = _taskLogic.CreateMission(model);

            Assert.IsTrue(result.result == 0);
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

            var res = _taskLogic.EditMissionProperty(model);

            Assert.IsTrue(res.result == 0);
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

    public class EF_UserLogic_UnitTest
    {
        private TaskLogic.IUser _userLogic;

        [SetUp]
        protected void SetUp()
        {
            string conStr = "Data Source=.;Initial Catalog=ShineTech_Test;User ID=sa;Password=sa";
            _userLogic = new TaskLogic.EF_UserLogic(conStr, "K");
        }

        [Test]
        public void Test_UserTest()
        {
            var model = _userLogic.UserLogin("Knight", "");

            Assert.AreEqual(1, model.UserId);
        }

        [Test]
        public void Test_RegisterUser()
        {
            // Memo 数据库中有长度限制 若是超长了 报错 "string or binary data would be truncated"
            var model = new TaskLogic.UserInfo() { UserName = "Kislter", UserPass = "S6A6F54C4C4S4D412ASD", Memo = "kitty" };
            bool flag = _userLogic.RegisterUser(model);

            Assert.IsTrue(flag);
        }

        [Test]
        public void Test_ModifyUserPass()
        {
            bool flag = _userLogic.ModifyUserPass("Knight", "", "abc");

            Assert.IsTrue(flag);
        }

        [Test]
        public void Test_RemoveUser()
        {
            bool flag = _userLogic.RemoveUser("Knight");

            Assert.IsTrue(flag);
        }
    }

    [TestFixture]
    public class EF_TaskLogic_UnitTest
    {
        private TaskLogic.ITaskLogic _taskLogic;

        [SetUp]
        protected void SetUp()
        {
            string conStr = "Data Source=.;Initial Catalog=ShineTech_Test;User ID=sa;Password=sa";
            var loger = new TaskLoger.TaskLoger();
            // 
            _taskLogic = new TaskLogic.EF_TaskLogic(4, conStr, "K");// .SimpleTaskLogic(_dbHelper, loger, "K");
        }
               

        [Test]
        public void Test_CreateMission()
        {
            var model = new TaskLogic.NewMission() { TaskTitle = "Isolation Life", ParentTaskId = 0, Priority = 2, DueTime = DateTime.Now.AddDays(30), Creater = 6, TaskMemo = "to save life of yourself" };

            var res = _taskLogic.CreateMission(model);

            Assert.IsTrue(res.result == 0);
        }

        [Test]
        public void Test_EditMission()
        {
            const int taskId = 3;
            var model = new TaskLogic.EditMission(taskId);
            model.TaskTitle = "Saved SunStar";
            model.TaskMemo = "Saving Stars";
            model.Priority = 4;
            model.DueTime = DateTime.Now.AddMonths(1);

            var res = _taskLogic.EditMissionProperty(model);

            Console.WriteLine("errCode = {0}, Message = {1}", res.result, res.message);
            Assert.IsTrue(res.result == 0);
        }

        [Test]
        public void Test_AssignMission()
        {
            const int uid = 4;
            const int taskId = 2;
            const int expectedValue = 0;

            var result = _taskLogic.AssignMission(taskId, uid);

            Console.WriteLine("errCode = {0}, message = {1}", result.result, result.message);
            Assert.AreEqual(expectedValue, result.result);
        }

        [Test]
        public void Test_AcceptMission()
        {
            const int taskId = 2;
            const int expectedValue = 0;

            var result = _taskLogic.AcceptMission(taskId);

            Console.WriteLine("errCode = {0}, message = {1}", result.result, result.message);
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
