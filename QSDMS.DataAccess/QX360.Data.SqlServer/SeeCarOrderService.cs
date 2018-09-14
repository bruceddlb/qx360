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
    /// 看车预约订单
    /// </summary>
    public class SeeCarOrderService : BaseSqlDataService, ISeeCarOrderService<SeeCarOrderEntity, SeeCarOrderEntity, Pagination>
    {
        public int QueryCount(SeeCarOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<SeeCarOrderEntity> GetPageList(SeeCarOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_SeeCarOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_SeeCarOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_SeeCarOrder, SeeCarOrderEntity>(pageList.ToList());
        }

        public List<SeeCarOrderEntity> GetList(SeeCarOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_SeeCarOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(para.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", para.sidx, para.sord);
            }
            var list = tbl_SeeCarOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_SeeCarOrder, SeeCarOrderEntity>(list.ToList());
        }

        public SeeCarOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_SeeCarOrder.SingleOrDefault("where SeeCarOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_SeeCarOrder, SeeCarOrderEntity>(model, null);
        }

        public bool Add(SeeCarOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<SeeCarOrderEntity, tbl_SeeCarOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(SeeCarOrderEntity entity)
        {

            var model = tbl_SeeCarOrder.SingleOrDefault("where SeeCarOrderId=@0", entity.SeeCarOrderId);
            model = EntityConvertTools.CopyToModel<SeeCarOrderEntity, tbl_SeeCarOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_SeeCarOrder.Delete("where SeeCarOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string GetOrderNo()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(SeeCarOrderNo,11,len(SeeCarOrderNo)-10)),0) from tbl_SeeCarOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
        public string ConverPara(SeeCarOrderEntity para)
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
                    sbWhere.AppendFormat(" and ShopId in({0})", str);
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
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberMobile)>0)", para.MemberMobile);
            }
            if (para.SeeCarOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SeeCarOrderNo)>0)", para.SeeCarOrderNo);
            }
            if (para.SeeCarOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SeeCarOrderNo)>0)", para.SeeCarOrderNo);
            }
            if (para.ShopCarName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ShopCarName)>0)", para.ShopCarName);
            }
            if (para.ShopName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ShopName)>0)", para.ShopName);
            }
          
            if (para.ShopId != null)
            {
                sbWhere.AppendFormat(" and ShopId='{0}'", para.ShopId);
            }
            if (para.ShopCarId != null)
            {
                sbWhere.AppendFormat(" and ShopCarId='{0}'", para.ShopCarId);
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
                sbWhere.AppendFormat(" and SeeCarOrderId in({0})", str);
            }
            return sbWhere.ToString();
        }
    }
}
