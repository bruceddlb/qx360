
using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>   
    /// 描 述：系统日志
    /// </summary>
    public class LogController : BaseController
    {
        #region 视图功能
        /// <summary>
        /// 日志管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       
        public ActionResult RemoveLog()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var data = LogBLL.Instance.GetPageList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveLog(int categoryId, string keepTime)
        {
            LogBLL.Instance.RemoveLog(categoryId, keepTime);
            return Success("清空成功。");
        }
        #endregion
    }
}
