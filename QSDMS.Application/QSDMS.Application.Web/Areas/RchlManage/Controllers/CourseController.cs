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
using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.Excel;
using QSDMS.Business.Cache;
using QSDMS.Business;


namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class CourseController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
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
            CourseEntity para = new CourseEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "coursename":
                            para.CourseName = queryParam["keyword"].ToString();
                            break;

                    }
                }
                if (!queryParam["classid"].IsEmpty())
                {
                    para.ClassId = queryParam["classid"].ToString();
                }
            }

            var pageList = CourseBLL.Instance.GetPageList(para, ref pagination);
            pageList.ForEach((o) =>
            {
                if (o.ClassId != null)
                {
                    var classmodel = ClassBLL.Instance.GetEntity(o.ClassId);
                    if (classmodel != null)
                    {
                        o.ClassName = classmodel.ClassName;
                    }
                }

            });
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

        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            CourseEntity para = new CourseEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["classid"].IsEmpty())
                {
                    para.ClassId = queryParam["classid"].ToString();
                }
            }
            var list = CourseBLL.Instance.GetList(para);
            return Content(list.ToJson());
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = CourseBLL.Instance.GetEntity(keyValue);
            return Content(data.ToJson());
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
                CourseBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "CourseController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                var entity = Serializer.DeserializeJson<CourseEntity>(json, true);
                if (entity != null)
                {
                    if (keyValue != "")
                    {
                        entity.CourseId = keyValue;
                        CourseBLL.Instance.Update(entity);
                    }
                    else
                    {
                        entity.CourseId = Util.Util.NewUpperGuid();
                        CourseBLL.Instance.Add(entity);
                    }

                }
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "CourseController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }
    }
}
