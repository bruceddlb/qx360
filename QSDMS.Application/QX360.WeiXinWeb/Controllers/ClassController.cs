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
    public class ClassController : BaseController
    {
        //
        // GET: /Class/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult List()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                ClassEntity para = new ClassEntity();
                var list = ClassBLL.Instance.GetList(para).OrderBy(o => o.SortNum);
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ClassController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
