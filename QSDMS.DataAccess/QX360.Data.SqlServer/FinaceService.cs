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
    /// 财务报表
    /// </summary>
    public class FinaceService : BaseSqlDataService, IFinaceService<FinaceEntity, FinaceEntity, Pagination>
    {
        public int QueryCount(FinaceEntity para)
        {
            throw new NotImplementedException();
        }

        public List<FinaceEntity> GetPageList(FinaceEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Finace");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Finace.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Finace, FinaceEntity>(pageList.ToList());
        }

        public List<FinaceEntity> GetList(FinaceEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Finace");
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
            var list = tbl_Finace.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Finace, FinaceEntity>(list.ToList());
        }

        public FinaceEntity GetEntity(string keyValue)
        {
            var model = tbl_Finace.SingleOrDefault("where FinaceId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Finace, FinaceEntity>(model, null);
        }

        public bool Add(FinaceEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<FinaceEntity, tbl_Finace>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(FinaceEntity entity)
        {

            var model = tbl_Finace.SingleOrDefault("where FinaceId=@0", entity.FinaceId);
            model = EntityConvertTools.CopyToModel<FinaceEntity, tbl_Finace>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Finace.Delete("where FinaceId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(FinaceEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();
            //当前用户对象
            var loginOperater = OperatorProvider.Provider.Current();
            if (loginOperater != null)
            {
                List<string> userDataAuthorize = loginOperater.UserDataAuthorize;
                if (userDataAuthorize != null && userDataAuthorize.Count > 0)
                {
                    var str = "";
                    foreach (var item in userDataAuthorize)
                    {
                        str += string.Format("'{0}',", item);
                    }
                    str = str.Substring(0, str.Length - 1);
                    sbWhere.AppendFormat(" and ObjectId in({0})", str);
                }
            }
            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.SourceType != null)
            {
                sbWhere.AppendFormat(" and SourceType='{0}'", para.SourceType);
            }

            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND CreateTime>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND CreateTime<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.Operate != null)
            {
                sbWhere.AppendFormat(" and Operate='{0}'", para.Operate);
            }
            return sbWhere.ToString();
        }

    }
}
