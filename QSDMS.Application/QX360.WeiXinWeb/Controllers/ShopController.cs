using iFramework.Framework;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QSMS.API.BaiduMap;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Business;
using Newtonsoft.Json;

namespace QX360.WeiXinWeb.Controllers
{
    public class ShopController : BaseController
    {
        private static object objLock = new object();
        //
        // GET: /Shop/

        public ActionResult Car()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }

        /// <summary>
        /// 获取驾校列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetList(int? pageIndex, string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                ShopEntity para = new ShopEntity();
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = queryJson.ToJObject();
                    if (!queryParam["keyword"].IsEmpty())
                    {
                        para.Name = queryParam["keyword"].ToString();
                    }
                    if (!queryParam["provinceid"].IsEmpty())
                    {
                        para.ProvinceId = queryParam["provinceid"].ToString();
                    }
                    if (!queryParam["cityid"].IsEmpty())
                    {
                        para.CityId = queryParam["cityid"].ToString();
                    }
                    if (!queryParam["countyid"].IsEmpty())
                    {
                        para.CountyId = queryParam["countyid"].ToString();
                    }
                    if (!queryParam["priceid"].IsEmpty())
                    {
                        para.PriceRange = queryParam["priceid"].ToString();
                    }
                    if (!queryParam["brandid"].IsEmpty())
                    {
                        para.BrandRange = queryParam["brandid"].ToString();
                    }
                    if (!queryParam["distanceid"].IsEmpty())
                    {
                        para.DistanceRange = (QX360.Model.Enums.DistanceRange)int.Parse(queryParam["distanceid"].ToString());
                    }
                    if (!queryParam["lat"].IsEmpty())
                    {
                        para.Lat = decimal.Parse(queryParam["lat"].ToString());
                    }
                    if (!queryParam["lng"].IsEmpty())
                    {
                        para.Lng = decimal.Parse(queryParam["lng"].ToString());
                    }

                }
                para.Status = (int)Model.Enums.UseStatus.启用;
                Pagination pagination = new Pagination();
                pagination.page = pageIndex ?? 1;
                pagination.rows = 10;
                var pageList = ShopBLL.Instance.GetPageList(para, ref pagination).OrderBy(o => o.SortNum).ToList();
                if (pageList != null)
                {
                    pageList.ForEach((page) =>
                    {
                        if (page.ShopId != null)
                        {

                            page.ShopCarList = ShopCarBLL.Instance.GetList(new ShopCarEntity() { ShopId = page.ShopId, Status = (int)Model.Enums.UseStatus.启用 }).OrderBy(o => o.SortNum).ToList();
                            if (page.ShopCarList != null)
                            {
                                if (para.PriceRange != null)
                                {
                                    switch ((QX360.Model.Enums.CarPriceRange)int.Parse(para.PriceRange))
                                    {
                                        case Enums.CarPriceRange.十万以内:
                                            page.ShopCarList = page.ShopCarList.FindAll(o => o.MaxPrice <= 10).ToList();

                                            break;
                                        case Enums.CarPriceRange.十万到二十万:
                                            page.ShopCarList = page.ShopCarList.Where((o) => { return o.LimitPrice >= 10 && o.MaxPrice <= 20; }).ToList();

                                            break;
                                        case Enums.CarPriceRange.二五到三十万:
                                            page.ShopCarList = page.ShopCarList.Where(o => o.LimitPrice >= 20 && o.MaxPrice <= 30).ToList();

                                            break;
                                        case Enums.CarPriceRange.三十到五十:
                                            page.ShopCarList = page.ShopCarList.Where(o => o.LimitPrice >= 30 && o.MaxPrice <= 50).ToList();

                                            break;
                                        case Enums.CarPriceRange.五十万以上:
                                            page.ShopCarList = page.ShopCarList.Where(o => o.LimitPrice >= 50).ToList();

                                            break;

                                    }
                                }
                                if (para.BrandRange != null)
                                {
                                    page.ShopCarList = page.ShopCarList.Where(o => o.BrandId == para.BrandRange).ToList();
                                }
                                if (para.Name != null)
                                {
                                    page.ShopCarList = page.ShopCarList.Where((o) => { return o.Name.IndexOf(para.Name) > -1; }).ToList();
                                }

                                page.ShopCarList.Foreach((o) =>
                                {
                                    if (o.FaceImage != null)
                                    {
                                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                        o.FaceImage = imageHost + o.FaceImage;
                                    }
                                });
                            }
                        }
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
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["IsEndPage"] = (pagination.total == (pageIndex ?? 1));
                result.ResultData["List"] = pageList;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取汽车对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCharModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var data = ShopCarBLL.Instance.GetEntity(id);
                if (data != null)
                {

                    //pic
                    var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.ShopCarId });
                    if (imageList != null)
                    {
                        data.AttachmentPicList = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                        data.AttachmentPicList.ForEach((o) =>
                        {
                            if (o.PicName != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                o.PicName = imageHost + o.PicName;
                            }
                        });
                    }
                    data.Shop = ShopBLL.Instance.GetEntity(data.ShopId);
                    if (data.Shop != null)
                    {
                        if (data.Shop.ProvinceId != null)
                        {
                            data.Shop.ProvinceName = AreaBLL.Instance.GetEntity(data.Shop.ProvinceId).AreaName;
                        }
                        if (data.Shop.CityId != null)
                        {
                            data.Shop.CityName = AreaBLL.Instance.GetEntity(data.Shop.CityId).AreaName;
                        }
                        if (data.Shop.CountyId != null)
                        {
                            data.Shop.CountyName = AreaBLL.Instance.GetEntity(data.Shop.CountyId).AreaName;
                        }
                        data.Shop.AddressInfo = data.Shop.ProvinceName + data.Shop.CityName + data.Shop.CountyName + data.Shop.AddressInfo;
                    }
                }

                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>GetSchoolModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询本人的看车订单
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetMySeeCar(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                SeeCarOrderEntity para = new SeeCarOrderEntity();
                switch (status)
                {
                    case (int)QX360.Model.Enums.SubscribeStatus.预约成功:
                        para.Status = (int)QX360.Model.Enums.SubscribeStatus.预约成功;
                        break;
                    case (int)QX360.Model.Enums.SubscribeStatus.已取消:
                        para.Status = (int)QX360.Model.Enums.SubscribeStatus.已取消;
                        break;
                }
                para.MemberId = LoginUser.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = SeeCarOrderBLL.Instance.GetList(para);
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>GetMySeeCar";
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
                SeeCarOrderEntity entity = new SeeCarOrderEntity();
                entity.SeeCarOrderId = id;
                entity.Status = (int)QX360.Model.Enums.SubscribeStatus.已取消;
                SeeCarOrderBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "取消成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>Cancel";
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
                    var order = JsonConvert.DeserializeObject<SeeCarOrderEntity>(data);
                    if (order == null)
                    {
                        return Json(result);
                    }
                    order.SeeCarOrderId = Util.NewUpperGuid();
                    order.SeeCarOrderNo = SeeCarOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)Model.Enums.SubscribeStatus.预约成功;
                    SeeCarOrderBLL.Instance.Add(order);

                    result.IsSuccess = true;
                    result.Message = "创建成功";

                    //写消息
                    string content = string.Format("车辆型号{0}", order.ShopCarName);
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), order.ServiceTime);
                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.看车预约提示, QX360.Model.Enums.NoticeType.预约提醒, LoginUser.UserId, order.MemberName, servicetime, content, order.SeeCarOrderNo);

                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
