using Aspose.Words;
using Aspose.Words.Drawing;
using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    /// <summary>
    /// 在线报名
    /// </summary>
    public class ApplyController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /Apply/

        public ActionResult Index()
        {
            return View();
        }
        //
        [AuthorizeFilter]
        public ActionResult Information()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }
        public ActionResult SchoolInfo()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult NetSigin()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult SiginNotice()
        {
            return View();
        }
        /// <summary>
        /// 获取banner广告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Banner()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var list = BannerBLL.Instance.GetList(null);
                list.Foreach((o) =>
                {
                    if (o.ImgPath != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        o.ImgPath = imageHost + o.ImgPath;
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>Banner";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证是否已报名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckHasApplay()
        {
            var result = new ReturnMessage(false) { Message = "验证失败!" };
            try
            {
                var num = ApplyOrderBLL.Instance.CheckHasApplay(LoginUser.UserId);
                result.IsSuccess = true;
                result.Message = "验证成功";
                result.ResultData["Data"] = num;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>CheckHasApplay";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查报名缴费
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult CheckHasApplyPay()
        {
            var result = new ReturnMessage(false) { Message = "检查报名缴费失败!" };
            try
            {
                if (LoginUser == null)
                {
                    result.Code = -1;
                    result.Message = "请先登陆";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //判断用户是否完成缴费
                int applaycount = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { MemberId = LoginUser.UserId, Status = (int)QX360.Model.Enums.ApplyStatus.已支付 }).Count();
                if (applaycount == 0)
                {
                    result.Code = -2;
                    result.Message = "请完成报名缴费后再预约学习";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>CheckHasApplyPay";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前报名信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMyOrderInfo()
        {
            var result = new ReturnMessage(false) { Message = "获取当前报名信息!" };
            try
            {
                var data = new ApplyOrderEntity();
                var list = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { MemberId = LoginUser.UserId });
                if (list.Count > 0)
                {
                    data = list.FirstOrDefault();
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["Data"] = data;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetMyOrderInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取用户的报名信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetApplayInfo(string memberid)
        {
            var result = new ReturnMessage(false) { Message = "获取当前报名信息!" };
            try
            {
                var data = new ApplyOrderEntity();
                var list = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { MemberId = memberid });
                if (list.Count > 0)
                {
                    data = list.FirstOrDefault();
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["Data"] = data;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetApplayInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 创建报名订单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult CreateOrder(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {
                lock (objLock)
                {
                    var order = JsonConvert.DeserializeObject<ApplyOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    order.ApplyOrderId = Util.NewUpperGuid();
                    order.ApplyOrderNo = ApplyOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.ApplyStatus.待支付;
                    //order.CreateId = LoginUser.UserId;
                    ApplyOrderBLL.Instance.Add(order);
                    result.IsSuccess = true;
                    result.Message = "创建成功";

                    //修改会员表中对应驾校信息
                    MemberEntity member = new MemberEntity();
                    member.SchoolId = order.SchoolId;
                    member.SchoolName = order.SchoolName;
                    member.MemberId = LoginUser.UserId;
                    MemberBLL.Instance.Update(member);

                    //送积分
                    GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.预约驾校成功, LoginUser.UserId, double.Parse(order.TotalMoney.ToString()), order.ApplyOrderNo);

                    //插入财务表
                    FinaceBLL.Instance.Add(new FinaceEntity()
                    {
                        FinaceId = QSDMS.Util.Util.NewUpperGuid(),
                        SourceType = (int)QX360.Model.Enums.FinaceSourceType.驾校报名,
                        CreateTime = DateTime.Now,
                        CosMoney = order.PayMoney,
                        Status = (int)QX360.Model.Enums.PaySatus.已支付,
                        MemberId = order.MemberId,
                        MemberName = order.MemberName,
                        PayType = (int)QX360.Model.Enums.PayType.微信支付,
                        Operate = (int)QX360.Model.Enums.FinaceOperateType.增加,
                        Remark = string.Format("报名学车|{0}|{1}", order.MemberName, order.ApplyOrderNo)

                    });

                    ////修改会员等级
                    //MemberEntity member = new MemberEntity();
                    //member.MemberId = order.MemberId;
                    //member.LevId = ((int)QX360.Model.Enums.UserType.VIP会员).ToString();
                    //member.LevName = QX360.Model.Enums.UserType.VIP会员.ToString();
                    //member.VipOverDate = DateTime.Now.AddYears(1);
                    //MemberBLL.Instance.Update(member);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "OrderController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取会员报名驾校和教练
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMyApplyInfo()
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var list = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { MemberId = LoginUser.UserId });
                if (list == null)
                {
                    result.Message = "您还未报名!";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var data = list.FirstOrDefault();
                if (data != null)
                {
                    if (data.SchoolId != null)
                    {
                        var school = SchoolBLL.Instance.GetEntity(data.SchoolId);
                        if (school != null)
                        {
                            if (school.FaceImage != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                school.FaceImage = imageHost + school.FaceImage;
                            }
                            if (school.ProvinceId != null)
                            {
                                school.ProvinceName = AreaBLL.Instance.GetEntity(school.ProvinceId).AreaName;
                            }
                            if (school.CityId != null)
                            {
                                school.CityName = AreaBLL.Instance.GetEntity(school.CityId).AreaName;
                            }
                            if (school.CountyId != null)
                            {
                                school.CountyName = AreaBLL.Instance.GetEntity(school.CountyId).AreaName;
                            }
                            school.AddressInfo = school.ProvinceName + school.CityName + school.CountyName + school.AddressInfo;
                            school.TagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = school.SchoolId });
                            data.School = school;
                        }
                    }
                    if (data.TeacherId != null)
                    {
                        var teacher = TeacherBLL.Instance.GetEntity(data.TeacherId);
                        if (teacher != null)
                        {
                            if (teacher.FaceImage != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                teacher.FaceImage = imageHost + teacher.FaceImage;
                            }
                            data.Teacher = teacher;
                        }
                    }
                }

                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MaCenterController>>GetMySchoolModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取教练下的学员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTeacherApplyList(string keyword)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                ApplyOrderEntity para = new ApplyOrderEntity();
                para.TeacherId = LoginTeacher.UserId;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    para.MemberName = keyword;
                }
                var list = ApplyOrderBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.Foreach((o) =>
                    {
                        if (o.MemberId != null)
                        {
                            var member = MemberBLL.Instance.GetEntity(o.MemberId);
                            if (member != null)
                            {
                                if (member.HeadIcon != null)
                                {
                                    var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                    member.HeadIcon = imageHost + member.HeadIcon;
                                }
                            }
                        }
                    });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MaCenterController>>GetTeacherApplyList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveNetSigin(string json)
        {
            var result = new ReturnMessage(false) { Message = "用户网签失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<NetSiginEntity>(json, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                if (entity == null)
                {
                    result.Message = "无效对象";
                    return Json(result);
                }
                var list = NetSiginBLL.Instance.GetList(new NetSiginEntity() { MemberId = LoginUser.UserId });
                if (list.Count() > 0)
                {
                    result.Message = "您已网签过了,无需重复操作";
                    return Json(result);
                }
                entity.NetSiginId = Util.NewUpperGuid();
                entity.MemberId = LoginUser.UserId;
                entity.CreateTime = DateTime.Now;
                NetSiginBLL.Instance.Add(entity);
                result.IsSuccess = true;
                result.Message = "网签成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>SaveNetSigin";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }


        /// <summary>
        /// 检查网签
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckHasSigin()
        {
            var result = new ReturnMessage(false) { Message = "检查网签失败!" };
            try
            {
                if (LoginUser == null)
                {
                    result.Code = -1;
                    result.Message = "请先登陆";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //判断用户是否完成缴费
                var list = NetSiginBLL.Instance.GetList(new NetSiginEntity() { MemberId = LoginUser.UserId });
                if (list.Count() > 0)
                {
                    result.Message = "您已网签过了,无需重复操作";
                    result.ResultData["Id"] = list.FirstOrDefault().NetSiginId;
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>CheckHasSigin";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
    }
}
