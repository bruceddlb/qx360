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
    /// 预约保险订单
    /// </summary>
    public class InsuranceOrderService : BaseSqlDataService, IInsuranceOrderService<InsuranceOrderEntity, InsuranceOrderEntity, Pagination>
    {
        public int QueryCount(InsuranceOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<InsuranceOrderEntity> GetPageList(InsuranceOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_InsuranceOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_InsuranceOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_InsuranceOrder, InsuranceOrderEntity>(pageList.ToList());
        }

        public List<InsuranceOrderEntity> GetList(InsuranceOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_InsuranceOrder");
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
            var list = tbl_InsuranceOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_InsuranceOrder, InsuranceOrderEntity>(list.ToList());
        }

        public InsuranceOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_InsuranceOrder.SingleOrDefault("where InsuranceOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_InsuranceOrder, InsuranceOrderEntity>(model, null);
        }

        public bool Add(InsuranceOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<InsuranceOrderEntity, tbl_InsuranceOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(InsuranceOrderEntity entity)
        {

            var model = tbl_InsuranceOrder.SingleOrDefault("where InsuranceOrderId=@0", entity.InsuranceOrderId);
            model = EntityConvertTools.CopyToModel<InsuranceOrderEntity, tbl_InsuranceOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_InsuranceOrder.Delete("where InsuranceOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(InsuranceOrderEntity para)
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
                    sbWhere.AppendFormat(" and InsuranceId in({0})", str);
                }
            }

            if (para == null)
            {
                return sbWhere.ToString();
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
            if (para.InsuranceOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',InsuranceOrderNo)>0)", para.InsuranceOrderNo);
            }
           
            if (para.InsuranceId != null)
            {
                sbWhere.AppendFormat(" and InsuranceId='{0}'", para.InsuranceId);
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
                sbWhere.AppendFormat(" and InsuranceOrderId in({0})", str);
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
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(InsuranceOrderNo,11,len(InsuranceOrderNo)-10)),0) from tbl_InsuranceOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
    }
}
