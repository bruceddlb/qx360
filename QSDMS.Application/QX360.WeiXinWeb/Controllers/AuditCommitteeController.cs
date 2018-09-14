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
    public class AuditCommitteeController : BaseController
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
        /// <param name="organizationid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List(string organizationid)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                var list = AuditCommitteeBLL.Instance.GetList(new AuditCommitteeEntity() { OrganizationId = organizationid }).OrderByDescending(i => i.CommitTime).ThenBy(i => i.CommitTime).ToList();
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditCommitteeController>>List";
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
                var entity = JsonConvert.DeserializeObject<AuditCommitteeEntity>(json);
                if (entity == null)
                {
                    return Json(result);
                }

                entity.AuditCommitteeId = Util.NewUpperGuid();
                if (entity.Type == 1)
                {
                    var order = AuditOrderBLL.Instance.GetEntity(entity.OrderId);
                    entity.MemberId = LoginUser.UserId;
                    entity.MemberName = LoginUser.NickName;
                    entity.MemberMobile = LoginUser.Mobile;
                    entity.CommitTime = DateTime.Now;
                    entity.OrganizationId = order.OrganizationId;
                    entity.OrganizationName = order.OrganizationName;
                    AuditCommitteeBLL.Instance.Add(entity);

                    //修改订单状态
                    order.Status = (int)QX360.Model.Enums.PaySatus.已评价;
                    AuditOrderBLL.Instance.Update(order);
                }
                if (entity.Type == 2)
                {
                    var order = GroupAuditOrderBLL.Instance.GetEntity(entity.OrderId);
                    entity.MemberId = LoginUser.UserId;
                    entity.MemberName = LoginUser.NickName;
                    entity.MemberMobile = LoginUser.Mobile;
                    entity.CommitTime = DateTime.Now;
                    entity.OrganizationId = order.OrganizationId;
                    entity.OrganizationName = order.OrganizationName;
                    AuditCommitteeBLL.Instance.Add(entity);

                    //修改订单状态
                    order.Status = (int)QX360.Model.Enums.PaySatus.已评价;
                    GroupAuditOrderBLL.Instance.Update(order);
                } if (entity.Type == 3)
                {
                    var order = TakeAuditOrderBLL.Instance.GetEntity(entity.OrderId);
                    entity.MemberId = LoginUser.UserId;
                    entity.MemberName = LoginUser.NickName;
                    entity.MemberMobile = LoginUser.Mobile;
                    entity.CommitTime = DateTime.Now;
                    entity.OrganizationId = order.OrganizationId;
                    entity.OrganizationName = order.OrganizationName;
                    AuditCommitteeBLL.Instance.Add(entity);

                    //修改订单状态
                    order.Status = (int)QX360.Model.Enums.PaySatus.已评价;
                    TakeAuditOrderBLL.Instance.Update(order);
                }


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
