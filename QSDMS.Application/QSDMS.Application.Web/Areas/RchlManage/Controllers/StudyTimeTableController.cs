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
    public class StudyTimeTableController : BaseController
    {
        //
        // GET: /QX360Manage/WorkTimeTable/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddTime()
        {
            return View();
        }

        /// <summary>
        /// 当前学车时段设置列表
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStudyTimeTableJson(string objectid)
        {
            var list = WorkTimeTableBLL.Instance.GetList(new WorkTimeTableEntity()
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
                WorkTimeTableBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyTimeTableController>>RemoveForm";
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
                var list = Serializer.DeserializeJson<List<WorkTimeTableEntity>>(json, true);
                if (list != null)
                {
                    int count = WorkTimeTableBLL.Instance.GetList(new WorkTimeTableEntity()
                    {
                        SchoolId = keyValue,

                    }).Count();
                    if (count > 0)//update
                    {
                        //修改

                        foreach (WorkTimeTableEntity worktimeItem in list)
                        {

                            if (worktimeItem.WorkTimeTableId == "")
                            {
                                worktimeItem.WorkTimeTableId = Util.Util.NewUpperGuid();

                                WorkTimeTableBLL.Instance.Add(worktimeItem);

                            }
                            else
                            {
                                WorkTimeTableBLL.Instance.Update(worktimeItem);
                            }
                        }
                    }
                    else//add
                    {
                        var num = 0;
                        foreach (WorkTimeTableEntity worktimeItem in list)
                        {
                            worktimeItem.WorkTimeTableId = Util.Util.NewUpperGuid();

                            WorkTimeTableBLL.Instance.Add(worktimeItem);


                        }

                    }
                }
                //同步对应数据
                //var worktimelist = WorkTimeTableBLL.Instance.GetList(new WorkTimeTableEntity()
                //{
                //    SchoolId = keyValue,

                //});
                //var teacherlist = TeacherBLL.Instance.GetList(new TeacherEntity() { SchoolId = keyValue });
                //foreach (var teacheritem in teacherlist)
                //{
                //    if (teacheritem.SchoolId != "" && teacheritem.SchoolId != "-1")
                //    {
                //        StudyFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, teacheritem.TeacherId);
                //    }
                //}

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyTimeTableController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }
    }
}
