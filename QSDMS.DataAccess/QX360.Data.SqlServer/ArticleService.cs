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
    /// 文章
    /// </summary>
    public class ArticleService : BaseSqlDataService, IArticleService<ArticleEntity, ArticleEntity, Pagination>
    {
        public int QueryCount(ArticleEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ArticleEntity> GetPageList(ArticleEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Article");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Article.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Article, ArticleEntity>(pageList.ToList());
        }

        public List<ArticleEntity> GetList(ArticleEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Article");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Article.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Article, ArticleEntity>(list.ToList());
        }

        public ArticleEntity GetEntity(string keyValue)
        {
            var model = tbl_Article.SingleOrDefault("where ArticleId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Article, ArticleEntity>(model, null);
        }

        public bool Add(ArticleEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ArticleEntity, tbl_Article>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ArticleEntity entity)
        {

            var model = tbl_Article.SingleOrDefault("where ArticleId=@0", entity.ArticleId);
            model = EntityConvertTools.CopyToModel<ArticleEntity, tbl_Article>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Article.Delete("where ArticleId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ArticleEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.Title != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Title)>0)", para.Title);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            return sbWhere.ToString();
        }
    }
}
