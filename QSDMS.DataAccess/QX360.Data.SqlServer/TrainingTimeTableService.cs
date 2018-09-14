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
    /// 意见管理
    /// </summary>
    public class TrainingTimeTableService : BaseSqlDataService, ITrainingTimeTableService<TrainingTimeTableEntity, TrainingTimeTableEntity, Pagination>
    {
        public int QueryCount(TrainingTimeTableEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingTimeTableEntity> GetPageList(TrainingTimeTableEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingTimeTable.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingTimeTable, TrainingTimeTableEntity>(pageList.ToList());
        }

        public List<TrainingTimeTableEntity> GetList(TrainingTimeTableEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TrainingTimeTable.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingTimeTable, TrainingTimeTableEntity>(list.ToList());
        }

        public TrainingTimeTableEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingTimeTable.SingleOrDefault("where TrainingTimeTableId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingTimeTable, TrainingTimeTableEntity>(model, null);
        }

        public bool Add(TrainingTimeTableEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingTimeTableEntity, tbl_TrainingTimeTable>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingTimeTableEntity entity)
        {

            var model = tbl_TrainingTimeTable.SingleOrDefault("where TrainingTimeTableId=@0", entity.TrainingTimeTableId);
            model = EntityConvertTools.CopyToModel<TrainingTimeTableEntity, tbl_TrainingTimeTable>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingTimeTable.Delete("where TrainingTimeTableId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingTimeTableEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }

            return sbWhere.ToString();
        }
    }
}
