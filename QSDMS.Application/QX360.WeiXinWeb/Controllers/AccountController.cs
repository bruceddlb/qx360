
using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AppStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{

    public class AccountController : BaseController
    {

        public ActionResult ClearCache()
        {
            string ms = "清除缓存成功";
            try
            {
                new QSDMS.Cache.Cache().RemoveCache();
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
            return Content(ms);
        }


        public ActionResult FinePwd()
        {
            return View();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="oauthflag"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string oauthflag)
        {
            //OnlyWeiXinLook();
            //自动授权开关 如果是1表示该微信没有绑定，授权获取openid后再跳回该页面显示内容执行操作
            if (oauthflag == "1")
            {
                return View();
            }
            var sig = Request.Params["sig"];
            if (sig == "out")
            {
                ClearCache();
                return View();
            }

            //OAuth2获取微信的相关信息,写入数据库后自动登陆
            string appid = QSDMS.Util.Config.GetValue("APPID");//"wx9b0af89f3c518363";           
            var redricturl = QSDMS.Util.Config.GetValue("webSite") + "/wx/Info?returnurl=" + Request.Params["returnUrl"];
            var url = OAuthApi.GetAuthorizeUrl(appid, redricturl, "dlb", OAuthScope.snsapi_userinfo);//可以获取微信信息
            Response.Redirect(url);
            return null;
            //return View();

        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Openid = Request.Params["openid"];
            return View();
        }


        [AjaxPage]
        [HttpPost]
        public JsonResult SendVerifySms(string mobile)
        {
            var result = new ReturnMessage(false) { Message = "短信发送失败!" };
            if (string.IsNullOrWhiteSpace(mobile))
            {
                result.Message = "电话号码不能为空";
                return Json(result);
            }
            try
            {
                result.IsSuccess = true;
                result.IsSuccess = true;
                result.Message = "获取成功";
                SmsVerifyHelper.SendMobileSms(mobile);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>SendVerifySms";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult Register(MemberEntity account)
        {
            var result = new ReturnMessage(false) { Message = "注册失败!" };
            try
            {
                //验证码是否正确
                var verifyCode = SmsVerifyHelper.GetMobileSmsCode(account.Mobile);
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    result.Message = "短信验证码已过期,请重新发送!";
                    return Json(result);
                }

                if (account.sms_verify_code != verifyCode)
                {
                    result.Message = "短信验证码输入有误!";
                    return Json(result);
                }

                if (!new System.Text.RegularExpressions.Regex("^1[0-9]{10}$").IsMatch(account.Mobile))
                {
                    result.Message = "请输入正确的手机号!";
                    return Json(result);
                }
                int count = MemberBLL.Instance.GetList(new MemberEntity() { Mobile = account.Mobile }).Count();
                if (count > 0)
                {
                    result.Message = "该号码已经注册了!";
                    return Json(result);
                }
                account.CreateTime = DateTime.Now;
                account.Point = 0;
                //account.MemberName = account.Mobile;
                var wxinfo = WxUserInfoBLL.Instance.GetEntity(account.OpenId);
                account.NikeName = wxinfo == null ? account.MemberName : wxinfo.Nickename;
                account.Status = (int)Enums.UseStatus.启用;
                account.MemberId = Util.NewUpperGuid();
                account.LevId = ((int)QX360.Model.Enums.UserType.预约记时会员).ToString();
                account.LevName = QX360.Model.Enums.UserType.预约记时会员.ToString();
                account.StudyHour1 = 0;
                account.StudyHour2 = 0;
                MemberBLL.Instance.Add(account);

                result.IsSuccess = true;
                result.ResultData["AccountId"] = account.MemberId;
                result.Message = "注册成功";

                //写入cookie
                //var userCookie = new HttpCookie("SSO");
                //userCookie.Expires = DateTime.Now.AddMonths(1);
                //userCookie.Values["openid"] = account.OpenId;
                //userCookie.Values["accountid"] = account.MemberId;
                //userCookie.Path = "/";
                //Response.Cookies.Add(userCookie);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>Register";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }

        [AjaxPage]
        [HttpPost]
        public JsonResult SendFindPwdVerifySms(string mobile)
        {
            var result = new ReturnMessage(false) { Message = "短信发送失败!" };
            if (string.IsNullOrWhiteSpace(mobile))
            {
                result.Message = "电话号码不能为空";
                return Json(result);
            }
            try
            {
                if (!new System.Text.RegularExpressions.Regex("^1[0-9]{10}$").IsMatch(mobile))
                {
                    result.Message = "请输入正确的手机号!";
                    return Json(result);
                }
                int count = MemberBLL.Instance.GetList(new MemberEntity() { Mobile = mobile }).Count();
                if (count == 0)
                {
                    result.Message = "该账号未注册!";
                    return Json(result);
                }

                result.IsSuccess = true;

                SmsVerifyHelper.SendMobileSms(mobile);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>SendVerifySms";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindPwd(MemberEntity account)
        {
            var result = new ReturnMessage(false) { Message = "重置密码失败!" };
            try
            {
                if (!new System.Text.RegularExpressions.Regex("^1[0-9]{10}$").IsMatch(account.Mobile))
                {
                    result.Message = "请输入正确的手机号!";
                    return Json(result);
                }
                if (string.IsNullOrWhiteSpace(account.Pwd))
                {
                    result.Message = "请输入新密码";
                    return Json(result);
                }
                var model = new MemberEntity();
                var list = MemberBLL.Instance.GetList(new MemberEntity() { Mobile = account.Mobile });
                if (list.Count() == 0)
                {
                    result.Message = "该账号不存在!";
                    return Json(result);
                }
                else
                {
                    model = list.FirstOrDefault();
                }

                //验证码是否正确
                var verifyCode = SmsVerifyHelper.GetMobileSmsCode(account.Mobile);
                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    result.Message = "短信验证码已过期,请重新发送!";
                    return Json(result);
                }
                if (account.sms_verify_code != verifyCode)
                {
                    result.Message = "短信验证码输入有误!";
                    return Json(result);
                }
                model.Pwd = account.Pwd;//找回密码
                MemberBLL.Instance.Update(model);
                result.IsSuccess = true;
                result.Message = "重置密码成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>Register";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login(string UserName, string UserPwd, string OpenId, string returnurl)
        {
            var result = new ReturnMessage(false) { Message = "登陆失败!" };
            try
            {
                var account = MemberBLL.Instance.CheckLogin(UserName, UserPwd);
                if (account != null)
                {
                    if (account.Status == (int)QX360.Model.Enums.UseStatus.禁用)
                    {
                        result.Message = "此账户已锁定!";
                        return Json(result);
                    }
                    //如果openid不一致之前注册的公共号换号的可能,自动更新当前的公共号信息
                    if (OpenId != account.OpenId)
                    {
                        account.OpenId = OpenId;
                        MemberBLL.Instance.Update(account);

                    }
                    //如果没有完善资料
                    //写入cookie
                    var userCookie = new HttpCookie("SSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["openid"] = account.OpenId;
                    userCookie.Values["accountid"] = account.MemberId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);

                    result.IsSuccess = true;
                    result.Message = "登陆成功";
                    result.ResultData["ReturnUrl"] = returnurl;

                    //送积分
                    GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.登陆操作, account.MemberId, 0, "");
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>Register";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldUserPwd"></param>
        /// <param name="NewUserPwd"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdatePwd(string OldUserPwd, string NewUserPwd)
        {
            var result = new ReturnMessage(false) { Message = "密码修改失败!" };
            try
            {
                var account = MemberBLL.Instance.GetEntity(LoginUser.UserId);
                if (account != null)
                {
                    if (account.Pwd != OldUserPwd)
                    {
                        result.Message = "请输入正确的密码!";
                        return Json(result);
                    }
                    account.Pwd = NewUserPwd;//修改密码
                    MemberBLL.Instance.Update(account);
                    result.IsSuccess = true;
                    result.Message = "密码修改成功!";
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>UpdatePwd";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserDetail()
        {
            var result = new ReturnMessage(false) { Message = "获取用户信息失败!" };
            try
            {
                if (LoginUser == null)
                {
                    result.Message = "请先登陆";
                    return Json(result);
                }
                var account = MemberBLL.Instance.GetEntity(LoginUser.UserId);
                if (account != null)
                {
                    if (account.HeadIcon == null)
                    {
                        if (account.OpenId != null)
                        {
                            var wxuser = WxUserInfoBLL.Instance.GetEntity(account.OpenId);
                            account.WxHeadIcon = wxuser == null ? "" : wxuser.HendIcon;
                        }
                    }
                    else {
                        account.WxHeadIcon = "";
                    }
                    var owner = OwnerBLL.Instance.GetList(new OwnerEntity() { MemberMobile = account.Mobile });
                    if (owner != null && owner.Count > 0)
                    {
                        account.Owner = owner.FirstOrDefault();
                    }
                }
                result.IsSuccess = true;
                result.ResultData["Data"] = account;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>GetUserDetail";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateInfo(string json)
        {
            var result = new ReturnMessage(false) { Message = "修改用户资料失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<MemberEntity>(json, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                if (entity == null)
                {
                    result.Message = "无效对象";
                    return Json(result);
                }
                entity.MemberId = LoginUser.UserId;
                MemberBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "修改成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>UpdateInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
        /// <summary>
        /// 获取会员积分信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPoint()
        {
            var result = new ReturnMessage(false) { Message = "获取用户积分信息失败!" };
            try
            {
                decimal CurrentPoint = 0;
                int TodayPoint = 0;
                int TotalPoint = 0;
                var account = MemberBLL.Instance.GetEntity(LoginUser.UserId);
                if (account != null && account.Point != null)
                {
                    CurrentPoint = account.Point ?? 0;
                }
                PointLogEntity para = new PointLogEntity();
                para.MemberId = LoginUser.UserId;
                para.sidx = "AddTime";
                para.sord = "desc";
                var pointList = PointLogBLL.Instance.GetList(para);
                if (pointList != null)
                {
                    TodayPoint = pointList.Where(o => o.Operate == (int)QX360.Model.Enums.PointOperateType.增加 && DateTime.Parse(o.AddTime.ToString()).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")).Sum((detail) => { return detail.Point ?? 0; });
                    TotalPoint = pointList.Where(o => o.Operate == (int)QX360.Model.Enums.PointOperateType.增加).Sum((detail) => { return detail.Point ?? 0; });
                }

                result.IsSuccess = true;
                result.ResultData["CurrentPoint"] = CurrentPoint;
                result.ResultData["TodayPoint"] = TodayPoint;
                result.ResultData["TotalPoint"] = TotalPoint;
                result.ResultData["List"] = pointList;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>GetUserDetail";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }
    }
}
