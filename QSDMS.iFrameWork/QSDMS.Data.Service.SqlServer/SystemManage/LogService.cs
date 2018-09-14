
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：系统日志
    /// </summary>
    public class LogService :  ILogService
    {
        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<LogEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Log where 1=1");
           
            var queryParam = queryJson.ToJObject();
            //日志分类
            if (!queryParam["Category"].IsEmpty())
            {
                int categoryId = queryParam["CategoryId"].ToInt();
                sql.Append(" and CategoryId=@0", categoryId);
            }
            //操作时间
            if (!queryParam["StartTime"].IsEmpty() && !queryParam["EndTime"].IsEmpty())
            {
                DateTime startTime = queryParam["StartTime"].ToDate();
                DateTime endTime = queryParam["EndTime"].ToDate().AddDays(1);
                sql.Append(" and OperateTime>=@0", startTime);
                sql.Append(" and OperateTime<=@0", endTime);
            }
            //操作用户Id
            if (!queryParam["OperateUserId"].IsEmpty())
            {
                string operateUserId = queryParam["OperateUserId"].ToString();
                sql.Append(" and OperateUserId=@0", operateUserId);               
            }
            //操作用户账户
            if (!queryParam["OperateAccount"].IsEmpty())
            {
                string OperateAccount = queryParam["OperateAccount"].ToString();
                sql.Append(" and (charindex(@0,OperateAccount)>0)", OperateAccount);               
            }
            //操作类型
            if (!queryParam["OperateType"].IsEmpty())
            {
                string operateType = queryParam["OperateType"].ToString();
                sql.Append(" and OperateType=@0", operateType);               
            }
            //功能模块
            if (!queryParam["Module"].IsEmpty())
            {
                string module = queryParam["Module"].ToString();
                sql.Append(" and (charindex(@0,Module)>0)", module);   
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = Base_Log.Page(pagination.page , pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            //分页对象           
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<Base_Log, LogEntity>(pageList);   
        }
        /// <summary>
        /// 日志实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public LogEntity GetEntity(string keyValue)
        {
            var log = Base_Log.SingleOrDefault("where LogId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Log, LogEntity>(log,null);   
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        public void RemoveLog(int categoryId, string keepTime)
        {
            DateTime operateTime = DateTime.Now;
            if (keepTime == "7")//保留近一周
            {
                operateTime = DateTime.Now.AddDays(-7);
            }
            else if (keepTime == "1")//保留近一个月
            {
                operateTime = DateTime.Now.AddMonths(-1);
            }
            else if (keepTime == "3")//保留近三个月
            {
                operateTime = DateTime.Now.AddMonths(-3);
            }
            Base_Log.Delete("where CategoryId=@0 and  OperateTime<=@1", categoryId, operateTime);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logEntity">对象</param>
        public void WriteLog(LogEntity logEntity)
        {
            logEntity.LogId = Guid.NewGuid().ToString();
            logEntity.OperateTime = DateTime.Now;
            logEntity.DeleteMark = 0;
            logEntity.EnabledMark = 1;
            logEntity.IPAddress = Net.Ip;
            logEntity.Host = Net.Host;
            logEntity.Browser = Net.Browser;
            Base_Log log = EntityConvertTools.CopyToModel<LogEntity, Base_Log>(logEntity,null);
            log.Insert();
        }
        #endregion
    }
}
