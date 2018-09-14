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
    public class WithDringCommitteeController : BaseController
    {
        //
        // GET: /QX360Manage/WithDringCommittee/

        public ActionResult List()
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
            WithDringCommitteeEntity para = new WithDringCommitteeEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "commitcontent":
                            para.CommitContent = queryParam["keyword"].ToString();
                            break;
                        case "teachername":
                            para.TeacherName = queryParam["keyword"].ToString();
                            break;

                    }
                }

            }

            var pageList = WithDringCommitteeBLL.Instance.GetPageList(para, ref pagination);
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
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var list = WithDringCommitteeBLL.Instance.GetList(new WithDringCommitteeEntity() { WithDringOrderId = keyValue });
            var data = new WithDringCommitteeEntity();
            if (list != null && list.Count > 0)
            {
                data = list.FirstOrDefault();
            }
            return Content(data.ToJson());
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                WithDringCommitteeBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "EvaluateController>>RemoveWithDringEvaluate";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }
    }
}
