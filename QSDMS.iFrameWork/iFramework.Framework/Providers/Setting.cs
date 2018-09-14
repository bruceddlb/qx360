using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace iFramework.Framework.Providers
{
    /// <summary>
    /// Setting对象
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 属性集合
        /// </summary>
        public NameValueCollection Attributes { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="attributes"></param>
        public Setting(XmlAttributeCollection attributes)
        {
            XmlAttribute nameAttribute = attributes["name"];

            if (nameAttribute == null)
            {
                throw new ConfigurationErrorsException();
            }

            this.Name = nameAttribute.Value;

            this.Attributes = new NameValueCollection();

            foreach (XmlAttribute attribute in attributes)
            {
                if (attribute.Name != "name")
                {
                    this.Attributes.Add(attribute.Name, attribute.Value);
                }
            }
        }
    }
}
