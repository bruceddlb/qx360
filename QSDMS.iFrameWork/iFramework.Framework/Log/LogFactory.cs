using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Web;
namespace iFramework.Framework.Log
{
    /// <summary>
    /// 日志初始化
    /// </summary>
    public class LogFactory
    {
        static LogFactory()
        {
            string configPath = "/Resources/Config/log4net.config";
            if (ConfigurationManager.AppSettings["log4netpath"] != null)
            {
                configPath = ConfigurationManager.AppSettings["log4netpath"].ToString();
            }
            string filename = "";
            if (System.Web.HttpContext.Current != null)
            {
                filename = HttpContext.Current.Server.MapPath(configPath);
            }
            else
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "/" + configPath;
            }
            FileInfo configFile = new FileInfo(filename);
            log4net.Config.XmlConfigurator.Configure(configFile);
        }
        public static Log GetLogger(Type type)
        {
            return new Log(LogManager.GetLogger(type));
        }
        public static Log GetLogger(string str)
        {
            return new Log(LogManager.GetLogger(str));
        }

    }
}
