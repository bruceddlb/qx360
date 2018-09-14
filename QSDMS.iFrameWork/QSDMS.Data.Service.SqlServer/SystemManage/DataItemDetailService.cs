
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：数据字典明细
    /// </summary>
    public class DataItemDetailService : IDataItemDetailService
    {
        #region 获取数据
        /// <summary>
        /// 明细列表
        /// </summary>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public IEnumerable<DataItemDetailEntity> GetList(string itemId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItemDetail where 1=1");
            if (!string.IsNullOrEmpty(itemId))
            {
                sql.Append(" and ItemId=@0", itemId);
            }
            sql.Append(" order by SortCode");
            var list = Base_DataItemDetail.Query(sql);
            return EntityConvertTools.CopyToList<Base_DataItemDetail, DataItemDetailEntity>(list.ToList());
        }

        /// <summary>
        /// 明细实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataItemDetailEntity GetEntity(string keyValue)
        {
            var detail = Base_DataItemDetail.SingleOrDefault("where ItemDetailId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_DataItemDetail, DataItemDetailEntity>(detail, null);
        }
        /// <summary>
        /// 获取数据字典列表（给绑定下拉框提供的）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataItemModel> GetDataItemList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  i.ItemId ,
                                    i.ItemCode AS EnCode ,
                                    d.ItemDetailId ,
                                    d.ParentId ,
                                    d.ItemCode ,
                                    d.ItemName ,
                                    d.ItemValue ,
                                    d.QuickQuery ,
                                    d.SimpleSpelling ,
                                    d.IsDefault ,
                                    d.SortCode ,
                                    d.EnabledMark,
                                    d.Description
                            FROM    Base_DataItemDetail d
                                    LEFT JOIN Base_DataItem i ON i.ItemId = d.ItemId
                            WHERE   1 = 1
                                    AND d.EnabledMark = 1
                                    AND d.DeleteMark = 0
                            ORDER BY d.SortCode ASC");
            return QSDMS_SQLDB.GetInstance().Fetch<DataItemModel>(strSql.ToString());
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 项目值不能重复
        /// </summary>
        /// <param name="itemValue">项目值</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public bool ExistItemValue(string itemValue, string keyValue, string itemId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItemDetail where 1=1 ");
            if (!string.IsNullOrEmpty(itemId))
            {
                sql.Append(" and (ItemValue=@0 and ItemId=@1)", itemValue, itemId);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ItemDetailId!=@0", keyValue);
            }
            return Base_DataItemDetail.Query(sql).Count() == 0 ? true : false;

        }
        /// <summary>
        /// 项目名不能重复
        /// </summary>
        /// <param name="itemName">项目名</param>
        /// <param name="keyValue">主键</param>
        /// <param name="itemId">分类Id</param>
        /// <returns></returns>
        public bool ExistItemName(string itemName, string keyValue, string itemId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_DataItemDetail where 1=1 ");
            if (!string.IsNullOrEmpty(itemId))
            {
                sql.Append(" and (ItemName=@0 and ItemId=@1)", itemName, itemId);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ItemDetailId!=@0", keyValue);
            }
            return Base_DataItemDetail.Query(sql).Count() == 0 ? true : false;

        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_DataItemDetail.Delete("where ItemDetailId=@0", keyValue);
        }
        /// <summary>
        /// 保存明细表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemDetailEntity">明细实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataItemDetailEntity dataItemDetailEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                dataItemDetailEntity.Modify(keyValue);
                Base_DataItemDetail detail = Base_DataItemDetail.SingleOrDefault("where ItemDetailId=@0", keyValue);
                detail = EntityConvertTools.CopyToModel<DataItemDetailEntity, Base_DataItemDetail>(dataItemDetailEntity, detail);
                detail.ItemDetailId = keyValue;
                detail.Update();
            }
            else
            {
                dataItemDetailEntity.Create();
                Base_DataItemDetail detail = EntityConvertTools.CopyToModel<DataItemDetailEntity, Base_DataItemDetail>(dataItemDetailEntity, null);
                detail.Insert();
            }
        }
        #endregion
    }
}
