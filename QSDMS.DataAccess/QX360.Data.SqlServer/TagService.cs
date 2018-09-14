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
    /// 标签管理
    /// </summary>
    public class TagService : BaseSqlDataService, ITagService<TagEntity, TagEntity, Pagination>
    {
        public int QueryCount(TagEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TagEntity> GetPageList(TagEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Tag");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Tag.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Tag, TagEntity>(pageList.ToList());
        }

        public List<TagEntity> GetList(TagEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Tag");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Tag.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Tag, TagEntity>(list.ToList());
        }

        public TagEntity GetEntity(string keyValue)
        {
            var model = tbl_Tag.SingleOrDefault("where TagId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Tag, TagEntity>(model, null);
        }

        public bool Add(TagEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TagEntity, tbl_Tag>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TagEntity entity)
        {

            var model = tbl_Tag.SingleOrDefault("where TagId=@0", entity.TagId);
            model = EntityConvertTools.CopyToModel<TagEntity, tbl_Tag>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Tag.Delete("where TagId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TagEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ObjectId != null)
            {
                sbWhere.AppendFormat(" and ObjectId='{0}'", para.ObjectId);
            }
            return sbWhere.ToString();
        }
    }
}
