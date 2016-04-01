using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using NUnit.Framework;

namespace DBHelper_UnitTest
{
    [TestFixture]
    public class SimpleDBHelper_UnitTest
    {
        private DBHelper.IDBHelper _dbHelper;
 
        [SetUp]
        protected void SetUp()
        {
            string conStr = "Data Source=.;Initial Catalog=JLYLY;User ID=sa;Password=5678lgysa";
            var loger = new TaskLoger.TaskLoger();

            _dbHelper = new DBHelper.SimpleDBHelper(DBHelper.DB_Type.SqlServer, conStr, loger);
        }

        [Test]
        public void Test_ExecuteSql()
        {
            const int expectedValue = 2;

            string sql = "update skjlb set skbz = 2 where skbh = 1083730";

            int result = _dbHelper.ExecuteSql(sql);

            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void Test_QueryResult()
        {
            const int showCount = 10;
            string sql = string.Format("select top {0} * from skjlb where skbh > 1083730", showCount);

            var dt = _dbHelper.QueryResult(sql);

            Assert.IsNotNull(dt);

            Assert.AreEqual(showCount, dt.Rows.Count);
        }

        [Test]
        public void Test_ExecuteDataReader()
        {
            int state = 4;

            string sqlEdit = string.Format("update skjlb set skbz = {0} where skbh = 1083730", state);

            _dbHelper.ExecuteSql(sqlEdit);

            string sqlQuery = "select skbz from skjlb where skbh = 1083730";

            int value = 0;

            using (var dr = _dbHelper.ExecuteDataReader(sqlQuery))
            {
                while (dr.Read())
                {
                    value = dr.GetInt32(0);
                }
            }

            Assert.AreEqual(state, value);
        }

    }
}
