using iFramework.Framework.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QSDMS.Util;
using QX360.Business;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class WxController : BaseController
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }

        [AuthorizeFilter]
        public ActionResult ApplyPay()
        {
            var notice = SettingsBLL.Instance.GetRemark("bmxz");
            ViewBag.Notice = notice;
            return View();
        }

        [AuthorizeFilter]
        public ActionResult WithDrivingPay()
        {
            var notice = SettingsBLL.Instance.GetRemark("pjxz");
            ViewBag.Notice = notice;
            return View();
        }
        //
        // GET: /WeiXin/
        [HttpGet]
        public ActionResult Info(string code, string state)
        {
            WriteDebug("code:" + code + "state:" + state);
            OAuthAccessTokenResult result = null;
            string appId1 = QSDMS.Util.Config.GetValue("APPID");//"wxf9ac40606ed2a28d";//"wx9b0af89f3c518363";
            string secret1 = QSDMS.Util.Config.GetValue("APPSECRET"); ;//"02b1e818ac46e0c828ece4f756a4e681";
            //通过，用code换取access_token
            try
            {
                //获取token
                result = OAuthApi.GetAccessToken(appId1, secret1, code);
                //Logger.Info("nickname:" + userInfo.nickname + "openid:" + userInfo.openid);
                var returnurl = Request.Params["returnurl"];
                //Logger.Debug("url:" + returnurl);
                //查找系统表中是否存在openid
                var account = MemberBLL.Instance.GetEntityForOpenId(result.openid);
                if (account == null)
                {
                    //获取微信信息
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                    WxUserInfoBLL.Instance.Delete(result.openid);
                    WxUserInfoBLL.Instance.Add(new Model.WxUserInfoEntity()
                    {
                        WxUserInfoId = userInfo.openid,
                        Nickename = userInfo.nickname,
                        HendIcon = userInfo.headimgurl,
                        Provice = userInfo.province,
                        City = userInfo.city,
                        Sex = userInfo.sex.ToString(),
                        County = userInfo.country
                    });

                    //微信没绑定
                    string url = "/Account/Login?oauthflag=1&openid=" + result.openid + "&returnurl=" + returnurl;
                    WriteDebug(url);
                    Response.Redirect(url);
                }
                else
                {
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                    WxUserInfoBLL.Instance.Delete(result.openid);
                    WxUserInfoBLL.Instance.Add(new Model.WxUserInfoEntity()
                    {
                        WxUserInfoId = userInfo.openid,
                        Nickename = userInfo.nickname,
                        HendIcon = userInfo.headimgurl,
                        Provice = userInfo.province,
                        City = userInfo.city,
                        Sex = userInfo.sex.ToString(),
                        County = userInfo.country
                    });

                    //绑定了微信自动登陆
                    var userCookie = new HttpCookie("SSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["openid"] = account.OpenId;
                    userCookie.Values["accountid"] = account.MemberId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);
                    Response.Redirect(returnurl == "" ? "/MyCenter/Index" : returnurl);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("程序出现异常：" + ex.ToString());
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                Logger.Warn("请求失败");
            }
            return null;
        }


        /// <summary>
        ///教练自动登陆
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Info2(string code, string state)
        {
            WriteDebug("code:" + code + "state:" + state);
            OAuthAccessTokenResult result = null;
            string appId1 = QSDMS.Util.Config.GetValue("APPID");//"wxf9ac40606ed2a28d";//"wx9b0af89f3c518363";
            string secret1 = QSDMS.Util.Config.GetValue("APPSECRET"); ;//"02b1e818ac46e0c828ece4f756a4e681";
            //通过，用code换取access_token
            try
            {
                //获取token
                result = OAuthApi.GetAccessToken(appId1, secret1, code);
                //Logger.Info("nickname:" + userInfo.nickname + "openid:" + userInfo.openid);
                var returnurl = Request.Params["returnurl"];
                //Logger.Debug("url:" + returnurl);
                //查找系统表中是否存在openid
                var account = TeacherBLL.Instance.GetEntityByOpenId(result.openid);
                if (account == null)
                {
                    //获取微信信息
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                    WxUserInfoBLL.Instance.Delete(result.openid);
                    WxUserInfoBLL.Instance.Add(new Model.WxUserInfoEntity()
                    {
                        WxUserInfoId = userInfo.openid,
                        Nickename = userInfo.nickname,
                        HendIcon = userInfo.headimgurl,
                        Provice = userInfo.province,
                        City = userInfo.city,
                        Sex = userInfo.sex.ToString(),
                        County = userInfo.country
                    });

                    //微信没绑定
                    string url = "/MaCenter/Login?oauthflag=1&openid=" + result.openid + "&returnurl=" + returnurl;
                    WriteDebug(url);
                    Response.Redirect(url);
                }
                else
                {
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                    WxUserInfoBLL.Instance.Delete(result.openid);
                    WxUserInfoBLL.Instance.Add(new Model.WxUserInfoEntity()
                    {
                        WxUserInfoId = userInfo.openid,
                        Nickename = userInfo.nickname,
                        HendIcon = userInfo.headimgurl,
                        Provice = userInfo.province,
                        City = userInfo.city,
                        Sex = userInfo.sex.ToString(),
                        County = userInfo.country
                    });

                    //绑定了微信自动登陆
                    var userCookie = new HttpCookie("MASSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["accountid"] = account.TeacherId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);
                    Response.Redirect(returnurl == "" ? "/MaCenter/Index" : returnurl);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("程序出现异常：" + ex.ToString());
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                Logger.Warn("请求失败");
            }
            return null;
        }

    }
}
