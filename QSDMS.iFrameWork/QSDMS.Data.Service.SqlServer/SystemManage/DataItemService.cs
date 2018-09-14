
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：数据字典分类
    /// </summary>
    public class DataItemService : IDataItemService
    {
        #region 获取数据
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItem where 1=1");
            sql.Append(" order by CreateDate desc");
            var list = Base_DataItem.Query(sql);
            return EntityConvertTools.CopyToList<Base_DataItem, DataItemEntity>(list.ToList());
        }
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataItemEntity GetEntity(string keyValue)
        {
            var detail = Base_DataItem.SingleOrDefault("where ItemId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_DataItem, DataItemEntity>(detail, null);
        }
        /// <summary>
        /// 根据分类编号获取实体对象
        /// </summary>
        /// <param name="ItemCode">编号</param>
        /// <returns></returns>
        public DataItemEntity GetEntityByCode(string ItemCode)
        {
            var detail = Base_DataItem.SingleOrDefault("where ItemCode=@0", ItemCode);
            return EntityConvertTools.CopyToModel<Base_DataItem, DataItemEntity>(detail, null);
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
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItem where 1=1 ");
            if (!string.IsNullOrEmpty(itemCode))
            {
                sql.Append(" and ItemCode=@0", itemCode);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ItemId!=@0", keyValue);
            }
            return Base_DataItem.Query(sql).Count() == 0 ? true : false;

        }
        /// <summary>
        /// 分类名称不能重复
        /// </summary>
        /// <param name="itemName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistItemName(string itemName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItem where 1=1 ");
            if (!string.IsNullOrEmpty(itemName))
            {
                sql.Append(" and ItemName=@0", itemName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ItemId!=@0", keyValue);
            }
            return Base_DataItem.Query(sql).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_DataItem.Delete("where ItemId=@0", keyValue);
        }
        /// <summary>
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemEntity">分类实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataItemEntity dataItemEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                dataItemEntity.Modify(keyValue);
                Base_DataItem dataitem = Base_DataItem.SingleOrDefault("where ItemId=@0", keyValue);
                dataitem = EntityConvertTools.CopyToModel<DataItemEntity, Base_DataItem>(dataItemEntity, dataitem);
                dataitem.ItemId = keyValue;
                dataitem.Update();
            }
            else
            {
                dataItemEntity.Create();
                Base_DataItem dataitem = EntityConvertTools.CopyToModel<DataItemEntity, Base_DataItem>(dataItemEntity, null);
                dataitem.Insert();
            }
        }
        #endregion
    }
}
