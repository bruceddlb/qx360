
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using QSDMS.Model;
using QSDMS.Data.IService;
using QSDMS.Util.WebControl;
using QSDMS.Util;
using QSDMS.Util.Extension;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：用户角色管理
    /// </summary>
    public class UserRoleService : IUserRoleService
    {

        public IEnumerable<UserRoleEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserRole ");
            var list = Base_UserRole.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserRole, UserRoleEntity>(list.ToList());
        }

        public IEnumerable<UserRoleEntity> GetRoleList(string userid)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserRole where UserId=@0", userid);
            var list = Base_UserRole.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserRole, UserRoleEntity>(list.ToList());
        }

        public IEnumerable<UserRoleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserRole where 1=1");
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();
                //switch (condition)
                //{
                //    case "EnCode":            //岗位编号
                //        sql.Append(" and (charindex(@0,EnCode)>0)", keyword);
                //        break;
                //    case "FullName":          //岗位名称
                //        sql.Append(" and (charindex(@0,FullName)>0)", keyword);
                //        break;
                //    default:
                //        break;
                //}
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = Base_UserRole.Page(pagination.page, pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            //分页对象          
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<Base_UserRole, UserRoleEntity>(pageList.ToList());
        }

        public UserRoleEntity GetEntity(string keyValue)
        {
            var role = Base_UserRole.SingleOrDefault("where UserRoleId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_UserRole, UserRoleEntity>(role, null);
        }

        public void RemoveForm(string keyValue)
        {
            Base_UserRole.Delete("where UserRoleId=@0", keyValue);
        }

        public void SaveForm(string keyValue, UserRoleEntity userRoleEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                Base_UserRole role = Base_UserRole.SingleOrDefault("where UserRoleId=@0", keyValue);
                role = EntityConvertTools.CopyToModel<UserRoleEntity, Base_UserRole>(userRoleEntity, role);
                role.UserRoleId = keyValue;
                role.Update();
            }
            else
            {
                Base_UserRole role = EntityConvertTools.CopyToModel<UserRoleEntity, Base_UserRole>(userRoleEntity, null);
                role.Insert();
            }
        }



    }
}
