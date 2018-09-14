
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：过滤IP
    /// </summary>
    public class FilterIPService : IFilterIPService
    {
        #region 获取数据
        /// <summary>
        /// 过滤IP列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="visitType">访问:0-拒绝，1-允许</param>
        /// <returns></returns>
        public IEnumerable<FilterIPEntity> GetList(string objectId, string visitType)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_FilterIP where 1=1 ");
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
            var list = Base_FilterIP.Query(sql);
            return EntityConvertTools.CopyToList<Base_FilterIP, FilterIPEntity>(list.ToList());
        }

        /// <summary>
        /// 过滤IP实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FilterIPEntity GetEntity(string keyValue)
        {
            var model = Base_FilterIP.SingleOrDefault("where FilterIPId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_FilterIP, FilterIPEntity>(model,null);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除过滤IP
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_FilterIP.Delete("where FilterIPId=@0", keyValue);
        }
        /// <summary>
        /// 保存过滤IP表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="filterIPEntity">过滤IP实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FilterIPEntity filterIPEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                filterIPEntity.Modify(keyValue);
                Base_FilterIP filterIP = Base_FilterIP.SingleOrDefault("where FilterIPId=@0", keyValue);
                filterIP = EntityConvertTools.CopyToModel<FilterIPEntity, Base_FilterIP>(filterIPEntity, filterIP);
                filterIP.FilterIPId = keyValue;
                filterIP.Update();
            }
            else
            {
                filterIPEntity.Create();
                Base_FilterIP filterIP = new Base_FilterIP();
                filterIP = EntityConvertTools.CopyToModel<FilterIPEntity, Base_FilterIP>(filterIPEntity,null);
                filterIP.Insert();
            }
        }
        #endregion

        /// <summary>
        /// 过滤IP列表
        /// </summary>
        /// <param name="objectId">对象Id，用逗号分隔</param>
        /// <param name="visitType">访问:0-拒绝，1-允许</param>
        /// <returns></returns>
        public IEnumerable<FilterIPEntity> GetAllList(string objectId, int visitType)
        {
            throw new System.NotImplementedException();
        }
    }
}
