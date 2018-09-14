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
    /// 空闲时间
    /// </summary>
    public class FreeDateService : BaseSqlDataService, IFreeDateService<FreeDateEntity, FreeDateEntity, Pagination>
    {
        public int QueryCount(FreeDateEntity para)
        {
            throw new NotImplementedException();
        }

        public List<FreeDateEntity> GetPageList(FreeDateEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_FreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_FreeDate.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_FreeDate, FreeDateEntity>(pageList.ToList());
        }

        public List<FreeDateEntity> GetList(FreeDateEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_FreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_FreeDate.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_FreeDate, FreeDateEntity>(list.ToList());
        }

        public FreeDateEntity GetEntity(string keyValue)
        {
            var model = tbl_FreeDate.SingleOrDefault("where FreeDateId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_FreeDate, FreeDateEntity>(model, null);
        }

        public bool Add(FreeDateEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<FreeDateEntity, tbl_FreeDate>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(FreeDateEntity entity)
        {

            var model = tbl_FreeDate.SingleOrDefault("where FreeDateId=@0", entity.FreeDateId);
            model = EntityConvertTools.CopyToModel<FreeDateEntity, tbl_FreeDate>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_FreeDate.Delete("where FreeDateId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(FreeDateEntity para)
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
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND FreeDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND FreeDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            return sbWhere.ToString();
        }



        public int ClearData()
        {
            return QX360_SQLDB.GetInstance().Execute("delete from tbl_FreeDate",null);
        }
    }
}
