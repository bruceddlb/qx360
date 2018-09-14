
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary> 
    /// 描 述：区域管理
    /// </summary>
    public class AreaService : IAreaService
    {
        #region 获取数据
        /// <summary>
        /// 区域列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AreaEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Area where 1=1 and Layer<>4 and EnabledMark<>0");

            sql.Append(" order by CreateDate desc");
            var list = Base_Area.Query(sql);
            return EntityConvertTools.CopyToList<Base_Area, AreaEntity>(list.ToList());

        }
        /// <summary>
        /// 区域列表
        /// </summary>
        /// <param name="parentId">节点Id</param>
        /// <param name="keyword">关键字查询</param>
        /// <returns></returns>
        public IEnumerable<AreaEntity> GetList(string parentId, string keyword)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Area where 1=1");
            if (!string.IsNullOrEmpty(parentId))
            {
                sql.Append(" and ParentId=@0", parentId);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql.Append(" and (charindex(@0,AreaCode)>0 or (charindex(@0,AreaName)>0", keyword);
            }
            var list = Base_Area.Query(sql);
            return EntityConvertTools.CopyToList<Base_Area, AreaEntity>(list.ToList());
        }
        /// <summary>
        /// 区域实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AreaEntity GetEntity(string keyValue)
        {
            var area = Base_Area.SingleOrDefault("where AreaId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Area, AreaEntity>(area,null);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_Area.Delete("where AreaId=@0", keyValue);
        }
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="{">区域实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, AreaEntity areaEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                areaEntity.Modify(keyValue);
                Base_Area area = Base_Area.SingleOrDefault("where AreaId=@0",keyValue);
                area = EntityConvertTools.CopyToModel<AreaEntity, Base_Area>(areaEntity,area);
                area.AreaId = keyValue;
                area.Update();
            }
            else
            {
                areaEntity.Create();
                Base_Area area = EntityConvertTools.CopyToModel<AreaEntity, Base_Area>(areaEntity,null);
                area.Insert();
            }
        }
        #endregion
    }
}
