using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
//
using TaskLoger;

namespace DBHelper
{
    public class SimpleDBHelper : IDBHelper, IDisposable
    {
        private DB_Type _dbType;

        private IDbConnection _con;

        private readonly ITaskLoger _loger;

        public SimpleDBHelper(DB_Type dbType, string conStr, ITaskLoger loger)
        {
            _dbType = dbType;
            _loger = loger;
            // 
            switch (dbType)
            {
                case DB_Type.SqlServer:
                    _con = new SqlConnection(conStr);
                    break;
            }
            _con.Open();
        }

        public DB_Type GetDataBaseType()
        {
            return _dbType;
        }

        public int ExecuteSql(string sqlText)
        {
            int value = -1;
            // 
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    value = cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                value = -2;
                _loger.LogError("SimpleDBHelper.ExecuteSql(string)", "", ex);
            }

            return value;
        }

        public int ExecuteSql(string sqlText, params IDataParameter[] parameters)
        {
            int value = -1;
            // 
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }
                    // 
                    value = cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                value = -2;
                _loger.LogError("SimpleDBHelper.ExecuteSql(string, params IDataParameter[])", "", ex);
            }

            return value;
        }

        public T ExecuteScalar<T>(string sqlText)
        {
            T value = default(T);
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    var obj = cmd.ExecuteScalar();
                    value = (T)obj;
                }
            }
            catch (System.Exception ex)
            {
                _loger.LogError("SimpleDBHelper.ExecuteScalar<T>(string)", "", ex);
            }

            return value;
        }

        public T ExecuteScalar<T>(string sqlText, params IDataParameter[] parameters)
        {
            T value = default(T);
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    foreach(var p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }
                    var obj = cmd.ExecuteScalar();
                    value = (T)obj;
                }
            }
            catch (System.Exception ex)
            {
                _loger.LogError("SimpleDBHelper.ExecuteScalar<T>(string, params IDataParameter[])", "", ex);
            }

            return value;
        }

        public TFuncReturn ExecuteFunction<TFuncReturn>(string functionName, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteDataReader(string sqlText)
        {
            var cmd = _con.CreateCommand();
            cmd.CommandText = sqlText;

            return cmd.ExecuteReader();
        }

        public IDataReader ExecuteDataReader(string sqlText, params IDataParameter[] parameters)
        {
            var cmd = _con.CreateCommand();
            cmd.CommandText = sqlText;
            //
            foreach (var p in parameters)
            {
                cmd.Parameters.Add(p);
            }

            return cmd.ExecuteReader();
        }

        private IDataAdapter GetDataAdapter(IDbCommand cmd)
        {
            // 
            switch (_dbType)
            {
                case DB_Type.SqlServer:
                    return new SqlDataAdapter((SqlCommand)cmd);
                default:
                    return null;
            }
        }

        public DataTable QueryResult(string sqlText)
        {
            DataTable dt;
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    var adapter = GetDataAdapter(cmd);
                    //
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    // 
                    dt = ds.Tables[0];
                }
            }
            catch (System.Exception ex)
            {
                dt = null;
                _loger.LogError("SimpleDBHelper.QueryResult(string)", "", ex);
            }

            return dt;
        }

        public DataTable QueryResult(string sqlText, params IDataParameter[] parameters)
        {
            DataTable dt;
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.CommandText = sqlText;
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }
                    var adapter = GetDataAdapter(cmd);                    
                    //
                    var ds = new DataSet();
                    adapter.Fill(ds);
                    // 
                    dt = ds.Tables[0];
                }
            }
            catch (System.Exception ex)
            {
                dt = null;
                _loger.LogError("SimpleDBHelper.QueryResult(string)", "", ex);
            }

            return dt;
        }

        public int QueryResultCount(string sqlText)
        {
            return ExecuteScalar<int>(sqlText);
        }

        public int QueryResultCount(string sqlText, params IDataParameter[] parameters)
        {
            return ExecuteScalar<int>(sqlText, parameters);
        }

        public bool ExecuteCmdSqls(params string[] cmdSqls)
        {
            bool flag = true;
            int count = 0;
            var trans = _con.BeginTransaction();
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    foreach(string sql in cmdSqls)
                    {
                        cmd.CommandText = sql;
                        int result = cmd.ExecuteNonQuery();
                        if(result > 0)
                        {
                            count++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                count = -1;
                _loger.LogError("SimpleDBHelper.ExecuteCmdSqls(params string[])", "", ex);
            }
            finally
            {
                if (count == cmdSqls.Length)
                {
                    trans.Commit();
                }
                else
                {
                    flag = false;
                    trans.Rollback();
                }                
            }

            return flag;
        }

        public bool ExecuteCmdParamsSql(params CmdSqlStruct[] cmdSqls)
        {
            bool flag = true;
            int count = 0;
            var trans = _con.BeginTransaction();
            try
            {
                using (var cmd = _con.CreateCommand())
                {
                    cmd.Transaction = trans;
                    foreach (var cmdSql in cmdSqls)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = cmdSql.CmdSql;
                        if (cmdSql.Parameters != null)
                        {
                            // 
                            foreach (var p in cmdSql.Parameters)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        //
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                count = -1;
                _loger.LogError("SimpleDBHelper.ExecuteCmdParamsSql(params CmdSqlStruct[])", "", ex);
            }
            finally
            {
                if (count == cmdSqls.Length)
                {
                    trans.Commit();
                }
                else
                {
                    flag = false;
                    trans.Rollback();
                }
            }

            return flag;
        }

        public void Dispose()
        {
            if (_con.State == ConnectionState.Open)
            {
                _con.Close();
            }
            _con.Dispose();
        }
    }
}
