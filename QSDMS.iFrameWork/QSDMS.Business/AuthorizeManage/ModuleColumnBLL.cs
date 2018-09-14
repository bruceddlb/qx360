
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary>   
    /// 描 述：系统视图
    /// </summary>
    public class ModuleColumnBLL : BaseBLL<IModuleColumnService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleColumnBLL m_Instance = new ModuleColumnBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleColumnBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModuleColumnBLL() { }

        #region 获取数据
        /// <summary>
        /// 视图列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleColumnEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 视图列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleColumnEntity> GetList(string moduleId)
        {
            return InstanceDAL.GetList(moduleId);
        }
        /// <summary>
        /// 视图实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleColumnEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 复制视图 
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        public void CopyForm(string keyValue, string moduleId)
        {
            try
            {
                ModuleColumnEntity moduleColumnEntity = this.GetEntity(keyValue);
                moduleColumnEntity.ModuleId = moduleId;
                InstanceDAL.AddEntity(moduleColumnEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
