using iFramework.Framework;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSMS.API.BaiduMap;
using Newtonsoft.Json;

namespace QX360.WeiXinWeb.Controllers
{
    public class ExamPlaceController : BaseController
    {
        //
        // GET: /ExamPlace/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取驾校列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List(string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                int trainingtype = 1;
                SchoolEntity para = new SchoolEntity();
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = queryJson.ToJObject();
                    if (!queryParam["keyword"].IsEmpty())
                    {
                        para.Name = queryParam["keyword"].ToString();
                    }
                    if (!queryParam["provinceid"].IsEmpty())
                    {
                        para.ProvinceId = queryParam["provinceid"].ToString();
                    }
                    if (!queryParam["cityid"].IsEmpty())
                    {
                        para.CityId = queryParam["cityid"].ToString();
                    }
                    if (!queryParam["countyid"].IsEmpty())
                    {
                        para.CountyId = queryParam["countyid"].ToString();
                    }
                    if (!queryParam["priceid"].IsEmpty())
                    {
                        para.TrainingPriceRange = (QX360.Model.Enums.PriceRange)int.Parse(queryParam["priceid"].ToString());
                    }
                    if (!queryParam["distanceid"].IsEmpty())
                    {
                        para.DistanceRange = (QX360.Model.Enums.DistanceRange)int.Parse(queryParam["distanceid"].ToString());
                    }
                    if (!queryParam["lat"].IsEmpty())
                    {
                        para.Lat = decimal.Parse(queryParam["lat"].ToString());
                    }
                    if (!queryParam["lng"].IsEmpty())
                    {
                        para.Lng = decimal.Parse(queryParam["lng"].ToString());
                    }
                    if (!queryParam["schoolid"].IsEmpty())
                    {
                        para.SchoolId = queryParam["schoolid"].ToString();
                    }
                    if (!queryParam["istraining"].IsEmpty())
                    {
                        para.IsTraining = int.Parse(queryParam["istraining"].ToString());
                    }
                    if (!queryParam["trainingtype"].IsEmpty())
                    {
                        trainingtype = int.Parse(queryParam["trainingtype"].ToString());
                    }

                }
                para.IsTraining = 1;
                para.Status = (int)Model.Enums.UseStatus.启用;
                var newlist = new List<SchoolEntity>();
                var flag = false;
                para.sidx = "sortnum";
                para.sord = "asc";
                var list = SchoolBLL.Instance.GetList(para);
                if (list != null)
                {
                    foreach (var page in list)
                    {

                        if (page.SchoolId != null)
                        {
                            page.TagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = page.SchoolId });
                            int count = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = page.SchoolId, ItemId = trainingtype.ToString() }).Count();
                            if (count > 0)
                            {
                                flag = true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (page.FaceImage != null)
                        {
                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                            page.FaceImage = imageHost + page.FaceImage;
                        }

                        if (page.Lat != null && page.Lng != null && para.Lat != null && para.Lng != null)
                        {
                            page.HowLong = HarvenSin.GetDistance(
                                new Point2D()
                                {
                                    Lng = (double)para.Lng,
                                    Lat = (double)para.Lat
                                },
                                new Point2D()
                                {
                                    Lng = (double)page.Lng,
                                    Lat = (double)page.Lat
                                }).ToString("f2");
                        }
                        else
                        {
                            page.HowLong = "未知";
                        }

                        //添加数据
                        if (flag)
                        {
                            newlist.Add(page);
                        }
                    }
                }
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["List"] = newlist;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                var account = ExamPlaceMasterBLL.Instance.CheckLogin(UserName, UserPwd);
                if (account != null)
                {
                    if (account.Status == (int)QX360.Model.Enums.UseStatus.禁用)
                    {
                        result.Message = "此账户已锁定!";
                        return Json(result);
                    }

                    //写入cookie
                    var userCookie = new HttpCookie("EXAMPLACESSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["accountid"] = account.ExamPlaceMasterId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);

                    result.IsSuccess = true;
                    result.Message = "登陆成功";
                    result.ResultData["ReturnUrl"] = returnurl;
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceController>>Login";
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
                var account = ExamPlaceMasterBLL.Instance.GetEntity(LoginExamPlace.UserId);
                if (account != null)
                {
                    if (account.MasterPwd != OldUserPwd)
                    {
                        result.Message = "请输入正确的密码!";
                        return Json(result);
                    }
                    account.MasterPwd = NewUserPwd;//修改密码
                    ExamPlaceMasterBLL.Instance.Update(account);
                    result.IsSuccess = true;
                    result.Message = "密码修改成功!";
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceMasterController>>UpdatePwd";
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
                if (LoginExamPlace == null)
                {
                    result.Message = "请先登陆";
                    return Json(result);
                }
                var account = ExamPlaceMasterBLL.Instance.GetEntity(LoginExamPlace.UserId);
                result.IsSuccess = true;
                result.ResultData["Data"] = account;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceMasterController>>GetUserDetail";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);

        }

        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateInfo(string json)
        {
            var result = new ReturnMessage(false) { Message = "修改用户资料失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<ExamPlaceMasterEntity>(json, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                if (entity == null)
                {
                    result.Message = "无效对象";
                    return Json(result);
                }
                entity.ExamPlaceMasterId = LoginExamPlace.UserId;
                ExamPlaceMasterBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "修改成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ExamPlaceMasterController>>UpdateInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
