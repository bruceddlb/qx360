using iFramework.Framework;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class MapPositionController : BaseController
    {
        //
        // GET: /MapPosition/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取当前坐标
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUserPoint()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var cookie = Request.Cookies.Get("MAPPOINT");
                if (cookie != null && cookie["lat"].ToString() != "")
                {
                    var data = new CurrentMapPoint()
                    {
                        Lat = decimal.Parse(cookie["lat"].ToString()),
                        Lng = decimal.Parse(cookie["lng"].ToString())
                    };
                    result.IsSuccess = true;
                    result.ResultData["Data"] = data;
                    result.Message = "获取成功";
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MapPositionController>>GetUserPoint";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置当前坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// 
        [HttpPost]
        public JsonResult SetUserPoint(decimal lat, decimal lng)
        {
            var result = new ReturnMessage(false) { Message = "设置成功!" };
            try
            {
                var userCookie = new HttpCookie("MAPPOINT");
                userCookie.Expires = DateTime.Now.AddMinutes(10);
                userCookie.Values["lat"] = lat.ToString();
                userCookie.Values["lng"] = lng.ToString();
                userCookie.Path = "/";
                Response.Cookies.Add(userCookie);
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MapPositionController>>SetUserPoint";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
    /// <summary>
    /// 当前坐标位置
    /// </summary>
    public class CurrentMapPoint
    {
        public decimal? Lat { get; set; }

        public decimal? Lng { get; set; }
    }
}
