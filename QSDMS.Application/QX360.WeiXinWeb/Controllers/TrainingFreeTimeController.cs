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
    public class TrainingFreeTimeController : BaseController
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
                var list = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
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

                        var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity() { TrainingFreeDateId = o.TrainingFreeDateId });
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
                ex.Data["Method"] = "TrainingFreeTimeController>>GetWeekDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                List<TrainingFreeDateEntity> list = new List<TrainingFreeDateEntity>();
                while (true)
                {
                    if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                    {
                        list.Add(new TrainingFreeDateEntity() { TrainingFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                    }
                    else
                    {
                        list.Add(new TrainingFreeDateEntity() { TrainingFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
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
                ex.Data["Method"] = "TrainingFreeTimeController>>GetCurrentWeekList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取工作时间段
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentWorkTimeList(string schoolid)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                List<TrainingTimeTableEntity> list = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity() { SchoolId = schoolid });                
                list = list.OrderBy(p => p.SortNum).ToList();
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>GetCurrentWorkTimeList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
