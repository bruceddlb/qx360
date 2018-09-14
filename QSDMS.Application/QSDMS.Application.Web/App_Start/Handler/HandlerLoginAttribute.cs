using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace QSDMS.Application.Web
{
    /// <summary>

    /// 描 述：登录认证（会话验证组件）
    /// </summary>
    public class HandlerLoginAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private LoginMode _customMode;
        /// <summary>默认构造</summary>
        /// <param name="Mode">认证模式</param>
        public HandlerLoginAttribute(LoginMode Mode)
        {
            _customMode = Mode;
        }
        /// <summary>
        /// 响应前执行登录验证,查看当前用户是否有效 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //登录拦截是否忽略
            if (_customMode == LoginMode.Ignore)
            {
                return;
            }
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
                WebHelper.WriteCookie("dms_login_error", "OnLine");//您的帐号已在其它地方登录,请重新登录
                filterContext.Result = new RedirectResult("~/Login/Default");
                return;
            }
            else if (OnLine == -1)
            {
                WebHelper.WriteCookie("dms_login_error", "-1");//缓存已超时,请重新登录
                //filterContext.Result = new RedirectResult("~/Login/Default");
                return;
            }
        }
    }
}