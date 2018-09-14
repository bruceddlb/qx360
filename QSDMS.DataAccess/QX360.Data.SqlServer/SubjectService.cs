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
    /// 驾校科目
    /// </summary>
    public class SubjectService : BaseSqlDataService, ISubjectService<SubjectEntity, SubjectEntity, Pagination>
    {
        public int QueryCount(SubjectEntity para)
        {
            throw new NotImplementedException();
        }

        public List<SubjectEntity> GetPageList(SubjectEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Subject");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Subject.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Subject, SubjectEntity>(pageList.ToList());
        }

        public List<SubjectEntity> GetList(SubjectEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Subject");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Subject.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Subject, SubjectEntity>(list.ToList());
        }

        public SubjectEntity GetEntity(string keyValue)
        {
            var model = tbl_Subject.SingleOrDefault("where SubjectId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Subject, SubjectEntity>(model, null);
        }

        public bool Add(SubjectEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<SubjectEntity, tbl_Subject>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(SubjectEntity entity)
        {

            var model = tbl_Subject.SingleOrDefault("where SubjectId=@0", entity.SubjectId);
            model = EntityConvertTools.CopyToModel<SubjectEntity, tbl_Subject>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Subject.Delete("where SubjectId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(SubjectEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.SubjectName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SubjectName)>0)", para.SubjectName);
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.ItemId != null)
            {
                sbWhere.AppendFormat(" and ItemId='{0}'", para.ItemId);
            }
          
            return sbWhere.ToString();
        }
    }
}
