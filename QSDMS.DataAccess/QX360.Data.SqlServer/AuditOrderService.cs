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
    /// 年检订单
    /// </summary>
    public class AuditOrderService : BaseSqlDataService, IAuditOrderService<AuditOrderEntity, AuditOrderEntity, Pagination>
    {
        public int QueryCount(AuditOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditOrderEntity> GetPageList(AuditOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditOrder, AuditOrderEntity>(pageList.ToList());
        }

        public List<AuditOrderEntity> GetList(AuditOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (para != null)
            {
                if (!string.IsNullOrWhiteSpace(para.sidx))
                {
                    sql.AppendFormat(" order by {0} {1}", para.sidx, para.sord);
                }
            }
            var list = tbl_AuditOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditOrder, AuditOrderEntity>(list.ToList());
        }

        public AuditOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditOrder.SingleOrDefault("where AuditOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditOrder, AuditOrderEntity>(model, null);
        }

        public bool Add(AuditOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditOrderEntity, tbl_AuditOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditOrderEntity entity)
        {

            var model = tbl_AuditOrder.SingleOrDefault("where AuditOrderId=@0", entity.AuditOrderId);
            model = EntityConvertTools.CopyToModel<AuditOrderEntity, tbl_AuditOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditOrder.Delete("where AuditOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditOrderEntity para)
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
                    sbWhere.AppendFormat(" and OrganizationId in({0})", str);
                }
            }
            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.AuditOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',AuditOrderNo)>0)", para.AuditOrderNo);
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.Mobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Mobile)>0)", para.Mobile);
            }
            if (para.CarNum != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarNum)>0)", para.CarNum);
            }

            if (para.OrganizationId != null)
            {
                sbWhere.AppendFormat(" and OrganizationId='{0}'", para.OrganizationId);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.NotStatus != null)
            {
                sbWhere.AppendFormat(" and Status<>'{0}'", para.NotStatus);
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.ServiceTime != null)
            {
                sbWhere.AppendFormat(" and ServiceTime='{0}'", para.ServiceTime);
            }
            if (para.CheckIds != null)
            {
                var str = "";
                foreach (var item in para.CheckIds)
                {
                    str += string.Format("'{0}',", item);
                }
                str = str.Substring(0, str.Length - 1);
                sbWhere.AppendFormat(" and AuditOrderId in({0})", str);
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
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(AuditOrderNo,11,len(AuditOrderNo)-10)),0) from tbl_AuditOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
    }
}
