
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QSDMS.Util.Extension;
using QSDMS.Util;
namespace QSDMS.Data.Service.SqlServer
{
    /// <summary> 
    /// 描 述：系统功能
    /// </summary>
    public class ModuleService : IModuleService
    {
        #region 获取数据
        /// <summary>
        /// 获取最大编号
        /// </summary>
        /// <returns></returns>
        public int GetSortCode()
        {
            int sortCode = Base_Module.Query("select * from Base_Module").Max(t => t.SortCode).ToInt();
            if (!string.IsNullOrEmpty(sortCode.ToString()))
            {
                return sortCode + 1;
            }
            return 100001;
        }
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModuleEntity> GetList()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM Base_Module Order By SortCode");
            var list = Base_Module.Query(strSql.ToString());
            return EntityConvertTools.CopyToList<Base_Module, ModuleEntity>(list.ToList());
        }
        /// <summary>
        /// 功能实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ModuleEntity GetEntity(string keyValue)
        {
            var model = Base_Module.SingleOrDefault("where ModuleId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Module, ModuleEntity>(model, null);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 功能编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Module where 1=1 ");
            if (!string.IsNullOrEmpty(enCode))
            {
                sql.Append(" and EnCode=@0", enCode);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ModuleId!=@0", keyValue);
            }
            return Base_Module.Query(sql).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 功能名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Module where 1=1 ");
            if (!string.IsNullOrEmpty(fullName))
            {
                sql.Append(" and FullName=@0", fullName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and ModuleId!=@0", keyValue);
            }
            return Base_Module.Query(sql).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除功能
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                QSDMS_SQLDB db = QSDMS_SQLDB.GetInstance();
                using (var tran = db.GetTransaction())
                {

                    int count = db.Fetch<Base_Module>("select * from Base_Module").FindAll(t => t.ParentId == keyValue).Count();
                    if (count > 0)
                    {
                        throw new Exception("当前所选数据有子节点数据！");
                    }
                    Base_Module.Delete("where ModuleId=@0", keyValue);
                    Base_ModuleButton.Delete("where ModuleId=@0", keyValue);
                    Base_ModuleColumn.Delete("where ModuleId=@0", keyValue);
                    //提交事务
                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moduleEntity">功能实体</param>
        /// <param name="moduleButtonList">按钮实体列表</param>
        /// <param name="moduleColumnList">视图实体列表</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ModuleEntity moduleEntity, List<ModuleButtonEntity> moduleButtonList, List<ModuleColumnEntity> moduleColumnList)
        {

            try
            {

                using (var tran = QSDMS_SQLDB.GetInstance().GetTransaction())
                {
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        moduleEntity.Modify(keyValue);
                        Base_Module moudle = Base_Module.SingleOrDefault("where ModuleId=@0", keyValue);
                        moudle = EntityConvertTools.CopyToModel<ModuleEntity, Base_Module>(moduleEntity, moudle);
                        moudle.ModuleId = keyValue;
                        moudle.Update();

                    }
                    else
                    {
                        moduleEntity.Create();
                        Base_Module moudle = new Base_Module();
                        moudle = EntityConvertTools.CopyToModel<ModuleEntity, Base_Module>(moduleEntity, null);
                        moudle.Insert();
                    }
                    //删除操作按钮
                    Base_ModuleButton.Delete("where ModuleId=@0", keyValue);
                    if (moduleButtonList != null)
                    {
                        foreach (ModuleButtonEntity buttonItem in moduleButtonList)
                        {
                            Base_ModuleButton modulebutton = new Base_ModuleButton();
                            modulebutton = EntityConvertTools.CopyToModel<ModuleButtonEntity, Base_ModuleButton>(buttonItem,null);
                            modulebutton.Insert();
                        }
                    }
                    //删除字段
                    Base_ModuleColumn.Delete("where ModuleId=@0", keyValue);
                    if (moduleColumnList != null)
                    {
                        foreach (ModuleColumnEntity columnItem in moduleColumnList)
                        {
                            Base_ModuleColumn modulecolumn = new Base_ModuleColumn();
                            modulecolumn = EntityConvertTools.CopyToModel<ModuleColumnEntity, Base_ModuleColumn>(columnItem,null);
                            modulecolumn.Insert();
                        }
                    }
                    tran.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
