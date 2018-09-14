using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
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
    public class FinaceController : BaseController
    {
        //
        // GET: /QX360Manage/Finace/

        public ActionResult List()
        {
            return View();
        }
        public ActionResult DayReport()
        {
            return View();
        }
        public ActionResult MonthReport()
        {
            return View();
        }
        public ActionResult WeekReport()
        {
            return View();
        }
        public ActionResult Detail()
        {
            return View();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            FinaceEntity para = new FinaceEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //操作时间
                if (!queryParam["StartTime"].IsEmpty() && !queryParam["EndTime"].IsEmpty())
                {
                    DateTime startTime = queryParam["StartTime"].ToDate();
                    DateTime endTime = queryParam["EndTime"].ToDate();
                    para.StartTime = startTime.ToString();
                    para.EndTime = endTime.ToString();
                }
                if (!queryParam["SourceType"].IsEmpty())
                {
                    para.SourceType = int.Parse(queryParam["SourceType"].ToString());
                }
            }

            var pageList = FinaceBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.SourceType != null)
                    {
                        o.SourceTypeName = ((QX360.Model.Enums.FinaceSourceType)o.SourceType).ToString();
                    }
                    if (o.PayType != null)
                    {
                        o.PayTypeName = ((QX360.Model.Enums.PayType)o.PayType).ToString();
                    }
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                    }
                });
            }
            var JsonData = new
            {
                rows = pageList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 日报表
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDayListJson(string day)
        {
            var watch = CommonHelper.TimerStart();
            FinaceEntity para = new FinaceEntity();

            DateTime startTime = day.ToDate();
            DateTime endTime = day.ToDate().AddDays(0);
            para.StartTime = startTime.ToString();
            para.EndTime = endTime.ToString();
            var list = FinaceBLL.Instance.GetList(para);
            decimal totalInCosMoney = 0;
            decimal totalOutCosMoney = 0;
            int inCount = 0;
            int outCount = 0;
            list.ForEach((o) =>
            {
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.增加)
                {
                    totalInCosMoney += o.CosMoney ?? 0;
                    inCount++;
                }
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.减少)
                {
                    totalOutCosMoney += o.CosMoney ?? 0;
                    outCount++;
                }
            });
            var JsonData = new
            {
                time = day.ToDate().ToString("yyyy-MM-dd"),
                count = list.Count(),
                inCount = inCount,
                outCount = outCount,
                totalInCosMoney = totalInCosMoney,
                totalOutCosMoney = totalOutCosMoney,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 周报表
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWeekListJson(string day)
        {
            var watch = CommonHelper.TimerStart();
            FinaceEntity para = new FinaceEntity();

            DateTime startTime = day.ToDate().AddDays(-7);
            DateTime endTime = day.ToDate().AddDays(0);
            para.StartTime = startTime.ToString();
            para.EndTime = endTime.ToString();
            var list = FinaceBLL.Instance.GetList(para);
            decimal totalInCosMoney = 0;
            decimal totalOutCosMoney = 0;
            int inCount = 0;
            int outCount = 0;
            list.ForEach((o) =>
            {
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.增加)
                {
                    totalInCosMoney += o.CosMoney ?? 0;
                    inCount++;
                }
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.减少)
                {
                    totalOutCosMoney += o.CosMoney ?? 0;
                    outCount++;
                }
            });
            var JsonData = new
            {
                time = day.ToDate().ToString("yyyy-MM-dd"),
                count = list.Count(),
                inCount = inCount,
                outCount = outCount,
                totalInCosMoney = totalInCosMoney,
                totalOutCosMoney = totalOutCosMoney,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 月报表
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMonthListJson(string day)
        {
            var watch = CommonHelper.TimerStart();
            FinaceEntity para = new FinaceEntity();

            DateTime startTime = day.ToDate();
            DateTime endTime = day.ToDate().AddMonths(1).AddDays(-1);//本月最后一天
            para.StartTime = startTime.ToString();
            para.EndTime = endTime.ToString();
            var list = FinaceBLL.Instance.GetList(para);
            decimal totalInCosMoney = 0;
            decimal totalOutCosMoney = 0;
            int inCount = 0;
            int outCount = 0;
            list.ForEach((o) =>
            {
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.增加)
                {
                    totalInCosMoney += o.CosMoney ?? 0;
                    inCount++;
                }
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.减少)
                {
                    totalOutCosMoney += o.CosMoney ?? 0;
                    outCount++;
                }
            });
            var JsonData = new
            {
                time = day.ToDate().ToString("yyyy-MM"),
                count = list.Count(),
                inCount = inCount,
                outCount = outCount,
                totalInCosMoney = totalInCosMoney,
                totalOutCosMoney = totalOutCosMoney,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDetailJson(string type, string date)
        {
            var watch = CommonHelper.TimerStart();
            FinaceEntity para = new FinaceEntity();
            DateTime startTime = date.ToDate();
            DateTime endTime = date.ToDate();
            string title = "";
            string subtitle = "";
            if (type == "1")
            {
                startTime = date.ToDate();
                endTime = date.ToDate();
                title = Convert.ToDateTime(endTime.ToString()).ToString("yyyy年MM月dd日") + "汇总对账单";
                subtitle = string.Format("{0} 23:59:59至{1} 23:59:59", Convert.ToDateTime(startTime.ToString()).ToString("yyyy-MM-dd"), Convert.ToDateTime(endTime.ToString()).ToString("yyyy-MM-dd"));


            }
            else if (type == "2")
            {
                startTime = date.ToDate();
                endTime = date.ToDate().AddMonths(1).AddDays(-1);//本月最后一天
                title = Convert.ToDateTime(endTime.ToString()).ToString("yyyy年MM月") + "汇总对账单";
                subtitle = string.Format("{0}至{1}", Convert.ToDateTime(startTime.ToString()).ToString("yyyy-MM"), Convert.ToDateTime(endTime.ToString()).ToString("yyyy-MM"));

            }
            else if (type == "3")
            {
                startTime = date.ToDate().AddDays(-7);
                endTime = date.ToDate().AddDays(0);
                title = Convert.ToDateTime(endTime.ToString()).ToString("yyyy年MM月dd日") + "近一周汇总对账单";
                subtitle = string.Format("{0}至{1}", Convert.ToDateTime(startTime.ToString()).ToString("yyyy-MM-dd"), Convert.ToDateTime(endTime.ToString()).ToString("yyyy-MM-dd"));

            }

            para.StartTime = startTime.ToString();
            para.EndTime = endTime.ToString();

            decimal currentInCosMoney = 0;
            decimal currentOutCosMoney = 0;
            decimal preInCosMoney = 0;
            decimal preOutCosMoney = 0;

            var currentInList = new List<FinaceEntity>();
            var currentOutList = new List<FinaceEntity>();
            var currentlist = FinaceBLL.Instance.GetList(para);
            currentlist.ForEach((o) =>
            {
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.增加)
                {
                    currentInCosMoney += o.CosMoney ?? 0;
                    currentInList.Add(o);
                }
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.减少)
                {
                    currentOutCosMoney += o.CosMoney ?? 0;
                    currentOutList.Add(o);

                }
            });

            //上一期
            if (type == "1")
            {
                startTime = date.ToDate().AddDays(-1);
                endTime = date.ToDate().AddDays(-1);
                para.StartTime = startTime.ToString();
                para.EndTime = endTime.ToString();
            }
            else if (type == "2")
            {
                startTime = date.ToDate().AddMonths(-1);
                endTime = date.ToDate().AddMonths(0).AddDays(-1);//本月最后一天
                para.StartTime = startTime.ToString();
                para.EndTime = endTime.ToString();
            }
            else if (type == "3")
            {
                startTime = date.ToDate().AddDays(-14);
                endTime = date.ToDate().AddDays(-7);
                para.StartTime = startTime.ToString();
                para.EndTime = endTime.ToString();
            }


            var prelist = FinaceBLL.Instance.GetList(para);
            prelist.ForEach((o) =>
            {
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.增加)
                {

                    preInCosMoney += o.CosMoney ?? 0;
                }
                if (o.Operate == (int)QX360.Model.Enums.FinaceOperateType.减少)
                {
                    preOutCosMoney += o.CosMoney ?? 0;

                }
            });
            var JsonData = new
            {
                title = title,
                subtitle = subtitle,
                currentInCosMoney = currentInCosMoney,
                currentOutCosMoney = currentOutCosMoney,
                preInCosMoney = preInCosMoney,
                preOutCosMoney = preOutCosMoney,
                currentInList = currentInList == null ? null : currentInList.GroupBy(t => t.SourceType)
                        .Select(g => new

                        {
                            Name = ((QX360.Model.Enums.FinaceSourceType)g.Key).ToString(),
                            TotalMoney = g.Sum(t => t.CosMoney)

                        }),
                currentOutList = currentOutList == null ? null : currentOutList.GroupBy(t => t.SourceType)
                        .Select(g => new

                        {
                            Name = ((QX360.Model.Enums.FinaceSourceType)g.Key).ToString(),
                            TotalMoney = g.Sum(t => t.CosMoney)

                        }),
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }
    }
}
