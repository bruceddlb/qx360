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
    public class StudyFreeTimeController : BaseController
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
                var list = StudyFreeDateBLL.Instance.GetList(new StudyFreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = DateTime.Now.ToString(), //Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                    EndTime = DateTime.Now.AddDays(6).ToString()// Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
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

                        var freetimelist = StudyFreeTimeBLL.Instance.GetList(new StudyFreeTimeEntity() { StudyFreeDateId = o.StudyFreeDateId });
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
                ex.Data["Method"] = "StudyFreeTimeController>>GetWeekDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="freedateid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFreeTimeList(string freedateid)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var list = StudyFreeTimeBLL.Instance.GetList(new StudyFreeTimeEntity() { StudyFreeDateId = freedateid });
                if (list != null)
                {
                    list = list.OrderBy(p => p.SortNum ?? 0).ToList();
                }
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyFreeTimeController>>GetFreeTimeList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="freetimeid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetFreeTime(string freetimeid)
        {
            var result = new ReturnMessage(false) { Message = "设置失败!" };
            try
            {
                var freeTime = StudyFreeTimeBLL.Instance.GetEntity(freetimeid);
                if (freeTime != null)
                {
                    freeTime.StudyFreeTimeId = freetimeid;
                    freeTime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                    StudyFreeTimeBLL.Instance.Update(freeTime);
                }
                result.IsSuccess = true;
                result.Message = "创建成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyFreeTimeController>>SetFreeTime";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
