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
    public class StudyFreeDateService : BaseSqlDataService, IStudyFreeDateService<StudyFreeDateEntity, StudyFreeDateEntity, Pagination>
    {
        public int QueryCount(StudyFreeDateEntity para)
        {
            throw new NotImplementedException();
        }

        public List<StudyFreeDateEntity> GetPageList(StudyFreeDateEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_StudyFreeDate.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_StudyFreeDate, StudyFreeDateEntity>(pageList.ToList());
        }

        public List<StudyFreeDateEntity> GetList(StudyFreeDateEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyFreeDate");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_StudyFreeDate.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_StudyFreeDate, StudyFreeDateEntity>(list.ToList());
        }

        public StudyFreeDateEntity GetEntity(string keyValue)
        {
            var model = tbl_StudyFreeDate.SingleOrDefault("where StudyFreeDateId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_StudyFreeDate, StudyFreeDateEntity>(model, null);
        }

        public bool Add(StudyFreeDateEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<StudyFreeDateEntity, tbl_StudyFreeDate>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(StudyFreeDateEntity entity)
        {

            var model = tbl_StudyFreeDate.SingleOrDefault("where StudyFreeDateId=@0", entity.StudyFreeDateId);
            model = EntityConvertTools.CopyToModel<StudyFreeDateEntity, tbl_StudyFreeDate>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_StudyFreeDate.Delete("where StudyFreeDateId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(StudyFreeDateEntity para)
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
            if (para.WorkdayItemId != null)
            {
                sbWhere.Append(base.FormatParameter(" AND WorkdayItemId='{0}'", para.WorkdayItemId));
            }
            return sbWhere.ToString();
        }



        public void ClearData()
        {
            //删除当前时间之前3个月的数据
            using (var tran = QX360_SQLDB.GetInstance().GetTransaction())
            {
                QX360_SQLDB.GetInstance().Execute(string.Format(@"
delete from tbl_StudyFreeTime where StudyFreeDateId in(select StudyFreeDateId from tbl_StudyFreeDate where FreeDate<='{0}')", DateTime.Now.AddMonths(-3)));

                QX360_SQLDB.GetInstance().Execute(string.Format("delete from tbl_StudyFreeDate where FreeDate<='{0}'", DateTime.Now.AddMonths(-3)));

                // Commit
                tran.Complete();
            }
        }
    }
}
