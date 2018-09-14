using Hydrosphere.Data;
using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.AccountMange.Controllers
{
    public class AttachmentPicController : BaseController
    {
        [HttpGet]
        [AjaxOnly]
        public ActionResult GetPicList(string objectid)
        {
            var result = new ReturnMessage(false) { Message = "获取图片失败!" };
            try
            {
                var data = tbl_AttachmentPic.Query("where ObjectId=@0", objectid);
                if (data != null)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["data"] = data;
                    return Success("成功", dic);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AttachmentPicController>>GetPicList";
                new ExceptionHelper().LogException(ex);               
            }
            return Error("失败");
        }

    }
}
