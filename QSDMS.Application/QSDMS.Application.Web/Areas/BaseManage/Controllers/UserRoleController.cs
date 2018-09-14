using QSDMS.Application.Web.Controllers;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.BaseManage.Controllers
{
    public class UserRoleController : BaseController
    {
        private RoleBLL roleBLL = new RoleBLL();
        private UserRoleBLL userRole = new UserRoleBLL();
        //
        // GET: /BaseManage/UserRole/

        public ActionResult AllotAuthorize()
        {
            return View();
        }
        /// <summary>
        /// 系统功能列表
        /// </summary>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RoleTreeJson(string userid)
        {
            var existRole = userRole.GetRoleList(userid);
            var data = roleBLL.GetList();
            var treeList = new List<TreeEntity>();
            foreach (RoleEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                tree.id = item.RoleId;
                tree.text = item.FullName;
                tree.value = item.RoleId;
                tree.title = "";
                tree.checkstate = existRole.Count(t => t.RoleId == item.RoleId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = "0";
                tree.img = "";
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
    }
}
