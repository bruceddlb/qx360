using iFramework.Framework.Log;
using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    //[AuthorizeFilter(PermissionMode.Enforce)]
    public class BaseController : Controller
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }
        /// <summary>
        /// 返回Json对象
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual ActionResult ToJsonResult(object data)
        {
            return Content(data.ToJson());
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual ActionResult Success(string message)
        {
            ReturnMessage result = new ReturnMessage();
            result.IsSuccess = true;
            result.Message = message;
            return Content(result.ToJson());
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual ActionResult Success(string message, IDictionary<string, object> data)
        {
            ReturnMessage result = new ReturnMessage();
            result.IsSuccess = true;
            result.Message = message;
            result.ResultData = data;
            return Content(result.ToJson());
        }
        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual ActionResult Error(string message)
        {
            ReturnMessage result = new ReturnMessage();
            result.IsSuccess = false;
            result.Message = message;
            return Content(result.ToJson());
        }
        //
        // GET: /BaseController/

        public Operator LoginUser
        {
            get
            {
                return OperatorProvider.Provider.Current();
            }
        }

        /// <summary>
        /// 不存在Action时处理
        /// </summary>
        /// <param name="actionName"></param>
        //protected override void HandleUnknownAction(string actionName)
        //{
        //    try
        //    {
        //        base.HandleUnknownAction(actionName);
        //    }
        //    catch
        //    {
        //        Response.Redirect("/error.html");
        //    }
        //}
    }
}
