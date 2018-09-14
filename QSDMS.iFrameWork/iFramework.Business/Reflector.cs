using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using iFramework.Framework.Log;
using iFramework.Framework;

namespace iFramework.Business
{
    /// <summary>
    /// 利用反射动态的创建对象
    /// </summary>
    public static class ReflectCreater
    {
     
        /// <summary>
        /// 根据接口创建对象实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            // 获取数据适配器程序集
            Assembly assembly = DataProviderProvider.Assembly;

            // 构造该类型对应实现的数据接口类型全名称
            string dll = assembly.ManifestModule.Name;
            string spacename = dll.Substring(0, dll.LastIndexOf('.'));
            string name = GetImplementedFullName(spacename, type.Name);

            try
            {
                // 创建实现的数据接口类型的实例
                object obj = assembly.CreateInstance(name);
                return obj;
            }
            catch (Exception ex)
            {
              
                //Log _logger = LogFactory.GetLogger("");
                //_logger.Warn(new ExceptionHelper().LogException(ex));
                ex.Data["Method"] = string.Format("查找类似程序集创建实例失败。{0}不存在{1}。 的本地化字符串。", spacename, name);
                new ExceptionHelper().LogException(ex);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取实现类的全名称
        /// </summary>
        /// <param name="namespaceName">实现类的默认命名空间</param>
        /// <param name="interfaceName">接口类名称</param>
        /// <returns>实现类的类型全名称</returns>
        private static string GetImplementedFullName(string namespaceName, string interfaceName)
        {
            // 去除接口中第一个字母[I]
            return namespaceName + "." + interfaceName.Substring(1);
        }
    }
}
