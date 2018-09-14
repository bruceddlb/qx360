
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary>  
    /// 描 述：数据库备份
    /// </summary>
    public class DataBaseBackupBLL : BaseBLL<IDataBaseBackupService>
    {
          /// <summary>
        /// 访问实例
        /// </summary>
        public static DataBaseBackupBLL m_Instance = new DataBaseBackupBLL();

         /// <summary>
        /// 访问实例
        /// </summary>
        public static DataBaseBackupBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataBaseBackupBLL() { }

        #region 获取数据
        /// <summary>
        /// 库备份列表
        /// </summary>
        /// <param name="dataBaseLinkId">连接库Id</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<DataBaseBackupEntity> GetList(string dataBaseLinkId, string queryJson)
        {
            return InstanceDAL.GetList(dataBaseLinkId, queryJson);
        }
        /// <summary>
        /// 库备份文件路径列表
        /// </summary>
        /// <param name="databaseBackupId">计划Id</param>
        /// <returns></returns>
        public IEnumerable<DataBaseBackupEntity> GetPathList(string databaseBackupId)
        {
            return InstanceDAL.GetPathList(databaseBackupId);
        }
        /// <summary>
        /// 库备份实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataBaseBackupEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除库备份
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
        /// 保存库备份表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataBaseBackupEntity">库备份实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataBaseBackupEntity dataBaseBackupEntity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, dataBaseBackupEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
