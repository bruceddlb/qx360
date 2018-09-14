
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>  
    /// 描 述：用户组管理
    /// </summary>
    public class UserGroupService : IUserGroupService
    {

        #region 获取数据
        /// <summary>
        /// 用户组列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Role where 1=1 and Category=@0 and EnabledMark=1 and DeleteMark=0", (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组);
            sql.Append(" order by CreateDate desc");
            var list = Base_Role.Query(sql);
            return EntityConvertTools.CopyToList<Base_Role, RoleEntity>(list.ToList());
        }
        /// <summary>
        /// 用户组列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Role where 1=1 and Category=@0", (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组);
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "EnCode":            //岗位编号
                        sql.Append(" and (charindex(@0,EnCode)>0)", keyword);
                        break;
                    case "FullName":          //岗位名称
                        sql.Append(" and (charindex(@0,FullName)>0)", keyword);
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = Base_Role.Page(pagination.page, pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            //分页对象

            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<Base_Role, RoleEntity>(pageList.ToList());
        }
        /// <summary>
        /// 用户组列表(ALL)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleEntity> GetAllList()
        {
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT  r.RoleId ,
				                    o.FullName AS OrganizeId ,
				                    r.Category ,
				                    r.EnCode ,
				                    r.FullName ,
				                    r.SortCode ,
				                    r.EnabledMark ,
				                    r.Description ,
				                    r.CreateDate
                    FROM    Base_Role r
				                    LEFT JOIN Base_Organize o ON o.OrganizeId = r.OrganizeId
                    WHERE   o.FullName is not null and r.Category = '{0}' and r.EnabledMark =1
                    ORDER BY o.FullName, r.SortCode", (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组);
            return QSDMS_SQLDB.GetInstance().Query<RoleEntity>("select * from (" + strSql.ToString() + ")a");
        }
        /// <summary>
        /// 用户组实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public RoleEntity GetEntity(string keyValue)
        {
            var role = Base_Role.SingleOrDefault("where RoleId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Role, RoleEntity>(role, null);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 组编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Role where 1=1 and Category=@0", (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组);
            if (!string.IsNullOrEmpty(enCode))
            {
                sql.Append(" and EnCode=@0", enCode);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and RoleId!=@0", keyValue);
            }
            return Base_Role.Query(sql).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 组名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Role where 1=1 and Category=@0", (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组);
            if (!string.IsNullOrEmpty(fullName))
            {
                sql.Append(" and FullName=@0", fullName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and RoleId!=@0", keyValue);
            }
            return Base_Role.Query(sql).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            Base_Role.Delete("where RoleId=@0", keyValue);
        }
        /// <summary>
        /// 保存用户组表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userGroupEntity">用户组实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, RoleEntity userGroupEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                userGroupEntity.Modify(keyValue);
                Base_Role role = Base_Role.SingleOrDefault("where roleid=@0", keyValue);
                role = EntityConvertTools.CopyToModel<RoleEntity, Base_Role>(userGroupEntity,role);
                role.RoleId = keyValue;
                role.Update();
            }
            else
            {
                userGroupEntity.Create();
                userGroupEntity.Category = (int)QSDMS.Model.Enums.RoleCategoryEnum.工作组;

                Base_Role role = EntityConvertTools.CopyToModel<RoleEntity, Base_Role>(userGroupEntity, null);
                role.Insert();
            }
        }
        #endregion
    }
}
