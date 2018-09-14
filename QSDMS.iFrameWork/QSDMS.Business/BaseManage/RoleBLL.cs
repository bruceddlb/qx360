
using iFramework.Business;
using QSDMS.Cache.Factory;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Business
{
    /// <summary> 
    /// 描 述：角色管理
    /// </summary>
    public class RoleBLL:BaseBLL<IRoleService>
    {
       
        /// <summary>
        /// 访问实例
        /// </summary>
        public static RoleBLL m_Instance = new RoleBLL();

         /// <summary>
        /// 访问实例
        /// </summary>
        public static RoleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public RoleBLL() { }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "RoleCache";

        #region 获取数据
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return InstanceDAL.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 角色列表all
        /// </summary>
        /// <returns></returns>
        public List<RoleEntity> GetAllList()
        {
            return InstanceDAL.GetAllList().ToList();
        }
        /// <summary>
        /// 角色实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public RoleEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 角色编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            return InstanceDAL.ExistEnCode(enCode, keyValue);
        }
        /// <summary>
        /// 角色名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            return InstanceDAL.ExistFullName(fullName, keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
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
        /// 保存角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, RoleEntity roleEntity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, roleEntity);
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
