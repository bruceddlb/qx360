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
    public class CourseService : BaseSqlDataService, ICourseService<CourseEntity, CourseEntity, Pagination>
    {
        public int QueryCount(CourseEntity para)
        {
            throw new NotImplementedException();
        }

        public List<CourseEntity> GetPageList(CourseEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Course");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Course.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Course, CourseEntity>(pageList.ToList());
        }

        public List<CourseEntity> GetList(CourseEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Course");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (para != null)
            {
                if (!string.IsNullOrWhiteSpace(para.sidx))
                {
                    sql.AppendFormat(" order by {0} {1}", para.sidx, para.sord);
                }
            }
            var list = tbl_Course.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Course, CourseEntity>(list.ToList());
        }

        public CourseEntity GetEntity(string keyValue)
        {
            var model = tbl_Course.SingleOrDefault("where CourseId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Course, CourseEntity>(model, null);
        }

        public bool Add(CourseEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<CourseEntity, tbl_Course>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(CourseEntity entity)
        {

            var model = tbl_Course.SingleOrDefault("where CourseId=@0", entity.CourseId);
            model = EntityConvertTools.CopyToModel<CourseEntity, tbl_Course>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Course.Delete("where CourseId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(CourseEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ClassId != null)
            {
                sbWhere.AppendFormat(" and ClassId='{0}'", para.ClassId);
            }
            if (para.CourseName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CourseName)>0)", para.CourseName);
            }

            return sbWhere.ToString();
        }
    }
}
