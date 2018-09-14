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
    public class TeacherCommitteeService : BaseSqlDataService, ITeacherCommitteeService<TeacherCommitteeEntity, TeacherCommitteeEntity, Pagination>
    {
        public int QueryCount(TeacherCommitteeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TeacherCommitteeEntity> GetPageList(TeacherCommitteeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TeacherCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TeacherCommittee.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TeacherCommittee, TeacherCommitteeEntity>(pageList.ToList());
        }

        public List<TeacherCommitteeEntity> GetList(TeacherCommitteeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TeacherCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TeacherCommittee.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TeacherCommittee, TeacherCommitteeEntity>(list.ToList());
        }

        public TeacherCommitteeEntity GetEntity(string keyValue)
        {
            var model = tbl_TeacherCommittee.SingleOrDefault("where TeacherCommitteeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TeacherCommittee, TeacherCommitteeEntity>(model, null);
        }

        public bool Add(TeacherCommitteeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TeacherCommitteeEntity, tbl_TeacherCommittee>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TeacherCommitteeEntity entity)
        {

            var model = tbl_TeacherCommittee.SingleOrDefault("where TeacherCommitteeId=@0", entity.TeacherCommitteeId);
            model = EntityConvertTools.CopyToModel<TeacherCommitteeEntity, tbl_TeacherCommittee>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TeacherCommittee.Delete("where TeacherCommitteeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TeacherCommitteeEntity para)
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
            if (para.StudyOrderId != null)
            {
                sbWhere.AppendFormat(" and StudyOrderId='{0}'", para.StudyOrderId);
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
