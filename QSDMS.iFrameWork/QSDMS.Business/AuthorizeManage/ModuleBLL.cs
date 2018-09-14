
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Business
{
    /// <summary>  
    /// 描 述：系统功能
    /// </summary>
    public class ModuleBLL : BaseBLL<IModuleService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleBLL m_Instance = new ModuleBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModuleBLL() { }

        #region 获取数据
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <returns></returns>
        public int GetSortCode()
        {
            return InstanceDAL.GetSortCode();
        }
        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="parentId">父级主键</param>
        /// <returns></returns>
        public List<ModuleEntity> GetList(string parentId = "")
        {
            var data = InstanceDAL.GetList().ToList();
            if (!string.IsNullOrEmpty(parentId))
            {
                data = data.FindAll(t => t.ParentId == parentId);
            }
            return data;
        }
        /// <summary>
        /// 获取功能实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 功能编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            return InstanceDAL.ExistEnCode(enCode, keyValue);
        }
        /// <summary>
        /// 功能名称不能重复
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
        /// 删除功能
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                InstanceDAL.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moduleEntity">功能实体</param>
        /// <param name="moduleButtonList">按钮实体列表</param>
        /// <param name="moduleColumnList">视图实体列表</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ModuleEntity moduleEntity, string moduleButtonListJson, string moduleColumnListJson)
        {
            try
            {
                var moduleButtonList = moduleButtonListJson.ToList<ModuleButtonEntity>();
                var moduleColumnList = moduleColumnListJson.ToList<ModuleColumnEntity>();
                InstanceDAL.SaveForm(keyValue, moduleEntity, moduleButtonList, moduleColumnList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
