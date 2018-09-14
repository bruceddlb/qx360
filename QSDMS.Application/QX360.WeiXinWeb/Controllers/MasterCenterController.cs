using iFramework.Framework;
using QSDMS.Business;
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
    public class MasterCenterController : BaseController
    {
        //
        // GET: /ExamPlaceCenter/
        [MasterAuthorizeFilter]
        public ActionResult Index()
        {
            ViewBag.Id = LoginMaster.UserId;
            return View();
        }

        //
        // GET: /MaCenter/
        public ActionResult Login()
        {

            var sig = Request.Params["sig"];
            if (sig == "out")
            {
                ClearCache();
            }
            return View();
        }
        [MasterAuthorizeFilter]
        public ActionResult Information()
        {

            return View();
        }
        [MasterAuthorizeFilter]
        public ActionResult Password()
        {

            return View();
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login(string UserName, string UserPwd, string returnurl)
        {
            var result = new ReturnMessage(false) { Message = "登陆失败!" };
            try
            {
                var account = UserBLL.Instance.CheckLogin(UserName, UserPwd);
                if (account != null)
                {
                    //写入cookie
                    var userCookie = new HttpCookie("MASTERSSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["accountid"] = account.UserId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);

                    result.IsSuccess = true;
                    result.Message = "登陆成功";
                    result.ResultData["ReturnUrl"] = returnurl;
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MasterCenterController>>Login";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldUserPwd"></param>
        /// <param name="NewUserPwd"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdatePwd(string OldUserPwd, string NewUserPwd)
        {
            var result = new ReturnMessage(false) { Message = "密码修改失败!" };
            try
            {
                var account = UserBLL.Instance.GetEntity(LoginMaster.UserId);
                if (account != null)
                {
                    if (account.Password != OldUserPwd)
                    {
                        result.Message = "请输入正确的密码!";
                        return Json(result);
                    }
                    account.Password = NewUserPwd;//修改密码
                    UserBLL.Instance.RevisePassword(LoginMaster.UserId, account.Password);
                    result.IsSuccess = true;
                    result.Message = "密码修改成功!";
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MasterCenterController>>UpdatePwd";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserDetail()
        {
            var result = new ReturnMessage(false) { Message = "获取用户信息失败!" };
            try
            {
                if (LoginMaster == null)
                {
                    result.Message = "请先登陆";
                    return Json(result);
                }
                var account = UserBLL.Instance.GetEntity(LoginMaster.UserId);
                result.IsSuccess = true;
                result.ResultData["Data"] = account;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MasterCenterController>>GetUserDetail";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }

        /// <summary>
        /// 获取对象信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMasterCenterModel(string id, string startTime, string endTime)
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var kiplist = new List<OrgEntity>();
                var data = UserBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (!string.IsNullOrWhiteSpace(data.HeadIcon))
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.HeadIcon = imageHost + data.HeadIcon;
                    }
                    //根据设置的对应机构信息获取对应的业务单数据
                    kiplist = GetOrgList(data.AuthorizeDataId, startTime, endTime);
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;
                result.ResultData["SubjectList"] = kiplist;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MasterCenterController>>GetMasterCenterModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据设置机构查询对应业务数据
        /// </summary>
        /// <returns></returns>
        public List<OrgEntity> GetOrgList(string ids, string startTime, string endTime)
        {
            var list = new List<OrgEntity>();
            if (ids != null)
            {
                var arr = ids.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    string id = arr[i].Split('|')[0];
                    //驾校 学员数 教练数 报名订单数
                    var schoolist = GetSchool(id, startTime, endTime);
                    schoolist.ForEach((o) =>
                    {
                        list.Add(o);
                    });
                    var examlist = GetExamPalce(id, startTime, endTime);
                    examlist.ForEach((o) =>
                    {
                        list.Add(o);
                    });
                    var auditlist = GetAudit(id, startTime, endTime);
                    auditlist.ForEach((o) =>
                    {
                        list.Add(o);
                    });
                }
            }
            else
            {  //未设置表示可以看到所有业务数据
                var schoolist = GetSchool("", startTime, endTime);
                schoolist.ForEach((o) =>
                {
                    list.Add(o);
                });
                var examlist = GetExamPalce("", startTime, endTime);
                examlist.ForEach((o) =>
                {
                    list.Add(o);
                });
                var auditlist = GetAudit("", startTime, endTime);
                auditlist.ForEach((o) =>
                {
                    list.Add(o);
                });
            }
            return list;
        }

        /// <summary>
        /// 驾校
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<OrgEntity> GetSchool(string id, string startTime, string endTime)
        {
            List<OrgEntity> list = new List<OrgEntity>();
            SchoolEntity para = new SchoolEntity();
            if (id != "")
            {
                para.SchoolId = id;
            }
            para.Status = (int)QX360.Model.Enums.UseStatus.启用;
            para.IsTraining = 0;
            var schoolList = SchoolBLL.Instance.GetList(para);
            if (schoolList.Count > 0)
            {
                schoolList.ForEach((o) =>
                {

                    var item = new OrgEntity();
                    item.Type = 1;
                    item.Id = o.SchoolId;
                    item.Name = o.Name;
                    item.KpiList = new List<KpiEntity>();
                    //学员数                       
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "学员总数",
                        Count = MemberBLL.Instance.GetList(new MemberEntity()
                        {
                            SchoolId = o.SchoolId,
                            Status = (int)QX360.Model.Enums.UseStatus.启用
                        }).Count().ToString(),

                    });
                    //教练数
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "教练总数",
                        Count = TeacherBLL.Instance.GetList(new TeacherEntity()
                        {
                            SchoolId = o.SchoolId,
                            Status = (int)QX360.Model.Enums.UseStatus.启用
                        }).Count().ToString(),

                    });
                    //报名订单数
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "报名订单",
                        Count = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            SchoolId = o.SchoolId
                        }).Where(p => p.Status != (int)QX360.Model.Enums.ApplyStatus.已取消).Count().ToString()

                    });
                    //学车订单
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "学车订单",
                        Count = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            SchoolId = o.SchoolId
                        }).Where(p => p.Status != (int)QX360.Model.Enums.StudySubscribeStatus.取消).Count().ToString()

                    });
                    list.Add(item);
                });
            }
            return list;
        }

        /// <summary>
        /// 考场
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<OrgEntity> GetExamPalce(string id, string startTime, string endTime)
        {
            List<OrgEntity> list = new List<OrgEntity>();
            SchoolEntity para = new SchoolEntity();
            if (id != "")
            {
                para.SchoolId = id;
            }
            para.Status = (int)QX360.Model.Enums.UseStatus.启用;
            para.IsTraining = 1;
            var schoolList = SchoolBLL.Instance.GetList(para);
            if (schoolList.Count > 0)
            {
                schoolList.ForEach((o) =>
                {

                    var item = new OrgEntity();
                    item.Type = 2;
                    item.Id = o.SchoolId;
                    item.Name = o.Name;
                    item.KpiList = new List<KpiEntity>();

                    //车辆
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "实训车辆数",
                        Count = TrainingCarBLL.Instance.GetList(new TrainingCarEntity()
                        {
                            SchoolId = o.SchoolId,
                            Status = (int)QX360.Model.Enums.UseStatus.启用
                        }).Count().ToString(),

                    });
                    //实训订单
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "实训订单",
                        Count = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            SchoolId = o.SchoolId
                        }).Where(p => p.Status != (int)QX360.Model.Enums.TrainingStatus.已取消).Count().ToString()

                    });
                    list.Add(item);
                });
            }
            return list;
        }

        /// <summary>
        /// 年审机构
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<OrgEntity> GetAudit(string id, string startTime, string endTime)
        {
            List<OrgEntity> list = new List<OrgEntity>();
            AuditOrganizationEntity para = new AuditOrganizationEntity();
            if (id != "")
            {
                para.OrganizationId = id;
            }
            para.Status = (int)QX360.Model.Enums.UseStatus.启用;

            var auditList = AuditOrganizationBLL.Instance.GetList(para);
            if (auditList.Count > 0)
            {
                auditList.ForEach((o) =>
                {

                    var item = new OrgEntity();
                    item.Type = 3;
                    item.Id = o.OrganizationId;
                    item.Name = o.Name;
                    item.KpiList = new List<KpiEntity>();
                    //年审订单
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "年审订单",
                        Count = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            OrganizationId = o.OrganizationId,
                        }).Where(p => p.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count().ToString()

                    });
                    //代审订单
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "代审订单",
                        Count = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            OrganizationId = o.OrganizationId,
                        }).Where(p => p.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count().ToString()

                    });
                    //代审订单
                    item.KpiList.Add(new KpiEntity()
                    {
                        Name = "集团年审订单",
                        Count = GroupAuditOrderBLL.Instance.GetList(new GroupAuditOrderEntity()
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            OrganizationId = o.OrganizationId,
                        }).Where(p => p.Status != (int)QX360.Model.Enums.PaySatus.已取消).Count().ToString()

                    });
                    list.Add(item);
                });
            }
            return list;
        }
    }

    /// <summary>
    /// 机构对象
    /// </summary>
    public class OrgEntity
    {
        public int Type { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }

        public List<KpiEntity> KpiList { get; set; }
    }
    /// <summary>
    /// 指标对象
    /// </summary>
    public class KpiEntity
    {


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 预约订单数
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNum { get; set; }
    }
}
