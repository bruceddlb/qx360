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
    public class AuditTimeController : Controller
    {
        //
        // GET: /AuditTime/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取机构的工作时间
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="timeSection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAuditTimeTable(string auditId, string timeSection)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                AuditTimeTableEntity data = new AuditTimeTableEntity();
                var list = AuditTimeTableBLL.Instance.GetList(new AuditTimeTableEntity() { AuditId = auditId, TimeSection = timeSection });
                if (list != null && list.Count > 0)
                {
                    data = list.FirstOrDefault();
                }
                result.IsSuccess = true;
                result.ResultData["Data"] = data;
                result.Message = "获取成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditTimeController>>GetAuditTimeTable";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
