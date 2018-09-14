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
    public class FreeTimeController : BaseController
    {
        //
        // GET: /FreeTime/

        public ActionResult Index()
        {
            return View();
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
                DateTime firsttime = QSDMS.Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = QSDMS.Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                List<FreeDateEntity> list = new List<FreeDateEntity>();
                while (true)
                {
                    if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                    {
                        list.Add(new FreeDateEntity() { FreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                    }
                    else
                    {
                        list.Add(new FreeDateEntity() { FreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
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
                ex.Data["Method"] = "FreeTimeController>>GetCurrentWeekList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                var list = FreeDateBLL.Instance.GetList(new FreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = QSDMS.Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                    EndTime = QSDMS.Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                }).OrderBy(p => p.FreeDate);
                if (list != null)
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
                            o.WeekName = QSDMS.Util.Time.GetChineseWeekDay(Converter.ParseDateTime(o.FreeDate));
                        }

                        var freetimelist = FreeTimeBLL.Instance.GetList(new FreeTimeEntity() { FreeDateId = o.FreeDateId });
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
                ex.Data["Method"] = "FreeTimeController>>GetCurrentWeekList";
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
                var list = FreeTimeBLL.Instance.GetList(new FreeTimeEntity() { FreeDateId = freedateid });
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
                ex.Data["Method"] = "FreeTimeController>>GetFreeTimeList";
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
                var freeTime = FreeTimeBLL.Instance.GetEntity(freetimeid);
                if (freeTime != null)
                {
                    freeTime.FreeTimeId = freetimeid;
                    freeTime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                    FreeTimeBLL.Instance.Update(freeTime);
                }
                result.IsSuccess = true;
                result.Message = "创建成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "FreeTimeController>>SetFreeTime";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
