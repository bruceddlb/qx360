using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFramework.Factory
{
    /// <summary>
    /// 数据适配器基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDataAdapter<T>
    {
        /// <summary>
        /// 创建提供访问实例的同步锁
        /// </summary>
        private object m_SyncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        static T m_provider = default(T);

        /// <summary>
        /// 提供访问实例入口
        /// </summary>
        public T InstanceDAL
        {
            get
            {
                if (m_provider == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_provider == null)
                        {
                            m_provider = DataAdapterReflector.CreateInstance<T>();
                        }
                    }
                }
                return m_provider;
            }
        }
    }
}
