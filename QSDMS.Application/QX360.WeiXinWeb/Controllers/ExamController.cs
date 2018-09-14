using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Business.Cache;
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
    public class ExamController : BaseController
    {
        //
        // GET: /Exam/
        [AuthorizeFilter]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EnCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetExamList(string EnCode)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                DataItemCache dataItemCache = new DataItemCache();
                var data = dataItemCache.GetDataItemList(EnCode);
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = data;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamController>>GetExamList";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 考试状态 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetExamStatus()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.ExamStatus));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.ExamStatus));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < names.Length; i++)
                {
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = names[i] });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamController>>GetExamStatus";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult List()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                ExamOrderEntity para = new ExamOrderEntity();
                para.MemberId = LoginUser.UserId;
                var list = ExamOrderBLL.Instance.GetList(para);
                list.ForEach((o) =>
                {
                    if (o.MemberId != null)
                    {
                        o.Member = MemberBLL.Instance.GetEntity(o.MemberId);
                        if (o.Member != null)
                        {
                            if (o.Member.HeadIcon != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                o.Member.HeadIcon = imageHost + o.Member.HeadIcon;
                            }
                        }
                    }
                    if (o.Status != null)
                    {
                        o.StatusName = ((Model.Enums.ExamStatus)o.Status).ToString();
                    }
                });
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateOrder(string data)
        {
            var result = new ReturnMessage(false) { Message = "创建订单失败!" };
            try
            {

                var order = JsonConvert.DeserializeObject<ExamOrderEntity>(data);
                if (order == null)
                {
                    return Json(result);
                }
                order.ExamId = Util.NewUpperGuid();
                order.CreateId = LoginUser.UserId;
                order.MemberId = LoginUser.UserId;
                order.MemberName = LoginUser.NickName;
                order.MemberMobile = LoginUser.Mobile;
                order.CreateTime = DateTime.Now;

                ExamOrderBLL.Instance.Add(order);
                result.IsSuccess = true;
                result.Message = "创建成功";
               
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "OrderController>>CreateOrder";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
