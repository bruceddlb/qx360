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
    /// 考试
    /// </summary>
    public class ExamOrderService : BaseSqlDataService, IExamOrderService<ExamOrderEntity, ExamOrderEntity, Pagination>
    {
        public int QueryCount(ExamOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ExamOrderEntity> GetPageList(ExamOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ExamOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_ExamOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_ExamOrder, ExamOrderEntity>(pageList.ToList());
        }

        public List<ExamOrderEntity> GetList(ExamOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ExamOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_ExamOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_ExamOrder, ExamOrderEntity>(list.ToList());
        }

        public ExamOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_ExamOrder.SingleOrDefault("where ExamId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_ExamOrder, ExamOrderEntity>(model, null);
        }

        public bool Add(ExamOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ExamOrderEntity, tbl_ExamOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ExamOrderEntity entity)
        {

            var model = tbl_ExamOrder.SingleOrDefault("where ExamId=@0", entity.ExamId);
            model = EntityConvertTools.CopyToModel<ExamOrderEntity, tbl_ExamOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_ExamOrder.Delete("where ExamId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ExamOrderEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberMobile)>0)", para.MemberMobile);
            }
            if (para.SubjectId != null)
            {
                sbWhere.AppendFormat(" and SubjectId='{0}'", para.SubjectId);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.CheckIds != null)
            {
                var str = "";
                foreach (var item in para.CheckIds)
                {
                    str += string.Format("'{0}',", item);
                }
                str = str.Substring(0, str.Length - 1);
                sbWhere.AppendFormat(" and ExamId in({0})", str);
            }
            return sbWhere.ToString();
        }
    }
}
