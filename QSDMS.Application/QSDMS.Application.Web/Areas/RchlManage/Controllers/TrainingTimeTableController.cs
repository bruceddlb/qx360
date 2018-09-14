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
    public class TrainingTimeTableController : BaseController
    {
        //
        // GET: /QX360Manage/TrainingTimeTable/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddTime()
        {
            return View();
        }
        /// <summary>
        /// 当前实训时段设置列表
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTrainingTimeTableJson(string objectid)
        {
            var list = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
            {
                SchoolId = objectid

            }).OrderBy(p => p.SortNum).ToList();
            return Content(list.ToJson());
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                TrainingTimeTableBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingTimeTableController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = keyValue,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    return Error("未设置当月工作日时间");
                }
                //反序列化
                var list = Serializer.DeserializeJson<List<TrainingTimeTableEntity>>(json, true);
                if (list != null)
                {
                    int count = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
                    {
                        SchoolId = keyValue,

                    }).Count();
                    if (count > 0)//update
                    {
                        //修改

                        foreach (TrainingTimeTableEntity worktimeItem in list)
                        {

                            if (worktimeItem.TrainingTimeTableId == "")
                            {
                                worktimeItem.TrainingTimeTableId = Util.Util.NewUpperGuid();

                                TrainingTimeTableBLL.Instance.Add(worktimeItem);

                            }
                            else
                            {
                                TrainingTimeTableBLL.Instance.Update(worktimeItem);
                            }
                        }
                    }
                    else//add
                    {

                        foreach (TrainingTimeTableEntity worktimeItem in list)
                        {
                            worktimeItem.TrainingTimeTableId = Util.Util.NewUpperGuid();
                            TrainingTimeTableBLL.Instance.Add(worktimeItem);

                        }

                    }
                }

                //同步对应数据
                //var worktimelist = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
                //{
                //    SchoolId = keyValue,
                //});
                //var carlist = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { SchoolId = keyValue });
                //foreach (var caritem in carlist)
                //{
                //    TrainingFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, caritem.TrainingCarId);
                //}
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingTimeTableController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }
    }
}
