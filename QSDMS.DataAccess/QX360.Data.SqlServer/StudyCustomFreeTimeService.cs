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
    /// 自定义学车时段
    /// </summary>
    public class StudyCustomFreeTimeService : BaseSqlDataService, IStudyCustomFreeTimeService<StudyCustomFreeTimeEntity, StudyCustomFreeTimeEntity, Pagination>
    {
        public int QueryCount(StudyCustomFreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<StudyCustomFreeTimeEntity> GetPageList(StudyCustomFreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyCustomFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_StudyCustomFreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_StudyCustomFreeTime, StudyCustomFreeTimeEntity>(pageList.ToList());
        }

        public List<StudyCustomFreeTimeEntity> GetList(StudyCustomFreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyCustomFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_StudyCustomFreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_StudyCustomFreeTime, StudyCustomFreeTimeEntity>(list.ToList());
        }

        public StudyCustomFreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_StudyCustomFreeTime.SingleOrDefault("where StudyCustomFreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_StudyCustomFreeTime, StudyCustomFreeTimeEntity>(model, null);
        }

        public bool Add(StudyCustomFreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<StudyCustomFreeTimeEntity, tbl_StudyCustomFreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(StudyCustomFreeTimeEntity entity)
        {

            var model = tbl_StudyCustomFreeTime.SingleOrDefault("where StudyCustomFreeTimeId=@0", entity.StudyCustomFreeTimeId);
            model = EntityConvertTools.CopyToModel<StudyCustomFreeTimeEntity, tbl_StudyCustomFreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_StudyCustomFreeTime.Delete("where StudyCustomFreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(StudyCustomFreeTimeEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.StudyFreeDateId != null)
            {
                sbWhere.AppendFormat(" and StudyFreeDateId='{0}'", para.StudyFreeDateId);
            }
            return sbWhere.ToString();
        }
    }
}
