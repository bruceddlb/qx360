
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：机构管理
    /// </summary>
    public class OrganizeService : IOrganizeService
    {
        #region 获取数据
        /// <summary>
        /// 机构列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrganizeEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Organize where 1=1");
            sql.Append(" order by SortCode");
            var list = Base_Organize.Query(sql);
            return EntityConvertTools.CopyToList<Base_Organize, OrganizeEntity>(list.ToList());
        }
        /// <summary>
        /// 机构实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OrganizeEntity GetEntity(string keyValue)
        {
            var organize = Base_Organize.SingleOrDefault("where OrganizeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Organize, OrganizeEntity>(organize, null);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 公司名称不能重复
        /// </summary>
        /// <param name="organizeName">公司名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Organize where 1=1 ");
            if (!string.IsNullOrEmpty(fullName))
            {
                sql.Append(" and FullName=@0", fullName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and OrganizeId!=@0", keyValue);
            }
            return Base_Organize.Query(sql).Count() == 0 ? true : false;

        }
        /// <summary>
        /// 外文名称不能重复
        /// </summary>
        /// <param name="enCode">外文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Organize where 1=1 ");
            if (!string.IsNullOrEmpty(enCode))
            {
                sql.Append(" and EnCode=@0", enCode);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and OrganizeId!=@0", keyValue);
            }
            return Base_Organize.Query(sql).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 中文名称不能重复
        /// </summary>
        /// <param name="shortName">中文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistShortName(string shortName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Organize where 1=1 ");
            if (!string.IsNullOrEmpty(shortName))
            {
                sql.Append(" and ShortName=@0", shortName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and OrganizeId!=@0", keyValue);
            }
            return Base_Organize.Query(sql).Count() == 0 ? true : false;

        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除机构
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            int count = QSDMS_SQLDB.GetInstance().Fetch<Base_Organize>("select * from Base_Organize").FindAll(t => t.ParentId == keyValue).Count();

            if (count > 0)
            {
                throw new Exception("当前所选数据有子节点数据！");
            }
            Base_Organize.Delete("where OrganizeId=@0", keyValue);
        }
        /// <summary>
        /// 保存机构表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="organizeEntity">机构实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, OrganizeEntity organizeEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                organizeEntity.Modify(keyValue);
                Base_Organize organize = Base_Organize.SingleOrDefault("where OrganizeId=@0", keyValue);
                organize = EntityConvertTools.CopyToModel<OrganizeEntity, Base_Organize>(organizeEntity, organize);
                organize.OrganizeId = keyValue;
                organize.Update();
            }
            else
            {
                organizeEntity.Create();
                Base_Organize organize = EntityConvertTools.CopyToModel<OrganizeEntity, Base_Organize>(organizeEntity,null);
                organize.Insert();
            }
        }
        #endregion
    }
}
