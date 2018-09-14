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
    /// 实训预约订单
    /// </summary>
    public class TrainingOrderService : BaseSqlDataService, ITrainingOrderService<TrainingOrderEntity, TrainingOrderEntity, Pagination>
    {
        public int QueryCount(TrainingOrderEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingOrderEntity> GetPageList(TrainingOrderEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingOrder");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingOrder.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingOrder, TrainingOrderEntity>(pageList.ToList());
        }

        public List<TrainingOrderEntity> GetList(TrainingOrderEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingOrder");
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
            var list = tbl_TrainingOrder.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingOrder, TrainingOrderEntity>(list.ToList());
        }

        public TrainingOrderEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingOrder.SingleOrDefault("where TrainingOrderId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingOrder, TrainingOrderEntity>(model, null);
        }

        public bool Add(TrainingOrderEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingOrderEntity, tbl_TrainingOrder>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingOrderEntity entity)
        {

            var model = tbl_TrainingOrder.SingleOrDefault("where TrainingOrderId=@0", entity.TrainingOrderId);
            model = EntityConvertTools.CopyToModel<TrainingOrderEntity, tbl_TrainingOrder>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingOrder.Delete("where TrainingOrderId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingOrderEntity para)
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
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.TrainingCarName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',TrainingCarName)>0)", para.TrainingCarName);
            }
            if (para.TrainingCarId != null)
            {
                sbWhere.AppendFormat(" and TrainingCarId='{0}'", para.TrainingCarId);
            }
            if (para.StartTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
            }
            if (para.EndTime != null)
            {
                sbWhere.Append(base.FormatParameter(" AND ServiceDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
            }
            if (para.TrainingOrderNo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',TrainingOrderNo)>0)", para.TrainingOrderNo);
            }


            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.TrainingType != null)
            {
                sbWhere.AppendFormat(" and TrainingType='{0}'", para.TrainingType);
            }
            if (para.CheckIds != null)
            {
                var str = "";
                foreach (var item in para.CheckIds)
                {
                    str += string.Format("'{0}',", item);
                }
                str = str.Substring(0, str.Length - 1);
                sbWhere.AppendFormat(" and TrainingOrderId in({0})", str);
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
            var count = QX360_SQLDB.GetInstance().ExecuteScalar<int>("select isnull(max(SUBSTRING(TrainingOrderNo,11,len(TrainingOrderNo)-10)),0) from tbl_TrainingOrder where  Convert(varchar(10), CreateTime,121)=@0", date);
            var orderNo = string.Format("{0}{1}", date.Replace("-", ""), (count + 1).ToString().PadLeft(5, '0'));
            return orderNo;
        }
    }
}
