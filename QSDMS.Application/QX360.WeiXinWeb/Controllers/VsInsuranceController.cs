using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSMS.API.BaiduMap;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class VsInsuranceController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /VsInsurance/
        [AuthorizeFilter]
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult VsAudit()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }

        /// <summary>
        /// 获取保险机构列表
        /// </summary>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetList(string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                int type = 1;
                InsuranceCommpayEntity para = new InsuranceCommpayEntity();
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = queryJson.ToJObject();
                    if (!queryParam["lat"].IsEmpty())
                    {
                        para.Lat = decimal.Parse(queryParam["lat"].ToString());
                    }
                    if (!queryParam["lng"].IsEmpty())
                    {
                        para.Lng = decimal.Parse(queryParam["lng"].ToString());
                    }
                    if (!queryParam["type"].IsEmpty())
                    {
                        type = int.Parse(queryParam["type"].ToString());
                    }
                }
                para.Status = (int)Model.Enums.UseStatus.启用;

                var pageList = InsuranceCommpayBLL.Instance.GetList(para).OrderBy(o => o.SortNum).ToList();
                if (pageList != null)
                {
                    pageList.ForEach((page) =>
                    {
                        if (page.FaceImage != null)
                        {
                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                            page.FaceImage = imageHost + page.FaceImage;
                        }
                        if (page.ProvinceId != null)
                        {
                            page.ProvinceName = AreaBLL.Instance.GetEntity(page.ProvinceId).AreaName;
                        }
                        if (page.CityId != null)
                        {
                            page.CityName = AreaBLL.Instance.GetEntity(page.CityId).AreaName;
                        }
                        if (page.CountyId != null)
                        {
                            page.CountyName = AreaBLL.Instance.GetEntity(page.CountyId).AreaName;
                        }
                        page.AddressInfo = page.ProvinceName + page.CityName + page.CountyName + page.AddressInfo;

                        if (page.Lat != null && page.Lng != null && para.Lat != null && para.Lng != null)
                        {
                            page.HowLong = HarvenSin.GetDistance(
                                new Point2D()
                                {
                                    Lng = (double)para.Lng,
                                    Lat = (double)para.Lat
                                },
                                new Point2D()
                                {
                                    Lng = (double)page.Lng,
                                    Lat = (double)page.Lat
                                }).ToString("f2");
                        }
                        else
                        {
                            page.HowLong = "未知";
                        }
                    });
                }
                switch (type)
                {
                    case 1://按距离
                        pageList = pageList.OrderBy(i => i.HowLong).ThenBy(i => i.HowLong).ToList();
                        break;
                    case 2://按默认排序
                        pageList = pageList.OrderBy(i => i.CreateTime).ThenBy(i => i.CreateTime).ToList();
                        break;
                }
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["List"] = pageList;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "VsInsuranceController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInsuranceModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var data = InsuranceCommpayBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (data.FaceImage != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.FaceImage = imageHost + data.FaceImage;
                    }
                    //pic
                    var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.InsuranceCommpayId });
                    if (imageList != null)
                    {
                        data.AttachmentPic = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                        data.AttachmentPic.ForEach((o) =>
                        {
                            if (o.PicName != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                o.PicName = imageHost + o.PicName;
                            }
                        });
                    }

                    //address
                    if (data.ProvinceId != null)
                    {
                        data.ProvinceName = AreaBLL.Instance.GetEntity(data.ProvinceId).AreaName;
                    }
                    if (data.CityId != null)
                    {
                        data.CityName = AreaBLL.Instance.GetEntity(data.CityId).AreaName;
                    }
                    if (data.CountyId != null)
                    {
                        data.CountyName = AreaBLL.Instance.GetEntity(data.CountyId).AreaName;
                    }
                    data.AddressInfo = data.ProvinceName + data.CityName + data.CountyName + data.AddressInfo;
                    result.IsSuccess = true;
                    result.Message = "获取成功!";
                    result.ResultData["Data"] = data;
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "VsInsuranceController>>GetInsuranceModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult CreateOrder(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {
                lock (objLock)
                {
                    var order = JsonConvert.DeserializeObject<InsuranceOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    order.InsuranceOrderId = Util.NewUpperGuid();
                    order.InsuranceOrderNo = InsuranceOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.SubscribeStatus.预约成功;
                    InsuranceOrderBLL.Instance.Add(order);
                    result.IsSuccess = true;
                    result.Message = "创建成功";

                    //写消息
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime);
                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.保险预约提示, QX360.Model.Enums.NoticeType.预约提醒, LoginUser.UserId, order.MemberName, servicetime, "预约保险", order.InsuranceOrderNo);

                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "VsInsuranceController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }


        /// <summary>
        /// 查询本人的保险预约订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMyInsurance(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                InsuranceOrderEntity para = new InsuranceOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }
                para.MemberId = LoginUser.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = InsuranceOrderBLL.Instance.GetList(para).OrderByDescending(p => p.CreateTime).ToList();
                list.Foreach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.SubscribeStatus)o.Status).ToString();
                    }
                    if (o.InsuranceId != null)
                    {
                        var insurance = InsuranceCommpayBLL.Instance.GetEntity(o.InsuranceId);
                        if (insurance != null)
                        {
                            if (insurance.ProvinceId != null)
                            {
                                insurance.ProvinceName = AreaBLL.Instance.GetEntity(insurance.ProvinceId).AreaName;
                            }
                            if (insurance.CityId != null)
                            {
                                insurance.CityName = AreaBLL.Instance.GetEntity(insurance.CityId).AreaName;
                            }
                            if (insurance.CountyId != null)
                            {
                                insurance.CountyName = AreaBLL.Instance.GetEntity(insurance.CountyId).AreaName;
                            }
                            insurance.AddressInfo = insurance.ProvinceName + insurance.CityName + insurance.CountyName + insurance.AddressInfo;
                            o.Insurance = insurance;
                        }
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TakeAuditController>>GetMyAudit";
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
                InsuranceOrderEntity entity = new InsuranceOrderEntity();
                entity.InsuranceOrderId = id;
                entity.Status = (int)QX360.Model.Enums.SubscribeStatus.已取消;
                InsuranceOrderBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "取消成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TakeAuditController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
