
using iFramework.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{
    public class BaseBLL<T> : BaseDataAdapter<T>
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
