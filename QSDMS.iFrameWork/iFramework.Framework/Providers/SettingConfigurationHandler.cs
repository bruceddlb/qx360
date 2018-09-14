using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace iFramework.Framework.Providers
{
    /// <summary>
    /// 处理对特定的配置节的访问
    /// </summary>
    public class SettingConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// 创建配置节处理程序
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            SettingConfiguration configuration = new SettingConfiguration();
            configuration.LoadFromConfiguration(section);
            return configuration;
        }
    }
}
