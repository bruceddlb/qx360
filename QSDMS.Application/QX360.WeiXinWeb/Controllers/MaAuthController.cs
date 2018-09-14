using iFramework.Framework.Log;
using QX360.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{


    /// <summary>
    /// Action权限过滤
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MaAuthorizeFilterAttribute : ActionFilterAttribute
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }

        /// <summary>默认构造</summary>
        /// <param name="Mode">认证模式</param>
        public MaAuthorizeFilterAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //获取Action是否有AjaxPage属性
            object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AjaxPageAttribute), true);
            bool isAjax = attrs.Length == 1;
            //标记ajax方法则不判断权限
            if (isAjax)
            {
                return;
            }
            //验证请求的action
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            var path = filterContext.HttpContext.Request.Path;
            if (path.Equals("/MaCenter/FinePwd", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/MaCenter/Success") || path.Equals("/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;//忽略对Login登录页和忘记密码页的拦截
            }

            /*
            1.判断是否已经登录                    
          */
            bool isUserLogin = false;//发布时候要改成false
            #region 从Cookie获取登录信息重置登录
            var cookie = filterContext.RequestContext.HttpContext.Request.Cookies.Get("MASSO");
            if (cookie != null && cookie["accountid"].ToString() != "")//登陆操作登陆
            {
                //Logger.Debug("有cookie");
                isUserLogin = true;
            }
            #endregion

            //判断是否已经登录
            if (!isUserLogin)
            {
                string url = "/MaCenter/Login";
                if (filterContext.HttpContext.Request.Url != null)
                {
                    if (!path.Equals("/"))
                        url = "/MaCenter/Login?returnUrl=" + System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString());
                }
                filterContext.Result = new RedirectResult(url);
                return;
            }
        }

    }

    public class CurrentTeacher
    {
        private static Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public static Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger("CurrentTeacher")); }
        }

        /// <summary>
        /// 当前登陆对象
        /// </summary>
        public static Teacher LoginTeacher
        {
            get
            {
                Teacher user = null;
                var cookie = HttpContext.Current.Request.Cookies.Get("MASSO");
                if (cookie != null && cookie["accountid"].ToString() != "")
                {
                    // Logger.Debug("accountid" + cookie["accountid"].ToString() + ";openid:" + cookie["openid"].ToString());
                    var account = TeacherBLL.Instance.GetEntity(cookie["accountid"].ToString());
                    if (account != null)
                    {
                        user = new Teacher();
                        user.UserId = account.TeacherId;
                        user.UserName = account.Name;
                        user.Mobile = account.Mobile;
                    }
                }
                return user;
            }

        }
    }
    /// <summary>
    /// 用户对象
    /// </summary>
    public class Teacher
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Mobile { get; set; }

    }

}

