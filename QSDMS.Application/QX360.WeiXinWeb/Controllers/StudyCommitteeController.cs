using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QX360.WeiXinWeb.Controllers
{
    /// <summary>
    /// 学员评价
    /// </summary>
    public class StudyCommitteeController : BaseController
    {
        //
        // GET: /StudyCommittee/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teacherid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List(string teacherid)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                var list = StudyCommitteeBLL.Instance.GetList(new StudyCommitteeEntity() { TeacherId = teacherid }).OrderByDescending(i => i.CommitTime).ThenBy(i => i.CommitTime).ToList(); ;
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyCommitteeController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Send(string json)
        {
            var result = new ReturnMessage(false) { Message = "提交失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<StudyCommitteeEntity>(json);
                if (entity == null)
                {
                    return Json(result);
                }

                entity.StudyCommitteeId = Util.NewUpperGuid();
                var order = StudyOrderBLL.Instance.GetEntity(entity.StudyOrderId);
                entity.TeacherId = order.TeacherId;
                entity.TeacherName = order.TeacherName;
                entity.MemberId = LoginUser.UserId;
                entity.MemberName = LoginUser.NickName;
                entity.MemberMobile = LoginUser.Mobile;
                entity.CommitTime = DateTime.Now;
                StudyCommitteeBLL.Instance.Add(entity);

                //修改订单状态
                order.Status = (int)QX360.Model.Enums.StudySubscribeStatus.学员评价;
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
