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
    public class TrainingOrderDetailService : BaseSqlDataService, ITrainingOrderDetailService<TrainingOrderDetailEntity, TrainingOrderDetailEntity, Pagination>
    {
        public int QueryCount(TrainingOrderDetailEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingOrderDetailEntity> GetPageList(TrainingOrderDetailEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingOrderDetail");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingOrderDetail.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingOrderDetail, TrainingOrderDetailEntity>(pageList.ToList());
        }

        public List<TrainingOrderDetailEntity> GetList(TrainingOrderDetailEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingOrderDetail");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TrainingOrderDetail.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingOrderDetail, TrainingOrderDetailEntity>(list.ToList());
        }

        public TrainingOrderDetailEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingOrderDetail.SingleOrDefault("where TrainingOrderDetailId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingOrderDetail, TrainingOrderDetailEntity>(model, null);
        }

        public bool Add(TrainingOrderDetailEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingOrderDetailEntity, tbl_TrainingOrderDetail>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingOrderDetailEntity entity)
        {

            var model = tbl_TrainingOrderDetail.SingleOrDefault("where TrainingOrderDetailId=@0", entity.TrainingOrderDetailId);
            model = EntityConvertTools.CopyToModel<TrainingOrderDetailEntity, tbl_TrainingOrderDetail>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingOrderDetail.Delete("where TrainingOrderDetailId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingOrderDetailEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.TrainingOrderId != null)
            {
                sbWhere.AppendFormat(" and TrainingOrderId='{0}'", para.TrainingOrderId);
            }
            if (para.TrainingFreeTimeId != null)
            {
                sbWhere.AppendFormat(" and TrainingFreeTimeId='{0}'", para.TrainingFreeTimeId);
            }
            return sbWhere.ToString();
        }
    }
}
