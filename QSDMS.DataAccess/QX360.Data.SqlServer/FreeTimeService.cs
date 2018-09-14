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
    public class FreeTimeService : BaseSqlDataService, IFreeTimeService<FreeTimeEntity, FreeTimeEntity, Pagination>
    {
        public int QueryCount(FreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<FreeTimeEntity> GetPageList(FreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_FreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_FreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_FreeTime, FreeTimeEntity>(pageList.ToList());
        }

        public List<FreeTimeEntity> GetList(FreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_FreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_FreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_FreeTime, FreeTimeEntity>(list.ToList());
        }

        public FreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_FreeTime.SingleOrDefault("where FreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_FreeTime, FreeTimeEntity>(model, null);
        }

        public bool Add(FreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<FreeTimeEntity, tbl_FreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(FreeTimeEntity entity)
        {

            var model = tbl_FreeTime.SingleOrDefault("where FreeTimeId=@0", entity.FreeTimeId);
            model = EntityConvertTools.CopyToModel<FreeTimeEntity, tbl_FreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_FreeTime.Delete("where FreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(FreeTimeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.FreeDateId != null)
            {
                sbWhere.AppendFormat(" and FreeDateId='{0}'", para.FreeDateId);
            }
            if (para.FreeStatus != null)
            {
                sbWhere.AppendFormat(" and FreeStatus='{0}'", para.FreeStatus);
            }
            return sbWhere.ToString();
        }

        public int ClearData()
        {
           return QX360_SQLDB.GetInstance().Execute("delete from tbl_FreeTime",null);
        }
    }
}
