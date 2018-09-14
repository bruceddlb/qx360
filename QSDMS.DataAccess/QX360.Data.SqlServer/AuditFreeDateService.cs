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
    /// 年审机构日期设置
    /// </summary>
    public class AuditFreeDateService : BaseSqlDataService, IAuditFreeDateService<AuditFreeDateEntity, AuditFreeDateEntity, Pagination>
    {
        public int QueryCount(AuditFreeDateEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditFreeDateEntity> GetPageList(AuditFreeDateEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditFreeDate.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditFreeDate, AuditFreeDateEntity>(pageList.ToList());
        }

        public List<AuditFreeDateEntity> GetList(AuditFreeDateEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_AuditFreeDate.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditFreeDate, AuditFreeDateEntity>(list.ToList());
        }

        public AuditFreeDateEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditFreeDate.SingleOrDefault("where AuditFreeDateId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditFreeDate, AuditFreeDateEntity>(model, null);
        }

        public bool Add(AuditFreeDateEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditFreeDateEntity, tbl_AuditFreeDate>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditFreeDateEntity entity)
        {

            var model = tbl_AuditFreeDate.SingleOrDefault("where AuditFreeDateId=@0", entity.AuditFreeDateId);
            model = EntityConvertTools.CopyToModel<AuditFreeDateEntity, tbl_AuditFreeDate>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditFreeDate.Delete("where AuditFreeDateId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditFreeDateEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ObjectId != null)
            {
                sbWhere.AppendFormat(" and ObjectId='{0}'", para.ObjectId);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND FreeDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND FreeDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
             if(para.WorkdayItemId!=null)
            {
                sbWhere.Append(base.FormatParameter(" AND WorkdayItemId='{0}'",para.WorkdayItemId));
            }
            return sbWhere.ToString();
        }
    }
}
