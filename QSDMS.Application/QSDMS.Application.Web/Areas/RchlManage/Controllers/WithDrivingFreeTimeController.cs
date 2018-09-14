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
    public class WithDrivingFreeTimeController : BaseController
    {
        //
        // GET: /QX360Manage/StudyFreeTime/

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
        public JsonResult GetWeekDateJson(string objectid)
        {
            var result = new ReturnMessage(false) { Message = "获取设置失败!" };
            try
            {
                //var list = GetCurrentWeekWithDrivingFreeDateList(objectid);
                //if (list != null && list.Count() > 0)
                //{
                //    result.IsSuccess = true;
                //    result.Code = 100;
                //    result.Message = "获取设置成功";
                //    result.ResultData["List"] = list;

                //}
                //else
                //{
                //这里初始化应该是服务自动执行，这里方便测试用 手动模式添加数据                   
                WithDrivingFreeTimeBLL.Instance.AddInitFreeTime(objectid);
                //查询数据
                var list = GetCurrentWeekWithDrivingFreeDateList(objectid);
                if (list != null && list.Count() > 0)
                {
                    result.IsSuccess = true;
                    result.Code = 100;
                    result.Message = "获取设置成功";
                    result.ResultData["List"] = list;

                }
            }
            //}
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingFreeTimeController>>GetWeekDateJson";
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
                var entity = WithDrivingFreeTimeBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.锁定;
                    WithDrivingFreeTimeBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingFreeTimeController>>Lock";
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
                var entity = WithDrivingFreeTimeBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                    WithDrivingFreeTimeBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "WithDrivingFreeTimeController>>Lock";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 查询当前所在周对象设置的时间
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public List<WithDrivingFreeDateEntity> GetCurrentWeekWithDrivingFreeDateList(string objectid)
        {
            var list = WithDrivingFreeDateBLL.Instance.GetList(new WithDrivingFreeDateEntity()
            {
                ObjectId = objectid,
                StartTime = DateTime.Now.ToString(), //Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                EndTime = DateTime.Now.AddDays(6).ToString() //Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
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

                    var freetimelist = WithDrivingFreeTimeBLL.Instance.GetList(new WithDrivingFreeTimeEntity() { WithDrivingFreeDateId = o.WithDrivingFreeDateId });
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
    }
}
