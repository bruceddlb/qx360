using iFramework.Framework;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class SubjectController : BaseController
    {
        //
        // GET: /Subject/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询学校科目
        /// </summary>
        /// <param name="schoolid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSubjectList(string schoolid)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                var list = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = schoolid });
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetSubjectList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询科目对象
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSubject(string subjectid)
        {
            var result = new ReturnMessage(false) { Message = "加载失败!" };
            try
            {
                var data = SubjectBLL.Instance.GetEntity(subjectid);
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetSubject";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询科目对象
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSubjectByItemId(string schollid, string itemid)
        {
            var result = new ReturnMessage(false) { Message = "加载失败!" };
            try
            {
                var data = new SubjectEntity();
                var list = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = schollid, ItemId = itemid });
                if (list != null && list.Count() > 0)
                {
                    data = list.FirstOrDefault();
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetSubject";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
