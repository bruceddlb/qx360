using QSDMS.Model;
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
    /// 微信消息模板
    /// </summary>
    public class WxTemplateService : IWxTemplateService<WxTemplateEntity, WxTemplateEntity, Pagination>
    {
        public int QueryCount(WxTemplateEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WxTemplateEntity> GetPageList(WxTemplateEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WxTemplate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WxTemplate.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WxTemplate, WxTemplateEntity>(pageList.ToList());
        }

        public List<WxTemplateEntity> GetList(WxTemplateEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WxTemplate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WxTemplate.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WxTemplate, WxTemplateEntity>(list.ToList());
        }

        public WxTemplateEntity GetEntity(string keyValue)
        {
            var model = tbl_WxTemplate.SingleOrDefault("where WxTemplateId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WxTemplate, WxTemplateEntity>(model, null);
        }

        public bool Add(WxTemplateEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WxTemplateEntity, tbl_WxTemplate>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WxTemplateEntity entity)
        {

            var model = tbl_WxTemplate.SingleOrDefault("where WxTemplateId=@0", entity.WxTemplateId);
            model = EntityConvertTools.CopyToModel<WxTemplateEntity, tbl_WxTemplate>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WxTemplate.Delete("where WxTemplateId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WxTemplateEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.Call_index != null)
            {
                sbWhere.AppendFormat(" and Call_index='{0}'", para.Call_index);
            }
            if (para.Title != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Title)>0)", para.Title);
            }
            if (para.TemplateId != null)
            {
                sbWhere.AppendFormat(" and TemplateId='{0}'", para.TemplateId);
            }

            return sbWhere.ToString();
        }
    }
}
