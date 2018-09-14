using iFramework.Framework;
using QX360.Business;
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
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class RuleController : BaseController
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
            RuleEntity para = new RuleEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;

                    }
                }
            }

            var pageList = RuleBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.RuleOperation != null)
                    {
                        o.RuleOperationName = ((QX360.Model.Enums.OperationType)o.RuleOperation).ToString();
                        if (o.RuleOperation == (int)QX360.Model.Enums.OperationType.登陆操作)
                        {
                            o.RuleDesc = string.Format("每日登陆系统增加{0}积分", o.GivePoint);
                        }
                        else
                        {
                            o.RuleDesc = string.Format("每消费{0}元赠送{1}积分", o.CosMoney, o.GivePoint);
                        }
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
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = RuleBLL.Instance.GetEntity(keyValue);
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
                RuleBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "RuleController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, RuleEntity entity)
        {
            try
            {
                if (keyValue != "")
                {
                    entity.RuleId = keyValue;
                    RuleBLL.Instance.Update(entity);
                }
                else
                {
                    entity.RuleId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    RuleBLL.Instance.Add(entity);
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "RuleController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

    }
}
