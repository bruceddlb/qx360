using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QX360.Data.IServices.Report;
using QX360.Model.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.SqlServer.Report
{
    public class AuditReportService : BaseSqlDataService, IAuditReportService<Pagination>
    {

        /// <summary>
        /// 执行存储过程 执行查询sql语句业务
        /// </summary>
        /// <param name="para"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<AuditCollectEntity> GetAuditCollectPageList(AuditCollectEntity para, ref Pagination pagination)
        {
            List<AuditCollectEntity> list = new List<AuditCollectEntity>();
            try
            {
                string pSQL = @"select Id,OrganizationId,OrganizationName,ServiceDate,ServiceTime,SubricType,count(1)SubricCount from (
            select '0'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId,OrganizationName,ServiceDate,ServiceTime,0 as SubricType from tbl_AuditOrder where Status<>4
            union all
            select '1'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId,OrganizationName,ServiceDate,ServiceTime,1 as SubricType from tbl_GroupAuditOrder where Status<>4
            union all
            select '2'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId,OrganizationName,ServiceDate,ServiceTime,2 as SubricType from tbl_TakeAuditOrder where Status<>4
            
            )a group by ID, OrganizationId,OrganizationName,ServiceDate,ServiceTime,SubricType";

                string sql = @"EXEC proc_GetRecordByPage @0,@1,@2,@3,@4,@5,@6,@7 OUTPUT,@8 OUTPUT";
                StringBuilder sbWhere = new StringBuilder();
                sbWhere.Append(" 1=1 ");
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
                if (para.OrganizationName != null)
                {
                    sbWhere.AppendFormat(" and (charindex('{0}',OrganizationName)>0)", para.OrganizationName);
                }
                if (para.StartTime != null)
                {
                    sbWhere.Append(base.FormatParameter(" AND ServiceDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
                }
                if (para.EndTime != null)
                {
                    sbWhere.Append(base.FormatParameter(" AND ServiceDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
                }
                if (para.SubricType != null)
                {
                    sbWhere.AppendFormat(" and SubricType={0}", para.SubricType);
                }
                if (para.ServiceTime != null)
                {
                    sbWhere.AppendFormat(" and ServiceTime='{0}'", para.ServiceTime);
                }
                //参数
                SqlParameter[] pars = {
                       new SqlParameter("@pSQL", pSQL),
                       new SqlParameter("@keyField", "Id"),
                       new SqlParameter("@orderFiled","ServiceDate desc"),
                       new SqlParameter("@PageSize",pagination.rows),
                       new SqlParameter("@PageIndex",pagination.page),
                       new SqlParameter("@OrderType","1"),
                       new SqlParameter("@strWhere",sbWhere.ToString()),
                       new SqlParameter("@pRecNums",SqlDbType.Int){Direction=ParameterDirection.Output},
                       new SqlParameter("@pRecPages",SqlDbType.Int){Direction=ParameterDirection.Output}
                    };

                var sql2 = PetaPoco.Sql.Builder.Append(sql, pars);
                // var aa = dbInstance.Fetch<dynamic>(sql2).ToList<dynamic>();
                var dbInstance = QX360_SQLDB.GetInstance();
                dbInstance.EnableAutoSelect = false;
                list = dbInstance.Fetch<AuditCollectEntity>(sql2).ToList();
                dbInstance.EnableAutoSelect = true;
                pagination.records = Convert.ToInt32(pars[7].Value);//输出参数的值  
            }
            catch (Exception e)
            {
                throw e;
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<AuditCollectEntity> GetAuditCollectList(AuditCollectEntity para)
        {
            List<AuditCollectEntity> list = new List<AuditCollectEntity>();
            try
            {
                string pSQL = @"select * from (
select Id,OrganizationId, OrganizationName,ServiceDate,ServiceTime,SubricType,count(1)SubricCount from (
            select '0'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId, OrganizationName,ServiceDate,ServiceTime,0 as SubricType from tbl_AuditOrder where Status<>4
            union all
            select '1'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId, OrganizationName,ServiceDate,ServiceTime,1 as SubricType from tbl_GroupAuditOrder where Status<>4
            union all
            select '2'+cast(ServiceDate as varchar(50))+OrganizationId as Id,OrganizationId, OrganizationName,ServiceDate,ServiceTime,2 as SubricType from tbl_TakeAuditOrder where Status<>4
            
            )a group by ID,OrganizationId, OrganizationName,ServiceDate,ServiceTime,SubricType
            )a";

                StringBuilder sbWhere = new StringBuilder();
                sbWhere.Append(" where 1=1 ");
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
                if (para.OrganizationName != null)
                {
                    sbWhere.AppendFormat(" and (charindex('{0}',OrganizationName)>0)", para.OrganizationName);
                }
                if (para.StartTime != null)
                {
                    sbWhere.Append(base.FormatParameter(" AND ServiceDate>='{0} 00:00:00'", Converter.ParseDateTime(para.StartTime).ToString("yyyy-MM-dd")));
                }
                if (para.EndTime != null)
                {
                    sbWhere.Append(base.FormatParameter(" AND ServiceDate<='{0} 23:59:59'", Converter.ParseDateTime(para.EndTime).ToString("yyyy-MM-dd")));
                }
                if (para.SubricType != null)
                {
                    sbWhere.AppendFormat(" and SubricType={0}", para.SubricType);
                }
                if (para.ServiceTime != null)
                {
                    sbWhere.AppendFormat(" and ServiceTime='{0}'", para.ServiceTime);
                }
                var dbInstance = QX360_SQLDB.GetInstance();
                dbInstance.EnableAutoSelect = false;
                list = dbInstance.Fetch<AuditCollectEntity>(pSQL + sbWhere.ToString()).ToList();
                dbInstance.EnableAutoSelect = true;

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }
    }
}
