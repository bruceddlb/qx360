
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：系统视图
    /// </summary>
    public class ModuleColumnService : IModuleColumnService
    {
        #region 获取数据
        /// <summary>
        /// 视图列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleColumnEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_ModuleColumn where 1=1 ");
            sql.Append(" order by SortCode");
            var list = Base_ModuleColumn.Query(sql);
            return EntityConvertTools.CopyToList<Base_ModuleColumn, ModuleColumnEntity>(list.ToList());
        }

        /// <summary>
        /// 视图列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleColumnEntity> GetList(string moduleId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_ModuleColumn where 1=1 ");
            //if (!string.IsNullOrEmpty(moduleId))
            //{
                sql.Append(" and ModuleId=@0", moduleId);
            //}
            sql.Append(" order by SortCode");
            var list = Base_ModuleColumn.Query(sql);
            return EntityConvertTools.CopyToList<Base_ModuleColumn, ModuleColumnEntity>(list.ToList());
        }
        /// <summary>
        /// 视图实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleColumnEntity GetEntity(string keyValue)
        {
            var model = Base_ModuleColumn.SingleOrDefault("where ModuleColumnId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_ModuleColumn, ModuleColumnEntity>(model,null);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加视图
        /// </summary>
        /// <param name="moduleButtonEntity">视图实体</param>
        public void AddEntity(ModuleColumnEntity moduleColumnEntity)
        {
            moduleColumnEntity.Create();
            Base_ModuleColumn moduleColumn = new Base_ModuleColumn();
            moduleColumn = EntityConvertTools.CopyToModel<ModuleColumnEntity, Base_ModuleColumn>(moduleColumnEntity,null);
            moduleColumn.Insert();
        }
        #endregion
    }
}
