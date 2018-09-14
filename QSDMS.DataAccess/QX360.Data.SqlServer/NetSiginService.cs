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
    /// 网签管理
    /// </summary>
    public class NetSiginService : BaseSqlDataService, INetSiginService<NetSiginEntity, NetSiginEntity, Pagination>
    {
        public int QueryCount(NetSiginEntity para)
        {
            throw new NotImplementedException();
        }

        public List<NetSiginEntity> GetPageList(NetSiginEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_NetSigin");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_NetSigin.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_NetSigin, NetSiginEntity>(pageList.ToList());
        }

        public List<NetSiginEntity> GetList(NetSiginEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_NetSigin");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_NetSigin.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_NetSigin, NetSiginEntity>(list.ToList());
        }

        public NetSiginEntity GetEntity(string keyValue)
        {
            var model = tbl_NetSigin.SingleOrDefault("where NetSiginId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_NetSigin, NetSiginEntity>(model, null);
        }

        public bool Add(NetSiginEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<NetSiginEntity, tbl_NetSigin>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(NetSiginEntity entity)
        {

            var model = tbl_NetSigin.SingleOrDefault("where NetSiginId=@0", entity.NetSiginId);
            model = EntityConvertTools.CopyToModel<NetSiginEntity, tbl_NetSigin>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_NetSigin.Delete("where NetSiginId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(NetSiginEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.MemberTel != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberTel)>0)", para.MemberTel);
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            return sbWhere.ToString();
        }
    }
}
