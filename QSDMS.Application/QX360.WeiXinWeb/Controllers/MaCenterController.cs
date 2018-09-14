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
    public class MaCenterController : BaseController
    {
        //
        // GET: /MaCenter/
        public ActionResult Login(string oauthflag)
        {
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
            var returnUrl = Request.Params["returnUrl"];
            //OAuth2获取微信的相关信息,写入数据库后自动登陆
            string appid = QSDMS.Util.Config.GetValue("APPID");//"wx9b0af89f3c518363";           
            var redricturl = QSDMS.Util.Config.GetValue("webSite") + "/wx/Info2?returnurl=" + returnUrl;
            var url = OAuthApi.GetAuthorizeUrl(appid, redricturl, "dlb", OAuthScope.snsapi_userinfo);//可以获取微信信息
            ViewBag.ReturnUrl = returnUrl;
            Response.Redirect(url);
            return null;
            //var returnUrl = Request.Params["returnUrl"];
            //ViewBag.ReturnUrl = returnUrl;
            //return View();
        }

        public void ClearCache()
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

        [MaAuthorizeFilter]
        public ActionResult Index()
        {
            ViewBag.Id = LoginTeacher.UserId;
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult LearnCar()
        {
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult SetTime()
        {
            ViewBag.ObjectId = LoginTeacher.UserId;
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult Student()
        {
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult WithDriving()
        {
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult Training()
        {
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult TeacherCommittee()
        {
            return View();
        }

        [MaAuthorizeFilter]
        public ActionResult SetStudyTime()
        {
            ViewBag.ObjectId = LoginTeacher.UserId;
            return View();
        }

        [MaAuthorizeFilter]
        public ActionResult WithDrivingSetTime()
        {
            ViewBag.ObjectId = LoginTeacher.UserId;
            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult Information()
        {

            return View();
        }
        [MaAuthorizeFilter]
        public ActionResult Password()
        {

            return View();
        }


        /// <summary>
        /// 教练平台-我的工作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult MyWork()
        {
            var result = new ReturnMessage(false) { Message = "获取用户积分信息失败!" };
            try
            {
                var list = new List<MyOrder>();
                int StudentCount = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { TeacherId = LoginTeacher.UserId }).Count();
                int StudyOrderCount = 0;
                var studyorderList = StudyOrderBLL.Instance.GetList(new StudyOrderEntity() { TeacherId = LoginTeacher.UserId, Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约成功 });
                if (studyorderList != null)
                {
                    StudyOrderCount = studyorderList.Count();
                    studyorderList.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            if (DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                MyOrder order = new MyOrder();
                                order.Type = 1;
                                order.OrderNo = o.StudyOrderNo;
                                order.ServiceDate = o.ServiceDate;
                                order.ServiceTime = o.ServiceTime;
                                order.MemberName = o.MemberName;
                                order.MemberMobile = o.MemberMobile;
                                order.Status = o.Status ?? 0;
                                order.StatusName = ((QX360.Model.Enums.StudySubscribeStatus)o.Status).ToString();
                                order.Title = "预约学车";
                                list.Add(order);
                            }
                        }
                    });

                }
                int WithDrivingCount = 0;
                var withDrivingList = WithDrivingOrderBLL.Instance.GetList(new WithDrivingOrderEntity() { TeacherId = LoginTeacher.UserId, Status = (int)QX360.Model.Enums.PaySatus.已支付 });
                if (withDrivingList != null)
                {
                    WithDrivingCount = withDrivingList.Count();
                    withDrivingList.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            if (DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                MyOrder order = new MyOrder();
                                order.Type = 2;
                                order.OrderNo = o.DrivingOrderNo;
                                order.ServiceDate = o.ServiceDate;
                                order.ServiceTime = o.ServiceTime;
                                order.MemberName = o.MemberName;
                                order.MemberMobile = o.MemberMobile;
                                order.Status = o.Status ?? 0;
                                order.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                                order.Title = "预约陪驾";
                                list.Add(order);
                            }
                        }
                    });
                }
                int trainingCount = 0;
                var trainingList = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { MemberId = LoginTeacher.UserId, Status = (int)QX360.Model.Enums.PaySatus.已支付 });
                if (trainingList != null)
                {
                    trainingCount = trainingList.Count();
                }
                result.IsSuccess = true;
                result.ResultData["StudentCount"] = StudentCount;
                result.ResultData["StudyOrderCount"] = StudyOrderCount;
                result.ResultData["WithDrivingCount"] = WithDrivingCount;
                result.ResultData["TrainingCount"] = trainingCount;
                result.ResultData["List"] = list;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MaCenterController>>MyWork";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    public class MyOrder
    {
        public string Title { get; set; }
        public int Type { get; set; }
        public string OrderNo { get; set; }

        public DateTime? ServiceDate { get; set; }
        public string ServiceTime { get; set; }
        public string MemberName { get; set; }
        public string MemberMobile { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
    }
}
