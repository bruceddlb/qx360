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
    /// 年检机构评价
    /// </summary>
    public class AuditCommitteeService : BaseSqlDataService, IAuditCommitteeService<AuditCommitteeEntity, AuditCommitteeEntity, Pagination>
    {
        public int QueryCount(AuditCommitteeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditCommitteeEntity> GetPageList(AuditCommitteeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditCommittee.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditCommittee, AuditCommitteeEntity>(pageList.ToList());
        }

        public List<AuditCommitteeEntity> GetList(AuditCommitteeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_AuditCommittee.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditCommittee, AuditCommitteeEntity>(list.ToList());
        }

        public AuditCommitteeEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditCommittee.SingleOrDefault("where AuditCommitteeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditCommittee, AuditCommitteeEntity>(model, null);
        }

        public bool Add(AuditCommitteeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditCommitteeEntity, tbl_AuditCommittee>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditCommitteeEntity entity)
        {

            var model = tbl_AuditCommittee.SingleOrDefault("where AuditCommitteeId=@0", entity.AuditCommitteeId);
            model = EntityConvertTools.CopyToModel<AuditCommitteeEntity, tbl_AuditCommittee>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditCommittee.Delete("where AuditCommitteeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditCommitteeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.OrganizationId != null)
            {
                sbWhere.AppendFormat(" and OrganizationId='{0}'", para.OrganizationId);
            }
            if (para.OrderId != null)
            {
                sbWhere.AppendFormat(" and OrderId='{0}'", para.OrderId);
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.OrganizationName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',OrganizationName)>0)", para.OrganizationName);
            }
            if (para.CommitContent != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CommitContent)>0)", para.CommitContent);
            }
            return sbWhere.ToString();
        }
    }
}
