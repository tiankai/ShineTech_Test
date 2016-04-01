using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLoger
{
    public interface ITaskLoger
    {
        void LogMsg(string where, string memo);

        void LogDebug(string where, string memo);

        void LogWarn(string where, string memo);

        void LogError(string where, string memo, Exception ex = null);
    }

    public class TaskLoger : ITaskLoger, IDisposable
    {
        private log4net.ILog _loger;

        public TaskLoger()
        {
            _loger = log4net.LogManager.GetLogger("TaskLoger");
        }

        public void LogMsg(string where, string memo)
        {
            _loger.Info(string.Format("{0}, {1}", where, memo));
        }

        public void LogDebug(string where, string memo)
        {
            _loger.Debug(string.Format("{0}, {1}", where, memo));
        }

        public void LogWarn(string where, string memo)
        {
            _loger.Warn(string.Format("{0}, {1}", where, memo));
        }

        public void LogError(string where, string memo, Exception ex = null)
        {
            _loger.Error(string.Format("{0}, {1}", where, memo));
        }

        public void Dispose()
        {
            log4net.LogManager.Shutdown();
        }
    }
}
