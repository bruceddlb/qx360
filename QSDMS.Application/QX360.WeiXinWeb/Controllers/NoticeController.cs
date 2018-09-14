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
    public class NoticeController : BaseController
    {
        /// <summary>
        /// 查询本人系统推送文章信息
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetMyNotice()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                NoticeEntity para = new NoticeEntity();
                para.CustermerId = LoginUser.UserId;
                para.sidx = "Createtime";
                para.sord = "desc";
                var list = NoticeBLL.Instance.GetList(para);

                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "NoticeController>>GetMyNotice";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询本人消息
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetArticleDetail(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var data = ArticleBLL.Instance.GetEntity(id);
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "NoticeController>>GetArticleDetail";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
