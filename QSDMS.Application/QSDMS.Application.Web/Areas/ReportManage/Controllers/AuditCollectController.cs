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

namespace QSDMS.Application.Web.Areas.ReportManage.Controllers
{
    public class AuditCollectController : BaseController
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
            AuditCollectEntity para = new AuditCollectEntity();
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
                if (!queryParam["keyword"].IsEmpty())
                {
                    para.OrganizationName = queryParam["keyword"].ToString();
                }
                if (!queryParam["type"].IsEmpty())
                {
                    int type = int.Parse(queryParam["type"].ToString());
                    para.SubricType = type;
                }
                if (!queryParam["TimeSpace"].IsEmpty())
                {
                    para.ServiceTime = queryParam["TimeSpace"].ToString();
                }

            }

            var pageList = new List<AuditCollectEntity>();
            try
            {
                pageList = AuditReportBLL.Instance.GetAuditCollectPageList(para, ref pagination);
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
            AuditCollectEntity para = new AuditCollectEntity();
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
                if (!queryParam["keyword"].IsEmpty())
                {
                    para.OrganizationName = queryParam["keyword"].ToString();
                }
                if (!queryParam["type"].IsEmpty())
                {
                    int type = int.Parse(queryParam["type"].ToString());
                    para.SubricType = type;
                }
                if (!queryParam["TimeSpace"].IsEmpty())
                {
                    para.ServiceTime = queryParam["TimeSpace"].ToString();
                }
                var list = AuditReportBLL.Instance.GetAuditCollectList(para);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        switch (item.SubricType)
                        {
                            case (int)QX360.Model.Enums.AuditType.个人年检:
                                item.SubricTypeName = QX360.Model.Enums.AuditType.个人年检.ToString();
                                break;
                            case (int)QX360.Model.Enums.AuditType.集团年检:
                                item.SubricTypeName = QX360.Model.Enums.AuditType.集团年检.ToString();
                                break;
                            case (int)QX360.Model.Enums.AuditType.代检:
                                item.SubricTypeName = QX360.Model.Enums.AuditType.代检.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "年审车辆预约报表";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "年审车辆预约报表.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubricTypeName", ExcelColumn = "预约类型", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrganizationName", ExcelColumn = "年检机构", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceDate", ExcelColumn = "预约时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "预约时段", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubricCount", ExcelColumn = "预约总数", Width = 15 });

                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<AuditCollectEntity>.ExcelDownload(list, excelconfig);
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
