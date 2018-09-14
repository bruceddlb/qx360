using iFramework.Framework;
using Newtonsoft.Json;
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
    public class WithDrivingController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /WithDriving/

        public ActionResult List()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Submit()
        {
            var notice = SettingsBLL.Instance.GetRemark("pjxz");
            ViewBag.Notice = notice;
            return View();
        }
        public ActionResult Teacher()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }

        public ActionResult Nav()
        {
            return View();
        }
        public ActionResult Xuanzhe()
        {

            return View();
        }
        public ActionResult Choose1()
        {

            return View();
        }
        public ActionResult Choose2()
        {

            return View();
        }



        /// <summary>
        /// 查询本人的实训订单
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetMyWithDriving(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                WithDrivingOrderEntity para = new WithDrivingOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }
                para.MemberId = LoginUser.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = WithDrivingOrderBLL.Instance.GetList(para);
                list.Foreach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingController>>GetMyWithDriving";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Cancel(string id)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            try
            {
                WithDrivingOrderEntity entity = new WithDrivingOrderEntity();
                entity.DrivingOrderId = id;
                entity.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                WithDrivingOrderBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "取消成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateOrder(string data, string freetimeid)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {
                lock (objLock)
                {
                    var order = JsonConvert.DeserializeObject<WithDrivingOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    //验证时间段是否有预约                 
                    var freetime = WithDrivingFreeTimeBLL.Instance.GetEntity(freetimeid);
                    if (freetime != null)
                    {
                        if (freetime.FreeStatus != (int)QX360.Model.Enums.FreeTimeStatus.空闲)
                        {
                            result.Message = "您下手晚了,请重新选择预约时间";
                            return Json(result);
                        }
                    }

                    order.DrivingOrderId = Util.NewUpperGuid();
                    order.DrivingOrderNo = WithDrivingOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.PaySatus.已支付;
                    if (WithDrivingOrderBLL.Instance.Add(order))
                    {
                        //更改时间状态
                        freetime = new WithDrivingFreeTimeEntity();
                        freetime.WithDrivingFreeTimeId = freetimeid;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                        WithDrivingFreeTimeBLL.Instance.Update(freetime);
                    }
                    result.IsSuccess = true;
                    result.Message = "创建成功";
                    //写消息
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime);
                    var teacher = TeacherBLL.Instance.GetEntity(order.TeacherId);
                    if (teacher != null)
                    {
                        string content = string.Format("预约陪驾,所需费用:{1},陪驾教练：{2},联系电话：{3},陪驾车辆:{0}", order.IsBandCar == 1 ? "教练提供" : "个人提供", order.Price, teacher.Name, teacher.Mobile);
                        SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.陪驾预约提示, QX360.Model.Enums.NoticeType.预约提醒, LoginUser.UserId, order.MemberName, servicetime, content, order.DrivingOrderNo);
                        //对象陪驾教练发送消息
                        content = string.Format("预约陪驾,所需费用:{1},陪驾车辆:{0},预约人：{2}，联系电话：{3}", order.IsBandCar == 1 ? "教练提供" : "个人提供", order.Price, order.MemberName, order.MemberMobile);
                        SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, order.TeacherId, servicetime, content, order.DrivingOrderNo);
                    }
                    //送积分
                    GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.预约陪驾完成, LoginUser.UserId, double.Parse(order.Price.ToString()), order.DrivingOrderNo);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取教练下的陪驾订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTeacherWithDrivingList(int status, string servicedate)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                WithDrivingOrderEntity para = new WithDrivingOrderEntity();
                switch (status)
                {
                    case (int)QX360.Model.Enums.PaySatus.待支付:
                        para.Status = (int)QX360.Model.Enums.PaySatus.待支付;
                        break;
                    case (int)QX360.Model.Enums.PaySatus.已取消:
                        para.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                        break;
                    case (int)QX360.Model.Enums.PaySatus.已完成:
                        para.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                        break;
                    case (int)QX360.Model.Enums.PaySatus.已支付:
                        para.Status = (int)QX360.Model.Enums.PaySatus.已支付;
                        break;
                }
                para.TeacherId = LoginTeacher.UserId;
                if (!string.IsNullOrWhiteSpace(servicedate))
                {
                    para.ServiceDate = DateTime.Parse(servicedate);
                }
                //para.StartTime = QSDMS.Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString();
                //para.EndTime = QSDMS.Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString();
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = WithDrivingOrderBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.Foreach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            var weekname = QSDMS.Util.Time.GetChineseWeekDay(Converter.ParseDateTime(o.ServiceDate));
                            o.ServiceTime = DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd") + " " + weekname + " " + o.ServiceTime;

                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                        }
                    });
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MaCenterController>>GetTeacherWithDrivingList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}
