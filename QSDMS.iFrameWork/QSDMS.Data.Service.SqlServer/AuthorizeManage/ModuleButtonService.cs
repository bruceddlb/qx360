
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：系统按钮
    /// </summary>
    public class ModuleButtonService : IModuleButtonService
    {
        #region 获取数据
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_ModuleButton where 1=1 ");
            sql.Append(" order by SortCode");
            var list = Base_ModuleButton.Query(sql);
            return EntityConvertTools.CopyToList<Base_ModuleButton, ModuleButtonEntity>(list.ToList());
        }
        /// <summary>
        /// 按钮列表
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetList(string moduleId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_ModuleButton where 1=1 ");
            //if (!string.IsNullOrEmpty(moduleId))
            //{
                sql.Append(" and ModuleId=@0", moduleId);
            //}
            sql.Append(" order by SortCode");
            var list = Base_ModuleButton.Query(sql);
            return EntityConvertTools.CopyToList<Base_ModuleButton, ModuleButtonEntity>(list.ToList());
        }
        /// <summary>
        /// 按钮实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleButtonEntity GetEntity(string keyValue)
        {
            var model = Base_ModuleButton.SingleOrDefault("where ModuleButtonId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_ModuleButton, ModuleButtonEntity>(model,null);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="moduleButtonEntity">按钮实体</param>
        public void AddEntity(ModuleButtonEntity moduleButtonEntity)
        {
            moduleButtonEntity.Create();
            Base_ModuleButton modelButton = new Base_ModuleButton();
            modelButton = EntityConvertTools.CopyToModel<ModuleButtonEntity, Base_ModuleButton>(moduleButtonEntity,null);
            modelButton.Insert();
        }
        #endregion
    }
}
