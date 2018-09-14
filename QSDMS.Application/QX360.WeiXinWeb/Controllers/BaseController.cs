using iFramework.Framework.Log;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
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
        /// <summary>
        /// 只能在微信里面浏览
        /// </summary>
        public void OnlyWeiXinLook()
        {
            String userAgent = Request.UserAgent;
            if (userAgent.IndexOf("MicroMessenger") <= -1)
            {
                Response.Write("请在微信浏览器里访问");
                Response.End();
            }
        }
        public void WriteDebug(string content)
        {
            if (QSDMS.Util.Config.GetValue("writedebug") == "1")
            {
                Logger.Debug(string.Format("时间：{0},内容：{1}", DateTime.Now, content));
            }
        }

        /// <summary>
        /// 用户对象
        /// </summary>

        protected User LoginUser
        {
            get
            {
                return CurrentUser.LoginUser;
            }
        }

        /// <summary>
        /// 教练对象
        /// </summary>
        protected Teacher LoginTeacher
        {
            get
            {
                return CurrentTeacher.LoginTeacher;
            }
        }

        /// <summary>
        /// 考场对象
        /// </summary>
        protected ExamPlace LoginExamPlace
        {
            get
            {
                return CurrentExamPlace.LoginExamPlace;
            }
        }
        /// <summary>
        /// 考场对象
        /// </summary>
        protected Master LoginMaster
        {
            get
            {
                return CurrentMaster.LoginMaster;
            }
        }

        /// <summary>
        /// 清楚缓存
        /// </summary>
        public virtual void ClearCache()
        {
            string ms = "清除缓存成功";
            try
            {
                foreach (string cookiename in Request.Cookies.AllKeys)
                {
                    HttpCookie cookies = Request.Cookies[cookiename];
                    if (cookies != null)
                    {
                        cookies.Expires = DateTime.Today.AddMonths(-1);
                        Response.Cookies.Add(cookies);
                        Request.Cookies.Remove(cookiename);
                    }
                }
                ms = "清除缓存成功";
            }
            catch (Exception ex)
            {
                ms = "清除缓存失败";

            }
        }
    }  
}
