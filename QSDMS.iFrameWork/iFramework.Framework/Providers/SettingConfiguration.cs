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
    /// 配置节点对象
    /// </summary>
    public class SettingConfiguration
    {
        /// <summary>
        /// 配置集合
        /// </summary>
        public Dictionary<string, Setting> Settings { get; private set; }

        /// <summary>
        /// 配置属性集合
        /// </summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public SettingConfiguration()
        {
            Settings = new Dictionary<string, Setting>();
            Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// 从配置文件中加载配置对象
        /// </summary>
        /// <param name="node"></param>
        internal void LoadFromConfiguration(XmlNode node)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    Attributes.Add(attribute.Name, attribute.Value);
                }
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "add":
                        Settings.Add(child.Attributes["name"].Value, new Setting(child.Attributes));
                        break;
                    case "remove":
                        Settings.Remove(child.Attributes["name"].Value);
                        break;
                    case "clear":
                        Settings.Clear();
                        break;
                }
            }
        }

        /// <summary>
        /// 检索配置节点
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static SettingConfiguration GetSettingConfiguration(string settingName)
        {
            return (SettingConfiguration)ConfigurationManager.GetSection("iFramework/" + settingName);
        }
    }
}
