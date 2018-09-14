
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：过滤时段
    /// </summary>
    public class FilterTimeService : IFilterTimeService
    {
        #region 获取数据
        /// <summary>
        /// 过滤时段列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="visitType">访问:0-拒绝，1-允许</param>
        /// <returns></returns>
        public IEnumerable<FilterTimeEntity> GetList(string objectId, string visitType)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_FilterTime where 1=1 ");
            if (!string.IsNullOrEmpty(visitType))
            {
                int _visittype = visitType.ToInt();
                sql.Append(" and VisitType=@0", _visittype);
            }
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            sql.Append(" order by CreateDate desc");
            var list = Base_FilterTime.Query(sql);
            return EntityConvertTools.CopyToList<Base_FilterTime, FilterTimeEntity>(list.ToList());
        }
        /// <summary>
        /// 过滤时段列表
        /// </summary>
        /// <param name="objectId">对象Id,用逗号隔开</param>
        /// <returns></returns>
        public IEnumerable<FilterTimeEntity> GetList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_FilterTime where 1=1 ");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            sql.Append(" order by CreateDate desc");
            var list = Base_FilterTime.Query(sql);
            return EntityConvertTools.CopyToList<Base_FilterTime, FilterTimeEntity>(list.ToList());
        }
        /// <summary>
        /// 过滤时段实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FilterTimeEntity GetEntity(string keyValue)
        {
            var model = Base_FilterTime.SingleOrDefault("where FilterTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_FilterTime, FilterTimeEntity>(model, null);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除过滤时段
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_FilterTime.Delete("where FilterTimeId=@0", keyValue);
        }
        /// <summary>
        /// 保存过滤时段表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="filterTimeEntity">过滤时段实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FilterTimeEntity filterTimeEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                filterTimeEntity.Modify(keyValue);
                Base_FilterTime filterTime = Base_FilterTime.SingleOrDefault("where FilterTimeId=@0", keyValue);
                filterTime = EntityConvertTools.CopyToModel<FilterTimeEntity, Base_FilterTime>(filterTimeEntity, filterTime);
                filterTime.FilterTimeId = keyValue;
                filterTime.Update();
            }
            else
            {
                filterTimeEntity.Create();
                filterTimeEntity.FilterTimeId = filterTimeEntity.ObjectId;
                Base_FilterTime filterTime = new Base_FilterTime();
                filterTime = EntityConvertTools.CopyToModel<FilterTimeEntity, Base_FilterTime>(filterTimeEntity, null);
                filterTime.Insert();
            }
        }
        #endregion
    }
}
