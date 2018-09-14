using QSDMS.Application.Web.Controllers;
using QX360.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QX360.Model;
using QSDMS.Util.WebControl;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class IndexController : BaseController
    {
        //
        // GET: /QX360Manage/Home/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetHomeJson()
        {
            HomeEntity data = new HomeEntity();
            data.StudyCount = StudyOrderBLL.Instance.GetList(null).Where(o => o.Status != (int)QX360.Model.Enums.StudySubscribeStatus.取消).Count();
            data.TrainingCount = TrainingOrderBLL.Instance.GetList(null).Where(o => o.Status != (int)QX360.Model.Enums.TrainingStatus.已取消).Count();
            data.WithDrivingCount = WithDrivingOrderBLL.Instance.GetList(null).Where(o => o.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count();
            data.AuditCount = AuditOrderBLL.Instance.GetList(null).Where(o => o.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count();
            data.TakeAuditCount = TakeAuditOrderBLL.Instance.GetList(null).Where(o => o.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count();
            Pagination pager = new Pagination();
            pager.page = 1;
            pager.rows = 10;
            pager.sidx = "CreateTime";
            pager.sord = "desc";
            ApplyOrderEntity para = new ApplyOrderEntity();
            para.Status = (int)QX360.Model.Enums.ApplyStatus.待支付;
            data.ApplyOrderList = ApplyOrderBLL.Instance.GetPageList(para, ref pager);
            data.TrainingOrderList = TrainingOrderBLL.Instance.GetPageList(new TrainingOrderEntity() { Status = (int)QX360.Model.Enums.TrainingStatus.待审核 },ref pager);
            return Content(data.ToJson());
        }


    }

    public class HomeEntity
    {

        public int StudyCount { get; set; }

        public int TrainingCount { get; set; }
        public int WithDrivingCount { get; set; }

        public int AuditCount { get; set; }

        public int TakeAuditCount { get; set; }

        public List<ApplyOrderEntity> ApplyOrderList { get; set; }

        public List<TrainingOrderEntity> TrainingOrderList { get; set; }
    }
}
