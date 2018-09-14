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
    /// 支付日志
    /// </summary>
    public class TradeLogService : BaseSqlDataService, ITradeLogService<TradeLogEntity, TradeLogEntity, Pagination>
    {
        public int QueryCount(TradeLogEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TradeLogEntity> GetPageList(TradeLogEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TradeLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TradeLog.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TradeLog, TradeLogEntity>(pageList.ToList());
        }

        public List<TradeLogEntity> GetList(TradeLogEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TradeLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TradeLog.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TradeLog, TradeLogEntity>(list.ToList());
        }

        public TradeLogEntity GetEntity(string keyValue)
        {
            var model = tbl_TradeLog.SingleOrDefault("where TradeLogId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TradeLog, TradeLogEntity>(model, null);
        }

        public bool Add(TradeLogEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TradeLogEntity, tbl_TradeLog>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TradeLogEntity entity)
        {

            var model = tbl_TradeLog.SingleOrDefault("where TradeLogId=@0", entity.TradeLogId);
            model = EntityConvertTools.CopyToModel<TradeLogEntity, tbl_TradeLog>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TradeLog.Delete("where TradeLogId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TradeLogEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.AccountId != null)
            {
                sbWhere.AppendFormat(" and AccountId='{0}'", para.AccountId);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND AddTime>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND AddTime<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }

            return sbWhere.ToString();
        }
    }
}
