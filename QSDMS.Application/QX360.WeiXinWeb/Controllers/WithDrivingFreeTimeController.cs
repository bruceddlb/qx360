using iFramework.Framework;
using QSDMS.Business.Cache;
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
    public class WithDrivingFreeTimeController : BaseController
    {
        //
        // GET: /WithDrivingFreeTime/

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
                var list = WithDrivingFreeDateBLL.Instance.GetList(new WithDrivingFreeDateEntity()
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

                        var freetimelist = WithDrivingFreeTimeBLL.Instance.GetList(new WithDrivingFreeTimeEntity() { WithDrivingFreeDateId = o.WithDrivingFreeDateId });
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
                ex.Data["Method"] = "WithDrivingFreeTimeController>>GetWeekDateJson";
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
                List<WithDrivingFreeDateEntity> list = new List<WithDrivingFreeDateEntity>();
                while (true)
                {
                    if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                    {
                        list.Add(new WithDrivingFreeDateEntity() { WithDrivingFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                    }
                    else
                    {
                        list.Add(new WithDrivingFreeDateEntity() { WithDrivingFreeDateId = Util.NewUpperGuid(), FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
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
                ex.Data["Method"] = "WithDrivingFreeTimeController>>GetCurrentWeekList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取工作时间段
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCurrentWorkTimeList()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.WithDrivingTimeSpaceType));
                //int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.WithDrivingTimeSpaceType));
                //List<WithDrivingFreeTimeEntity> list = new List<WithDrivingFreeTimeEntity>();
                //for (int i = 0; i < names.Length; i++)
                //{
                //    list.Add(new WithDrivingFreeTimeEntity() { WorkTimeTableId = values[i].ToString(), TimeSection = names[i] });
                //}
                List<WithDrivingFreeTimeEntity> list = new List<WithDrivingFreeTimeEntity>();
                DataItemCache dataItemCache = new DataItemCache();
                var dataItemList = dataItemCache.GetDataItemList("pjsd");
                foreach (var dataitem in dataItemList)
                {
                    list.Add(new WithDrivingFreeTimeEntity() { WorkTimeTableId = dataitem.ItemDetailId.ToString(), TimeSection = dataitem.ItemName, Remark = dataitem.Description, SortNum = dataitem.SortCode });
                }
                list = list.OrderBy(p => p.SortNum).ToList();
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingFreeTimeController>>GetCurrentWorkTimeList";
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
                var list = WithDrivingFreeTimeBLL.Instance.GetList(new WithDrivingFreeTimeEntity() { WithDrivingFreeDateId = freedateid });
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
                ex.Data["Method"] = "WithDrivingFreeTimeController>>GetFreeTimeList";
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
                var freeTime = WithDrivingFreeTimeBLL.Instance.GetEntity(freetimeid);
                if (freeTime != null)
                {
                    freeTime.WithDrivingFreeTimeId = freetimeid;
                    freeTime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                    WithDrivingFreeTimeBLL.Instance.Update(freeTime);
                }
                result.IsSuccess = true;
                result.Message = "创建成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingFreeTimeController>>SetFreeTime";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
