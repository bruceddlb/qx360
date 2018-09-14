using QSDMS.Model;
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
    /// 驾校报名订单
    /// </summary>
    public class ApplyOrderService :BaseSqlDataService, IApplyOrderService<ApplyOrderEntity, ApplyOrderEntity, Pagination>
    {
        public int QueryCount(ApplyOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ApplyOrderEntity> GetPageList(ApplyOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ApplyOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_ApplyOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_ApplyOrder, ApplyOrderEntity>(pageList.ToList());
        }

        public List<ApplyOrderEntity> GetList(ApplyOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ApplyOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_ApplyOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_ApplyOrder, ApplyOrderEntity>(list.ToList());
        }

        public ApplyOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_ApplyOrder.SingleOrDefault("where ApplyOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_ApplyOrder, ApplyOrderEntity>(model, null);
        }

        public bool Add(ApplyOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ApplyOrderEntity, tbl_ApplyOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ApplyOrderEntity entity)
        {

            var model = tbl_ApplyOrder.SingleOrDefault("where ApplyOrderId=@0", entity.ApplyOrderId);
            model = EntityConvertTools.CopyToModel<ApplyOrderEntity, tbl_ApplyOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_ApplyOrder.Delete("where ApplyOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ApplyOrderEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();
            //当前用户对象
            var loginOperater = OperatorProvider.Provider.Current();
            if (loginOperater != null)
            {
                List<string> userDataAuthorize = loginOperater.UserDataAuthorize;
                if (userDataAuthorize != null && userDataAuthorize.Count > 0)
                {
                    var str = "";
                    foreach (var item in userDataAuthorize)
                    {
                        str += string.Format("'{0}',", item);
                    }
                    str = str.Substring(0, str.Length - 1);
                    sbWhere.AppendFormat(" and SchoolId in({0})", str);
                }
            }
            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.AddressInfo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',AddressInfo)>0)", para.AddressInfo);
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberMobile)>0)", para.MemberMobile);
            }
            if (para.ApplyOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ApplyOrderNo)>0)", para.ApplyOrderNo);
            }
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.TeacherName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',TeacherName)>0)", para.TeacherName);
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId ='{0}'", para.MemberId);
            }
           
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.TeacherId != null)
            {
                sbWhere.AppendFormat(" and TeacherId='{0}'", para.TeacherId);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            if (para.CheckIds != null)
            {
                var str = "";
                foreach (var item in para.CheckIds)
                {
                    str += string.Format("'{0}',", item);
                }
                str = str.Substring(0, str.Length - 1);
                sbWhere.AppendFormat(" and ApplyOrderId in({0})", str);
            }
            return sbWhere.ToString();
        }

        /// <summary>
        /// 返回业务单号
        /// </summary>
        /// <returns></returns>
        public string GetOrderNo()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(ApplyOrderNo,11,len(ApplyOrderNo)-10)),0) from tbl_ApplyOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
    }
}
