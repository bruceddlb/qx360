using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class TrainingController : BaseController
    {
        //
        // GET: /Training/

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Car()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult Time()
        {
            return View();
        }
        public ActionResult Return()
        {
            return View();
        }
        public ActionResult Info()
        {
            return View();
        }
        public ActionResult Nav()
        {
            return View();
        }

        [MaAuthorizeFilter]
        public ActionResult Time2()
        {
            return View();
        }

    }
}
