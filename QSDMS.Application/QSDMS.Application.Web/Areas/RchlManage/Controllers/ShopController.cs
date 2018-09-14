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

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class ShopController : BaseController
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
            ShopEntity para = new ShopEntity();
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

            var pageList = ShopBLL.Instance.GetPageList(para, ref pagination);
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
                    if (o.ShopId != null)
                    {
                        o.CarCount = ShopCarBLL.Instance.GetList(new ShopCarEntity()
                        {
                            ShopId = o.ShopId

                        }).Count();
                        o.WeekSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                        {
                            ShopId = o.ShopId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                        }).Count();
                        o.TotalSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                        {
                            ShopId = o.ShopId

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
            var list = ShopBLL.Instance.GetList(null);
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
            var data = ShopBLL.Instance.GetEntity(keyValue);
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
                ShopBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, ShopEntity entity)
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
                    entity.ShopId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.CreateId = LoginUser.UserId;
                    ShopBLL.Instance.Add(entity);
                }
                else
                {
                    entity.ShopId = keyValue;
                    ShopBLL.Instance.Update(entity);
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>SaveForm";
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
                var entity = ShopBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    ShopBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>UnLockAccount";
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
                var entity = ShopBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    ShopBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ShopController>>UnLockAccount";
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
                ShopEntity para = new ShopEntity();
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

                var list = ShopBLL.Instance.GetList(para);
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
                        if (o.ShopId != null)
                        {
                            o.CarCount = ShopCarBLL.Instance.GetList(new ShopCarEntity()
                            {
                                ShopId = o.ShopId

                            }).Count();
                            o.WeekSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                            {
                                ShopId = o.ShopId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                            }).Count();
                            o.TotalSeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                            {
                                ShopId = o.ShopId

                            }).Count();
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                    });


                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "车店信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "车店信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "车店名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "地址", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectName", ExcelColumn = "联系人", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectTel", ExcelColumn = "联系电话", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarCount", ExcelColumn = "车店车辆", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekSeeCarOrderCount", ExcelColumn = "本周看车订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalSeeCarOrderCount", ExcelColumn = "总看车订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<ShopEntity>.ExcelDownload(list, excelconfig);
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
