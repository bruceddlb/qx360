
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary>  
    /// 描 述：系统日志
    /// </summary>
    public class LogBLL : BaseBLL<ILogService>, ILogService
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static LogBLL m_Instance = new LogBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static LogBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        private LogBLL() { }



        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<LogEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return InstanceDAL.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 日志实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public LogEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
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
            try
            {
                InstanceDAL.RemoveLog(categoryId, keepTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logEntity">对象</param>
        public void WriteLog(LogEntity logEntity)
        {
            try
            {
                InstanceDAL.WriteLog(logEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
