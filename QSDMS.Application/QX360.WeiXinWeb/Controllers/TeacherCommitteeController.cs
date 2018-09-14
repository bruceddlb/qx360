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
    public class TeacherCommitteeController : BaseController
    {
        //
        // GET: /TeacherCommittee/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Send(string json)
        {
            var result = new ReturnMessage(false) { Message = "提交失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<TeacherCommitteeEntity>(json);
                if (entity == null)
                {
                    return Json(result);
                }

                entity.TeacherCommitteeId = Util.NewUpperGuid();
                var order = StudyOrderBLL.Instance.GetEntity(entity.StudyOrderId);
                entity.MemberId = order.MemberId;
                entity.MemberName = order.MemberName;
                entity.TeacherId = LoginTeacher.UserId;
                entity.TeacherName = LoginTeacher.UserName;
                entity.CommitTime = DateTime.Now;
                TeacherCommitteeBLL.Instance.Add(entity);

                //修改订单状态
                order.Status = (int)QX360.Model.Enums.StudySubscribeStatus.教练评价;
                StudyOrderBLL.Instance.Update(order);
                result.IsSuccess = true;
                result.Message = "提交成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ComplaintController>>Send";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
