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
    /// 陪驾空闲时间
    /// </summary>
    public class WithDrivingFreeTimeService : BaseSqlDataService, IWithDrivingFreeTimeService<WithDrivingFreeTimeEntity, WithDrivingFreeTimeEntity, Pagination>
    {
        public int QueryCount(WithDrivingFreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WithDrivingFreeTimeEntity> GetPageList(WithDrivingFreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WithDrivingFreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WithDrivingFreeTime, WithDrivingFreeTimeEntity>(pageList.ToList());
        }

        public List<WithDrivingFreeTimeEntity> GetList(WithDrivingFreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WithDrivingFreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WithDrivingFreeTime, WithDrivingFreeTimeEntity>(list.ToList());
        }

        public WithDrivingFreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_WithDrivingFreeTime.SingleOrDefault("where WithDrivingFreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WithDrivingFreeTime, WithDrivingFreeTimeEntity>(model, null);
        }

        public bool Add(WithDrivingFreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WithDrivingFreeTimeEntity, tbl_WithDrivingFreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WithDrivingFreeTimeEntity entity)
        {

            var model = tbl_WithDrivingFreeTime.SingleOrDefault("where WithDrivingFreeTimeId=@0", entity.WithDrivingFreeTimeId);
            model = EntityConvertTools.CopyToModel<WithDrivingFreeTimeEntity, tbl_WithDrivingFreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WithDrivingFreeTime.Delete("where WithDrivingFreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WithDrivingFreeTimeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.WithDrivingFreeDateId != null)
            {
                sbWhere.AppendFormat(" and WithDrivingFreeDateId='{0}'", para.WithDrivingFreeDateId);
            }
            if (para.FreeStatus != null)
            {
                sbWhere.AppendFormat(" and FreeStatus='{0}'", para.FreeStatus);
            }
            if (para.TimeSection != null)
            {
                sbWhere.AppendFormat(" and TimeSection='{0}'", para.TimeSection);
            }
            return sbWhere.ToString();
        }
    }
}
