
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary> 
    /// 描 述：数据字典分类
    /// </summary>
    public class DataItemBLL : BaseBLL<IDataItemService>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static DataItemBLL m_Instance = new DataItemBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static DataItemBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataItemBLL() { }


        #region 获取数据
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemEntity> GetList()
        {
            return InstanceDAL.GetList();
        }
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataItemEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        /// <summary>
        /// 根据分类编号获取实体对象
        /// </summary>
        /// <param name="ItemCode">编号</param>
        /// <returns></returns>
        public DataItemEntity GetEntityByCode(string ItemCode)
        {
            return InstanceDAL.GetEntityByCode(ItemCode);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 分类编号不能重复
        /// </summary>
        /// <param name="itemCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistItemCode(string itemCode, string keyValue)
        {
            return InstanceDAL.ExistItemCode(itemCode, keyValue);
        }
        /// <summary>
        /// 分类名称不能重复
        /// </summary>
        /// <param name="itemName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistItemName(string itemName, string keyValue)
        {
            return InstanceDAL.ExistItemName(itemName, keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
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
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemEntity">分类实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataItemEntity dataItemEntity)
        {
            try
            {
                InstanceDAL.SaveForm(keyValue, dataItemEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
