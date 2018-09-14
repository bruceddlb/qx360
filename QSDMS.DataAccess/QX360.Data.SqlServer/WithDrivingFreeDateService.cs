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
    /// 陪驾日期设置
    /// </summary>
    public class WithDrivingFreeDateService : BaseSqlDataService, IWithDrivingFreeDateService<WithDrivingFreeDateEntity, WithDrivingFreeDateEntity, Pagination>
    {
        public int QueryCount(WithDrivingFreeDateEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WithDrivingFreeDateEntity> GetPageList(WithDrivingFreeDateEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WithDrivingFreeDate.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WithDrivingFreeDate, WithDrivingFreeDateEntity>(pageList.ToList());
        }

        public List<WithDrivingFreeDateEntity> GetList(WithDrivingFreeDateEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WithDrivingFreeDate.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WithDrivingFreeDate, WithDrivingFreeDateEntity>(list.ToList());
        }

        public WithDrivingFreeDateEntity GetEntity(string keyValue)
        {
            var model = tbl_WithDrivingFreeDate.SingleOrDefault("where WithDrivingFreeDateId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WithDrivingFreeDate, WithDrivingFreeDateEntity>(model, null);
        }

        public bool Add(WithDrivingFreeDateEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WithDrivingFreeDateEntity, tbl_WithDrivingFreeDate>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WithDrivingFreeDateEntity entity)
        {

            var model = tbl_WithDrivingFreeDate.SingleOrDefault("where WithDrivingFreeDateId=@0", entity.WithDrivingFreeDateId);
            model = EntityConvertTools.CopyToModel<WithDrivingFreeDateEntity, tbl_WithDrivingFreeDate>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WithDrivingFreeDate.Delete("where WithDrivingFreeDateId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WithDrivingFreeDateEntity para)
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
    }
}
