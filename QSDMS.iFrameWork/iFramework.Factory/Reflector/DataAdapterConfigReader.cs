using iFramework.Framework.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iFramework.Factory
{
    /// <summary>
    /// 数据适配器配置读取.
    /// </summary>
    internal class DataAdapterConfigReader
    {
        private static Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }

        const string DataAdapterDirName = "";//"NOASDataAdapter";
        private static string ConfigFileName =System.Configuration.ConfigurationManager.AppSettings["BusinissDataAdapterConfig"].ToString();// 业务系统数据适配文件配置;
        const string DefaultAdapterNodeName = "DefaultDataAdapter";
        const string FilePathAttributeName = "File";
        const string AdapterSetsNodeName = "Adapters";
        const string AdapterNodeName = "Adapter";


        /// <summary>
        /// 获取默认适配器文件路径.
        /// </summary> 
        /// <exception cref="FileNotFoundException">无法找到配置文件,或者配置文件中配置的DefaultDataAdapter对应的文件无法找到.</exception>
        /// <returns></returns>
        public static string GetDefualtDataAdapterFilePath()
        {
            string configFilePath = GetAdapterConfigFilePath();

            XElement root = XElement.Load(configFilePath);
            XElement defaultAdapter = root.Element(DefaultAdapterNodeName);
            if (defaultAdapter != null)
            {
                XAttribute atr = defaultAdapter.Attribute(FilePathAttributeName);
                if (atr != null)
                {
                    return GetTargetFilePath(atr.Value);
                }
            }

            string meg = string.Format("数据适配配置文件:{0},中没有配置DefaultDataAdapter,或者配置错误。/r/n配置文件路径为:{1}", ConfigFileName, configFilePath);
            _logger.Error(meg);
            throw new ConfigurationErrorsException(meg);
        }

        /// <summary>
        /// 获取指定的接口配置的适配器.
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public static string GetAdapterFilePath(string interfaceName)
        {
            string configFilePath = GetAdapterConfigFilePath();

            XElement root = XElement.Load(configFilePath);

            IEnumerable<XElement> teles =
                from el in root.Element(AdapterSetsNodeName).Elements(AdapterNodeName)
                where (string)el.Attribute("Interface") == interfaceName
                select el;

            foreach (XElement ele in teles)
            {
                XAttribute atr = ele.Attribute("File");
                if (atr != null)
                {
                    return GetTargetFilePath(atr.Value);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取配置文件中的所有适配器信息.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAdapterFiles()
        {
            List<string> lstRes = new List<string>() { GetDefualtDataAdapterFilePath() };
            string configFilePath = GetAdapterConfigFilePath();

            XElement root = XElement.Load(configFilePath);

            IEnumerable<XElement> teles =
                from el in root.Element(AdapterSetsNodeName).Elements(AdapterNodeName)
                select el;

            foreach (XElement ele in teles)
            {
                XAttribute atr = ele.Attribute("File");
                if (atr != null)
                {
                    string path = GetTargetFilePath(atr.Value);

                    lstRes.Add(path);
                }
            }

            return lstRes;
        }


        /// <summary>
        /// 获取当前接口是否包括特殊配置
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public static string FindSpecialAdapterType(string interfaceName)
        {
            string SpecialAdapterType = "";
            //SpecialAdapterType
            string configFilePath = GetAdapterConfigFilePath();

            XElement root = XElement.Load(configFilePath);

            IEnumerable<XElement> teles =
                from el in root.Element(AdapterSetsNodeName).Elements(AdapterNodeName)
                select el;

            foreach (XElement ele in teles)
            {
                XAttribute atr = ele.Attribute("Interface");
                if (atr != null)
                {
                    if (atr.Value.Equals(interfaceName, StringComparison.OrdinalIgnoreCase))
                    {
                        XAttribute special = ele.Attribute("SpecialAdapterType");
                        if (special != null)
                        {
                            SpecialAdapterType = special.Value;
                        }
                        continue;
                    }
                }
            }

            return SpecialAdapterType;
        }

        /// <summary>
        /// 获取配置文件的路径.
        /// </summary>
        /// <returns></returns>
        private static string GetAdapterConfigFilePath()
        {
            return GetTargetFilePath(ConfigFileName);
        }

        /// <summary>
        /// 获取适配器文件的文件路径.
        /// </summary>
        /// <returns></returns>
        private static string GetTargetFilePath(string fileName)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string configFilePath = string.Format("{0}{1}\\{2}", basePath, DataAdapterDirName, fileName);

            if (!File.Exists(configFilePath))
            {
                // 适应Web程序的程序集目录
                configFilePath = string.Format("{0}\\Bin{1}\\{2}", basePath, DataAdapterDirName, fileName);

                if (!File.Exists(configFilePath))
                {
                    string message = string.Format("无法在程序目录下找到指定文件:{0}。/r/n对应文件路径为:{1}", ConfigFileName, configFilePath);
                    _logger.Error(message);
                    throw new FileNotFoundException(message);
                }
            }

            // 重新构造路径.
            return FormatPath(configFilePath);
        }

        /// <summary>
        /// 格式化路径, 将路径字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string FormatPath(string path)
        {
            return Path.GetFullPath(path);
        }
    }

    /// <summary>
    /// XML配置信息
    /// </summary>
    internal class AdapterItem
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Interface { get; set; }

        /// <summary>
        /// DLL地址
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 特殊规则实现的类
        /// </summary>
        public string SpecialType { get; set; }
    }
}
