
using iFramework.Business;
using QSDMS.Cache.Factory;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary> 
    /// 描 述：数据授权管理
    /// </summary>
    public class UserAuthorizeBLL : BaseBLL<IUserAuthorizeService>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static UserAuthorizeBLL m_Instance = new UserAuthorizeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static UserAuthorizeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserAuthorizeBLL() { }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "UserAuthorizeCache";

        #region 获取数据
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserAuthorizeEntity> GetList()
        {
            return InstanceDAL.GetList();
        }

        /// <summary>
        /// 根据用户编号获取对应查询数据权限对象
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<string> GetUserAuthorizeListStr(string userid)
        {
            List<string> newlist = null;
            var list = InstanceDAL.GetUserAuthorizeList(userid);
            if (list != null)
            {
                newlist = new List<string>();
                foreach (var item in list)
                {
                    newlist.Add(item.ObjectId);
                }
            }
            return newlist;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IEnumerable<UserAuthorizeEntity> GetUserAuthorizeList(string userid)
        {
            return InstanceDAL.GetUserAuthorizeList(userid);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserAuthorizeEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return InstanceDAL.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 职位实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserAuthorizeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除职位
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                InstanceDAL.RemoveForm(keyValue);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, UserAuthorizeEntity entity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, entity);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
