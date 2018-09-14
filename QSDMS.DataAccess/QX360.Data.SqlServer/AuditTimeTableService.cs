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
    /// 年审机构工作时间
    /// </summary>
    public class AuditTimeTableService : BaseSqlDataService, IAuditTimeTableService<AuditTimeTableEntity, AuditTimeTableEntity, Pagination>
    {
        public int QueryCount(AuditTimeTableEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditTimeTableEntity> GetPageList(AuditTimeTableEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditTimeTable.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditTimeTable, AuditTimeTableEntity>(pageList.ToList());
        }

        public List<AuditTimeTableEntity> GetList(AuditTimeTableEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_AuditTimeTable.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditTimeTable, AuditTimeTableEntity>(list.ToList());
        }

        public AuditTimeTableEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditTimeTable.SingleOrDefault("where AuditTimeTableId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditTimeTable, AuditTimeTableEntity>(model, null);
        }

        public bool Add(AuditTimeTableEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditTimeTableEntity, tbl_AuditTimeTable>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditTimeTableEntity entity)
        {

            var model = tbl_AuditTimeTable.SingleOrDefault("where AuditTimeTableId=@0", entity.AuditTimeTableId);
            model = EntityConvertTools.CopyToModel<AuditTimeTableEntity, tbl_AuditTimeTable>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditTimeTable.Delete("where AuditTimeTableId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditTimeTableEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.AuditId != null)
            {
                sbWhere.AppendFormat(" and AuditId='{0}'", para.AuditId);
            }
            if (para.TimeSection != null)
            {
                sbWhere.AppendFormat(" and TimeSection='{0}'", para.TimeSection);
            }

            return sbWhere.ToString();
        }
    }
}
