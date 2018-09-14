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
    /// 规则管理
    /// </summary>
    public class RuleService : BaseSqlDataService, IRuleService<RuleEntity, RuleEntity, Pagination>
    {
        public int QueryCount(RuleEntity para)
        {
            throw new NotImplementedException();
        }

        public List<RuleEntity> GetPageList(RuleEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Rule");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Rule.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Rule, RuleEntity>(pageList.ToList());
        }

        public List<RuleEntity> GetList(RuleEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Rule");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Rule.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Rule, RuleEntity>(list.ToList());
        }

        public RuleEntity GetEntity(string keyValue)
        {
            var model = tbl_Rule.SingleOrDefault("where RuleId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Rule, RuleEntity>(model, null);
        }

        public bool Add(RuleEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<RuleEntity, tbl_Rule>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(RuleEntity entity)
        {

            var model = tbl_Rule.SingleOrDefault("where RuleId=@0", entity.RuleId);
            model = EntityConvertTools.CopyToModel<RuleEntity, tbl_Rule>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Rule.Delete("where RuleId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(RuleEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.RuleOperation != null)
            {
                sbWhere.AppendFormat(" and RuleOperation='{0}'", para.RuleOperation);
            }
            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }
          
            return sbWhere.ToString();
        }
    }
}
