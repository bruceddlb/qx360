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
    public class CourseItemService : BaseSqlDataService, ICourseItemService<CourseItemEntity, CourseItemEntity, Pagination>
    {
        public int QueryCount(CourseItemEntity para)
        {
            throw new NotImplementedException();
        }

        public List<CourseItemEntity> GetPageList(CourseItemEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_CourseItem");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_CourseItem.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_CourseItem, CourseItemEntity>(pageList.ToList());
        }

        public List<CourseItemEntity> GetList(CourseItemEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_CourseItem");
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
            var list = tbl_CourseItem.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_CourseItem, CourseItemEntity>(list.ToList());
        }

        public CourseItemEntity GetEntity(string keyValue)
        {
            var model = tbl_CourseItem.SingleOrDefault("where CourseItemId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_CourseItem, CourseItemEntity>(model, null);
        }

        public bool Add(CourseItemEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<CourseItemEntity, tbl_CourseItem>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(CourseItemEntity entity)
        {

            var model = tbl_CourseItem.SingleOrDefault("where CourseItemId=@0", entity.CourseItemId);
            model = EntityConvertTools.CopyToModel<CourseItemEntity, tbl_CourseItem>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_CourseItem.Delete("where CourseItemId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(CourseItemEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.CourseId != null)
            {
                sbWhere.AppendFormat(" and CourseId='{0}'", para.CourseId);
            }
            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }

            return sbWhere.ToString();
        }
    }
}
