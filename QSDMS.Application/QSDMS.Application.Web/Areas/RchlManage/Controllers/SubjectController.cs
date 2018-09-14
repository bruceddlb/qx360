using iFramework.Framework;
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
using QSDMS.Application.Web.Controllers;
using QSDMS.Model;
using QSDMS.Business;
using QSDMS.Util.Excel;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class SubjectController : BaseController
    {
        //
        // GET: /QX360Manage/Subject/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            SubjectEntity para = new SubjectEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["schoolid"].IsEmpty())
                {
                    para.SchoolId = queryParam["schoolid"].ToString();
                }
            }
            var list = SubjectBLL.Instance.GetList(para);
            return Content(list.ToJson());
        }
        [HttpGet]
        public ActionResult GetFormJson(string schollid, string itemid)
        {
            var data = new SubjectEntity();
            var list = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = schollid, ItemId = itemid });
            if (list != null && list.Count() > 0)
            {
                data = list.FirstOrDefault();
            }
            return Content(data.ToJson());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]

        public ActionResult RemoveForm(string keyValue)
        {
            SubjectBLL.Instance.Delete(keyValue);
            return Success("删除成功。");

        }
    }
}
