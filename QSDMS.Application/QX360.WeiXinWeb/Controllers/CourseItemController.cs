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
    public class CourseItemController : BaseController
    {
        //
        // GET: /CourseItem/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult List(string courseid)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                CourseItemEntity para = new CourseItemEntity();
                para.CourseId = courseid;
                var list = CourseItemBLL.Instance.GetList(para).OrderBy(o => o.SortNum);
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "CourseItemController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
