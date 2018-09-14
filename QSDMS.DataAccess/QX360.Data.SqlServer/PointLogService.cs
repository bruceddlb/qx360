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
    /// 积分日志
    /// </summary>
    public class PointLogService : BaseSqlDataService, IPointLogService<PointLogEntity, PointLogEntity, Pagination>
    {
        public int QueryCount(PointLogEntity para)
        {
            throw new NotImplementedException();
        }

        public List<PointLogEntity> GetPageList(PointLogEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_PointLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_PointLog.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_PointLog, PointLogEntity>(pageList.ToList());
        }

        public List<PointLogEntity> GetList(PointLogEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_PointLog");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(para.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", para.sidx, para.sord);
            }
            var list = tbl_PointLog.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_PointLog, PointLogEntity>(list.ToList());
        }

        public PointLogEntity GetEntity(string keyValue)
        {
            var model = tbl_PointLog.SingleOrDefault("where PointLogId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_PointLog, PointLogEntity>(model, null);
        }

        public bool Add(PointLogEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<PointLogEntity, tbl_PointLog>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(PointLogEntity entity)
        {

            var model = tbl_PointLog.SingleOrDefault("where PointLogId=@0", entity.PointLogId);
            model = EntityConvertTools.CopyToModel<PointLogEntity, tbl_PointLog>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_PointLog.Delete("where PointLogId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(PointLogEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.StartTime != null)
            {
                sbWhere.AppendFormat(" and CONVERT(varchar(10), AddTime, 120)='{0}'", para.StartTime);
            }
            if (para.ObjectId != null)
            {
                sbWhere.AppendFormat(" and ObjectId='{0}'", para.ObjectId);
            }

            return sbWhere.ToString();
        }
    }
}
