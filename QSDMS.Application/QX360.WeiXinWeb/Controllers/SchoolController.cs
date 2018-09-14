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

namespace QX360.WeiXinWeb.Controllers
{
    public class SchoolController : BaseController
    {
        //
        // GET: /School/

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
        public JsonResult List(int? pageIndex, string queryJson)
        {
            var result = new ReturnMessage(false) { Message = "加载列表失败!" };
            try
            {
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

                }
                para.Status = (int)Model.Enums.UseStatus.启用;
                Pagination pagination = new Pagination();
                pagination.page = pageIndex ?? 1;
                pagination.rows = 10;
                //pagination.sidx = "SortNum";
                //pagination.sord = "asc";
                //var pageList = SchoolBLL.Instance.GetPageList(para, ref pagination);
                var currentpage = new List<SchoolEntity>();
                var pageList = GetAllShoolList(para);
                var qpage = pageList.AsQueryable();
                qpage = qpage.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                currentpage = qpage.ToList();
                pagination.records = pageList.Count();

                if (currentpage != null)
                {
                    currentpage.ForEach((page) =>
                    {
                        if (page.FaceImage != null)
                        {
                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                            page.FaceImage = imageHost + page.FaceImage;
                        }
                        if (page.SchoolId != null)
                        {
                            page.TagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = page.SchoolId });
                            page.TeacherList = TeacherBLL.Instance.GetList(new TeacherEntity() { SchoolId = page.SchoolId, Status = (int)Model.Enums.UseStatus.启用 }).OrderBy(o => o.SortNum).ToList();
                            if (page.TeacherList != null)
                            {
                                page.TeacherList.Foreach((o) =>
                                {
                                    if (o.FaceImage != null)
                                    {
                                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                        o.FaceImage = imageHost + o.FaceImage;
                                    }
                                });
                            }
                        }
                        //if (page.Lat != null && page.Lng != null && para.Lat != null && para.Lng != null)
                        //{
                        //    page.HowLong = HarvenSin.GetDistance(
                        //        new Point2D()
                        //        {
                        //            Lng = (double)para.Lng,
                        //            Lat = (double)para.Lat
                        //        },
                        //        new Point2D()
                        //        {
                        //            Lng = (double)page.Lng,
                        //            Lat = (double)page.Lat
                        //        }).ToString("f2");
                        //}
                        //else
                        //{
                        //    page.HowLong = "未知";
                        //}
                    });
                }
                result.IsSuccess = true;
                result.Message = "加载列表成功!";
                result.ResultData["IsEndPage"] = (pagination.total == (pageIndex ?? 1));
                result.ResultData["List"] = currentpage;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>List";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询所有驾校信息 按当前位置坐标距离最近排序
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<SchoolEntity> GetAllShoolList(SchoolEntity para)
        {
            var list = SchoolBLL.Instance.GetList(para);
            list.ForEach((o) =>
            {
                if (o.Lat != null && o.Lng != null && para.Lat != null && para.Lng != null)
                {
                    o.HowLong = HarvenSin.GetDistance(
                        new Point2D()
                        {
                            Lng = (double)para.Lng,
                            Lat = (double)para.Lat
                        },
                        new Point2D()
                        {
                            Lng = (double)o.Lng,
                            Lat = (double)o.Lat
                        }).ToString("f2");
                }
                else
                {
                    o.HowLong = "未知";
                }
            });
            //list = list.OrderByDescending(d => d.HowLong).ToList();
            list = list.OrderBy(i => i.HowLong).ThenBy(i => i.SortNum).ToList();
            return list;
        }
        /// <summary>
        /// 获取驾校相关对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSchoolModel(string id)
        {
            var result = new ReturnMessage(false) { Message = "获取对象失败!" };
            try
            {
                var data = SchoolBLL.Instance.GetEntity(id);
                if (data != null)
                {
                    if (data.FaceImage != null)
                    {
                        var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                        data.FaceImage = imageHost + data.FaceImage;
                    }
                    //pic
                    var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.SchoolId });
                    if (imageList != null)
                    {
                        data.AttachmentPicList = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                        data.AttachmentPicList.ForEach((o) =>
                        {
                            if (o.PicName != null)
                            {
                                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                                o.PicName = imageHost + o.PicName;
                            }
                        });
                    }
                    //subject
                    var subjectlist = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = data.SchoolId });
                    data.SubjectList = subjectlist;
                    //tag
                    var tagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = data.SchoolId });
                    data.TagList = tagList;
                    //teacher
                    data.TeacherList = TeacherBLL.Instance.GetList(new TeacherEntity() { SchoolId = data.SchoolId, Status = (int)Model.Enums.UseStatus.启用, sidx = "SortNum", sord = "asc" });
                    data.TeacherList.Foreach((o) =>
                    {
                        if (o.FaceImage != null)
                        {
                            var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                            o.FaceImage = imageHost + o.FaceImage;
                        }
                    });
                    //TrainingCar
                    data.TrainingCarList = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { SchoolId = data.SchoolId, Status = (int)Model.Enums.UseStatus.启用 });
                    //address
                    if (data.ProvinceId != null)
                    {
                        data.ProvinceName = AreaBLL.Instance.GetEntity(data.ProvinceId).AreaName;
                    }
                    if (data.CityId != null)
                    {
                        data.CityName = AreaBLL.Instance.GetEntity(data.CityId).AreaName;
                    }
                    if (data.CountyId != null)
                    {
                        data.CountyName = AreaBLL.Instance.GetEntity(data.CountyId).AreaName;
                    }
                    data.AddressInfo = data.ProvinceName + data.CityName + data.CountyName + data.AddressInfo;
                }

                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["Data"] = data;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyController>>GetSchoolModel";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
