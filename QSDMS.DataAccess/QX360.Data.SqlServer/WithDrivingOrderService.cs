﻿using QSDMS.Model;
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
    /// 陪练预约订单
    /// </summary>
    public class WithDrivingOrderService : BaseSqlDataService, IWithDrivingOrderService<WithDrivingOrderEntity, WithDrivingOrderEntity, Pagination>
    {
        public int QueryCount(WithDrivingOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<WithDrivingOrderEntity> GetPageList(WithDrivingOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_WithDrivingOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_WithDrivingOrder, WithDrivingOrderEntity>(pageList.ToList());
        }

        public List<WithDrivingOrderEntity> GetList(WithDrivingOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_WithDrivingOrder");
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
            var list = tbl_WithDrivingOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_WithDrivingOrder, WithDrivingOrderEntity>(list.ToList());
        }

        public WithDrivingOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_WithDrivingOrder.SingleOrDefault("where DrivingOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_WithDrivingOrder, WithDrivingOrderEntity>(model, null);
        }

        public bool Add(WithDrivingOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<WithDrivingOrderEntity, tbl_WithDrivingOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(WithDrivingOrderEntity entity)
        {

            var model = tbl_WithDrivingOrder.SingleOrDefault("where DrivingOrderId=@0", entity.DrivingOrderId);
            model = EntityConvertTools.CopyToModel<WithDrivingOrderEntity, tbl_WithDrivingOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_WithDrivingOrder.Delete("where DrivingOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(WithDrivingOrderEntity para)
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

            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberMobile)>0)", para.MemberMobile);
            }
            if (para.TeacherName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',TeacherName)>0)", para.TeacherName);
            }
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.DrivingOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',DrivingOrderNo)>0)", para.DrivingOrderNo);
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
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

            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.ServiceDate != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate='{0}'", Converter.ParseDateTime(para.ServiceDate).ToString("yyyy-MM-dd")));
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
                sbWhere.AppendFormat(" and DrivingOrderId in({0})", str);
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
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(DrivingOrderNo,11,len(DrivingOrderNo)-10)),0) from tbl_WithDrivingOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
    }
}
