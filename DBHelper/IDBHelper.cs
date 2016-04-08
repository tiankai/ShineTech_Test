using System;
//
using System.Data;

namespace DBHelper
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DB_Type : uint
    {
        SqlServer = 0,

        Oracle = 1,

        MySql,

        Sqlite
    }

    public struct CmdSqlStruct
    {
        public string CmdSql { get; set; }

        public IDataParameter[] Parameters { get; set; }
    }

    public interface IDBHelper
    {
        /// <summary>
        /// 执行sql语句，并返回受影响的行数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <returns></returns>
        int ExecuteSql(string sqlText);
        /// <summary>
        /// 执行多个sql语句
        /// </summary>
        /// <param name="cmdSqls">[增加、删除、修改]sql命令</param>
        /// <returns></returns>
        bool ExecuteCmdSqls(params string[] cmdSqls);
        /// <summary>
        /// 执行sql语句，并返回受影响的行数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int ExecuteSql(string sqlText, params IDataParameter[] parameters);
        /// <summary>
        /// 执行多个sql语句
        /// </summary>
        /// <param name="cmdSqls">[增加、删除、修改]sql命令</param>
        /// <returns></returns>
        bool ExecuteCmdParamsSql(params CmdSqlStruct[] cmdSqls);
        /// <summary>
        /// 返回结果集的第一行第一列
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sqlText);
        /// <summary>
        /// 返回结果集的第一行第一列
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sqlText, params IDataParameter[] parameters);
        /// <summary>
        /// 执行存储函数
        /// </summary>
        /// <typeparam name="TFuncReturn">存储函数返回值</typeparam>
        /// <param name="functionName">存储函数名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        TFuncReturn ExecuteFunction<TFuncReturn>(string functionName, params IDataParameter[] parameters);
        /// <summary>
        ///  执行sql语句，并返回IDataReader
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string sqlText);
        /// <summary>
        /// 执行sql语句，并返回IDataReader
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        IDataReader ExecuteDataReader(string sqlText, params IDataParameter[] parameters);
        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        DataTable QueryResult(string sqlText);
        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        DataTable QueryResult(string sqlText, params IDataParameter[] parameters);
        /// <summary>
        /// 查询结果集数量
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <returns></returns>
        int QueryResultCount(string sqlText);
        /// <summary>
        /// 查询结果集数量
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int QueryResultCount(string sqlText, params IDataParameter[] parameters);
    }
}
