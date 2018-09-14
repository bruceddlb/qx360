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
    public class ClassService : BaseSqlDataService, IClassService<ClassEntity, ClassEntity, Pagination>
    {
        public int QueryCount(ClassEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ClassEntity> GetPageList(ClassEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Class");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Class.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Class, ClassEntity>(pageList.ToList());
        }

        public List<ClassEntity> GetList(ClassEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Class");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Class.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Class, ClassEntity>(list.ToList());
        }

        public ClassEntity GetEntity(string keyValue)
        {
            var model = tbl_Class.SingleOrDefault("where ClassId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Class, ClassEntity>(model, null);
        }

        public bool Add(ClassEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ClassEntity, tbl_Class>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ClassEntity entity)
        {

            var model = tbl_Class.SingleOrDefault("where ClassId=@0", entity.ClassId);
            model = EntityConvertTools.CopyToModel<ClassEntity, tbl_Class>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Class.Delete("where ClassId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ClassEntity para)
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
            if (para.ClassName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ClassName)>0)", para.ClassName);
            }

            return sbWhere.ToString();
        }
    }
}
