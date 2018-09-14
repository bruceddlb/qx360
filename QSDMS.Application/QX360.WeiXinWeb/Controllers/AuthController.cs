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
    /// Ajax页面自定义属性，对于Ajax请求的Action请添加此属性[AjaxPage]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxPageAttribute : Attribute
    {

    }

    /// <summary>
    /// Action权限过滤
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
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
        public AuthorizeFilterAttribute()
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
            if (path.Equals("/Account/FinePwd", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/Success", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/Test", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/Login", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/SelectRole", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/Register", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/", StringComparison.CurrentCultureIgnoreCase))
            {
                return;//忽略对Login登录页和忘记密码页的拦截
            }

            /*
            1.判断是否已经登录                    
          */
            bool isUserLogin = false;//发布时候要改成false
            #region 从Cookie获取登录信息重置登录
            var cookie = filterContext.RequestContext.HttpContext.Request.Cookies.Get("SSO");
            if (cookie != null && cookie["accountid"].ToString() != "")//登陆操作登陆
            {
                //Logger.Debug("有cookie");
                isUserLogin = true;
            }
            #endregion
            //Logger.Debug("aa:" + isUserLogin);
            //判断是否已经登录
            if (!isUserLogin)
            {
                string url = "/Account/Login";
                if (filterContext.HttpContext.Request.Url != null)
                {
                    if (!path.Equals("/"))
                        url = "/Account/Login?returnUrl=" + System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString());
                }
                filterContext.Result = new RedirectResult(url);
                return;
            }

        }

    }

    public class CurrentUser
    {
        private static Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public static Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger("CurrentUser")); }
        }

        /// <summary>
        /// 当前登陆对象
        /// </summary>
        public static User LoginUser
        {
            get
            {
                User user = null;
                var cookie = HttpContext.Current.Request.Cookies.Get("SSO");
                if (cookie != null && cookie["accountid"].ToString() != "")
                {
                    // Logger.Debug("accountid" + cookie["accountid"].ToString() + ";openid:" + cookie["openid"].ToString());
                    var account = MemberBLL.Instance.GetEntity(cookie["accountid"].ToString());
                    if (account != null)
                    {
                        user = new User();
                        user.UserId = account.MemberId;
                        user.UserName = account.MemberName;
                        user.NickName = account.NikeName;
                        user.OpenId = account.OpenId;
                        user.Mobile = account.Mobile;
                        user.MemberLevId = account.LevId;
                        user.MemberLevName = account.LevName;
                        user.SchoolId = account.SchoolId;
                    }
                }
                else
                {
                    //user = new User();
                    //user.UserId = "17C82008099C457FB6676C8A92B76BAF";

                }
                return user;
            }

        }
    }
    /// <summary>
    /// 用户对象
    /// </summary>
    public class User
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Mobile { get; set; }
        public string OpenId { get; set; }

        public string IdCard { get; set; }

        public string Token { get; set; }
        public string MemberLevId { get; set; }
        public string MemberLevName { get; set; }
        public string SchoolId { get; set; }
        public string TeacherId { get; set; }
        public string MemberTypeName { get; set; }

        public string FaceImage { get; set; }

    }
}
