using iFramework.Framework;
using iFramework.Framework.Log;
using iFramework.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iFramework.Business
{
    /// <summary>
    /// 数据适配器提供程序
    /// </summary>
    internal static class DataProviderProvider
    {
        /// <summary>
        /// 默认数据适配器程序集
        /// </summary>
        private static Assembly m_Assembly = null;

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static DataProviderProvider()
        {
            string assembly = null;

            try
            {
                ProviderConfiguration configuration = ProviderConfiguration.GetProviderConfiguration("data");

                // 配置节点[data]不能为空
                if (configuration == null)
                {
                    throw new ConfigurationErrorsException();
                }

                // 配置节点[providers]不能为空
                if (configuration.Providers == null || configuration.Providers.Count == 0)
                {
                    throw new ConfigurationErrorsException();
                }

                // 配置属性[defaultProvider]不能为空
                if (string.IsNullOrEmpty(configuration.DefaultProviderName))
                {
                    throw new ConfigurationErrorsException();
                }

                Provider provider = configuration.Providers[configuration.DefaultProviderName];

                // 默认的[provider]不能为空
                if (provider == null)
                {
                    throw new ConfigurationErrorsException();
                }

                assembly = provider.Attributes["assembly"];

                // 配置属性[assembly]不能为空
                if (string.IsNullOrEmpty(assembly))
                {
                    throw new ConfigurationErrorsException();
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = string.Format("数据适配器配置文件错误:{0}", ex.Message);              
                new ExceptionHelper().LogException(ex);
            }

            try
            {
                // 加载数据适配器程序集
                m_Assembly = Assembly.Load(assembly);
            }
            catch (Exception ex)
            {                
                ex.Data["Method"] = string.Format("程序集加载失败。{0}。", ex.Message);
                new ExceptionHelper().LogException(ex);
            }
        }

        /// <summary>
        /// 默认数据适配器程序集
        /// </summary>
        public static Assembly Assembly
        {
            get { return m_Assembly; }
        }
    }
}
