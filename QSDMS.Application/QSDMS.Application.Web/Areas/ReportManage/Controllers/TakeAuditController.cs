using iFramework.Framework;
using QSDMS.Util.WebControl;
using QX360.Business;
using QX360.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.Excel;
using QSDMS.Business.Cache;
using QSDMS.Application.Web.Areas.ReportManage.Models;
using QX360.Model.Report;
using QX360.Business.Report;
using QSDMS.Business;

namespace QSDMS.Application.Web.Areas.ReportManage.Controllers
{
    public class TakeAuditController : BaseController
    {
        //
        // GET: /ReportManage/AuditCollect/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
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
            TakeAuditOrderEntity para = new TakeAuditOrderEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["StartTime"].IsEmpty() && !queryParam["EndTime"].IsEmpty())
                {
                    DateTime startTime = queryParam["StartTime"].ToDate();
                    DateTime endTime = queryParam["EndTime"].ToDate();
                    para.StartTime = startTime.ToString();
                    para.EndTime = endTime.ToString();
                }
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "cartypename":
                            para.CarTypeName = queryParam["keyword"].ToString();
                            break;
                        case "carnum":
                            para.CarNum = queryParam["keyword"].ToString();
                            break;
                        case "organizationname":
                            para.OrganizationName = queryParam["keyword"].ToString();
                            break;
                    }
                }

                if (!queryParam["TimeSpace"].IsEmpty())
                {
                    para.ServiceTime = queryParam["TimeSpace"].ToString();
                }

            }
            para.NotStatus = (int)QX360.Model.Enums.PaySatus.已取消;
            var pageList = new List<TakeAuditOrderEntity>();
            try
            {
                pageList = TakeAuditOrderBLL.Instance.GetPageList(para, ref pagination);
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

                });
            }
            catch (Exception ex)
            {

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
        /// 导出EXCEL
        /// </summary>
        public void ExportExcel(string queryJson)
        {
            string cacheKey = Request["cacheid"] as string;
            HttpRuntime.Cache[cacheKey + "-state"] = "processing";
            HttpRuntime.Cache[cacheKey + "-row"] = "0";
            TakeAuditOrderEntity para = new TakeAuditOrderEntity();
            try
            {
                //这里要url解码
                var queryParam = Server.UrlDecode(queryJson).ToJObject();
                if (!queryParam["StartTime"].IsEmpty() && !queryParam["EndTime"].IsEmpty())
                {
                    DateTime startTime = queryParam["StartTime"].ToDate();
                    DateTime endTime = queryParam["EndTime"].ToDate();
                    para.StartTime = startTime.ToString();
                    para.EndTime = endTime.ToString();
                }
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "cartypename":
                            para.CarTypeName = queryParam["keyword"].ToString();
                            break;
                        case "carnum":
                            para.CarNum = queryParam["keyword"].ToString();
                            break;
                        case "organizationname":
                            para.OrganizationName = queryParam["keyword"].ToString();
                            break;
                    }
                }

                if (!queryParam["TimeSpace"].IsEmpty())
                {
                    para.ServiceTime = queryParam["TimeSpace"].ToString();
                }
                para.NotStatus = (int)QX360.Model.Enums.PaySatus.已取消;
                var list = TakeAuditOrderBLL.Instance.GetList(para);
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

                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "代审车辆预约明细报表";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "代审车辆预约明细报表.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "用户名", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarNum", ExcelColumn = "车牌号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarTypeName", ExcelColumn = "车辆类型", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "联系电话", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "上门地址", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceDate", ExcelColumn = "预约时间", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "预约时段", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrganizationName", ExcelColumn = "代审机构", Width = 15 });

                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<TakeAuditOrderEntity>.ExcelDownload(list, excelconfig);
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
