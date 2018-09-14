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
    public class MonthWorkDayController : BaseController
    {
        //
        // GET: /MonthWorkDay/

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
        public ActionResult GetWeekDateJson(string objectid)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {             
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = objectid,
                    StartTime = DateTime.Now.ToString(), //Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                    EndTime = DateTime.Now.AddDays(6).ToString()// Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    result.Code = -100;
                    result.Message = "未设置当月工作日时间";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = monthworklist;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MonthWorkDayController>>GetWeekDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
