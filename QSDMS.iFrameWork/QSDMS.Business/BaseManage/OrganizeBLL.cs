
using iFramework.Business;
using QSDMS.Cache.Factory;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Business
{
    /// <summary>
    /// 描 述：机构管理
    /// </summary>
    public class OrganizeBLL : BaseBLL<IOrganizeService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static OrganizeBLL m_Instance = new OrganizeBLL();

         /// <summary>
        /// 访问实例
        /// </summary>
        public static OrganizeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public OrganizeBLL() { }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "OrganizeCache";

        #region 获取数据
        /// <summary>
        /// 机构列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrganizeEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 机构实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OrganizeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 公司名称不能重复
        /// </summary>
        /// <param name="organizeName">公司名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            return InstanceDAL.ExistFullName(fullName, keyValue);
        }
        /// <summary>
        /// 外文名称不能重复
        /// </summary>
        /// <param name="enCode">外文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            return InstanceDAL.ExistEnCode(enCode, keyValue);
        }
        /// <summary>
        /// 中文名称不能重复
        /// </summary>
        /// <param name="shortName">中文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistShortName(string shortName, string keyValue)
        {
            return InstanceDAL.ExistShortName(shortName, keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除机构
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
        /// 保存机构表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="organizeEntity">机构实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, OrganizeEntity organizeEntity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, organizeEntity);
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
