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

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class TrainingCarController : BaseController
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
            TrainingCarEntity para = new TrainingCarEntity();
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
                    var StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString();
                    var EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString();
                    pageList.ForEach((o) =>
                    {
                        if (o.TrainingCarId != null)
                        {
                            o.WeekApplayCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                            }).Count();
                            o.TotalApplayCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId

                            }).Count();
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
        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            TrainingCarEntity para = new TrainingCarEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["schoolid"].IsEmpty())
                {
                    para.SchoolId = queryParam["schoolid"].ToString();
                }
                if (!queryParam["trainingtype"].IsEmpty())
                {
                    para.TrainingType = int.Parse(queryParam["trainingtype"].ToString());
                }
            }
            var list = TrainingCarBLL.Instance.GetList(para);
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
            var data = TrainingCarBLL.Instance.GetEntity(keyValue);
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
                TrainingFreeDateBLL.Instance.DeleteByObjectId(keyValue);
                TrainingCarBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, TrainingCarEntity entity)
        {
            try
            {
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = entity.SchoolId,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    return Error("未设置当月工作日时间,请前去设置后再操作");
                }
                List<TrainingTimeTableEntity> worktimelist = null;
                worktimelist = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
                  {
                      SchoolId = entity.SchoolId

                  }).OrderBy(p => p.SortNum).ToList();
                if (worktimelist.Count() == 0)
                {
                    return Error("未设置该考场实训时段,请前去设置后再操作");
                }
                if (keyValue != "")
                {
                    entity.TrainingCarId = keyValue;
                    TrainingCarBLL.Instance.Update(entity);
                }
                else
                {
                    entity.TrainingCarId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    if (TrainingCarBLL.Instance.Add(entity))
                    {

                        TrainingFreeTimeBLL.Instance.AddInitFreeTime(monthworklist,worktimelist, entity.TrainingCarId);
                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>SaveForm";
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
                var entity = TrainingCarBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    TrainingCarBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>UnLockAccount";
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
                var entity = TrainingCarBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    TrainingCarBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingCarController>>UnLockAccount";
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
                TrainingCarEntity para = new TrainingCarEntity();
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
                        if (o.TrainingCarId != null)
                        {
                            o.WeekApplayCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                            }).Count();
                            o.TotalApplayCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                            {
                                TrainingCarId = o.TrainingCarId

                            }).Count();
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                    });


                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "实训车辆信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "实训车辆信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "实训车辆名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarNumber", ExcelColumn = "车牌号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "所属考场", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingTypeName", ExcelColumn = "实训类型", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekApplayCount", ExcelColumn = "本周预约次数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalApplayCount", ExcelColumn = "预约总次数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
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
                    if (data.Columns.Count != 5)
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
                                TrainingCarEntity entity = new TrainingCarEntity();
                                entity.TrainingCarId = Util.Util.NewUpperGuid();
                                entity.Name = row[0].ToString();
                                var schoolList = SchoolBLL.Instance.GetList(new SchoolEntity() { Name = row[1].ToString() });
                                if (schoolList != null && schoolList.Count() > 0)
                                {
                                    var school = schoolList.FirstOrDefault();
                                    entity.SchoolId = school.SchoolId;
                                    entity.SchoolName = school.Name;
                                }
                                entity.CarNumber = row[2].ToString();
                                if (row[3].ToString() != "")
                                {
                                    int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.TrainingType));
                                    string[] texts = (string[])System.Enum.GetNames(typeof(QX360.Model.Enums.TrainingType));
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        if (texts[i] == row[3].ToString())
                                        {
                                            entity.TrainingType = values[i];
                                            entity.TrainingTypeName = row[3].ToString();
                                            break;
                                        }
                                    }
                                }
                                entity.Remark = row[4].ToString();

                                entity.CreateTime = DateTime.Now;
                                entity.CreateId = LoginUser.UserId;
                                TrainingCarBLL.Instance.Add(entity);
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
                ex.Data["Method"] = "OrderLogisticController>>Import";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取当前一周日期
        /// </summary>
        /// <returns></returns>
        public List<FreeDateEntity> GetCurrentWeekList()
        {
            DateTime firsttime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
            DateTime endTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now);
            List<FreeDateEntity> list = new List<FreeDateEntity>();
            while (true)
            {
                var dateid = Util.Util.NewUpperGuid();
                if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                {
                    list.Add(new FreeDateEntity() { FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = Util.Time.GetChineseWeekDay(firsttime) });
                }
                else
                {
                    list.Add(new FreeDateEntity() { FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = Util.Time.GetChineseWeekDay(firsttime) });
                }
                firsttime = firsttime.AddDays(1);
                if (firsttime > endTime)
                {
                    break;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取时间段
        /// </summary>
        /// <returns></returns>
        public List<KeyValueEntity> GetNormalTimeList()
        {
            DataItemCache dataItemCache = new DataItemCache();
            var data = dataItemCache.GetDataItemList("sjd");
            List<KeyValueEntity> list = new List<KeyValueEntity>();
            if (data != null)
            {
                var itemdataList = data.OrderBy(i => i.SortCode).ThenBy(i => i.SortCode).ToList();
                itemdataList.ForEach((o) =>
                {
                    list.Add(new KeyValueEntity() { ItemId = o.ItemDetailId.ToString(), ItemName = o.ItemName, SortNum = o.SortCode ?? 0 });
                });
            }
            return list;
        }
    }
}
