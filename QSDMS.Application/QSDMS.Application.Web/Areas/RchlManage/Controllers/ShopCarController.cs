using iFramework.Framework;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Business;
using QSDMS.Util;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.Excel;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class ShopCarController : BaseController
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
            ShopCarEntity para = new ShopCarEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;
                        case "shopname":
                            para.ShopName = queryParam["keyword"].ToString();
                            break;
                        case "sortdesc":
                            para.SortDesc = queryParam["keyword"].ToString();
                            break;

                    }
                }
            }

            var pageList = ShopCarBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.ShopCarId != null)
                    {
                        o.WeekSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                        {
                            ShopCarId = o.ShopCarId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                        }).Count();
                        o.TotalSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                        {
                            ShopCarId = o.ShopCarId

                        }).Count();
                    }
                });
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDataListJson()
        {
            var list = ShopCarBLL.Instance.GetList(null);
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
            var data = ShopCarBLL.Instance.GetEntity(keyValue);
            if (data != null)
            {
                var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.ShopCarId });
                if (imageList != null)
                {
                    data.AttachmentPicList = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                }
            }
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
                AttachmentPicBLL.Instance.DeleteByObjectId(keyValue);
                ShopCarBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopCarController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                //反序列化
                var entity = Serializer.DeserializeJson<ShopCarEntity>(json, true);
                if (entity != null)
                {
                    if (keyValue == "")
                    {
                        entity.ShopCarId = Util.Util.NewUpperGuid();
                        entity.CreateTime = DateTime.Now;
                        entity.CreateId = LoginUser.UserId;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        if (ShopCarBLL.Instance.Add(entity))
                        {

                            if (entity.AttachmentPicList != null)
                            {
                                int index = 0;
                                AttachmentPicBLL.Instance.DeleteByObjectId(entity.ShopCarId);
                                foreach (var picitem in entity.AttachmentPicList)
                                {
                                    if (picitem != null)
                                    {
                                        AttachmentPicEntity pic = new AttachmentPicEntity();
                                        pic.PicId = Util.Util.NewUpperGuid();
                                        pic.PicName = picitem.PicName;
                                        pic.SortNum = index;
                                        pic.ObjectId = entity.ShopCarId;
                                        pic.Type = (int)QX360.Model.Enums.AttachmentPicType.商城汽车;
                                        AttachmentPicBLL.Instance.Add(pic);
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                    else
                    {
                        //修改
                        entity.ShopCarId = keyValue;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        if (ShopCarBLL.Instance.Update(entity))
                        {

                            if (entity.AttachmentPicList != null)
                            {
                                int index = 0;
                                AttachmentPicBLL.Instance.DeleteByObjectId(entity.ShopCarId);
                                foreach (var picitem in entity.AttachmentPicList)
                                {

                                    if (picitem != null)
                                    {
                                        AttachmentPicEntity pic = new AttachmentPicEntity();
                                        pic.PicId = Util.Util.NewUpperGuid();
                                        pic.PicName = picitem.PicName;
                                        pic.SortNum = index;
                                        pic.ObjectId = entity.ShopCarId;
                                        pic.Type = (int)QX360.Model.Enums.AttachmentPicType.商城汽车;
                                        AttachmentPicBLL.Instance.Add(pic);
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopCarController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Disabled(string keyValue)
        {
            try
            {
                var entity = ShopCarBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    ShopCarBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopCarController>>UnLockAccount";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enabled(string keyValue)
        {
            try
            {
                var entity = ShopCarBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    ShopCarBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopCarController>>UnLockAccount";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }


        /// <summary>
        /// 导出EXCEL
        /// </summary>
        public void ExportExcel(string queryJson)
        {
            string cacheKey = Request["cacheid"] as string;
            HttpRuntime.Cache[cacheKey + "-state"] = "processing";
            HttpRuntime.Cache[cacheKey + "-row"] = "0";
            try
            {
                //这里要url解码
                var queryParam = Server.UrlDecode(queryJson).ToJObject();
                ShopCarEntity para = new ShopCarEntity();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;
                        case "shopname":
                            para.ShopName = queryParam["keyword"].ToString();
                            break;
                        case "sortdesc":
                            para.SortDesc = queryParam["keyword"].ToString();
                            break;

                    }
                }
                var list = ShopCarBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.ForEach((o) =>
                    {
                        if (o.ShopCarId != null)
                        {
                            o.WeekSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                            {
                                ShopCarId = o.ShopCarId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                            }).Count();
                            o.TotalSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                            {
                                ShopCarId = o.ShopCarId

                            }).Count();
                        }
                        if (o.LimitPrice != null && o.MaxPrice != null)
                        {
                            o.ConsultPrice = o.LimitPrice + "~" + o.MaxPrice;
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "车辆信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "车辆信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "车辆名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "BrandName", ExcelColumn = "品牌名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ShopName", ExcelColumn = "所属车店", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsultPrice", ExcelColumn = "参考价", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SortDesc", ExcelColumn = "车辆简介", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekSeeCarOrderCount", ExcelColumn = "本周被预约次数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalSeeCarOrderCount", ExcelColumn = "总被预约次数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<ShopCarEntity>.ExcelDownload(list, excelconfig);
                    HttpRuntime.Cache[cacheKey + "-state"] = "done";
                }
            }
            catch (Exception)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }
        }

    }
}
