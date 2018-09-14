using iFramework.Framework;
using QSDMS.Util.WebControl;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Application.Web.Controllers;
using QX360.Business;
using Newtonsoft.Json;
using QSDMS.Business;
using QSDMS.Util.Excel;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class AuditOrganizationController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult Import()
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
            AuditOrganizationEntity para = new AuditOrganizationEntity();
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
            var pageList = AuditOrganizationBLL.Instance.GetPageList(para, ref pagination);
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

                    if (o.OrganizationId != null)
                    {
                        o.WeekAuditOrderCount = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                        {
                            OrganizationId = o.OrganizationId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                        }).Count();
                        o.TotalAuditOrderCount = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                        {
                            OrganizationId = o.OrganizationId
                        }).Count();
                        o.WeekTakeAuditOrderCount = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                        {
                            OrganizationId = o.OrganizationId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                        }).Count();
                        o.TotalTakeAuditOrderCount = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                        {
                            OrganizationId = o.OrganizationId
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
            var list = AuditOrganizationBLL.Instance.GetList(null);
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
            var data = AuditOrganizationBLL.Instance.GetEntity(keyValue);
            if (data != null)
            {
                var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.OrganizationId });
                if (imageList != null)
                {
                    data.AttachmentPic = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                }

                //List<string> imgList = new List<string>();
                //if (imageList != null)
                //{
                //    imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum);
                //    foreach (var item in imageList)
                //    {
                //        imgList.Add(item.PicName);
                //    }
                //    //产品图片字符串 在前端需特殊处理
                //    data.ImageListStr = imgList.ToJson();
                //}

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
                AuditOrganizationBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditOrganizationController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, AuditOrganizationEntity entity)
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
                    entity.OrganizationId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreateId = LoginUser.UserId;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    entity.PriceContent = entity.PriceContent == null ? "" : entity.PriceContent.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    AuditOrganizationBLL.Instance.Add(entity);
                    if (entity.ImageListStr != null)
                    {
                        //删除
                        AttachmentPicBLL.Instance.DeleteByObjectId(entity.OrganizationId);
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
                                pic.ObjectId = entity.OrganizationId;
                                pic.Type = (int)QX360.Model.Enums.AttachmentPicType.年检机构;
                                AttachmentPicBLL.Instance.Add(pic);
                            }
                            index++;
                        }
                    }
                }
                else
                {
                    entity.OrganizationId = keyValue;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    entity.PriceContent = entity.PriceContent == null ? "" : entity.PriceContent.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    AuditOrganizationBLL.Instance.Update(entity);
                    if (entity.ImageListStr != null)
                    {
                        //删除
                        AttachmentPicBLL.Instance.DeleteByObjectId(entity.OrganizationId);
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
                                pic.ObjectId = entity.OrganizationId;
                                pic.Type = (int)QX360.Model.Enums.AttachmentPicType.年检机构;
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
                ex.Data["Method"] = "AuditOrganizationController>>Register";
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
                var entity = AuditOrganizationBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    AuditOrganizationBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditOrganizationController>>UnLockAccount";
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
                var entity = AuditOrganizationBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    AuditOrganizationBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditOrganizationController>>UnLockAccount";
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
                AuditOrganizationEntity para = new AuditOrganizationEntity();
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

                var list = AuditOrganizationBLL.Instance.GetList(para);
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

                        if (o.OrganizationId != null)
                        {
                            o.WeekAuditOrderCount = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                            {
                                OrganizationId = o.OrganizationId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                            }).Count();
                            o.TotalAuditOrderCount = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                            {
                                OrganizationId = o.OrganizationId
                            }).Count();
                            o.WeekTakeAuditOrderCount = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                            {
                                OrganizationId = o.OrganizationId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                            }).Count();
                            o.TotalTakeAuditOrderCount = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                            {
                                OrganizationId = o.OrganizationId
                            }).Count();
                            if (o.IsTake != null)
                            {
                                o.IsTakeName = o.IsTake == 1 ? "是" : "否";
                            }
                            if (o.Status != null)
                            {
                                o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                            }
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "年检机构信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "年检机构信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "机构名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "地址", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectName", ExcelColumn = "联系人", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectTel", ExcelColumn = "联系电话", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MakeMoney", ExcelColumn = "年检费用", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekAuditOrderCount", ExcelColumn = "本周年检订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalAuditOrderCount", ExcelColumn = "总年检订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekTakeAuditOrderCount", ExcelColumn = "本周代审订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalTakeAuditOrderCount", ExcelColumn = "总代审订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IsTakeName", ExcelColumn = "是否代审", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<AuditOrganizationEntity>.ExcelDownload(list, excelconfig);
                    HttpRuntime.Cache[cacheKey + "-state"] = "done";
                }
            }
            catch (Exception)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }
        }
        /// <summary>
        /// 导入数据
        /// </summary>        
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ImportExcel(HttpPostedFileBase file)
        {
            int count = 0;
            var result = new ReturnMessage(false) { Message = "上传失败!" };
            try
            {
                var data = ExcelHelper.ExcelImport(file.InputStream);
                if (data != null)
                {
                    if (data.Columns.Count != 8)
                    {
                        result.Message = "请按照模板格式正确填写内容!";
                        return Json(result);
                    }
                    int successcount = 0, errorcount = 0;
                    foreach (System.Data.DataRow row in data.Rows)
                    {
                        try
                        {
                            if (row[0].ToString() != "")
                            {
                                AuditOrganizationEntity entity = new AuditOrganizationEntity();
                                entity.OrganizationId = Util.Util.NewUpperGuid();
                                entity.Name = row[0].ToString();
                                var provincelist = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[1].ToString(); });
                                if (provincelist != null && provincelist.Count() > 0)
                                {
                                    var province = provincelist.FirstOrDefault();
                                    entity.ProvinceId = province.AreaId;
                                    entity.ProvinceName = province.AreaName;
                                }

                                var citylist = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[2].ToString(); });
                                if (citylist != null && citylist.Count() > 0)
                                {
                                    var city = citylist.FirstOrDefault();
                                    entity.CityId = city.AreaId;
                                    entity.CityName = city.AreaName;
                                }

                                var countyList = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[3].ToString(); });
                                if (countyList != null && countyList.Count() > 0)
                                {
                                    var county = countyList.FirstOrDefault();
                                    entity.CountyId = county.AreaId;
                                    entity.CountyName = county.AreaName;
                                }
                                entity.AddressInfo = row[4].ToString();
                                entity.ConectName = row[5].ToString();
                                entity.ConectTel = row[6].ToString();
                                entity.IsTake = row[7].ToString() == "是" ? 1 : 0;
                                entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                                entity.CreateTime = DateTime.Now;
                                entity.CreateId = LoginUser.UserId;
                                AuditOrganizationBLL.Instance.Add(entity);
                                successcount++;
                            }
                        }
                        catch (Exception)
                        {

                            errorcount++;
                        }
                    }

                    result.IsSuccess = true;
                    result.Message = string.Format("导入成功({0}条),失败({1}条))!", successcount, errorcount);
                }
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AuditOrganizationController>>Import";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
