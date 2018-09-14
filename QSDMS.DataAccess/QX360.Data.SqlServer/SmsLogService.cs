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
    /// 短信发送日志
    /// </summary>
    public class SmsLogService : BaseSqlDataService, ISmsLogService<SmsLogEntity, SmsLogEntity, Pagination>
    {
        public int QueryCount(SmsLogEntity para)
        {
            throw new NotImplementedException();
        }

        public List<SmsLogEntity> GetPageList(SmsLogEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_SmsLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_SmsLog.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_SmsLog, SmsLogEntity>(pageList.ToList());
        }

        public List<SmsLogEntity> GetList(SmsLogEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_SmsLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_SmsLog.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_SmsLog, SmsLogEntity>(list.ToList());
        }

        public SmsLogEntity GetEntity(string keyValue)
        {
            var model = tbl_SmsLog.SingleOrDefault("where SmsLogId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_SmsLog, SmsLogEntity>(model, null);
        }

        public bool Add(SmsLogEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<SmsLogEntity, tbl_SmsLog>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(SmsLogEntity entity)
        {

            var model = tbl_SmsLog.SingleOrDefault("where SmsLogId=@0", entity.SmsLogId);
            model = EntityConvertTools.CopyToModel<SmsLogEntity, tbl_SmsLog>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_SmsLog.Delete("where SmsLogId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(SmsLogEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.Caption != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Caption)>0)", para.Caption);
            }
            if (para.RecivMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',RecivMobile)>0)", para.RecivMobile);
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
                sbWhere.AppendFormat(" and Status={0}", para.Status);
            }
            if (para.NoticeType != null)
            {
                sbWhere.AppendFormat(" and NoticeType={0}", para.NoticeType);
            }

            return sbWhere.ToString();
        }
    }
}
