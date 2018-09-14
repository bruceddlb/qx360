using iFramework.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class TeacherController : BaseController
    {


        /// <summary>
        ///查询教练列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List(int? pageIndex, string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                TeacherEntity para = new TeacherEntity();
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = queryJson.ToJObject();

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

                    if (!queryParam["isTakeCar"].IsEmpty())
                    {
                        para.IsTakeCar = int.Parse(queryParam["isTakeCar"].ToString());
                    }

                }
                para.Status = (int)Model.Enums.UseStatus.启用;
                para.IsWithDriving = 1;
                Pagination pagination = new Pagination();
                pagination.page = pageIndex ?? 1;
                pagination.rows = 10;
                var pageList = TeacherBLL.Instance.GetPageList(para, ref pagination).OrderBy(o => o.SortNum).ToList();
                if (pageList != null)
                {
                    pageList.ForEach((page) =>
                    {
                        if (page.FaceImage != null)
                        {
                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                            page.FaceImage = imageHost + page.FaceImage;
                        }
                    });
                }
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["IsEndPage"] = (pagination.total == (pageIndex ?? 1));
                result.ResultData["List"] = pageList;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据可预约时间查询教练列表
        /// </summary>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List2(string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
                TeacherEntity para = new TeacherEntity();
                var freedate = "";
                var timesection = "";
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = queryJson.ToJObject();

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

                    if (!queryParam["isTakeCar"].IsEmpty())
                    {
                        para.IsTakeCar = int.Parse(queryParam["isTakeCar"].ToString());
                    }
                    if (!queryParam["freedate"].IsEmpty())
                    {
                        freedate = queryParam["freedate"].ToString();
                    }
                    if (!queryParam["timesection"].IsEmpty())
                    {
                        timesection = Server.UrlDecode(queryParam["timesection"].ToString());
                    }

                }
                para.Status = (int)Model.Enums.UseStatus.启用;
                para.IsWithDriving = 1;
                var list = TeacherBLL.Instance.GetList(para).OrderBy(o => o.SortNum).ToList();
                var newlist = new List<TeacherEntity>();
                if (list != null)
                {
                    Dictionary<string, string> tempdic = new Dictionary<string, string>();
                    //查询指定的时间
                    var freedateList = WithDrivingFreeDateBLL.Instance.GetList(new WithDrivingFreeDateEntity()
                       {
                           StartTime = freedate,
                           EndTime = freedate
                       });
                    //查询空闲的时间
                    var freetimeList = WithDrivingFreeTimeBLL.Instance.GetList(new WithDrivingFreeTimeEntity()
                           {
                               FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲,
                               TimeSection = timesection
                           });
                    //管理查询时间对应空闲时间
                    var joinList = from a in freetimeList
                                   join b in freedateList
                                       on a.WithDrivingFreeDateId equals b.WithDrivingFreeDateId
                                   select new
                                   {
                                       WithDrivingFreeDateId = a.WithDrivingFreeDateId,
                                       FreetimeId = a.WithDrivingFreeTimeId,
                                       TimeSection = a.TimeSection,
                                       ObjectId = b.ObjectId,
                                       FreeDate = b.FreeDate
                                   };
                    //处理新集合
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        for (int j = 0; j < joinList.ToList().Count; j++)
                        {
                            var join = joinList.ToList()[j];
                            if (item.TeacherId == join.ObjectId)
                            {
                                newlist.Add(item);
                            }
                        }
                    }
                    newlist.ForEach((page) =>
                   {
                       if (page.FaceImage != null)
                       {
                           var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                           page.FaceImage = imageHost + page.FaceImage;
                       }
                   });
                    result.IsSuccess = true;
                    result.Message = "加载列表成功!";
                    result.ResultData["List"] = newlist;
                    result.ResultData["FreeTimeList"] = joinList.ToList();

                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTeacherModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                var data = TeacherBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (data.SchoolId != null)
                    {
                        data.School = SchoolBLL.Instance.GetEntity(data.SchoolId);
                    }
                    if (data.FaceImage != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.FaceImage = imageHost + data.FaceImage;
                    }
                    if (data.ProvinceId != null)
                    {
                        data.ProvinceName = AreaBLL.Instance.GetEntity(data.ProvinceId).AreaName;
                    }
                    if (data.CityId != null)
                    {
                        data.CityName = AreaBLL.Instance.GetEntity(data.CityId).AreaName;
                    }
                    //if (data.CountyId != null)
                    //{
                    //    data.CountyName = AreaBLL.Instance.GetEntity(data.CountyId).AreaName;
                    //}
                    data.AddressInfo = data.ProvinceName + data.CityName + data.ServicesAreaNames;
                }
                result.IsSuccess = true;
                result.Message = "success";
                result.ResultData["Data"] = data;
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>GetTeacherModel";
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
        public JsonResult Login(string UserName, string UserPwd, string OpenId, string returnurl)
        {
            var result = new ReturnMessage(false) { Message = "登陆失败!" };
            try
            {
                var account = TeacherBLL.Instance.CheckLogin(UserName, UserPwd);
                if (account != null)
                {
                    if (account.Status == (int)QX360.Model.Enums.UseStatus.禁用)
                    {
                        result.Message = "此账户已锁定!";
                        return Json(result);
                    }
                    if (OpenId != account.OpenId)
                    {
                        account.OpenId = OpenId;
                        TeacherBLL.Instance.Update(account);

                    }
                    //写入cookie
                    var userCookie = new HttpCookie("MASSO");
                    userCookie.Expires = DateTime.Now.AddMonths(1);
                    userCookie.Values["accountid"] = account.TeacherId;
                    userCookie.Path = "/";
                    Response.Cookies.Add(userCookie);

                    result.IsSuccess = true;
                    result.Message = "登陆成功";
                    result.ResultData["ReturnUrl"] = returnurl;
                }

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>Login";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult SetWithDrivingStatus(string status, string id)
        {
            var result = new ReturnMessage(false) { Message = "设置失败!" };
            try
            {
                TeacherEntity entity = new TeacherEntity();
                entity.TeacherId = id;
                entity.IsWithDriving = int.Parse(status);
                TeacherBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "设置成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>SetWithDrivingStatus";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetServiceArea()
        {
            var result = new ReturnMessage(false) { Message = "获取服务区域失败!" };
            try
            {
                var teacher = TeacherBLL.Instance.GetEntity(LoginTeacher.UserId);
                if (teacher != null)
                {
                    if (teacher.CityId != null)
                    {
                        var list = AreaBLL.Instance.GetList().Where((o) => { return o.ParentId == teacher.CityId; }).OrderBy(i => i.SortCode).ToList();
                        result.ResultData["List"] = list;
                        result.ResultData["ServiceAreaIds"] = teacher.ServicesAreaIds;
                        result.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>GetServiceArea";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveServiceArea(string ids, string names)
        {
            var result = new ReturnMessage(false) { Message = "设置失败!" };
            try
            {
                TeacherEntity entity = new TeacherEntity();
                entity.TeacherId = LoginTeacher.UserId;
                entity.ServicesAreaIds = ids;
                entity.ServicesAreaNames = names;
                TeacherBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "设置成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>SaveServiceArea";
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
                var account = TeacherBLL.Instance.GetEntity(LoginTeacher.UserId);
                if (account != null)
                {
                    if (account.Pwd != OldUserPwd)
                    {
                        result.Message = "请输入正确的密码!";
                        return Json(result);
                    }
                    account.Pwd = NewUserPwd;//修改密码
                    TeacherBLL.Instance.Update(account);
                    result.IsSuccess = true;
                    result.Message = "密码修改成功!";
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>UpdatePwd";
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
                if (LoginTeacher == null)
                {
                    result.Message = "请先登陆";
                    return Json(result);
                }
                var account = TeacherBLL.Instance.GetEntity(LoginTeacher.UserId);
                result.IsSuccess = true;
                result.ResultData["Data"] = account;
                result.Message = "获取成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>GetUserDetail";
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
                var entity = JsonConvert.DeserializeObject<TeacherEntity>(json, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
                if (entity == null)
                {
                    result.Message = "无效对象";
                    return Json(result);
                }
                entity.TeacherId = LoginTeacher.UserId;
                TeacherBLL.Instance.Update(entity);
                result.IsSuccess = true;
                result.Message = "修改成功!";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>UpdateInfo";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
