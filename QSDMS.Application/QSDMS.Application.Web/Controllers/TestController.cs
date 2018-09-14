using QSDMS.Business;
using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QSDMS.Application.Web.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
          //  List<TestEntity> list = TestBLL.Instance.Query();
            return View();
        }
    }
}
