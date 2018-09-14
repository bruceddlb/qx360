using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFramework.Business
{
    /// <summary>
    /// 删除对象委托
    /// </summary>
    public delegate void DeletedEventHandler();

    /// <summary>
    /// 提供数据访问的业务逻辑类基类
    /// </summary>
    /// <typeparam name="T">数据适配接口类型</typeparam>
    public abstract class BaseBLL<T>
    {
        /// <summary>
        /// 创建提供访问实例的同步锁
        /// </summary>
        private object m_SyncRoot = new object();

        /// <summary>
        /// 提供访问实例入口私有变量
        /// </summary>
        private T m_InstanceDAL = default(T);

        /// <summary>
        /// 提供访问实例入口
        /// </summary>
        protected T InstanceDAL
        {
            get
            {
                if (m_InstanceDAL == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_InstanceDAL == null)
                        {
                            m_InstanceDAL = (T)ReflectCreater.CreateInstance(typeof(T));
                        }
                    }
                }
                return m_InstanceDAL;
            }
        }

        /// <summary>
        /// 删除对象事件
        /// </summary>
        protected event DeletedEventHandler EventDeleted;

        /// <summary>
        /// 调用删除对象事件
        /// </summary>
        internal void Deleted()
        {
            if (EventDeleted != null)
            {
                EventDeleted();
            }
        }
    }
}
