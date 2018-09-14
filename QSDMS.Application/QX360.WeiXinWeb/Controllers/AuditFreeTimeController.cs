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
    public class AuditFreeTimeController : Controller
    {
        //
        // GET: /AuditFreeTime/

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
                var list = AuditFreeDateBLL.Instance.GetList(new AuditFreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = DateTime.Now.ToString(), //Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                    EndTime = DateTime.Now.AddDays(6).ToString() // Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                }).OrderBy(p => p.FreeDate);
                if (list != null && list.Count() > 0)
                {
                    list.Foreach((o) =>
                    {
                        if (o.FreeDate != null)
                        {
                            if (Converter.ParseDateTime(o.FreeDate).DayOfWeek == DateTime.Now.DayOfWeek)
                            {
                                o.IsCurrentDay = true;
                            }
                            o.Week = Convert.ToInt32(Converter.ParseDateTime(o.FreeDate).DayOfWeek);
                            o.WeekName = Time.GetChineseWeekDay(Converter.ParseDateTime(o.FreeDate));
                        }

                        var freetimelist = AuditFreeTimeBLL.Instance.GetList(new AuditFreeTimeEntity() { AuditFreeDateId = o.AuditFreeDateId });
                        if (freetimelist != null)
                        {
                            freetimelist = freetimelist.OrderBy(p => p.SortNum ?? 0).ToList();
                            o.FreeTimeList = freetimelist;
                        }
                    });

                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditFreeTimeController>>GetWeekDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                AuditFreeTimeEntity data = new AuditFreeTimeEntity();
                var list = AuditFreeTimeBLL.Instance.GetList(new AuditFreeTimeEntity() { AuditFreeDateId = auditId, TimeSection = timeSection });
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
        /// <summary>
        /// 获取当前日期的一周时间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCurrentWeekList()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                DateTime firsttime = DateTime.Now;
                DateTime endTime = DateTime.Now.AddDays(6);
                List<AuditFreeDateEntity> list = new List<AuditFreeDateEntity>();
                while (true)
                {
                    if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                    {
                        list.Add(new AuditFreeDateEntity() { AuditFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                    }
                    else
                    {
                        list.Add(new AuditFreeDateEntity() { AuditFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                    }
                    firsttime = firsttime.AddDays(1);
                    if (firsttime > endTime)
                    {
                        break;
                    }
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditFreeTimeController>>GetCurrentWeekList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
      
    }
}
