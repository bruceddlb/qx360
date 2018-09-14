using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.WebControl;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Model;
using QX360.Business;
using QSDMS.Business;
using QSDMS.Util.Excel;
using Aspose.Words;
using Aspose.Words.Drawing;
using System.IO;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class NetSiginController : BaseController
    {

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }



        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            NetSiginEntity para = new NetSiginEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.MemberTel = queryParam["keyword"].ToString();
                            break;

                    }
                }
                //驾校
                if (!queryParam["SchoolId"].IsEmpty())
                {
                    para.SchoolId = queryParam["SchoolId"].ToString();
                }

            }
            var pageList = NetSiginBLL.Instance.GetPageList(para, ref pagination);
            pageList.ForEach((o) =>
            {
                if (o.SchoolId != null)
                {
                    var school = SchoolBLL.Instance.GetEntity(o.SchoolId);
                    if (school != null)
                    {
                        o.SchoolName = school.Name;
                    }
                }

            });
            var JsonData = new
            {
                rows = pageList,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        [HttpGet]
        public ActionResult GetListJson()
        {
            var list = NetSiginBLL.Instance.GetList(null);
            return Content(list.ToJson());
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = NetSiginBLL.Instance.GetEntity(keyValue);
            return Content(data.ToJson());
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
                NetSiginBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "NetSiginController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, NetSiginEntity entity)
        {
            try
            {
                if (keyValue != "")
                {
                    entity.NetSiginId = keyValue;
                    NetSiginBLL.Instance.Update(entity);
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "NetSiginController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="productOutId"></param>
        /// <param name="conditions"></param>
        public ActionResult ExportExcel(string queryJson)
        {
            string cacheKey = Request["cacheid"] as string;
            HttpRuntime.Cache[cacheKey + "-state"] = "processing";
            HttpRuntime.Cache[cacheKey + "-row"] = "0";
            try
            {
                var queryParam = Server.UrlDecode(queryJson).ToJObject();
                var data = NetSiginBLL.Instance.GetEntity(queryParam["Id"].ToString());
                if (data != null)
                {
                    //查询对应驾校的合同模板
                    var school = SchoolBLL.Instance.GetEntity(data.SchoolId);
                    if (school.NetSiginPath != null)
                    {
                        string templatePath = Server.MapPath(school.NetSiginPath);
                        Document doc = new Document(templatePath);
                        foreach (Bookmark mark in doc.Range.Bookmarks)
                        {
                            if (mark != null)
                            {
                                switch (mark.Name.ToLower())
                                {
                                    case "name":
                                        mark.Text = "";
                                        DocumentBuilder builder = new DocumentBuilder(doc);
                                        string imgPath = Server.MapPath(GetSiginPic(data.SiginName));
                                        if (System.IO.File.Exists(imgPath))
                                        {
                                            builder.MoveToBookmark("name");
                                            builder.InsertImage(imgPath, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 90, 30, WrapType.Square);
                                        }
                                        break;
                                    case "tel":
                                        mark.Text = data.MemberTel;
                                        break;
                                    case "photo":
                                        mark.Text = "";
                                        builder = new DocumentBuilder(doc);
                                        imgPath = Server.MapPath(data.HeadIcon);
                                        if (System.IO.File.Exists(imgPath))
                                        {
                                            builder.MoveToBookmark("photo");
                                            builder.InsertImage(imgPath, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 70, 90, WrapType.Square);

                                            //Shape shape = builder.InsertImage(imgPath);
                                            //shape.WrapType = WrapType.None;
                                            // 设置x,y坐标和高宽.
                                            //shape.Left = 0;
                                            //shape.Top = 0;
                                            //shape.Width = 180;
                                            //shape.Height = 220;
                                            //builder.InsertImage(imgPath, RelativeHorizontalPosition.InsideMargin, 1, RelativeVerticalPosition.Margin, 1, 150, 180, WrapType.Through);
                                        }

                                        break;

                                    case "time":
                                        if (data.CreateTime != null)
                                        {
                                            mark.Text = Convert.ToDateTime(data.CreateTime).ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                            mark.Text = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        HttpRuntime.Cache[cacheKey + "-state"] = "done";
                        //doc.Save("aa");
                        //doc.Save(Guid.NewGuid().ToString() + ".doc", SaveFormat.Doc, SaveType.OpenInWord,this.PartialView);
                        var docStream = new MemoryStream();
                        doc.Save(docStream, SaveFormat.Doc);
                        return File(docStream.ToArray(), "application/msword", Guid.NewGuid().ToString() + ".doc");
                    }
                }
            }
            catch (Exception)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }
            return View();
        }

        /// <summary>
        /// 保存签名图
        /// </summary>
        /// <param name="baseimage"></param>
        /// <returns></returns>
        public string GetSiginPic(string baseimage)
        {
            string str = "";
            string dir = "/File/SiginPic/";
            //站点文件目录
            string fileDir = this.HttpContext.Server.MapPath("~" + dir);
            //文件名称
            string fileName = "head_" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            //保存文件所在站点位置
            string filePath = Path.Combine(fileDir, fileName);

            if (!System.IO.Directory.Exists(fileDir))
                System.IO.Directory.CreateDirectory(fileDir);
            if (baseimage.Length > 0)
            {
                string base64pics = baseimage.Split(',')[1];
                //var UserPhoto = @"iVBORw0KGgoAAAANSUhEUgAAAlgAAAD6CAYAAAB9LTkQAAAdBUlEQVR4Xu3du871xl4H4Nk7305yCVsIoV0goEBcADRU0EMJEmxEDRXQUNBxqoASiYMEBQX36000EADF4CEBAiJoxAXQJFkh4OsvN6ZOGstj+3/2DPj55Oi5Mtrz+GZsf17x15eX0n+ECBAgAABAgQIhAp8JbQ0hREgQIAAAQIECCQByyQgQIAAAQIECAQLCFjBoIojQIAAAQIECAhY5gABAgQIECBAIFhAwAoGVRwBAgQIECBAQMAyBwgQIECAAAECwQICVjCo4ggQIECAAAECApY5QIAAAQIECBAIFhCwgkEVR4AAAQIECBAQsMwBAgQIECBAgECwgIAVDKo4AgQIECBAgICAZQ4QIECAAAECBIIFBKxgUMURIECAAAECBAQsc4AAAQIECBAgECwgYAWDKo4AAQIECBAgIGCZAwQIECBAgACBYAEBKxhUcQQIECBAgAABAcscIECAAAECBAgECwhYwaCKI0CAAAECBAgIWOYAAQIECBAgQCBYQMAKBlUcAQIECBAgQEDAMgcIECBAgAABAsECAlYwqOIIECBAgAABAgKWOUCAAAECBAgQCBYQsIJBFUeAAAECBAgQELDMAQIECBAgQIBAsICAFQyqOAIECBAgQICAgGUOECBAgAABAgSCBQSsYFDFESBAgAABAgQELHOAAAECBAgQIBAsIGAFgyqOAAECBAgQICBgmQMECBAgQIAAgWABASsYVHEECBAgQIAAAQHLHCBAgAABAgQIBAsIWMGgiiNAgAABAgQICFjmAAECBAgQIEAgWEDACgZVHAECBAgQIEBAwDIHCBAgQIAAAQLBAgJWMKjiCBAgQIAAAQICljlAgAABAgQIEAgWELCCQRVHgAABAgQIEBCwzAECBAgQIECAQLCAgBUMqjgCBAgQIECAgIBlDhAgQIAAAQIEggUErGBQX360REgQIAAAQIEBCxzgAABAgQIECAQLCBgBYMqjgABAgQIECAgYJkDBAgQIECAAIFgAQErGFRxBAgQIECAAAEByxwgQIAAAQIECAQLCFjBoIojQIAAAQIECAhY5gABAgQIECBAIFhAwAoGVRwBAgQIECBAQMAyBwgQIECAAAECwQICVjCo4ggQIECAAAECApY5QIAAAQIECBAIFhCwgkEVR4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";
                byte[] arr2 = Convert.FromBase64String(base64pics);
                using (MemoryStream ms2 = new MemoryStream(arr2))
                {
                    System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms2);
                    bmp2.Save(filePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    str = dir + fileName + ".png";
                }
            }

            return str;
        }
    }

}
