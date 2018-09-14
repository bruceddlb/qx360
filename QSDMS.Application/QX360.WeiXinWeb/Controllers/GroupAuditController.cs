using QSDMS.Util;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QX360.Business;
using QSDMS.Business;
using QSMS.API.BaiduMap;
using iFramework.Framework;
using Newtonsoft.Json;

namespace QX360.WeiXinWeb.Controllers
{
    public class GroupAuditController : BaseController
    {
        //
        // GET: /GroupAudit/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询本人的年检订单
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]

        public JsonResult GetMyAudit(int status)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                GroupAuditOrderEntity para = new GroupAuditOrderEntity();
                if (status != -1)
                {
                    para.Status = status;
                }

                para.MemberId = LoginUser.UserId;
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = GroupAuditOrderBLL.Instance.GetList(para);
                list.Foreach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                    }
                    if (o.OrganizationId != null)
                    {
                        o.Audit = AuditOrganizationBLL.Instance.GetEntity(o.OrganizationId);
                        if (o.Audit != null)
                        {
                            if (o.Audit.ProvinceId != null)
                            {
                                o.Audit.ProvinceName = AreaBLL.Instance.GetEntity(o.Audit.ProvinceId).AreaName;
                            }
                            if (o.Audit.CityId != null)
                            {
                                o.Audit.CityName = AreaBLL.Instance.GetEntity(o.Audit.CityId).AreaName;
                            }
                            if (o.Audit.CountyId != null)
                            {
                                o.Audit.CountyName = AreaBLL.Instance.GetEntity(o.Audit.CountyId).AreaName;
                            }
                            o.Audit.AddressInfo = o.Audit.ProvinceName + o.Audit.CityName + o.Audit.CountyName + o.Audit.AddressInfo;

                        }
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "GroupAuditController>>GetMyAudit";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Cancel(string id)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            try
            {
                GroupAuditOrderEntity entity = new GroupAuditOrderEntity();
                entity.GroupAuditOrderId = id;
                entity.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                GroupAuditOrderBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "取消成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "GroupAuditController>>Cancel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
