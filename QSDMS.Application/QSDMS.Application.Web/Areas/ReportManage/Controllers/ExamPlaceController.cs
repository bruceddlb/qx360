using QSDMS.Application.Web.Controllers;
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
using QSDMS.Util.WebControl;
using iFramework.Framework;
using System.Data;

namespace QSDMS.Application.Web.Areas.ReportManage.Controllers
{
    public class ExamPlaceController : BaseController
    {
        //
        // GET: /ReportManage/ExamPlace/

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
            string time = "";
            string examplaceid = "";
            int trainingtype = 0;
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["examplaceid"].IsEmpty())
                {
                    examplaceid = queryParam["examplaceid"].ToString();
                }
                if (!queryParam["trainingtype"].IsEmpty())
                {
                    trainingtype = int.Parse(queryParam["trainingtype"].ToString());
                }
                if (!queryParam["Time"].IsEmpty())
                {
                    time = queryParam["Time"].ToString();
                }
            }

            //处理假分页
            DataTable dtpage = new DataTable();
            int totalCount = 0;
            try
            {
                DataTable dt = GetDatable(time, examplaceid, trainingtype);
                if (dt != null)
                {
                    dtpage = dt.Clone();
                    DataRow[] rows = GetTableRows(dt, pagination.page, pagination.rows);
                    dtpage.Rows.Clear();
                    foreach (DataRow row in rows)
                    {
                        dtpage.Rows.Add(row.ItemArray);
                    }
                    pagination.records = dt.Rows.Count;
                }

            }
            catch (Exception ex)
            {

            }
            var JsonData = new
            {
                rows = dtpage,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };

            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 动态组装表格数据返还dt
        /// </summary>
        /// <param name="time"></param>
        /// <param name="examplaceid"></param>
        /// <param name="trainingtype"></param>
        /// <returns></returns>
        public DataTable GetDatable(string time, string examplaceid, int trainingtype)
        {
            //string time = "2018-05-01";
            DataTable dt = new DataTable();
            if (time == "" || examplaceid == "")
            {
                return dt;
            }
            //构建表头
            dt.Columns.Add("TimeSection", typeof(string));
            var traningcarlist = TrainingCarBLL.Instance.GetList(new TrainingCarEntity()
            {
                SchoolId = examplaceid,//"E07B085877904C99B5546A02352ED196",
                TrainingType = trainingtype//1
            }).OrderBy((o) => o.SortNum).ToList();
            traningcarlist.ForEach((o) =>
            {
                dt.Columns.Add("car_" + o.TrainingCarId, typeof(string));
            });
            //构建内容
            List<string> timelist = new List<string>();
            //查询预约时间 时间已一个车的设置时间
            var fistcar = traningcarlist.First();
            //查询预约的日期,根据日期查询对应的时间段，原则上一个车对应日期只有一条记录
            var freedatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity() { ObjectId = fistcar.TrainingCarId, StartTime = time, EndTime = time });
            if (freedatelist != null)
            {

                //查询时间段 
                //系统时间段 每个车的时间都一样，所以这里默认某一个车的对应日期的时间日期即可
                var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                {
                    TrainingFreeDateId = freedatelist.First().TrainingFreeDateId
                }).OrderBy((o) => o.SortNum);

                freetimelist.Foreach((o) =>
                {
                    timelist.Add(o.TimeSection);
                });

            }
            //自定义的时间段 这里每个车的的情况可能不一样，所以要处理每个车的对应日期

            //循环每个车查询对应的时间
            foreach (var caritem in traningcarlist)
            {
                var _freedatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity() { ObjectId = caritem.TrainingCarId, StartTime = time, EndTime = time });
                if (_freedatelist != null)
                {
                    foreach (var _freedateitem in _freedatelist)
                    {
                        var custorfreetimelist = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                        {
                            TrainingFreeDateId = _freedateitem.TrainingFreeDateId
                        }).OrderBy((o) => o.SortNum);
                        if (custorfreetimelist != null && custorfreetimelist.Count() > 0)
                        {
                            
                            custorfreetimelist.Foreach((o) =>
                            {
                                var flag = true;
                                foreach (string item in timelist)
                                {
                                    if (item == o.TimeSection)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    timelist.Add(o.TimeSection);
                                }

                            });
                        }
                    }
                }
            }

            //已当前的时间为基准构建数据
            foreach (var item in timelist)
            {
                DataRow dr = dt.NewRow();
                dr["TimeSection"] = item;
                traningcarlist.ForEach((o) =>
                {
                    string txt = "";
                    //查询对应车辆的预约信息
                    //根据时间段和日期id查询对应的时间段id，这里有系统的和自定义的
                    var _freedate = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                    {
                        StartTime = time,
                        EndTime = time,
                        ObjectId = o.TrainingCarId
                    }).FirstOrDefault();
                    if (_freedate != null)
                    {
                        var _freetimeid = "";
                        //查询时间段
                        var _freetime = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                        {
                            TrainingFreeDateId = _freedate.TrainingFreeDateId,
                            TimeSection = item
                        }).FirstOrDefault();
                        if (_freetime != null)
                        {
                            _freetimeid = _freetime.TrainingFreeTimeId;

                        }
                        var _cusfreetime = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                        {
                            TrainingFreeDateId = _freedate.TrainingFreeDateId,
                            TimeSection = item
                        }).FirstOrDefault();
                        if (_cusfreetime != null)
                        {
                            _freetimeid = _cusfreetime.TrainingCustomFreeTimeId;

                        }
                        if (_freetimeid != "")
                        {
                            //查询订单明细表
                            var orderdetail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity()
                            {
                                TrainingFreeTimeId = _freetimeid
                            }).FirstOrDefault();
                            if (orderdetail != null)
                            {
                                var order = TrainingOrderBLL.Instance.GetEntity(orderdetail.TrainingOrderId);
                                if (order != null)
                                {
                                    txt = string.Format("{0},{1}", order.MemberName, order.MemberMobile);
                                }
                            }
                        }

                    }
                    dr["car_" + o.TrainingCarId] = txt;
                });
                dt.Rows.Add(dr);
            }
            return dt;
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
                string time = "";
                string examplaceid = "";
                int trainingtype = 0;
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    var queryParam = Server.UrlDecode(queryJson).ToJObject();
                    if (!queryParam["examplaceid"].IsEmpty())
                    {
                        examplaceid = queryParam["examplaceid"].ToString();
                    }
                    if (!queryParam["trainingtype"].IsEmpty())
                    {
                        trainingtype = int.Parse(queryParam["trainingtype"].ToString());
                    }
                    if (!queryParam["Time"].IsEmpty())
                    {
                        time = queryParam["Time"].ToString();
                    }
                }
                DataTable dt = GetDatable(time, examplaceid, trainingtype);
                if (dt != null)
                {
                    var scholl = SchoolBLL.Instance.GetEntity(examplaceid);
                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = scholl == null ? "" : scholl.Name + time + "-考场预约业务明细-";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "考场预约业务明细.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TimeSection", ExcelColumn = "预约时段", Width = 20 });
                    var list = TrainingCarBLL.Instance.GetList(new TrainingCarEntity()
                    {
                        SchoolId = examplaceid,
                        TrainingType = trainingtype
                    }).OrderBy((o) => o.SortNum).ToList();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "car_" + item.TrainingCarId, ExcelColumn = item.Name, Width = 20 });
                        }
                    }
                    //调用导出方法
                    ExcelHelper.ExcelDownload(dt, excelconfig);
                    HttpRuntime.Cache[cacheKey + "-state"] = "done";
                }
            }
            catch (Exception)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }
        }

        /// <summary>
        /// 获取表头信息
        /// </summary>
        /// <param name="examplaceid"></param>
        /// <param name="trainingtype"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetJsonModelAndNames(string examplaceid, int trainingtype)
        {
            List<string> headlist = new List<string>();
            try
            {
                var list = TrainingCarBLL.Instance.GetList(new TrainingCarEntity()
                {
                    SchoolId = examplaceid,
                    TrainingType = trainingtype
                }).OrderBy((o) => o.SortNum).ToList();
                if (list != null)
                {
                    headlist.Add("{label:'时间',name :'TimeSection',index:'TimeSection',width : '200'}");
                    foreach (var item in list)
                    {
                        var head = "{label:'" + item.Name + "',name :'car_" + item.TrainingCarId + "',index:'car_" + item.TrainingCarId + "',width : '200'}";
                        headlist.Add(head);
                    }
                }
            }
            catch (Exception)
            {

            }
            return Json(headlist.ToJson(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回当前页数据
        /// </summary>
        /// <param name="dtAllEas"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public DataRow[] GetTableRows(DataTable dtAllEas, int PageIndex, int PageSize)
        {
            var rows = dtAllEas.Rows.Cast<DataRow>();
            var curRows = rows.Skip(PageIndex - 1).Take(PageSize).ToArray();
            return curRows;
        }
    }
}
