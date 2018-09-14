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
    public class StudyController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /Study/
        [AuthorizeFilter]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }
        public ActionResult Teacher()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Time()
        {
            return View();
        }

        public ActionResult NoTeacher()
        {
            return View();
        }

        /// <summary>
        /// 查询本人的学车订单
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetMyStudyOrder(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                StudyOrderEntity para = new StudyOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }

                para.MemberId = LoginUser.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = StudyOrderBLL.Instance.GetList(para);
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyController>>GetMyStudyOrder";
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
                //判断取消的权限
                int lev = 0;
                int.TryParse(LoginUser.MemberLevId, out lev);
                var order = StudyOrderBLL.Instance.GetEntity(id);
                DateTime orderdatetime = DateTime.Now;
                if (order != null)
                {
                    orderdatetime = DateTime.Parse(order.CreateTime.ToString());
                }
                int space = 0;
                switch (lev)
                {
                    case (int)QX360.Model.Enums.UserType.预约记时会员:
                        int.TryParse(SettingsBLL.Instance.GetValue("canoffordertime_normal"), out space);
                        break;
                    case (int)QX360.Model.Enums.UserType.VIP会员:
                        int.TryParse(SettingsBLL.Instance.GetValue("canoffordertime_vip"), out space);
                        break;
                }
                if (DateTime.Now > orderdatetime.AddHours(space))
                {
                    result.Message = string.Format("该订单已无法取消,只能取消{0}小时内的预约", space);
                    return Json(result);
                }
                StudyOrderBLL.Instance.Cancel(id);
                result.IsSuccess = true;
                result.Message = "取消成功";
                string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime.TrimEnd(','));
                var teacher = TeacherBLL.Instance.GetEntity(order.TeacherId);
                string txt = string.Format("预约学车,预约人:{0},联系电话:{1},预约机构:{2},个人原因取消预约", teacher.Name, teacher.Mobile, order.SchoolName);
                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.学车预约提示, QX360.Model.Enums.NoticeType.取消提醒, LoginUser.UserId, order.MemberName, servicetime, txt, order.StudyOrderNo);

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取学车订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult GetStudyModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {
                var data = StudyOrderBLL.Instance.GetEntity(id);
                result.IsSuccess = true;
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyController>>GetStudyModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建学车订单
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
                    var order = JsonConvert.DeserializeObject<StudyOrderEntity>(data);
                    if (order == null)
                    {
                        result.Message = "无效对象";
                        return Json(result);
                    }
                    if (order.DetailList == null || order.DetailList.Count == 0)
                    {
                        result.Message = "请选择预约时间";
                        return Json(result);
                    }
                    var member = MemberBLL.Instance.GetEntity(LoginUser.UserId);
                    if (member == null)
                    {
                        result.Message = "会员对象无效";
                        return Json(result);
                    }

                    //验证时间段是否有预约
                    var hasflag = false;
                    foreach (var item in order.DetailList)
                    {

                        var freetime = StudyFreeTimeBLL.Instance.GetEntity(item.StudyFreeTimeId);
                        if (freetime != null)
                        {
                            if (freetime.FreeStatus != (int)QX360.Model.Enums.FreeTimeStatus.空闲)
                            {
                                hasflag = true;
                                break;
                            }
                        }
                    }
                    if (hasflag)
                    {
                        result.Message = "您下手晚了,请重新选择预约时间";
                        return Json(result);
                    }


                    //判断课时是否还有
                    if (order.DetailList != null)
                    {

                        int hour1 = 0;
                        int hour2 = 0;
                        foreach (var item in order.DetailList)
                        {
                            if (item.TimeType == (int)QX360.Model.Enums.WorkTimeType.白班)
                            {
                                hour1++;
                            }
                            else if (item.TimeType == (int)QX360.Model.Enums.WorkTimeType.夜班)
                            {
                                hour2++;
                            }
                        }
                        if ((member.StudyHour1 ?? 0) < hour1)
                        {
                            result.Message = "白班课时不足,预约此时段失败";
                            return Json(result);
                        }
                        if ((member.StudyHour2 ?? 0) < hour2)
                        {
                            result.Message = "夜班课时不足,预约此时段失败";
                            return Json(result);
                        }
                    }
                    //判断是否存在有未评价的订单
                    var commitcount = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                    {
                        MemberId = LoginUser.UserId,
                        Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约完成
                    }).Count();
                    if (commitcount > 0)
                    {
                        result.Message = "请先对上次学车订单进行评价后再次预约学习";
                        return Json(result);
                    }
                    order.StudyOrderId = Util.NewUpperGuid();
                    order.StudyOrderNo = StudyOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.StudySubscribeStatus.预约成功;
                    order.MemberId = LoginUser.UserId;
                    order.MemberName = LoginUser.NickName;
                    order.MemberMobile = LoginUser.Mobile;
                    string _ServiceTime = "";
                    if (StudyOrderBLL.Instance.Add(order))
                    {
                        if (order.DetailList != null)
                        {
                            foreach (var item in order.DetailList)
                            {
                                StudyOrderDetailEntity detail = new StudyOrderDetailEntity();
                                detail.StudyOrderDetailId = Util.NewUpperGuid();
                                detail.TimeType = item.TimeType;
                                detail.ServiceTime = item.ServiceTime;
                                detail.ServiceDate = item.ServiceDate;
                                detail.StudyOrderId = order.StudyOrderId;
                                detail.StudyFreeTimeId = item.StudyFreeTimeId;
                                if (StudyOrderDetailBLL.Instance.Add(detail))
                                {
                                    _ServiceTime += detail.ServiceTime + ",";
                                    //修改预约时间状态
                                    StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                                    freetime.StudyFreeTimeId = detail.StudyFreeTimeId;
                                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                                    StudyFreeTimeBLL.Instance.Update(freetime);
                                    //修改对应课时
                                    if (detail.TimeType == (int)QX360.Model.Enums.WorkTimeType.白班)
                                    {
                                        member.MemberId = LoginUser.UserId;
                                        member.StudyHour1 = member.StudyHour1 - 1;
                                        MemberBLL.Instance.Update(member);
                                    }
                                    else if (detail.TimeType == (int)QX360.Model.Enums.WorkTimeType.夜班)
                                    {
                                        member.MemberId = LoginUser.UserId;
                                        member.StudyHour2 = member.StudyHour2 - 1;
                                        MemberBLL.Instance.Update(member);
                                    }

                                }

                            }
                        }
                    }
                    result.IsSuccess = true;
                    result.Message = "创建成功";
                    //写消息
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                    var teacher = TeacherBLL.Instance.GetEntity(order.TeacherId);
                    string txt = string.Format("预约学车,预约人:{0},联系电话:{1},预约机构:{2}", teacher.Name, teacher.Mobile, order.SchoolName);
                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.学车预约提示, QX360.Model.Enums.NoticeType.预约提醒, LoginUser.UserId, order.MemberName, servicetime, txt, order.StudyOrderNo);

                    txt = string.Format("预约学车,预约人:{0},联系电话:{1}", order.MemberName, order.MemberMobile);
                    SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, teacher.TeacherId, servicetime, txt, order.StudyOrderNo);


                    //送积分
                    GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.预约学车完成, LoginUser.UserId, 0, order.StudyOrderNo);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取教练下的本周学车订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTeacherStudyList(int status, string servicedate)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                StudyOrderEntity para = new StudyOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
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
                var list = StudyOrderBLL.Instance.GetList(para);
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
                            o.StatusName = ((QX360.Model.Enums.StudySubscribeStatus)o.Status).ToString();
                        }
                    });
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MaCenterController>>GetTeacherStudyList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
