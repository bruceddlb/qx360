using iFramework.Framework;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class ExamPlaceCenterController : BaseController
    {
        //
        // GET: /ExamPlaceCenter/
        [ExamPlaceAuthorizeFilter]
        public ActionResult Index()
        {
            ViewBag.Id = LoginExamPlace.UserId;
            return View();
        }

        [ExamPlaceAuthorizeFilter]
        public ActionResult Training()
        {
            return View();
        }
        [ExamPlaceAuthorizeFilter]
        public ActionResult Choose1()
        {
            return View();
        }
        [ExamPlaceAuthorizeFilter]
        public ActionResult Choose2()
        {
            return View();
        }
        //
        // GET: /MaCenter/
        public ActionResult Login()
        {

            var sig = Request.Params["sig"];
            if (sig == "out")
            {
                ClearCache();
            }
            return View();
        }
        [ExamPlaceAuthorizeFilter]
        public ActionResult Information()
        {

            return View();
        }
        [ExamPlaceAuthorizeFilter]
        public ActionResult Password()
        {

            return View();
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


        /// <summary>
        /// 教练平台-我的工作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult MyWork()
        {
            var result = new ReturnMessage(false) { Message = "获取信息失败!" };
            try
            {
                var list = new List<MyTraingOrder>();
                //int count = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { SchoolId = LoginExamPlace.UserId });
                int Count1 = 0;
                var trainingList1 = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { SchoolId = LoginExamPlace.UserId, Status = (int)QX360.Model.Enums.TrainingStatus.已支付, TrainingType = (int)QX360.Model.Enums.TrainingType.科目二 });
                if (trainingList1 != null)
                {
                    Count1 = trainingList1.Count();
                    trainingList1.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            if (DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                MyTraingOrder order = new MyTraingOrder();
                                order.Type = 1;
                                order.OrderNo = o.TrainingOrderNo;
                                order.ServiceDate = o.ServiceDate;
                                order.ServiceTime = o.ServiceTime;
                                order.MemberName = o.MemberName;
                                order.MemberMobile = o.MemberMobile;
                                order.Status = o.Status ?? 0;
                                order.StatusName = ((QX360.Model.Enums.TrainingStatus)o.Status).ToString();
                                order.CarName = o.TrainingCarName;
                                order.CarNum = o.TrainingCarNumber;
                                order.Title = "科目二";
                                list.Add(order);
                            }
                        }
                    });

                }
                int Count2 = 0;
                var trainingList2 = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { SchoolId = LoginExamPlace.UserId, Status = (int)QX360.Model.Enums.TrainingStatus.已支付, TrainingType = (int)QX360.Model.Enums.TrainingType.科目三 });
                if (trainingList2 != null)
                {
                    Count2 = trainingList2.Count();
                    trainingList2.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            if (DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                MyTraingOrder order = new MyTraingOrder();
                                order.Type = 1;
                                order.OrderNo = o.TrainingOrderNo;
                                order.ServiceDate = o.ServiceDate;
                                order.ServiceTime = o.ServiceTime;
                                order.MemberName = o.MemberName;
                                order.MemberMobile = o.MemberMobile;
                                order.Status = o.Status ?? 0;
                                order.StatusName = ((QX360.Model.Enums.TrainingStatus)o.Status).ToString();
                                order.CarName = o.TrainingCarName;
                                order.CarNum = o.TrainingCarNumber;
                                order.Title = "科目三";
                                list.Add(order);
                            }
                        }
                    });

                }

                result.IsSuccess = true;

                result.ResultData["Count1"] = Count1;
                result.ResultData["Count2"] = Count2;
                result.ResultData["List"] = list;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceCenterController>>MyWork";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMyTraingOrder(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                TrainingOrderEntity para = new TrainingOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }

                para.SchoolId = LoginExamPlace.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = TrainingOrderBLL.Instance.GetList(para);
                list.Foreach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.TrainingStatus)o.Status).ToString();
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceCenterController>>GetMyTraingOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取驾校相关对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetExamPlaceCenterModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var subject = new List<SubjectEntity>();
                var data = ExamPlaceMasterBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (!string.IsNullOrWhiteSpace(data.FaceImage))
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.FaceImage = imageHost + data.FaceImage;
                    }
                    if (data.ExamPlaceIds != null)
                    {
                        var arr = data.ExamPlaceIds.Split(',');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var item = arr[i];
                            if (item != "")
                            {
                                var subjectlist = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = item });
                                if (subjectlist != null && subjectlist.Count > 0)
                                {
                                    subject.Add(subjectlist.FirstOrDefault());
                                }
                            }
                        }
                    }
                }

                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;
                result.ResultData["SubjectList"] = subject;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceCenterController>>GetExamPlaceCenterModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    public class MyTraingOrder
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
        public string CarName { get; set; }
        public string CarNum { get; set; }
    }
}
