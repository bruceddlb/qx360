using iFramework.Framework;
using QSDMS.Business;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class AreaController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(Duration = 7200)]
        public JsonResult GetAreaList(int? layer)
        {
            var result = new ReturnMessage(false) { Message = "获取区域列表失败!" };
            try
            {
                var data = AreaBLL.Instance.GetList().Where((o) => { return o.Layer == layer; }).OrderBy(i => i.SortCode).ToList();
                result.IsSuccess = true;
                result.Message = "获取区域列表成功!";
                result.ResultData["List"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "EngineeringController>>GetAreaList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
