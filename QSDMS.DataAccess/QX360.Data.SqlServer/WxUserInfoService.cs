using QSDMS.Util;
using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.SqlServer
{
    /// <summary>
    /// 意见管理
    /// </summary>
    public class WxUserInfoService : BaseSqlDataService, IWxUserInfoService<WxUserInfoEntity, WxUserInfoEntity, Pagination>
    {
        public int QueryCount(WxUserInfoEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WxUserInfoEntity> GetPageList(WxUserInfoEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WxUserInfo");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WxUserInfo.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WxUserInfo, WxUserInfoEntity>(pageList.ToList());
        }

        public List<WxUserInfoEntity> GetList(WxUserInfoEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WxUserInfo");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WxUserInfo.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WxUserInfo, WxUserInfoEntity>(list.ToList());
        }

        public WxUserInfoEntity GetEntity(string keyValue)
        {
            var model = tbl_WxUserInfo.SingleOrDefault("where WxUserInfoId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WxUserInfo, WxUserInfoEntity>(model, null);
        }

        public bool Add(WxUserInfoEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WxUserInfoEntity, tbl_WxUserInfo>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WxUserInfoEntity entity)
        {

            var model = tbl_WxUserInfo.SingleOrDefault("where WxUserInfoId=@0", entity.WxUserInfoId);
            model = EntityConvertTools.CopyToModel<WxUserInfoEntity, tbl_WxUserInfo>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WxUserInfo.Delete("where WxUserInfoId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WxUserInfoEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.WxUserInfoId != null)
            {
                sbWhere.AppendFormat(" and WxUserInfoId='{0}'", para.WxUserInfoId);
            }           

            return sbWhere.ToString();
        }
    }
}
