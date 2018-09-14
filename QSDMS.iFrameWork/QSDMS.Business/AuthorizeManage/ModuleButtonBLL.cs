
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary> 
    /// 描 述：系统按钮
    /// </summary>
    public class ModuleButtonBLL : BaseBLL<IModuleButtonService>
    {
       /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleButtonBLL m_Instance = new ModuleButtonBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static ModuleButtonBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ModuleButtonBLL() { }

        #region 获取数据
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList(string moduleId)
        {
            return InstanceDAL.GetList(moduleId);
        }
        /// <summary>
        /// 按钮实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleButtonEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 复制按钮
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="moduleId">功能主键</param>
        /// <returns></returns>
        public void CopyForm(string keyValue, string moduleId)
        {
            try
            {
                ModuleButtonEntity moduleButtonEntity = this.GetEntity(keyValue);
                moduleButtonEntity.ModuleId = moduleId;
                InstanceDAL.AddEntity(moduleButtonEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
