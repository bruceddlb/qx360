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
    public class AuditTimeTableController : BaseController
    {
        //
        // GET: /QX360Manage/AuditTimeTable/

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
        public ActionResult GetAuditTimeTableJson(string objectid)
        {
            var list = AuditTimeTableBLL.Instance.GetList(new AuditTimeTableEntity()
            {
                AuditId = objectid

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
                AuditTimeTableBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditTimeTableController>>RemoveForm";
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
                var list = Serializer.DeserializeJson<List<AuditTimeTableEntity>>(json, true);
                if (list != null)
                {
                    int count = AuditTimeTableBLL.Instance.GetList(new AuditTimeTableEntity()
                    {
                        AuditId = keyValue,

                    }).Count();
                    if (count > 0)//update
                    {
                        //修改

                        foreach (AuditTimeTableEntity worktimeItem in list)
                        {
                            if (worktimeItem.AuditTimeTableId == "")
                            {
                                worktimeItem.AuditTimeTableId = Util.Util.NewUpperGuid();
                                AuditTimeTableBLL.Instance.Add(worktimeItem);
                            }
                            else
                            {
                                AuditTimeTableBLL.Instance.Update(worktimeItem);
                            }
                        }
                    }
                    else//add
                    {
                        var num = 0;
                        foreach (AuditTimeTableEntity worktimeItem in list)
                        {
                            worktimeItem.AuditTimeTableId = Util.Util.NewUpperGuid();
                            AuditTimeTableBLL.Instance.Add(worktimeItem);
                        }
                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditTimeTableController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }
    }
}
