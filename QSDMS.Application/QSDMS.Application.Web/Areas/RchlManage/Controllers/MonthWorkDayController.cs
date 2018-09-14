using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QSDMS.Util;
using QSDMS.Application.Web.Controllers;
using QX360.Business;
using iFramework.Framework;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class MonthWorkDayController : BaseController
    {
        //
        // GET: /QX360Manage/MonthWorkDay/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取一个月的日期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMonthDayWeekList(string type)
        {
            //当前月
            DateTime firsttime = Util.Time.GetCurrentMonthFirstDay(DateTime.Now);
            DateTime endTime = Util.Time.GetCurrentMonthLastDay(DateTime.Now);

            if (type == "2")//下月
            {
                firsttime = Util.Time.GetCurrentMonthFirstDay(DateTime.Now.AddMonths(1));
                endTime = Util.Time.GetCurrentMonthLastDay(DateTime.Now.AddMonths(1));
            }
            string title = Convert.ToDateTime(firsttime.ToString()).ToString("yyyy-MM");
            List<MonthWorkDayEntity> list = new List<MonthWorkDayEntity>();
            while (true)
            {
                var dateid = Util.Util.NewUpperGuid();
                list.Add(new MonthWorkDayEntity()
                {
                    MonthWorkDayId = dateid,
                    WorkDay = firsttime,
                    Week = Convert.ToInt32(firsttime.DayOfWeek),
                    WeekName = Util.Time.GetChineseWeekDay(firsttime)

                });
                firsttime = firsttime.AddDays(1);
                if (firsttime > endTime)
                {
                    break;
                }
            }
            var JsonData = new
           {
               title = title,
               data = list

           };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 获取当前月设置
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMonthDateJson(string objectid, string type)
        {
            //当前月
            DateTime firsttime = Util.Time.GetCurrentMonthFirstDay(DateTime.Now);
            DateTime endTime = Util.Time.GetCurrentMonthLastDay(DateTime.Now);
            if (type == "2")//下月
            {
                firsttime = Util.Time.GetCurrentMonthFirstDay(DateTime.Now.AddMonths(1));
                endTime = Util.Time.GetCurrentMonthLastDay(DateTime.Now.AddMonths(1));
            }
            //当前年月
            string yearmonth = Convert.ToDateTime(firsttime.ToString()).ToString("yyyy-MM");
            var list = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                       {
                           ObjectId = objectid,
                           YearMonth = yearmonth,

                       }).OrderBy(p => p.WorkDay).ToList();
            if (list.Count() == 0)
            {
                list = new List<MonthWorkDayEntity>();
                while (true)
                {
                    var dateid = Util.Util.NewUpperGuid();
                    list.Add(new MonthWorkDayEntity()
                    {
                        MonthWorkDayId = dateid,
                        WorkDay = firsttime,
                        Week = Convert.ToInt32(firsttime.DayOfWeek),
                        WeekName = Util.Time.GetChineseWeekDay(firsttime),
                        DateType = 0

                    });
                    firsttime = firsttime.AddDays(1);
                    if (firsttime > endTime)
                    {
                        break;
                    }
                }
            }
            var JsonData = new
            {
                title = yearmonth,
                data = list

            };
            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 保存月工作时间
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string yearMonth, string json)
        {
            try
            {
                //反序列化
                var list = Serializer.DeserializeJson<List<MonthWorkDayEntity>>(json, true);
                if (list != null)
                {
                    int count = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                    {
                        ObjectId = keyValue,
                        YearMonth = yearMonth,

                    }).Count();
                    if (count > 0)//update
                    {
                        //修改
                        foreach (MonthWorkDayEntity workdayItem in list)
                        {
                            workdayItem.WeekName = Util.Time.GetChineseWeekDay(DateTime.Parse(workdayItem.WorkDay.ToString()));
                            MonthWorkDayBLL.Instance.Update(workdayItem);
                            MonthWorkDayBLL.Instance.SynBusData(workdayItem.ObjectType, workdayItem.DateType, workdayItem.WorkDay, workdayItem.MonthWorkDayId);
                        }
                    }
                    else//add
                    {
                        foreach (MonthWorkDayEntity workdayItem in list)
                        {
                            workdayItem.WeekName = Util.Time.GetChineseWeekDay(DateTime.Parse(workdayItem.WorkDay.ToString()));
                            MonthWorkDayBLL.Instance.Add(workdayItem);
                        }

                    }
                }



                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MonthWorkDayController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }
    }
}
