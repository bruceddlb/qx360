using iFramework.Framework;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Application.Web.Controllers;
using QSMS.API;
namespace QSDMS.Application.Web.Areas.ArticleManage.Controllers
{
    public class ArticleController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            //var watch = CommonHelper.TimerStart();
            //var sql = PetaPoco.Sql.Builder.Append(@"select * from tbl_Article where 1=1");

            //var queryParam = queryJson.ToJObject();

            //if (!queryParam["keyword"].IsEmpty())
            //{
            //    string keyword = queryParam["keyword"].ToString();
            //    sql.Append(" and (charindex(@0,ArticleTitle)>0 or charindex(@0,SortDesc)>0)", keyword);
            //}

            ////类型
            //if (!queryParam["categoryId"].IsEmpty())
            //{
            //    string categoryId = queryParam["categoryId"].ToString();
            //    sql.Append(" and CategoryId=@0", categoryId);
            //}
            //if (!string.IsNullOrWhiteSpace(pagination.sidx))
            //{
            //    sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            //}
            //var currentpage = tbl_Article.Page(pagination.page, pagination.rows, sql);
            ////数据对象
            //var pageList = currentpage.Items;
            //if (pageList != null)
            //{
            //    foreach (var item in pageList)
            //    {
            //        if (item.CategoryId != null)
            //        {
            //            var cate = tbl_ArticleCategory.SingleOrDefault("where classid=@0",item.CategoryId);
            //            if (cate != null)
            //            {
            //                item.CategoryName = cate.ClassName;
            //            }
            //        }
            //    }
            //}
            ////分页对象           
            //pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            //var JsonData = new
            //{
            //    rows = pageList,
            //    total = pagination.total,
            //    page = pagination.page,
            //    records = pagination.records,
            //    costtime = CommonHelper.TimerEnd(watch)
            //};
            //return Content(JsonData.ToJson());
            return Content("");
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
           // var data = tbl_Article.SingleOrDefault("where ArticleId=@0", keyValue);
            return Content("");//data.ToJson()
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpPost]
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

       
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public JsonResult SavePic()
        {
            RsMessage rs = new RsMessage();
            try
            {
                HttpPostedFileBase file = Request.Files["fileDataFileName"];
                var imageHost = System.Configuration.ConfigurationManager.AppSettings["ImageHost"] == "" ? string.Format("http://{0}{1}", Request.Url.Host, Request.Url.Port == 80 ? "" : ":" + Request.Url.Port) : System.Configuration.ConfigurationManager.AppSettings["ImageHost"];
                var url = string.Format("{0}/Upload/UploadFile", imageHost);
                FileClient fileClient = new FileClient();
                var result = fileClient.Post(url, string.Empty, file);
                if (result.IsSuccess)
                {                  
                    rs.success = true;
                    rs.file_path = imageHost + "/" + result.ResultData["files"].ToString();
                }
            }
            catch (Exception ex)
            {
                rs.success = false;
                ex.Data["Method"] = "ArticleController>>SavePic";
                new ExceptionHelper().LogException(ex);
            }
            return Json(rs);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue)
        {
            try
            {

               
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

    }

    /// <summary>
    /// simditor 内容本地上传图片所需对象 这里属性固定不能修改
    /// </summary>
    public class RsMessage
    {
        public bool success { get; set; }

        public string file_path { get; set; }

    }
}
