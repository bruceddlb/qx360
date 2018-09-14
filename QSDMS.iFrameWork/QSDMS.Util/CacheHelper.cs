using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace QSDMS.Util
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class CacheHelper
    {
        private Cache m_Cache = System.Web.HttpRuntime.Cache;

        private CacheHelper()
        {

        }

        private static CacheHelper m_Instance;
        private static object syncObject = new object();

        /// <summary>
        /// 单例
        /// </summary>
        public static CacheHelper Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (syncObject)
                    {
                        m_Instance = new CacheHelper();
                    }
                }

                return m_Instance;
            }
        }

        #region 查询cache
        /// <summary>
        /// 查询cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return Instance.m_Cache.Get(key);
        }

        /// <summary>
        /// 查询cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)Convert.ChangeType(Instance.m_Cache.Get(key), typeof(T));
        }
        #endregion

        #region 查询cache
        /// <summary>
        /// 查询cache
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key当中包含的关键字</param>
        /// <returns></returns>
        public IList<T> Query<T>(string key) where T : class
        {
            IList<T> result = new List<T>();
            IDictionaryEnumerator dict = Instance.m_Cache.GetEnumerator();
            dict.Reset();
            while (dict.MoveNext())
            {
                if (Convert.ToString(dict.Key).Contains(key))
                {
                    T temp = dict.Value as T;
                    if (temp != null)
                    {
                        result.Add(temp);
                    }
                }
            }

            return result;
        }
        #endregion

        #region 添加cache
        /// <summary>
        /// 添加cache
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">缓存的分钟数</param>
        public void Add(string key, object value, int cacheTime = 0)
        {
            if (value != null)
            {
                if (cacheTime <= 0)
                {
                    Instance.m_Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
                }
                else
                {
                    Instance.m_Cache.Add(key, value, null, DateTime.Now.AddMinutes(cacheTime), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, null);
                }
            }
        }
        #endregion

        #region 更新cache
        /// <summary>
        /// 更新cache
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="cacheTime">缓存的分钟数</param>
        public void Update(string key, object value, int cacheTime = 0)
        {
            if (value != null)
            {
                Remove(key);
                Add(key, value, cacheTime);
            }
        }
        #endregion

        #region 移除cache
        /// <summary>
        /// 添加cache
        /// </summary>
        /// <param name="key">键</param>
        public void Remove(string key)
        {

            Instance.m_Cache.Remove(key);
        }
        #endregion

        #region 清理cache
        /// <summary>
        /// 清理cache
        /// </summary>
        public void Clear()
        {
            var CacheList = Instance.m_Cache.GetEnumerator();
            while (CacheList.MoveNext())
            {
                Remove(CacheList.Entry.Key.ToString());
            }
        }

        /// <summary>
        /// 获取当前缓存的所有keys
        /// </summary>
        /// <returns></returns>
        public IList<string> Keys()
        {
            IList<string> keys = new List<string>();
            var CacheList = Instance.m_Cache.GetEnumerator();
            while (CacheList.MoveNext())
            {
                keys.Add(CacheList.Entry.Key.ToString());
            }
            return keys;
        }

        /// <summary>
        /// 清理关键字相关的cache
        /// </summary>
        /// <param name="key">关键字</param>
        public void Clear(string key)
        {
            var CacheList = Instance.m_Cache.GetEnumerator();
            while (CacheList.MoveNext())
            {
                if (CacheList.Entry.Key.ToString().Contains(string.Format("{0}", key)))
                {
                    Remove(CacheList.Entry.Key.ToString());
                }
            }
        }
        #endregion

        #region 当前缓存的数量
        /// <summary>
        /// 当前缓存的数量
        /// </summary>
        public int Count
        {
            get
            {
                return Instance.m_Cache.Count;
            }
        }
        #endregion
    }
}