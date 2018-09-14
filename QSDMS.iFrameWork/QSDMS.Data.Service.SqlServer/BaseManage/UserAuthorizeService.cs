
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
    /// 描 述：用户数据授权管理
    /// </summary>
    public class UserAuthorizeService : IUserAuthorizeService
    {
        public IEnumerable<UserAuthorizeEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserAuthorize ");
            var list = Base_UserAuthorize.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserAuthorize, UserAuthorizeEntity>(list.ToList());
        }
        public IEnumerable<UserAuthorizeEntity> GetUserAuthorizeList(string userid)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserAuthorize where UserId=@0", userid);
            var list = Base_UserAuthorize.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserAuthorize, UserAuthorizeEntity>(list.ToList());
        }

        public IEnumerable<UserAuthorizeEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserAuthorize where 1=1");
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();

            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = Base_UserAuthorize.Page(pagination.page, pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            //分页对象          
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<Base_UserAuthorize, UserAuthorizeEntity>(pageList.ToList());
        }

        public UserAuthorizeEntity GetEntity(string keyValue)
        {
            var role = Base_UserAuthorize.SingleOrDefault("where UserAuthorizeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_UserAuthorize, UserAuthorizeEntity>(role, null);
        }

        public void RemoveForm(string keyValue)
        {
            Base_UserAuthorize.Delete("where UserAuthorizeId=@0", keyValue);
        }

        public void SaveForm(string keyValue, UserAuthorizeEntity userAuthorizeEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                Base_UserAuthorize model = Base_UserAuthorize.SingleOrDefault("where UserAuthorizeId=@0", keyValue);
                model = EntityConvertTools.CopyToModel<UserAuthorizeEntity, Base_UserAuthorize>(userAuthorizeEntity, model);
                model.UserAuthorizeId = keyValue;
                model.Update();
            }
            else
            {
                Base_UserAuthorize model = EntityConvertTools.CopyToModel<UserAuthorizeEntity, Base_UserAuthorize>(userAuthorizeEntity, null);
                model.Insert();
            }
        }



    }
}
