using QSDMS.Util.WebControl;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iFramework.Framework;
using QX360.Model;
using QX360.Business;
using QSDMS.Application.Web.Controllers;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class SettingsController : BaseController
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
            SettingsEntity para = new SettingsEntity();
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

            var pageList = SettingsBLL.Instance.GetPageList(para, ref pagination);

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
            var data = SettingsBLL.Instance.GetEntity(keyValue);
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
                SettingsBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "SettingsController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, SettingsEntity entity)
        {
            try
            {
                entity.Remark = entity.Remark == null ? "" : entity.Remark.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                if (keyValue != "")
                {
                    entity.SettingId = keyValue;
                    SettingsBLL.Instance.Update(entity);
                }
                else
                {
                    entity.SettingId = Util.Util.NewUpperGuid();
                    SettingsBLL.Instance.Add(entity);
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "SettingsController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }
        }

    }
}
