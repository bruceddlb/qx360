using QSDMS.Business;
using System.Web.Mvc;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QSDMS.Model;
using QSDMS.Application.Web.Controllers;

namespace QSDMS.Application.Web.Areas.AuthorizeManage.Controllers
{
    /// <summary>
    /// 描 述：系统视图
    /// </summary>
    public class ModuleColumnController : BaseController
    {
        private ModuleColumnBLL moduleColumnBLL = new ModuleColumnBLL();

        #region 视图功能
        /// <summary>
        /// 视图表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BatchAdd()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 视图列表 
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string moduleId)
        {
            var data = moduleColumnBLL.GetList(moduleId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 视图列表 
        /// </summary>
        /// <returns>返回树形列表Json</returns>
        [HttpGet]
        public ActionResult GetTreeListJson(string moduleId)
        {
            var data = moduleColumnBLL.GetList(moduleId);
            if (data != null)
            {
                return Content(data.ToJson());
            }
            return null;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 视图列表Json转换视图树形Json 
        /// </summary>
        /// <param name="moduleColumnJson">视图列表</param>
        /// <returns>返回树形列表Json</returns>
        [HttpPost]
        public ActionResult ListToListTreeJson(string moduleColumnJson)
        {
            var data = from items in moduleColumnJson.ToList<ModuleColumnEntity>() orderby items.SortCode select items;
            return Content(data.ToJson());
        }
        #endregion
    }
}
