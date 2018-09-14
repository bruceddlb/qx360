
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
    /// 描 述：用户角色管理
    /// </summary>
    public class UserRoleBLL:BaseBLL<IUserRoleService>
    {
       
         /// <summary>
        /// 访问实例
        /// </summary>
        public static UserRoleBLL m_Instance = new UserRoleBLL();

         /// <summary>
        /// 访问实例
        /// </summary>
        public static UserRoleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserRoleBLL() { }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "UserRoleCache";

        #region 获取数据
        /// <summary>
        /// 职位列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserRoleEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        public IEnumerable<UserRoleEntity> GetRoleList(string userid)
        {
            return InstanceDAL.GetRoleList(userid);
        }
        /// <summary>
        /// 职位列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserRoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return InstanceDAL.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 职位实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserRoleEntity GetEntity(string keyValue)
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
        /// 保存职位表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="jobEntity">职位实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, UserRoleEntity userRoleEntity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, userRoleEntity);
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
