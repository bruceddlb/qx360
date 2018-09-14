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
    /// 实训自定义时间
    /// </summary>
    public class TrainingCustomFreeTimeService : BaseSqlDataService, ITrainingCustomFreeTimeService<TrainingCustomFreeTimeEntity, TrainingCustomFreeTimeEntity, Pagination>
    {
        public int QueryCount(TrainingCustomFreeTimeEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingCustomFreeTimeEntity> GetPageList(TrainingCustomFreeTimeEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingCustomFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingCustomFreeTime.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingCustomFreeTime, TrainingCustomFreeTimeEntity>(pageList.ToList());
        }

        public List<TrainingCustomFreeTimeEntity> GetList(TrainingCustomFreeTimeEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingCustomFreeTime");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_TrainingCustomFreeTime.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingCustomFreeTime, TrainingCustomFreeTimeEntity>(list.ToList());
        }

        public TrainingCustomFreeTimeEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingCustomFreeTime.SingleOrDefault("where TrainingCustomFreeTimeId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingCustomFreeTime, TrainingCustomFreeTimeEntity>(model, null);
        }

        public bool Add(TrainingCustomFreeTimeEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingCustomFreeTimeEntity, tbl_TrainingCustomFreeTime>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingCustomFreeTimeEntity entity)
        {

            var model = tbl_TrainingCustomFreeTime.SingleOrDefault("where TrainingCustomFreeTimeId=@0", entity.TrainingCustomFreeTimeId);
            model = EntityConvertTools.CopyToModel<TrainingCustomFreeTimeEntity, tbl_TrainingCustomFreeTime>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingCustomFreeTime.Delete("where TrainingCustomFreeTimeId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingCustomFreeTimeEntity para)
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
            if (para.TimeSection != null)
            {
                sbWhere.AppendFormat(" and TimeSection='{0}'", para.TimeSection);
            }
            return sbWhere.ToString();
        }
    }
}
