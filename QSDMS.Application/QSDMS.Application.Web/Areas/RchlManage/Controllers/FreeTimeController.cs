using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util.Extension;
using QSDMS.Util;
using QX360.Business;
using QSDMS.Application.Web.Controllers;
using iFramework.Framework;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class FreeTimeController : BaseController
    {
        //
        // GET: /QX360Manage/FreeTime/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddTime()
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

            var list = FreeDateBLL.Instance.GetList(new FreeDateEntity()
            {
                ObjectId = objectid,
                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
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
                        o.WeekName = Util.Time.GetChineseWeekDay(Converter.ParseDateTime(o.FreeDate));
                    }

                    var freetimelist = FreeTimeBLL.Instance.GetList(new FreeTimeEntity() { FreeDateId = o.FreeDateId });
                    if (freetimelist != null)
                    {
                        freetimelist = freetimelist.OrderBy(p => p.SortNum ?? 0).ToList();
                        o.FreeTimeList = freetimelist;
                    }
                });

            }
            return Content(list.ToJson());
        }
        /// <summary>
        /// 获取当前日期的一周时间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCurrentWeekList()
        {
            DateTime firsttime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
            DateTime endTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now);
            List<FreeDateEntity> list = new List<FreeDateEntity>();
            while (true)
            {
                var dateid = Util.Util.NewUpperGuid();
                if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                {
                    list.Add(new FreeDateEntity() { FreeTimeList = GetNormalFreeTimeList(dateid), FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = Util.Time.GetChineseWeekDay(firsttime) });
                }
                else
                {
                    list.Add(new FreeDateEntity() { FreeTimeList = GetNormalFreeTimeList(dateid), FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = Util.Time.GetChineseWeekDay(firsttime) });
                }
                firsttime = firsttime.AddDays(1);
                if (firsttime > endTime)
                {
                    break;
                }
            }
            return Content(list.ToJson());
        }

        public List<FreeTimeEntity> GetNormalFreeTimeList(string freedateId)
        {
            List<FreeTimeEntity> list = new List<FreeTimeEntity>();
            //早上8点-晚上6点 每间隔1小时
            int star = 8;
            int end = 18;
            for (; star < end; )
            {
                FreeTimeEntity time = new FreeTimeEntity();
                time.FreeTimeId = QSDMS.Util.Util.NewUpperGuid();
                time.FreeDateId = freedateId;
                time.StartTime = star.ToString().PadLeft(2, '0');
                time.EndTime = (star + 1).ToString().PadLeft(2, '0');
                time.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                list.Add(time);
                star = star + 1;
            }
            return list;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                //反序列化
                var list = Serializer.DeserializeJson<List<FreeDateEntity>>(json, true);
                if (list != null)
                {
                    int count = FreeDateBLL.Instance.GetList(new FreeDateEntity()
                    {
                        ObjectId = keyValue,
                        StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                        EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                    }).Count();
                    if (count > 0)//update
                    {
                        //修改
                        foreach (FreeDateEntity freedateItem in list)
                        {
                            //删除没有被预约的时间段
                            var freetimeList = FreeTimeBLL.Instance.GetList(new FreeTimeEntity() { FreeDateId = freedateItem.FreeDateId });
                            if (freetimeList != null)
                            {
                                foreach (var freetimeItem in freetimeList)
                                {
                                    if (freetimeItem.FreeStatus == (int)QX360.Model.Enums.FreeTimeStatus.空闲)
                                    {
                                        FreeTimeBLL.Instance.Delete(freetimeItem.FreeTimeId);
                                    }
                                }
                            }
                            //增加空闲时间
                            if (freedateItem.FreeTimeList != null)
                            {
                                foreach (var freetimeItem in freedateItem.FreeTimeList)
                                {
                                    if (freetimeItem.FreeStatus != (int)QX360.Model.Enums.FreeTimeStatus.已预约)
                                    {
                                        freetimeItem.FreeTimeId = Util.Util.NewUpperGuid();
                                        FreeTimeBLL.Instance.Add(freetimeItem);
                                    }
                                }
                            }
                        }
                    }
                    else//add
                    {
                        foreach (FreeDateEntity freedateItem in list)
                        {
                            freedateItem.CreateId = LoginUser.UserId;
                            freedateItem.CreateTime = DateTime.Now;
                            FreeDateBLL.Instance.Add(freedateItem);
                            if (freedateItem.FreeTimeList != null)
                            {
                                foreach (var freetimeItem in freedateItem.FreeTimeList)
                                {
                                    freetimeItem.FreeTimeId = Util.Util.NewUpperGuid();
                                    FreeTimeBLL.Instance.Add(freetimeItem);
                                }
                            }
                        }

                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "FreeTimeController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }
    }
}
