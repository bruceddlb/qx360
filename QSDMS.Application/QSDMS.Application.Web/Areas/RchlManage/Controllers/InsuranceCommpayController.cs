using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Business;
using QSDMS.Business;
using QSDMS.Util;
using QSDMS.Util.Excel;
using Newtonsoft.Json;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class InsuranceCommpayController : BaseController
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
            InsuranceCommpayEntity para = new InsuranceCommpayEntity();
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
                        case "conecttel":
                            para.ConectTel = queryParam["keyword"].ToString();
                            break;
                        case "addressinfo":
                            para.AddressInfo = queryParam["keyword"].ToString();
                            break;

                    }
                }
            }

            var pageList = InsuranceCommpayBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.ProvinceId != null)
                    {
                        o.ProvinceName = AreaBLL.Instance.GetEntity(o.ProvinceId).AreaName;
                    }
                    if (o.CityId != null)
                    {
                        o.CityName = AreaBLL.Instance.GetEntity(o.CityId).AreaName;
                    }
                    if (o.CountyId != null)
                    {
                        o.CountyName = AreaBLL.Instance.GetEntity(o.CountyId).AreaName;
                    }
                    o.AddressInfo = o.ProvinceName + o.CityName + o.CountyName + o.AddressInfo;

                    o.WeekInsuranceOrderCount = InsuranceOrderBLL.Instance.GetList(new InsuranceOrderEntity()
                    {
                        InsuranceId = o.InsuranceCommpayId,
                        StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                        EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                    }).Count();

                    o.TotalInsuranceOrderCount = InsuranceOrderBLL.Instance.GetList(new InsuranceOrderEntity()
                    {
                        InsuranceId = o.InsuranceCommpayId
                    }).Count();
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
            var list = InsuranceCommpayBLL.Instance.GetList(null);
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
            var data = InsuranceCommpayBLL.Instance.GetEntity(keyValue);
            if (data != null)
            {
                var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.InsuranceCommpayId });
                if (imageList != null)
                {
                    data.AttachmentPic = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
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
                InsuranceCommpayBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "InsuranceCommpayController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, InsuranceCommpayEntity entity)
        {
            try
            {
                var result = new ReturnMessage(false) { Message = "编辑失败!" };
                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    result.Message = "名称不能为空";
                    return Json(result);
                }
                if (keyValue == "")
                {
                    entity.InsuranceCommpayId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreateId = LoginUser.UserId;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    InsuranceCommpayBLL.Instance.Add(entity);
                    if (entity.ImageListStr != null)
                    {
                        //删除
                        AttachmentPicBLL.Instance.DeleteByObjectId(entity.InsuranceCommpayId);
                        //插入
                        var imags = JsonConvert.DeserializeObject<List<string>>(entity.ImageListStr);
                        int index = 0;
                        foreach (string item in imags)
                        {

                            if (item != "")
                            {
                                AttachmentPicEntity pic = new AttachmentPicEntity();
                                pic.PicId = Util.Util.NewUpperGuid();
                                pic.PicName = item;
                                pic.SortNum = index;
                                pic.ObjectId = entity.InsuranceCommpayId;
                                pic.Type = (int)QX360.Model.Enums.AttachmentPicType.保险机构;
                                AttachmentPicBLL.Instance.Add(pic);
                            }
                            index++;
                        }
                    }
                }
                else
                {
                    entity.InsuranceCommpayId = keyValue;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    InsuranceCommpayBLL.Instance.Update(entity);
                    if (entity.ImageListStr != null)
                    {
                        //删除
                        AttachmentPicBLL.Instance.DeleteByObjectId(entity.InsuranceCommpayId);
                        //插入
                        var imags = JsonConvert.DeserializeObject<List<string>>(entity.ImageListStr);
                        int index = 0;
                        foreach (string item in imags)
                        {

                            if (item != "")
                            {
                                AttachmentPicEntity pic = new AttachmentPicEntity();
                                pic.PicId = Util.Util.NewUpperGuid();
                                pic.PicName = item;
                                pic.SortNum = index;
                                pic.ObjectId = entity.InsuranceCommpayId;
                                pic.Type = (int)QX360.Model.Enums.AttachmentPicType.保险机构;
                                AttachmentPicBLL.Instance.Add(pic);
                            }
                            index++;
                        }
                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "InsuranceCommpayController>>SaveForm";
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
                var entity = InsuranceCommpayBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    InsuranceCommpayBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "InsuranceCommpayController>>UnLockAccount";
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
                var entity = InsuranceCommpayBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    InsuranceCommpayBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "InsuranceCommpayController>>UnLockAccount";
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
                InsuranceCommpayEntity para = new InsuranceCommpayEntity();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    //类型
                    if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                    {
                        var condition = queryParam["condition"].ToString().ToLower();
                        switch (condition)
                        {
                            case "name":
                                para.Name = queryParam["keyword"].ToString();
                                break;
                            case "conecttel":
                                para.ConectTel = queryParam["keyword"].ToString();
                                break;
                            case "addressinfo":
                                para.AddressInfo = queryParam["keyword"].ToString();
                                break;

                        }
                    }
                }

                var list = InsuranceCommpayBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.ForEach((o) =>
                    {
                        if (o.ProvinceId != null)
                        {
                            o.ProvinceName = AreaBLL.Instance.GetEntity(o.ProvinceId).AreaName;
                        }
                        if (o.CityId != null)
                        {
                            o.CityName = AreaBLL.Instance.GetEntity(o.CityId).AreaName;
                        }
                        if (o.CountyId != null)
                        {
                            o.CountyName = AreaBLL.Instance.GetEntity(o.CountyId).AreaName;
                        }
                        o.AddressInfo = o.ProvinceName + o.CityName + o.CountyName + o.AddressInfo;

                        o.WeekInsuranceOrderCount = InsuranceOrderBLL.Instance.GetList(new InsuranceOrderEntity()
                        {
                            InsuranceId = o.InsuranceCommpayId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                        }).Count();

                        o.TotalInsuranceOrderCount = InsuranceOrderBLL.Instance.GetList(new InsuranceOrderEntity()
                        {
                            InsuranceId = o.InsuranceCommpayId
                        }).Count();
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "保险公司信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "保险公司信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "公司名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "地址", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectName", ExcelColumn = "联系人", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectTel", ExcelColumn = "联系电话", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekInsuranceOrderCount", ExcelColumn = "本周预约订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalInsuranceOrderCount", ExcelColumn = "总预约订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<InsuranceCommpayEntity>.ExcelDownload(list, excelconfig);
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
