using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace iFramework.Framework.Providers
{
    /// <summary>
    /// 提供程序配置节点对象
    /// </summary>
    public class ProviderConfiguration
    {
        /// <summary>
        /// 默认提供程序名称
        /// </summary>
        public string DefaultProviderName { get; private set; }

        /// <summary>
        /// 提供程序集合
        /// </summary>
        public Dictionary<string, Provider> Providers { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ProviderConfiguration()
        {
            Providers = new Dictionary<string, Provider>();
        }

        /// <summary>
        /// 从配置文件中加载配置对象
        /// </summary>
        /// <param name="node"></param>
        internal void LoadFromConfiguration(XmlNode node)
        {
            XmlAttribute attribute = node.Attributes["defaultProvider"];

            if (attribute == null)
            {
                throw new ConfigurationErrorsException();
            }

            DefaultProviderName = attribute.Value;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "providers")
                {
                    LoadProviders(child);
                }
            }
        }

        /// <summary>
        /// 加载配置到提供程序集合
        /// </summary>
        /// <param name="node"></param>
        private void LoadProviders(XmlNode node)
        {
            foreach (XmlNode provider in node.ChildNodes)
            {
                switch (provider.Name)
                {
                    case "add":
                        Providers.Add(provider.Attributes["name"].Value, new Provider(provider.Attributes));
                        break;
                    case "remove":
                        Providers.Remove(provider.Attributes["name"].Value);
                        break;
                    case "clear":
                        Providers.Clear();
                        break;
                }
            }
        }

        /// <summary>
        /// 检索默认的配置节点
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static ProviderConfiguration GetProviderConfiguration(string providerName)
        {
            return (ProviderConfiguration)ConfigurationManager.GetSection("iFramework/" + providerName);
        }
    }
}
