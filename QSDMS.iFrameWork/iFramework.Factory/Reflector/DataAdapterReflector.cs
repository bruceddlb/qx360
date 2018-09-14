using iFramework.Framework.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iFramework.Factory
{ /// <summary>
    /// 数据适配器反射辅助类.
    /// </summary>
    public class DataAdapterReflector
    {
        private static Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }
        private class AdapterClassTypeInfo
        {
            public string FilePath { get; set; }
            public string InterfaceName { get; set; }
            public Type Type { get; set; }

            /// <summary>
            /// 特殊实现
            /// </summary>
            public string SpecialType { get; set; }
        }

        private static Assembly _defaultAssembly = null;
        private static List<AdapterClassTypeInfo> _lstClassTypeInfo = new List<AdapterClassTypeInfo>();
        private static Dictionary<string, Assembly> _dicInterfaceAssembly = new Dictionary<string, Assembly>();
        private static AdapterAutoUpdater _updater = null;
        private static Object _asycObj = new object();

        static DataAdapterReflector()
        {
            string targetPath = DataAdapterConfigReader.GetDefualtDataAdapterFilePath();

            _defaultAssembly = LoadAssembly(targetPath);

            _updater = new AdapterAutoUpdater();
        }

        /// <summary>
        /// 更新指令路径的程序集.
        /// </summary>
        /// <param name="file"></param>
        public static void RefreshAssembly(string file)
        {
            lock (_asycObj)
            {
                if (_dicInterfaceAssembly.ContainsKey(file))
                {
                    _dicInterfaceAssembly.Remove(file);
                    _lstClassTypeInfo.RemoveAll(c => c.FilePath == file);
                }
            }
        }

        /// <summary>
        /// 创建指定类型接口的实现类对象.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            Type interfaceType = typeof(T);

            string interfaceName = interfaceType.FullName;

            Type tg = null;

            lock (_asycObj)
            {
                var typeInfo = _lstClassTypeInfo.FirstOrDefault(c => c.InterfaceName == interfaceName);
                if (typeInfo != null)
                {
                    tg = typeInfo.Type;
                }
                else
                {
                    string targetFileName = DataAdapterConfigReader.GetAdapterFilePath(interfaceName);

                    Assembly asm = null;

                    if (targetFileName != null)
                    {
                        if (_dicInterfaceAssembly.ContainsKey(targetFileName))
                        {
                            asm = _dicInterfaceAssembly[targetFileName];
                        }
                        else
                        {
                            if (!File.Exists(targetFileName))
                            {
                                string message = string.Format("无法找到文件:{0}。", targetFileName);
                                _logger.Error(message);
                                throw new Exception(message);
                            }

                            asm = LoadAssembly(targetFileName);
                            _dicInterfaceAssembly[targetFileName] = asm;
                        }
                    }
                    else
                    {
                        asm = _defaultAssembly;
                    }
                    string specialAdapter = DataAdapterConfigReader.FindSpecialAdapterType(interfaceName);
                    tg = FindImplementType(asm, interfaceType, tg, specialAdapter);

                    _lstClassTypeInfo.Add(
                        new AdapterClassTypeInfo()
                        {
                            FilePath = targetFileName,
                            InterfaceName = interfaceName,
                            Type = tg
                        });
                }
            }

            if (tg != null)
            {
                return (T)Activator.CreateInstance(tg);
            }
            else
            {
                string message = string.Format("无法找到接口:{0} 的适配器", interfaceName);
                _logger.Error(message);
                throw new Exception(message);
            }
        }



        private static Assembly LoadAssembly(string targetPath)
        {
            try
            {
                Assembly asm = Assembly.LoadFile(targetPath);
                return asm;
            }
            catch (Exception ex)
            {
                string message = string.Format("数据适配反射失败{0}{1}。", Environment.NewLine, ex.Message);
                _logger.Error(message);
                throw new Exception(message);
            }
        }

        private static Type FindImplementType(Assembly asm, Type targetType, Type tg, string specialAdapterType)
        {
            if (!string.IsNullOrEmpty(specialAdapterType))
            {
                tg = asm.GetType(specialAdapterType, false);
            }
            if (tg == null)
            {
                foreach (Type tp in asm.GetExportedTypes())
                {
                    if (tp.IsClass && targetType.IsAssignableFrom(tp))
                    {
                        tg = tp;
                    }
                }
            }
            return tg;
        }
    }
}
