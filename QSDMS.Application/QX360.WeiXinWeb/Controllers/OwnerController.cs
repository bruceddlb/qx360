using iFramework.Framework;
using Newtonsoft.Json;
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
    public class OwnerController : BaseController
    {
        //
        // GET: /Owner/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CreateOwner(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建车主信息失败!" };
            try
            {

                var entity = JsonConvert.DeserializeObject<OwnerEntity>(data);
                if (entity == null)
                {
                    return Json(result);
                }
                var list = OwnerBLL.Instance.GetList(new OwnerEntity() { MemberMobile = entity.MemberMobile });

                if (list.Count > 0)
                {
                    entity.OwnerId = list.FirstOrDefault().OwnerId;
                    OwnerBLL.Instance.Update(entity);

                }
                else
                {
                    entity.OwnerId = Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    OwnerBLL.Instance.Add(entity);
                }
                result.IsSuccess = true;
                result.Message = "创建成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "OwnerController>>CreateOwner";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
