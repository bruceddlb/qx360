using log4net;
using System;
namespace iFramework.Framework.Log
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {     
        
        /// <summary>
        /// 日志操作
        /// </summary>

        private ILog logger;      
        public Log(ILog log)
        {
            this.logger = log;
        }
        public void Debug(object message)
        {
            this.logger.Debug("\r\n" + message);
        }
        public void Error(object message)
        {
            this.logger.Error("\r\n" + message);
        }
        public void Info(object message)
        {
            this.logger.Info("\r\n" + message);
        }
        public void Warn(object message)
        {
            this.logger.Warn("\r\n" + message);
        }
    }
}
