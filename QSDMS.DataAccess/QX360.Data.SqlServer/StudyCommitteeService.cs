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
    public class StudyCommitteeService : BaseSqlDataService, IStudyCommitteeService<StudyCommitteeEntity, StudyCommitteeEntity, Pagination>
    {
        public int QueryCount(StudyCommitteeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<StudyCommitteeEntity> GetPageList(StudyCommitteeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_StudyCommittee.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_StudyCommittee, StudyCommitteeEntity>(pageList.ToList());
        }

        public List<StudyCommitteeEntity> GetList(StudyCommitteeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyCommittee");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_StudyCommittee.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_StudyCommittee, StudyCommitteeEntity>(list.ToList());
        }

        public StudyCommitteeEntity GetEntity(string keyValue)
        {
            var model = tbl_StudyCommittee.SingleOrDefault("where StudyCommitteeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_StudyCommittee, StudyCommitteeEntity>(model, null);
        }

        public bool Add(StudyCommitteeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<StudyCommitteeEntity, tbl_StudyCommittee>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(StudyCommitteeEntity entity)
        {

            var model = tbl_StudyCommittee.SingleOrDefault("where StudyCommitteeId=@0", entity.StudyCommitteeId);
            model = EntityConvertTools.CopyToModel<StudyCommitteeEntity, tbl_StudyCommittee>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_StudyCommittee.Delete("where StudyCommitteeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(StudyCommitteeEntity para)
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
