using iFramework.Framework;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Controllers
{   
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminDefault()
        {
            return View();
        }
        public ActionResult DefaultIndex() {
           // ViewBag.aa = OperatorProvider.Provider.Current().UserName;
            return View();
        }
        public ActionResult Desktop() {
            return View();
        }

        public ActionResult AdminPretty()
        {
            return View();
        }
        public ActionResult AdminPrettyDesktop()
        {
            return View();
        }

        public ActionResult AdminFirst() {
            return View();
        }
        public ActionResult AdminFirstDesktop()
        {
            return View();
        }

        /// <summary>
        /// 访问功能
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <param name="moduleName">功能模块</param>
        /// <param name="moduleUrl">访问路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VisitModule(string moduleId, string moduleName, string moduleUrl)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = (int)QSDMS.Model.Enums.LogCategoryEnum.登陆;
            logEntity.OperateTypeId = ((int)OperationType.Visit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Visit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ModuleId = moduleId;
            logEntity.Module = moduleName;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "访问地址：" + moduleUrl;
            LogBLL.Instance.WriteLog(logEntity);
            return Content(moduleId);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveCache()
        {            
            try
            {
                new Cache.Cache().RemoveCache();
                return Success("清除缓存成功");
            }
            catch (Exception ex)
            {
                new ExceptionHelper().LogException(ex);
                return Error("清除缓存失败");
            }
        }
    }
}
