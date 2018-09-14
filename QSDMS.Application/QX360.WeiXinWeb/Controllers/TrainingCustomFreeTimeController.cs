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
    public class TrainingCustomFreeTimeController : BaseController
    {
        //
        // GET: /TrainingCustomFreeTime/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取当前对象相关的日期列表
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCustomDateJson(string freedateid)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var list = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                {
                    TrainingFreeDateId = freedateid
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCustomFreeTimeController>>GetCustomDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
