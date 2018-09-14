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
    public class WorkTimeTableService : BaseSqlDataService, IWorkTimeTableService<WorkTimeTableEntity, WorkTimeTableEntity, Pagination>
    {
        public int QueryCount(WorkTimeTableEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WorkTimeTableEntity> GetPageList(WorkTimeTableEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WorkTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WorkTimeTable.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WorkTimeTable, WorkTimeTableEntity>(pageList.ToList());
        }

        public List<WorkTimeTableEntity> GetList(WorkTimeTableEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WorkTimeTable");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_WorkTimeTable.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WorkTimeTable, WorkTimeTableEntity>(list.ToList());
        }

        public WorkTimeTableEntity GetEntity(string keyValue)
        {
            var model = tbl_WorkTimeTable.SingleOrDefault("where WorkTimeTableId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WorkTimeTable, WorkTimeTableEntity>(model, null);
        }

        public bool Add(WorkTimeTableEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WorkTimeTableEntity, tbl_WorkTimeTable>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WorkTimeTableEntity entity)
        {

            var model = tbl_WorkTimeTable.SingleOrDefault("where WorkTimeTableId=@0", entity.WorkTimeTableId);
            model = EntityConvertTools.CopyToModel<WorkTimeTableEntity, tbl_WorkTimeTable>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WorkTimeTable.Delete("where WorkTimeTableId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WorkTimeTableEntity para)
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
