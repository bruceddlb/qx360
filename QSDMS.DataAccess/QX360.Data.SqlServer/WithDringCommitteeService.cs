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
    public class WithDringCommitteeService : BaseSqlDataService, IWithDringCommitteeService<WithDringCommitteeEntity, WithDringCommitteeEntity, Pagination>
    {
        public int QueryCount(WithDringCommitteeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WithDringCommitteeEntity> GetPageList(WithDringCommitteeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDringCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WithDringCommittee.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WithDringCommittee, WithDringCommitteeEntity>(pageList.ToList());
        }

        public List<WithDringCommitteeEntity> GetList(WithDringCommitteeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDringCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WithDringCommittee.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WithDringCommittee, WithDringCommitteeEntity>(list.ToList());
        }

        public WithDringCommitteeEntity GetEntity(string keyValue)
        {
            var model = tbl_WithDringCommittee.SingleOrDefault("where WithDringCommitteeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WithDringCommittee, WithDringCommitteeEntity>(model, null);
        }

        public bool Add(WithDringCommitteeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WithDringCommitteeEntity, tbl_WithDringCommittee>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WithDringCommitteeEntity entity)
        {

            var model = tbl_WithDringCommittee.SingleOrDefault("where WithDringCommitteeId=@0", entity.WithDringCommitteeId);
            model = EntityConvertTools.CopyToModel<WithDringCommitteeEntity, tbl_WithDringCommittee>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WithDringCommittee.Delete("where WithDringCommitteeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WithDringCommitteeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.TeacherId != null)
            {
                sbWhere.AppendFormat(" and TeacherId='{0}'", para.TeacherId);
            }
            if (para.WithDringOrderId != null)
            {
                sbWhere.AppendFormat(" and WithDringOrderId='{0}'", para.WithDringOrderId);
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.TeacherName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',TeacherName)>0)", para.TeacherName);
            }
            if (para.CommitContent != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CommitContent)>0)", para.CommitContent);
            }

            return sbWhere.ToString();
        }
    }
}
