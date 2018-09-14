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
    public class MonthWorkDayService : BaseSqlDataService, IMonthWorkDayService<MonthWorkDayEntity, MonthWorkDayEntity, Pagination>
    {
        public int QueryCount(MonthWorkDayEntity para)
        {
            throw new NotImplementedException();
        }

        public List<MonthWorkDayEntity> GetPageList(MonthWorkDayEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_MonthWorkDay");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_MonthWorkDay.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_MonthWorkDay, MonthWorkDayEntity>(pageList.ToList());
        }

        public List<MonthWorkDayEntity> GetList(MonthWorkDayEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_MonthWorkDay");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_MonthWorkDay.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_MonthWorkDay, MonthWorkDayEntity>(list.ToList());
        }

        public MonthWorkDayEntity GetEntity(string keyValue)
        {
            var model = tbl_MonthWorkDay.SingleOrDefault("where MonthWorkDayId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_MonthWorkDay, MonthWorkDayEntity>(model, null);
        }

        public bool Add(MonthWorkDayEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<MonthWorkDayEntity, tbl_MonthWorkDay>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(MonthWorkDayEntity entity)
        {

            var model = tbl_MonthWorkDay.SingleOrDefault("where MonthWorkDayId=@0", entity.MonthWorkDayId);
            model = EntityConvertTools.CopyToModel<MonthWorkDayEntity, tbl_MonthWorkDay>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_MonthWorkDay.Delete("where MonthWorkDayId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(MonthWorkDayEntity para)
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
            if (para.YearMonth != null)
            {
                sbWhere.AppendFormat(" and YearMonth='{0}'", para.YearMonth);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND WorkDay>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND WorkDay<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            return sbWhere.ToString();
        }
    }
}
