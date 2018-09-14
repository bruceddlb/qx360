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
    public class StudyOrderDetailService : BaseSqlDataService, IStudyOrderDetailService<StudyOrderDetailEntity, StudyOrderDetailEntity, Pagination>
    {
        public int QueryCount(StudyOrderDetailEntity para)
        {
            throw new NotImplementedException();
        }

        public List<StudyOrderDetailEntity> GetPageList(StudyOrderDetailEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyOrderDetail");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_StudyOrderDetail.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_StudyOrderDetail, StudyOrderDetailEntity>(pageList.ToList());
        }

        public List<StudyOrderDetailEntity> GetList(StudyOrderDetailEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_StudyOrderDetail");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_StudyOrderDetail.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_StudyOrderDetail, StudyOrderDetailEntity>(list.ToList());
        }

        public StudyOrderDetailEntity GetEntity(string keyValue)
        {
            var model = tbl_StudyOrderDetail.SingleOrDefault("where StudyOrderDetailId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_StudyOrderDetail, StudyOrderDetailEntity>(model, null);
        }

        public bool Add(StudyOrderDetailEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<StudyOrderDetailEntity, tbl_StudyOrderDetail>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(StudyOrderDetailEntity entity)
        {

            var model = tbl_StudyOrderDetail.SingleOrDefault("where StudyOrderDetailId=@0", entity.StudyOrderDetailId);
            model = EntityConvertTools.CopyToModel<StudyOrderDetailEntity, tbl_StudyOrderDetail>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_StudyOrderDetail.Delete("where StudyOrderDetailId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(StudyOrderDetailEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }          
            if (para.StudyOrderId != null)
            {
                sbWhere.AppendFormat(" and StudyOrderId='{0}'", para.StudyOrderId);
            }

            return sbWhere.ToString();
        }
    }
}
