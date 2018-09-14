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

namespace QSDMS.Application.Web.Areas.ReportManage.Controllers
{
    public class TrainingCarController : BaseController
    {
        //
        // GET: /ReportManage/TrainingCar/

        public ActionResult SubscribeList()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }
        public ActionResult Detail2()
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
            TrainingCarEntity para = new TrainingCarEntity();
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
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;
                        case "schoolname":
                            para.SchoolName = queryParam["keyword"].ToString();
                            break;
                        case "carnumber":
                            para.CarNumber = queryParam["keyword"].ToString();
                            break;
                    }
                }
            }
            var pageList = new List<TrainingCarEntity>();
            try
            {
                pageList = TrainingCarBLL.Instance.GetPageList(para, ref pagination);
                if (pageList != null)
                {
                    pageList.ForEach((o) =>
                    {
                        int subricCount = 0;
                        int nosubricCount = 0;
                        if (o.TrainingCarId != null)
                        {
                            //预约时段总数 包括系统定制预约时间段和后台创建加班时间段
                            var orderList = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId,
                                StartTime = para.StartTime,
                                EndTime = para.EndTime
                            }).Where(p => p.Status != (int)QX360.Model.Enums.TrainingStatus.已取消).ToList();
                            if (orderList != null)
                            {
                                orderList.ForEach((order) =>
                                {
                                    //查询预约明细
                                    var orderDetailList = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity()
                                        {
                                            TrainingOrderId = order.TrainingOrderId
                                        });
                                    subricCount += orderDetailList.Count();
                                });
                            }
                            o.TotalSubricCount = subricCount;
                            //未预约时段总数
                            var freeadatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                            {
                                ObjectId = o.TrainingCarId,
                                StartTime = para.StartTime,
                                EndTime = para.EndTime
                            });
                            if (freeadatelist != null)
                            {
                                freeadatelist.ForEach((freedate) =>
                                {
                                    //查询预约明细
                                    var fgreeTimeList = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                                    {
                                        TrainingFreeDateId = freedate.TrainingFreeDateId,
                                        FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲
                                    });
                                    nosubricCount += fgreeTimeList.Count();
                                });
                            }
                            o.TotalNoSubricCount = nosubricCount;
                        }
                    });
                }
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
        /// 查看预约详细
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageDetailListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            TrainingOrderEntity para = new TrainingOrderEntity();
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
                if (!queryParam["TrainingCarId"].IsEmpty())
                {
                    para.TrainingCarId = queryParam["TrainingCarId"].ToString();
                }
            }
            var pageList = new List<TrainingOrderEntity>();
            try
            {

                pageList = TrainingOrderBLL.Instance.GetPageList(new TrainingOrderEntity()
               {
                   TrainingCarId = para.TrainingCarId,
                   StartTime = para.StartTime,
                   EndTime = para.EndTime
               }, ref pagination).Where(p => p.Status != (int)QX360.Model.Enums.TrainingStatus.已取消).ToList();

                pageList.ForEach((o) =>
                {
                    string datetime = "";
                    var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = o.TrainingOrderId });

                    detail.ForEach((d) =>
                    {
                        datetime += d.ServiceTime + ",";

                    });
                    o.ServiceTime = string.Format("{0}", datetime.TrimEnd(','));
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
        /// 查看未预约详细
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageDetail2ListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            TrainingFreeDateEntity para = new TrainingFreeDateEntity();
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
                if (!queryParam["TrainingCarId"].IsEmpty())
                {
                    para.ObjectId = queryParam["TrainingCarId"].ToString();
                }
            }
            var pageList = new List<TrainingFreeTimeExt>();
            var page = new List<TrainingFreeTimeExt>();
            try
            {

                var freeDateList = TrainingFreeDateBLL.Instance.GetList(para);
                freeDateList.ForEach((freedate) =>
                                {
                                    var freetimeList = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                                    {
                                        TrainingFreeDateId = freedate.TrainingFreeDateId,
                                        FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲
                                    });
                                    freetimeList.ForEach((freetime) =>
                                    {
                                        pageList.Add(new TrainingFreeTimeExt()
                                        {
                                            FreeDate = freedate.FreeDate.ToString(),
                                            TimeSection = freetime.TimeSection
                                        });
                                    });
                                });
                //处理假分页
                var qpage = pageList.AsQueryable();
                qpage = qpage.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                page = qpage.ToList();
                pagination.records = pageList.Count();
            }
            catch (Exception ex)
            {

            }
            var JsonData = new
            {
                rows = page,
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
            TrainingCarEntity para = new TrainingCarEntity();
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
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;
                        case "schoolname":
                            para.SchoolName = queryParam["keyword"].ToString();
                            break;
                        case "carnumber":
                            para.CarNumber = queryParam["keyword"].ToString();
                            break;
                    }
                }
                var list = TrainingCarBLL.Instance.GetList(para);
                if (list != null)
                {
                    var StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString();
                    var EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString();
                    list.ForEach((o) =>
                    {
                        int subricCount = 0;
                        int nosubricCount = 0;
                        string subricinfo = "";
                        string nosubricinfo = "";
                        if (o.TrainingCarId != null)
                        {
                            //预约时段总数 包括系统定制预约时间段和后台创建加班时间段
                            var orderList = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId,
                                StartTime = para.StartTime,
                                EndTime = para.EndTime
                            }).Where(p => p.Status != (int)QX360.Model.Enums.TrainingStatus.已取消).ToList();
                            if (orderList != null)
                            {
                                orderList.ForEach((order) =>
                                {
                                    //查询预约明细
                                    var orderDetailList = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity()
                                    {
                                        TrainingOrderId = order.TrainingOrderId
                                    });
                                    subricCount += orderDetailList.Count();
                                    subricinfo += order.ServiceDate + " " + order.ServiceTime + " " + order.MemberName + "(" + order.MemberMobile + ")" + "/";
                                });

                            }
                            o.TotalSubricCount = subricCount;
                            o.SubricInfo = subricinfo.TrimEnd('/');
                            //未预约时段总数
                            var freeadatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                            {
                                ObjectId = o.TrainingCarId,
                                StartTime = para.StartTime,
                                EndTime = para.EndTime
                            });
                            if (freeadatelist != null)
                            {
                                freeadatelist.ForEach((freedate) =>
                                {
                                    if (freedate.FreeDate != null)
                                    {
                                        nosubricinfo += Convert.ToDateTime(freedate.FreeDate).ToString("yyyy-MM-dd");
                                        //查询预约明细
                                        string freetimeinfo = "";
                                        var freeTimeList = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                                        {
                                            TrainingFreeDateId = freedate.TrainingFreeDateId,
                                            FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲
                                        });
                                        freeTimeList.ForEach((freeTime) =>
                                        {
                                            freetimeinfo += freeTime.TimeSection + " ";
                                        });
                                        nosubricCount += freeTimeList.Count();
                                        nosubricinfo += " " + freetimeinfo + "/";
                                    }
                                });
                            }
                            o.TotalNoSubricCount = nosubricCount;
                            o.NoSubricInfo = nosubricinfo.TrimEnd('/');
                        }
                    });


                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "实训车辆预约报表";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "实训车辆预约报表.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarNumber", ExcelColumn = "车牌号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "实训车辆名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "所属考场", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingTypeName", ExcelColumn = "实训类型", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalSubricCount", ExcelColumn = "已预约时段总数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubricInfo", ExcelColumn = "已预约时段详情", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalNoSubricCount", ExcelColumn = "未预约时段总数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "NoSubricInfo", ExcelColumn = "未预约时段详情", Width = 20 });

                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<TrainingCarEntity>.ExcelDownload(list, excelconfig);
                    HttpRuntime.Cache[cacheKey + "-state"] = "done";
                }
            }
            catch (Exception)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }
        }


    }

    public class TrainingFreeTimeExt
    {
        public string FreeDate { get; set; }
        public string TimeSection { get; set; }
    }
}
