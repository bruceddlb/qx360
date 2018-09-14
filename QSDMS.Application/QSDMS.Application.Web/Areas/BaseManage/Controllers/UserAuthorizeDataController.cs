using QSDMS.Application.Web.Controllers;
using QSDMS.Business;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.BaseManage.Controllers
{
    public class UserAuthorizeDataController : BaseController
    {
        private SchoolBLL schoolBLL = new SchoolBLL();
        private AuditOrganizationBLL auditOrganizationBLL = new AuditOrganizationBLL();
        private InsuranceCommpayBLL insuranceCommpayBLL = new InsuranceCommpayBLL();
        private ShopBLL shopBLL = new ShopBLL();
        private UserAuthorizeBLL userAuthorize = new UserAuthorizeBLL();

        /// <summary>
        /// 机构数据授权设置 多角色机构
        /// </summary>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AuthorizeDataTreeJson(string userid)
        {
            var existAuthorizeData = userAuthorize.GetUserAuthorizeListStr(userid);
            var data = new List<AuthorizeOrganizeEntity>();
            //驾校
            AuthorizeOrganizeEntity o = new AuthorizeOrganizeEntity();
            o.ParentId = "0";
            o.ObjectId = "A001";
            o.Level = 1;
            o.ObjectName = QX360.Model.Enums.OrganizeType.驾校机构.ToString();
            o.ObjectType = (int)QX360.Model.Enums.OrganizeType.驾校机构;
            data.Add(o);

            //考场
            o = new AuthorizeOrganizeEntity();
            o.ParentId = "0";
            o.ObjectId = "A002";
            o.Level = 1;
            o.ObjectName = QX360.Model.Enums.OrganizeType.考场机构.ToString();
            o.ObjectType = (int)QX360.Model.Enums.OrganizeType.考场机构;
            data.Add(o);
            //年检机构
            o = new AuthorizeOrganizeEntity();
            o.ParentId = "0";
            o.ObjectId = "A003";
            o.Level = 1;
            o.ObjectName = QX360.Model.Enums.OrganizeType.年检机构.ToString();
            o.ObjectType = (int)QX360.Model.Enums.OrganizeType.年检机构;
            data.Add(o);
            //保险机构
            o = new AuthorizeOrganizeEntity();
            o.ParentId = "0";
            o.ObjectId = "A004";
            o.Level = 1;
            o.ObjectName = QX360.Model.Enums.OrganizeType.保险机构.ToString();
            o.ObjectType = (int)QX360.Model.Enums.OrganizeType.保险机构;
            data.Add(o);
            //店铺机构
            o = new AuthorizeOrganizeEntity();
            o.ParentId = "0";
            o.ObjectId = "A005";
            o.Level = 1;
            o.ObjectName = QX360.Model.Enums.OrganizeType.店铺机构.ToString();
            o.ObjectType = (int)QX360.Model.Enums.OrganizeType.店铺机构;
            data.Add(o);
            //驾校
            var schoollist = schoolBLL.GetList(new SchoolEntity() { IsTraining = 0 });
            if (schoollist != null)
            {

                foreach (var item in schoollist)
                {
                    o = new AuthorizeOrganizeEntity();
                    o.ParentId = "A001";
                    o.ObjectId = item.SchoolId;
                    o.ObjectName = item.Name;
                    o.ObjectType = (int)QX360.Model.Enums.OrganizeType.驾校机构;
                    data.Add(o);
                }
            }
          
            //考场
            schoollist = schoolBLL.GetList(new SchoolEntity() { IsTraining = 1 });
            if (schoollist != null)
            {

                foreach (var item in schoollist)
                {
                    o = new AuthorizeOrganizeEntity();
                    o.ParentId = "A002";
                    o.ObjectId = item.SchoolId;
                    o.ObjectName = item.Name;
                    o.ObjectType = (int)QX360.Model.Enums.OrganizeType.驾校机构;
                    data.Add(o);
                }
            }

            //年检机构
           
            var auditOrganizationlist = auditOrganizationBLL.GetList(null);
            if (auditOrganizationlist != null)
            {

                foreach (var item in auditOrganizationlist)
                {
                    o = new AuthorizeOrganizeEntity();
                    o.ParentId = "A003";
                    o.ObjectId = item.OrganizationId;
                    o.ObjectName = item.Name;
                    o.ObjectType = (int)QX360.Model.Enums.OrganizeType.年检机构;
                    data.Add(o);
                }
            }
            //保险机构
            var insurancelist = insuranceCommpayBLL.GetList(null);
            if (insurancelist != null)
            {

                foreach (var item in insurancelist)
                {
                    o = new AuthorizeOrganizeEntity();
                    o.ParentId = "A004";
                    o.ObjectId = item.InsuranceCommpayId;
                    o.ObjectName = item.Name;
                    o.ObjectType = (int)QX360.Model.Enums.OrganizeType.保险机构;
                    data.Add(o);
                }
            }
            //店铺机构
            var shoplist = shopBLL.GetList(null);
            if (shoplist != null)
            {
                foreach (var item in shoplist)
                {
                    o = new AuthorizeOrganizeEntity();
                    o.ParentId = "A005";
                    o.ObjectId = item.ShopId;
                    o.ObjectName = item.Name;
                    o.ObjectType = (int)QX360.Model.Enums.OrganizeType.店铺机构;
                    data.Add(o);
                }
            }
           
            var treeList = new List<TreeEntity>();
            foreach (AuthorizeOrganizeEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.ObjectId) == 0 ? false : true;
                if (hasChildren == false && item.Level == 1)
                {
                    continue;
                }
                tree.id = item.ObjectId;
                tree.text = item.ObjectName;
                tree.value = item.ObjectId;
                tree.title = "";
                tree.checkstate = existAuthorizeData.Count(t => t == item.ObjectId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = "";
                tree.Level = item.Level;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }

    }

    /// <summary>
    /// 机构对象 驾校 年检机构
    /// </summary>
    public class AuthorizeOrganizeEntity
    {
        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
        public int ObjectType { get; set; }
        public string ParentId { get; set; }
        public int Level { get; set; }
    }
}
