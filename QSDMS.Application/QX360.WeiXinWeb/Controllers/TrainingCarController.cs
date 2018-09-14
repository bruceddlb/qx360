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
using QSDMS.Util.Extension;
namespace QX360.WeiXinWeb.Controllers
{
    public class TrainingCarController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /TrainingCar/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTrainingCarList(string schoolid, string trainingtype)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var list = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { SchoolId = schoolid, TrainingType = int.Parse(trainingtype) }).OrderBy(o => o.SortNum);
                foreach (var page in list)
                {
                    if (page.FaceImage != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        page.FaceImage = imageHost + page.FaceImage;
                    }

                }


                result.IsSuccess = true;
                result.Message = "success";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>GetTrainingCarModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTrainingCarModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var data = TrainingCarBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (data.SchoolId != null)
                    {
                        data.School = SchoolBLL.Instance.GetEntity(data.SchoolId);
                    }
                    if (data.FaceImage != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.FaceImage = imageHost + data.FaceImage;
                    }
                }
                result.IsSuccess = true;
                result.Message = "success";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>GetTrainingCarModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询本人的实训订单
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
                //switch (status)
                //{
                //    case (int)QX360.Model.Enums.SubscribeStatus.预约成功:
                //        para.Status = (int)QX360.Model.Enums.SubscribeStatus.预约成功;
                //        break;
                //    case (int)QX360.Model.Enums.SubscribeStatus.已取消:
                //        para.Status = (int)QX360.Model.Enums.SubscribeStatus.已取消;
                //        break;
                //}
                if (status != -1)
                {
                    para.Status = status;
                }

                para.MemberId = LoginUser.UserId;
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
                ex.Data["Method"] = "TrainingCarController>>GetMyTraingOrder";
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
                var order = TrainingOrderBLL.Instance.GetEntity(id);
                TrainingOrderBLL.Instance.Cancel(id);
                result.IsSuccess = true;
                result.Message = "取消成功";

                string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime.TrimEnd(','));
                var trainingCar = TrainingCarBLL.Instance.GetEntity(order.TrainingCarId);
                string txt = string.Format("实训预约,预约车辆:{0},车牌:{1},预约机构:{2},个人原因取消预约", trainingCar.Name, trainingCar.CarNumber, order.SchoolName);
                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.取消提醒, LoginUser.UserId, order.MemberName, servicetime, txt, order.TrainingOrderNo);

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateOrder(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {

                lock (objLock)
                {
                    var order = JsonConvert.DeserializeObject<TrainingOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    //验证时间段是否有预约
                    var hasflag = false;
                    foreach (var item in order.DetailList)
                    {

                        var freetime = TrainingFreeTimeBLL.Instance.GetEntity(item.TrainingFreeTimeId);
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

                    order.TrainingOrderId = Util.NewUpperGuid();
                    order.TrainingOrderNo = TrainingOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.TrainingStatus.待审核;
                    order.MemberId = LoginUser.UserId;
                    order.MemberName = LoginUser.NickName;
                    order.MemberMobile = LoginUser.Mobile;
                    order.UserType = (int)QX360.Model.Enums.TrainingUserType.学员;
                    string _ServiceTime = "";
                    if (TrainingOrderBLL.Instance.Add(order))
                    {
                        if (order.DetailList != null)
                        {
                            foreach (var item in order.DetailList)
                            {
                                TrainingOrderDetailEntity detail = new TrainingOrderDetailEntity();
                                detail.TrainingOrderDetailId = Util.NewUpperGuid();
                                detail.ServiceTime = item.ServiceTime;
                                detail.ServiceDate = item.ServiceDate;
                                detail.TrainingOrderId = order.TrainingOrderId;
                                detail.TrainingFreeTimeId = item.TrainingFreeTimeId;
                                if (TrainingOrderDetailBLL.Instance.Add(detail))
                                {
                                    _ServiceTime += detail.ServiceTime + ",";
                                    //修改预约时间状态
                                    TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                                    freetime.TrainingFreeTimeId = detail.TrainingFreeTimeId;
                                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.锁定;
                                    TrainingFreeTimeBLL.Instance.Update(freetime);
                                }

                            }

                        }
                    }
                    result.IsSuccess = true;
                    result.Message = "创建成功";

                    //写消息
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                    string txt = "预约实训," + order.SchoolName + order.TrainingTypeName + "考场实训";
                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.预约提醒, LoginUser.UserId, order.MemberName, servicetime, txt, order.TrainingOrderNo);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
        [HttpPost]
        public JsonResult CreateOrder2(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {
                lock (objLock)
                {
                    var order = JsonConvert.DeserializeObject<TrainingOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    //验证时间段是否有预约
                    var hasflag = false;
                    foreach (var item in order.DetailList)
                    {

                        var freetime = TrainingFreeTimeBLL.Instance.GetEntity(item.TrainingFreeTimeId);
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

                    int _TrainingStatus = (int)Model.Enums.TrainingStatus.待支付;
                    int _FreeTimeStatus = (int)Model.Enums.FreeTimeStatus.已预约;
                    //判断教练师傅黑名单，黑名单需要审核
                    var blackList = BlackListBLL.Instance.GetList(new BlackListEntity() { ObjectId = LoginTeacher.UserId });
                    if (blackList != null && blackList.Count > 0)
                    {
                        _TrainingStatus = (int)Model.Enums.TrainingStatus.待审核;
                        _FreeTimeStatus = (int)Model.Enums.FreeTimeStatus.锁定;
                    }
                    order.TrainingOrderId = Util.NewUpperGuid();
                    order.TrainingOrderNo = TrainingOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = _TrainingStatus;
                    order.MemberId = LoginTeacher.UserId;
                    order.MemberName = LoginTeacher.UserName;
                    order.MemberMobile = LoginTeacher.Mobile;
                    order.UserType = (int)QX360.Model.Enums.TrainingUserType.教练;
                    string _ServiceTime = "";
                    if (TrainingOrderBLL.Instance.Add(order))
                    {
                        if (order.DetailList != null)
                        {
                            foreach (var item in order.DetailList)
                            {
                                TrainingOrderDetailEntity detail = new TrainingOrderDetailEntity();
                                detail.TrainingOrderDetailId = Util.NewUpperGuid();
                                detail.ServiceTime = item.ServiceTime;
                                detail.ServiceDate = item.ServiceDate;
                                detail.TrainingOrderId = order.TrainingOrderId;
                                detail.TrainingFreeTimeId = item.TrainingFreeTimeId;
                                if (TrainingOrderDetailBLL.Instance.Add(detail))
                                {
                                    _ServiceTime += detail.ServiceTime + ",";
                                    //修改预约时间状态
                                    TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                                    freetime.TrainingFreeTimeId = detail.TrainingFreeTimeId;
                                    freetime.FreeStatus = _FreeTimeStatus;
                                    TrainingFreeTimeBLL.Instance.Update(freetime);
                                }
                            }
                        }
                    }
                    result.IsSuccess = true;
                    result.Message = "创建成功";

                    //写消息
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                    string tex = "预约实训," + order.SchoolName + order.TrainingTypeName + "考场实训";
                    if (_TrainingStatus == (int)Model.Enums.TrainingStatus.待审核)
                    {
                        tex = "预约实训," + order.SchoolName + order.TrainingTypeName + "考场实训,该订单等待管理员审核。";
                    }
                    SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, LoginTeacher.UserId, servicetime, tex, order.TrainingOrderNo);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetTeacherTrainingList(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                TrainingOrderEntity para = new TrainingOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }

                para.MemberId = LoginTeacher.UserId;
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
                ex.Data["Method"] = "MaCenterController>>GetTeacherWithDrivingList";
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
        public JsonResult Cancel2(string id)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            try
            {
                var order = TrainingOrderBLL.Instance.GetEntity(id);
                TrainingOrderBLL.Instance.Cancel(id);
                result.IsSuccess = true;
                result.Message = "取消成功";

                string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime.TrimEnd(','));
                var trainingCar = TrainingCarBLL.Instance.GetEntity(order.TrainingCarId);
                string txt = string.Format("实训预约,预约车辆:{0},车牌:{1},预约机构:{2},个人原因取消预约", trainingCar.Name, trainingCar.CarNumber, order.SchoolName);
                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.取消提醒, LoginTeacher.UserId, order.MemberName, servicetime, txt, order.TrainingOrderNo);

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetGetSubInfo(string freetimeid)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            try
            {
                ExtTrainingEntity rs = GetInfo(freetimeid);
                result.ResultData["Info"] = rs;
                result.IsSuccess = true;
                result.Message = "成功";


            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>GetGetSubInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetTrainingCar(string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            List<ExtTrainingCarEntity> carList = new List<ExtTrainingCarEntity>();
            try
            {
                var queryParam = queryJson.ToJObject();

                if (!queryParam["trainingTimeTableId"].IsEmpty() && !queryParam["freedate"].IsEmpty() && !queryParam["trainingtype"].IsEmpty() && !queryParam["schoolid"].IsEmpty())
                {
                    //查询实训时间表


                    //查询指定的时间
                    var freedateList = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                    {
                        StartTime = queryParam["freedate"].ToString(),
                        EndTime = queryParam["freedate"].ToString()
                    });

                    foreach (var freetdate in freedateList)
                    {
                        var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                        {
                            TrainingTimeTableId = queryParam["trainingTimeTableId"].ToString(),
                            TrainingFreeDateId = freetdate.TrainingFreeDateId
                        });
                        if (freetimelist != null && freetimelist.Count() > 0)
                        {
                            var freetime = freetimelist.FirstOrDefault();
                            if (freetdate.ObjectId != null)
                            {
                                var car = TrainingCarBLL.Instance.GetEntity(freetdate.ObjectId);
                                if (car != null)
                                {
                                    if (car.SchoolId == queryParam["schoolid"].ToString() && car.TrainingType == int.Parse(queryParam["trainingtype"].ToString()))
                                    {
                                        ExtTrainingCarEntity carentity = new ExtTrainingCarEntity();
                                        carentity.Name = car.Name;
                                        carentity.CarNumber = car.CarNumber;
                                        carentity.TrainingType = car.TrainingType;
                                        carentity.TrainingTypeName = car.TrainingTypeName;
                                        carentity.Info = GetInfo(freetime.TrainingFreeTimeId);
                                        carentity.SortNum = car.SortNum;
                                        string faceimage = "";
                                        if (car.FaceImage != null)
                                        {
                                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                            carentity.FaceImage = imageHost + car.FaceImage;
                                        }
                                        carList.Add(carentity);
                                    }
                                }
                            }
                        }
                    }
                }

                result.ResultData["List"] = carList.OrderBy(o => o.SortNum);
                result.IsSuccess = true;
                result.Message = "成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>GetTrainingCar";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取预约信息
        /// </summary>
        /// <param name="freetimeid"></param>
        /// <returns></returns>
        public ExtTrainingEntity GetInfo(string freetimeid)
        {
            ExtTrainingEntity entity = new ExtTrainingEntity();
            try
            {

                var list = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingFreeTimeId = freetimeid });
                if (list != null && list.Count > 0)
                {
                    var detail = list.FirstOrDefault();
                    if (detail != null)
                    {
                        var order = TrainingOrderBLL.Instance.GetEntity(detail.TrainingOrderId);
                        if (order != null)
                        {
                            if (order.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                            {
                                var member = MemberBLL.Instance.GetEntity(order.MemberId);
                                entity.SubNoticeInfo = string.Format("{0},{1}", member.MemberName, member.Mobile);
                                entity.Mobile = member.Mobile;
                            }
                            else if (order.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                            {
                                var teacher = TeacherBLL.Instance.GetEntity(order.MemberId);
                                entity.SubNoticeInfo = string.Format("{0},{1},{2}", teacher.Name, teacher.Mobile, teacher.SchoolName);
                                entity.Mobile = teacher.Mobile;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>GetInfo";
                new ExceptionHelper().LogException(ex);
            }
            return entity;
        }

        /// <summary>
        /// 判断注册会员是否是教练 根据电话号码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckMemberIsTeacher()
        {
            var result = new ReturnMessage(false) { Message = "检查失败!" };
            try
            {
                if (LoginUser == null)
                {
                    result.IsSuccess = true;
                    result.Code = -1;
                    result.Message = "请先登陆";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var teacherlist = TeacherBLL.Instance.GetList(new TeacherEntity() { Mobile = LoginUser.Mobile });
                if (teacherlist.Count > 0)
                {
                    result.IsSuccess = true;
                    result.Code = 1;
                    result.Message = "会员是教练";
                }
                else {
                    result.IsSuccess = true;
                    result.Code = 0;
                    result.Message = "不是教练是会员";
                }
              
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>CheckMemberIsTeacher";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }

    public class ExtTrainingCarEntity : TrainingCarEntity
    {
        public ExtTrainingEntity Info { get; set; }

    }
    public class ExtTrainingEntity
    {
        public string SubNoticeInfo { get; set; }
        public string Mobile { get; set; }
    }
}
