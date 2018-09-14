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
    /// 年审空闲时间
    /// </summary>
    public class AuditFreeTimeService : BaseSqlDataService, IAuditFreeTimeService<AuditFreeTimeEntity, AuditFreeTimeEntity, Pagination>
    {
        public int QueryCount(AuditFreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditFreeTimeEntity> GetPageList(AuditFreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditFreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditFreeTime, AuditFreeTimeEntity>(pageList.ToList());
        }

        public List<AuditFreeTimeEntity> GetList(AuditFreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_AuditFreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditFreeTime, AuditFreeTimeEntity>(list.ToList());
        }

        public AuditFreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditFreeTime.SingleOrDefault("where AuditFreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditFreeTime, AuditFreeTimeEntity>(model, null);
        }

        public bool Add(AuditFreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditFreeTimeEntity, tbl_AuditFreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditFreeTimeEntity entity)
        {

            var model = tbl_AuditFreeTime.SingleOrDefault("where AuditFreeTimeId=@0", entity.AuditFreeTimeId);
            model = EntityConvertTools.CopyToModel<AuditFreeTimeEntity, tbl_AuditFreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditFreeTime.Delete("where AuditFreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditFreeTimeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.AuditFreeDateId != null)
            {
                sbWhere.AppendFormat(" and AuditFreeDateId='{0}'", para.AuditFreeDateId);
            }
            if (para.FreeStatus != null)
            {
                sbWhere.AppendFormat(" and FreeStatus='{0}'", para.FreeStatus);
            }
            if (para.TimeSection != null)
            {
                sbWhere.AppendFormat(" and TimeSection='{0}'", para.TimeSection);
            }
            return sbWhere.ToString();
        }
    }
}
