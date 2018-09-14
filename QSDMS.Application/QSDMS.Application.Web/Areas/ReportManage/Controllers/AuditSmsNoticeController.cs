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

namespace QSDMS.Application.Web.Areas.ReportManage.Controllers
{
    public class AuditSmsNoticeController : Controller
    {
        //
        // GET: /ReportManage/AuditSmsNotice/

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
            SmsLogEntity para = new SmsLogEntity();
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
                    para.RecivMobile = queryParam["keyword"].ToString();
                }
            }
            para.NoticeType = (int)QX360.Model.Enums.SMNoticeType.年审短信;
            var pageList = new List<SmsLogEntity>();
            try
            {
                pageList = SmsLogBLL.Instance.GetPageList(para, ref pagination);
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
            SmsLogEntity para = new SmsLogEntity();
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
                    para.RecivMobile = queryParam["keyword"].ToString();
                }
                para.NoticeType = (int)QX360.Model.Enums.SMNoticeType.年审短信;
                var list = SmsLogBLL.Instance.GetList(para);
                if (list != null)
                {
                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "年审车辆短信发送报表";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "年审车辆短信发送报表.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RecivMobile", ExcelColumn = "车主号码", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateTime", ExcelColumn = "发送时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Exception", ExcelColumn = "发送状态", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Caption", ExcelColumn = "内容", Width = 15 });

                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<SmsLogEntity>.ExcelDownload(list, excelconfig);
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
