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
    public class TrainingFreeTimeService : BaseSqlDataService, ITrainingFreeTimeService<TrainingFreeTimeEntity, TrainingFreeTimeEntity, Pagination>
    {
        public int QueryCount(TrainingFreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingFreeTimeEntity> GetPageList(TrainingFreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingFreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingFreeTime, TrainingFreeTimeEntity>(pageList.ToList());
        }

        public List<TrainingFreeTimeEntity> GetList(TrainingFreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TrainingFreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingFreeTime, TrainingFreeTimeEntity>(list.ToList());
        }

        public TrainingFreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingFreeTime.SingleOrDefault("where TrainingFreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingFreeTime, TrainingFreeTimeEntity>(model, null);
        }

        public bool Add(TrainingFreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingFreeTimeEntity, tbl_TrainingFreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingFreeTimeEntity entity)
        {

            var model = tbl_TrainingFreeTime.SingleOrDefault("where TrainingFreeTimeId=@0", entity.TrainingFreeTimeId);
            model = EntityConvertTools.CopyToModel<TrainingFreeTimeEntity, tbl_TrainingFreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingFreeTime.Delete("where TrainingFreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingFreeTimeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.TrainingFreeDateId != null)
            {
                sbWhere.AppendFormat(" and TrainingFreeDateId='{0}'", para.TrainingFreeDateId);
            }
            if (para.FreeStatus != null)
            {
                sbWhere.AppendFormat(" and FreeStatus='{0}'", para.FreeStatus);
            }
            if (para.TrainingTimeTableId != null)
            {
                sbWhere.AppendFormat(" and TrainingTimeTableId='{0}'", para.TrainingTimeTableId);
            }
            if (para.TimeSection != null)
            {
                sbWhere.AppendFormat(" and TimeSection='{0}'", para.TimeSection);
            }
            return sbWhere.ToString();
        }

        public int ClearData()
        {
            return QX360_SQLDB.GetInstance().Execute("delete from tbl_TrainingFreeTime", null);
        }
    }
}
