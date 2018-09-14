using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace iFramework.Framework.Providers
{
    /// <summary>
    /// 提供程序接口
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// 设置配置属性
        /// </summary>
        void SetAttributes(NameValueCollection attributes);

        /// <summary>
        /// 启动提供程序
        /// </summary>
        void Start();
    }
}
