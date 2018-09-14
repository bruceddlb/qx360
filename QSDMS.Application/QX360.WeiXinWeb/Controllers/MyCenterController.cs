using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class MyCenterController : BaseController
    {
        //
        // GET: /MyCenter/
        [AuthorizeFilter]
        public ActionResult Audit()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Complaint()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult LearnCar()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Message()
        {
            return View();

        }
        [AuthorizeFilter]
        public ActionResult Point()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult School()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult SeeCar()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult SetUp()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult TakeAudit()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Training()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Vip()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult WithDriving()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Information()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Password()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Insurance()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult ArticleDetail()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult StudyCommittee()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult AuditCommittee()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult WithDringCommittee()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult GroupAudit()
        {
            return View();
        }

    }
}
