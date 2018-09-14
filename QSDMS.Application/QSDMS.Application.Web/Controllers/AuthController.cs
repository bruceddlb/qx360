using iFramework.Framework.Log;
using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QSDMS.Application.Web
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
        private PermissionMode _customMode;
        /// <summary>默认构造</summary>
        /// <param name="Mode">认证模式</param>
        public AuthorizeFilterAttribute(PermissionMode Mode)
        {
            _customMode = Mode;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //是否忽略
            if (_customMode == PermissionMode.Ignore)
            {
                return;
            }
            //Log _logger = LogFactory.GetLogger(this.GetType().ToString());
            //_logger.Info(filterContext.HttpContext.Request.Url);
            
            /*
            1.判断是否已经登录                    
            */
            //登录是否过期
            if (OperatorProvider.Provider.IsOverdue())
            {
                WebHelper.WriteCookie("dms_login_error", "Overdue");//登录已超时,请重新登录
                filterContext.Result = new RedirectResult("~/Login/Default");
                return;
            }
            //是否已登录
            var OnLine = OperatorProvider.Provider.IsOnLine();
            if (OnLine == 0)
            {
                //WebHelper.WriteCookie("dms_login_error", "OnLine");//您的帐号已在其它地方登录,请重新登录
                //filterContext.Result = new RedirectResult("~/Login/Default");
                //return;
            }
            else if (OnLine == -1)
            {
                WebHelper.WriteCookie("dms_login_error", "-1");//缓存已超时,请重新登录
                filterContext.Result = new RedirectResult("~/Login/Default");
                return;
            }

            //验证请求的action
            if (filterContext == null)
            { 
                throw new ArgumentNullException("filterContext"); 
            }

            var path = filterContext.HttpContext.Request.Path;
            //if (path.Equals("/Login/Default", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/FindPassword", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Account/LoginOut", StringComparison.CurrentCultureIgnoreCase) || path.Equals("/Menu/GetMenu", StringComparison.CurrentCultureIgnoreCase))
            //    return;//忽略对Login登录页和忘记密码页的拦截

            bool isUserLogin = false;
            #region 从Cookie获取登录信息
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[OperatorProvider.LoginCookieIdentityUserKey];
            if (authCookie != null)
            {
                isUserLogin = true;
            }
            #endregion

            //获取Action是否有AjaxPage属性
            object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AjaxPageAttribute), true);
            bool isAjax = attrs.Length == 1;
            //判断是否已经登录
            if (!isUserLogin)
            {
                if (isAjax)
                {
                    filterContext.Result = new ContentResult { Content = "{\"Message\":\"未登录\"}" };
                    return;
                }
                else
                {
                   
                    string url = "/Login/Default";
                    if (filterContext.HttpContext.Request.Url != null)
                    {
                        if (!path.Equals("/"))
                            url = "/Login/Default?ReturnUrl=" + System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.ToString());
                    }
                    filterContext.Result = new RedirectResult(url);
                    return;
                }
            }
            else
            {
                //标记ajax方法则不判断权限
                if (isAjax)
                {
                    return;
                }                
                //if (string.IsNullOrWhiteSpace(CurrentUser.LoginUser.UserId))
                //{
                //    filterContext.Result = new RedirectResult("/Account/Login");
                //    return;
                //}
                //if (CurrentUser.LoginUser.Role == null || CurrentUser.LoginUser.Role.PermissionList == null)
                //{
                //    //filterContext.Result = new ContentResult { Content = "{\"Message\":\"无相应的操作权限!\"}" };
                //    filterContext.Result = new RedirectResult("/Error/NoPower");
                //    return;
                //}

                ////string controller = filterContext.Controller.ToString().Split('.').LastOrDefault().Replace("Controller", "");

                //if (!CurrentUser.LoginUser.Role.PermissionList.Exists((p) => { return p.Controller.ToLower() == controller.ToLower() && p.Action == "*"; }) && !CurrentUser.LoginUser.Role.PermissionList.Exists((p) => { return p.Controller.ToLower() == controller.ToLower() && p.Action.ToLower() == filterContext.ActionDescriptor.ActionName.ToLower(); }))
                //{
                //    //filterContext.Result = new ContentResult { Content = "{\"Message\":\"无相应的操作权限!\"}" };           
                //    filterContext.Result = new RedirectResult("/Error/NoPower");
                //    //HttpContext.Current.Response.Redirect("/Error/NoPower");

                //    return;
                //}

            }
        }

        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;


            foreach (IPAddress IPA in Dns.GetHostAddresses(System.Web.HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }
}
