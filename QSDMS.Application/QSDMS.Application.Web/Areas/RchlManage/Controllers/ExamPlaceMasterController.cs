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
    public class ExamPlaceMasterController : BaseController
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
            ExamPlaceMasterEntity para = new ExamPlaceMasterEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "masteraccount":
                            para.MasterAccount = queryParam["masteraccount"].ToString();
                            break;
                        case "mastername":
                            para.MasterName = queryParam["mastername"].ToString();
                            break;
                        case "mastertel":
                            para.MasterTel = queryParam["mastertel"].ToString();
                            break;
                    }
                }

            }
            //查看自己创建的账户信息          
            if (LoginUser.Account != Util.Config.GetValue("SysAccount") && LoginUser.Account != "System")
            {
                para.CreateId = LoginUser.UserId;
            }

            var pageList = ExamPlaceMasterBLL.Instance.GetPageList(para, ref pagination);

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
            var data = ExamPlaceMasterBLL.Instance.GetEntity(keyValue);
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
                ExamPlaceMasterBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceMasterController>>RemoveForm";
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
                var entity = Serializer.DeserializeJson<ExamPlaceMasterEntity>(json, true);
                if (entity != null)
                {
                    if (keyValue != "")
                    {
                        entity.ExamPlaceMasterId = keyValue;
                        ExamPlaceMasterBLL.Instance.Update(entity);
                    }
                    else
                    {
                        int count = ExamPlaceMasterBLL.Instance.GetList(new ExamPlaceMasterEntity() { MasterAccount = entity.MasterAccount }).Count();
                        if (count > 0)
                        {
                            return Error("该管理账户已存在");
                        }
                        entity.ExamPlaceMasterId = Util.Util.NewUpperGuid();
                        entity.CreateId = LoginUser.UserId;
                        entity.CreateTime = DateTime.Now;
                        ExamPlaceMasterBLL.Instance.Add(entity);
                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceMasterController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

    }
}
