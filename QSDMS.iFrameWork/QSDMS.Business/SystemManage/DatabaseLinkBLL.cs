
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace QSDMS.Business
{
    /// <summary>  
    /// 描 述：数据库连接管理
    /// </summary>
    public class DataBaseLinkBLL:BaseBLL<IDataBaseLinkService>
    {
     

           /// <summary>
        /// 访问实例
        /// </summary>
        public static DataBaseLinkBLL m_Instance = new DataBaseLinkBLL();

         /// <summary>
        /// 访问实例
        /// </summary>
        public static DataBaseLinkBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataBaseLinkBLL() { }

        #region 获取数据
        /// <summary>
        /// 库连接列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataBaseLinkEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 库连接实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataBaseLinkEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除库连接
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
        /// 保存库连接表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="databaseLinkEntity">库连接实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataBaseLinkEntity databaseLinkEntity)
        {
            try
            {
                #region 测试连接数据库
                DbConnection dbConnection = null;
                string ServerAddress = "";
                switch (databaseLinkEntity.DbType)
                {
                    case "SqlServer":
                        dbConnection = new SqlConnection(databaseLinkEntity.DbConnection);
                        ServerAddress = dbConnection.DataSource;
                        break;
                    default:
                        break;
                }
                dbConnection.Close();
                databaseLinkEntity.ServerAddress = ServerAddress;
                #endregion
                InstanceDAL.SaveForm(keyValue, databaseLinkEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
