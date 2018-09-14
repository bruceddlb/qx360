using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class TrainingFreeTimeController : BaseController
    {
        //
        // GET: /QX360Manage/StudyFreeTime/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 当前对象的工作时间
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTraingCarWeekDateJson(string objectid)
        {
            var result = new ReturnMessage(false) { Message = "获取设置失败!" };
            try
            {
                var list = GetCurrentWeekTrainingFreeDateList(objectid);
                if (list != null && list.Count() > 0)
                {
                    result.IsSuccess = true;
                    result.Code = 100;
                    result.Message = "获取设置成功";
                    result.ResultData["List"] = list;
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>GetWeekDateJson";
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
        public JsonResult GetWeekDateJson(string objectid, string schoolid)
        {
            var result = new ReturnMessage(false) { Message = "获取设置失败!" };
            try
            {
                //var list = GetCurrentWeekTrainingFreeDateList(objectid);
                //if (list != null && list.Count() > 0)
                //{
                //    result.IsSuccess = true;
                //    result.Code = 100;
                //    result.Message = "获取设置成功";
                //    result.ResultData["List"] = list;

                //}
                //else
                //{
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = schoolid,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    result.Code = -100;
                    result.Message = "未设置当月工作日时间";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //这里初始化应该是服务自动执行，这里方便测试用 手动模式添加数据
                var worktimelist = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
                {
                    SchoolId = schoolid

                }).OrderBy(p => p.SortNum).ToList();
                if (worktimelist.Count() == 0)
                {
                    result.Code = -200;
                    result.Message = "未设置该驾校实训时段";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                TrainingFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, objectid);
                //查询数据
                var list = GetCurrentWeekTrainingFreeDateList(objectid);
                if (list != null && list.Count() > 0)
                {
                    result.IsSuccess = true;
                    result.Code = 100;
                    result.Message = "获取设置成功";
                    result.ResultData["List"] = list;

                }
                //}
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>GetWeekDateJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Lock(string keyValue)
        {
            try
            {
                var entity = TrainingFreeTimeBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.锁定;
                    TrainingFreeTimeBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>Lock";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UnLock(string keyValue)
        {
            try
            {
                var entity = TrainingFreeTimeBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                    TrainingFreeTimeBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>Lock";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 查询当前所在周对象设置的时间
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public List<TrainingFreeDateEntity> GetCurrentWeekTrainingFreeDateList(string objectid)
        {
            var list = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
            {
                ObjectId = objectid,
                StartTime = DateTime.Now.ToString(),// Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                EndTime = DateTime.Now.AddDays(6).ToString() // Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
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
                        o.WeekName = Util.Time.GetChineseWeekDay(Converter.ParseDateTime(o.FreeDate));
                    }

                    var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity() { TrainingFreeDateId = o.TrainingFreeDateId });
                    if (freetimelist != null)
                    {
                        freetimelist = freetimelist.OrderBy(p => p.SortNum ?? 0).ToList();
                        o.FreeTimeList = freetimelist;
                    }
                });
                return list.ToList();
            }
            return null;
        }


        /// <summary>
        /// 获取已预约的自定义时间段
        /// </summary>
        /// <param name="fredateid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFreeTimeCustomerJson(string fredateid)
        {
            var result = new ReturnMessage(false) { Message = "获取设置失败!" };
            try
            {
                var list = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                {
                    TrainingFreeDateId = fredateid
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingFreeTimeController>>GetFreeTimeCustomerJson";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
