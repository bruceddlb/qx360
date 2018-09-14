using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.Excel;
using QSDMS.Business.Cache;
using QSDMS.Business;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class TeacherController : BaseController
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
            TeacherEntity para = new TeacherEntity();
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
                        case "schoolid":
                            para.SchoolId = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.Mobile = queryParam["keyword"].ToString();
                            break;
                        case "carnumber":
                            para.CarNumber = queryParam["keyword"].ToString();
                            break;
                    }
                }

                if (!queryParam["schoolid"].IsEmpty())
                {
                    para.SchoolId = queryParam["schoolid"].ToString();
                }
                if (!queryParam["iswithdriving"].IsEmpty())
                {
                    para.IsWithDriving = int.Parse(queryParam["iswithdriving"].ToString());
                }
                if (!queryParam["istakecar"].IsEmpty())
                {
                    para.IsTakeCar = int.Parse(queryParam["istakecar"].ToString());
                }
                if (!queryParam["provinceid"].IsEmpty())
                {
                    para.ProvinceId = queryParam["provinceid"].ToString();
                }
                if (!queryParam["cityid"].IsEmpty())
                {
                    para.CityId = queryParam["cityid"].ToString();
                }
                if (!queryParam["countyid"].IsEmpty())
                {
                    para.CountyId = queryParam["countyid"].ToString();
                }
                if (!queryParam["simplespelling"].IsEmpty())
                {
                    para.SimpleSpelling = queryParam["simplespelling"].ToString();
                }
            }
            pagination.sidx = "sortnum";
            pagination.sord = "asc";
            var pageList = TeacherBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                var StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString();
                var EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString();
                pageList.ForEach((o) =>
                {
                    if (o.TeacherId != null)
                    {
                        if (o.ProvinceId != null)
                        {
                            o.ProvinceName = AreaBLL.Instance.GetEntity(o.ProvinceId).AreaName;
                        }
                        if (o.CityId != null)
                        {
                            o.CityName = AreaBLL.Instance.GetEntity(o.CityId).AreaName;
                        }
                        o.AddressInfo = o.ProvinceName + o.CityName + o.ServicesAreaNames;

                        o.StudentCount = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { TeacherId = o.TeacherId }).Count();
                        o.WeekStudyOrderCount = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                        {
                            TeacherId = o.TeacherId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                        }).Count();
                        o.WeekWithDrivingOrderCount = WithDrivingOrderBLL.Instance.GetList(new WithDrivingOrderEntity()
                        {
                            TeacherId = o.TeacherId,
                            StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                            EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

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

        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            TeacherEntity para = new TeacherEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["schoolid"].IsEmpty())
                {
                    para.SchoolId = queryParam["schoolid"].ToString();
                }
            }
            var list = TeacherBLL.Instance.GetList(para).OrderBy(o => o.SortNum).ToList();
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
            var data = TeacherBLL.Instance.GetEntity(keyValue);
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
                //FreeDateBLL.Instance.DeleteByObjectId(keyValue);
                StudyFreeDateBLL.Instance.DeleteByObjectId(keyValue);
                WithDrivingFreeDateBLL.Instance.DeleteByObjectId(keyValue);
                TeacherBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, TeacherEntity entity)
        {
            try
            {
                List<MonthWorkDayEntity> monthworklist = null;
                List<WorkTimeTableEntity> worktimelist = null;
                if (entity.SchoolId != null && entity.SchoolId != "-1")
                {
                    string yearmonth = Convert.ToDateTime(DateTime.Now.ToString()).ToString("yyyy-MM");
                    DateTime firsttime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                    DateTime endTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                    monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                   {
                       ObjectId = entity.SchoolId,
                       YearMonth = yearmonth,
                       StartTime = firsttime.ToString(),
                       EndTime = endTime.ToString()

                   }).OrderBy(p => p.WorkDay).ToList();
                    if (monthworklist.Count() == 0)
                    {
                        return Error("未设置当月工作日时间,请前去设置后再操作");
                    }
                    worktimelist = WorkTimeTableBLL.Instance.GetList(new WorkTimeTableEntity()
                   {
                       SchoolId = entity.SchoolId

                   }).OrderBy(p => p.SortNum).ToList();
                    if (worktimelist.Count() == 0)
                    {
                        return Error("未设置该驾校学车时段,请前去设置后再操作");
                    }

                }
                if (keyValue != "")
                {
                    entity.TeacherId = keyValue;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    TeacherBLL.Instance.Update(entity);
                }
                else
                {
                    //int count = TeacherBLL.Instance.GetList(new TeacherEntity() { MasterAccount = entity.MasterAccount }).Count();
                    //if (count > 0)
                    //{
                    //    return Error("该管理账户已存在");
                    //}
                    int count = TeacherBLL.Instance.GetList(new TeacherEntity() { Mobile = entity.Mobile }).Count();
                    if (count > 0)
                    {
                        return Error("该电话号码已存在");
                    }
                    entity.TeacherId = Util.Util.NewUpperGuid();
                    entity.Pwd = "888888";
                    entity.CreateTime = DateTime.Now;
                    entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                    if (TeacherBLL.Instance.Add(entity))
                    {

                        //如果教练对应有驾校则初始化配置时间信息
                        if (entity.SchoolId != "-1" && monthworklist != null && worktimelist != null)
                        {
                            StudyFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, entity.TeacherId);
                        }
                        if (entity.IsWithDriving == 1)
                        {
                            WithDrivingFreeTimeBLL.Instance.AddInitFreeTime(entity.TeacherId);
                        }
                    }
                }

                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>SaveForm";
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
                var entity = TeacherBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    TeacherBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>UnLockAccount";
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
                var entity = TeacherBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    TeacherBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TeacherController>>UnLockAccount";
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
                TeacherEntity para = new TeacherEntity();
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
                        case "schoolid":
                            para.SchoolId = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.Mobile = queryParam["keyword"].ToString();
                            break;
                    }
                }
                if (!queryParam["schoolid"].IsEmpty())
                {
                    para.SchoolId = queryParam["schoolid"].ToString();
                }
                if (!queryParam["iswithdriving"].IsEmpty())
                {
                    para.IsWithDriving = int.Parse(queryParam["iswithdriving"].ToString());
                }
                if (!queryParam["istakecar"].IsEmpty())
                {
                    para.IsTakeCar = int.Parse(queryParam["istakecar"].ToString());
                }
                if (!queryParam["provinceid"].IsEmpty())
                {
                    para.ProvinceId = queryParam["provinceid"].ToString();
                }
                if (!queryParam["cityid"].IsEmpty())
                {
                    para.CityId = queryParam["cityid"].ToString();
                }
                if (!queryParam["countyid"].IsEmpty())
                {
                    para.CountyId = queryParam["countyid"].ToString();
                }
                var list = TeacherBLL.Instance.GetList(para);
                if (list != null)
                {
                    var StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString();
                    var EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString();
                    list.ForEach((o) =>
                    {
                        if (o.TeacherId != null)
                        {
                            if (o.ProvinceId != null)
                            {
                                o.ProvinceName = AreaBLL.Instance.GetEntity(o.ProvinceId).AreaName;
                            }
                            if (o.CityId != null)
                            {
                                o.CityName = AreaBLL.Instance.GetEntity(o.CityId).AreaName;
                            }
                            o.AddressInfo = o.ProvinceName + o.CityName + o.ServicesAreaNames;

                            o.StudentCount = ApplyOrderBLL.Instance.GetList(new ApplyOrderEntity() { TeacherId = o.TeacherId }).Count();
                            o.WeekStudyOrderCount = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                            {
                                TeacherId = o.TeacherId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()
                            }).Count();
                            o.WeekWithDrivingOrderCount = WithDrivingOrderBLL.Instance.GetList(new WithDrivingOrderEntity()
                            {
                                TeacherId = o.TeacherId,
                                StartTime = Util.Time.CalculateFirstDateOfWeek(DateTime.Now).ToString(),
                                EndTime = Util.Time.CalculateLastDateOfWeek(DateTime.Now).ToString()

                            }).Count();
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                        if (o.IsWithDriving != null)
                        {
                            o.IsWithDrivingName = o.IsWithDriving == 1 ? "是" : "否";
                        }
                        if (o.IsTakeCar != null)
                        {
                            o.IsTakeCarName = o.IsTakeCar == 1 ? "是" : "否";
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "教练信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "教练信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "教练名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "所属驾校", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "LevName", ExcelColumn = "教练等级", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "联系方式", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IdCard", ExcelColumn = "身份证号", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarNumber", ExcelColumn = "车牌号码", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StudentCount", ExcelColumn = "学员数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "服务范围", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IsWithDrivingName", ExcelColumn = "是否陪驾", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "IsTakeCarName", ExcelColumn = "是否带车", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekStudyOrderCount", ExcelColumn = "本周学车订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WeekWithDrivingOrderCount", ExcelColumn = "本周陪驾订单数", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 15 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<TeacherEntity>.ExcelDownload(list, excelconfig);
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
                    if (data.Columns.Count != 7)
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
                                int hascount = TeacherBLL.Instance.GetList(new TeacherEntity() { Mobile = row[1].ToString() }).Count();
                                if (hascount > 0)
                                {
                                    continue;
                                }
                                TeacherEntity entity = new TeacherEntity();
                                entity.TeacherId = Util.Util.NewUpperGuid();
                                entity.Name = row[0].ToString();
                                entity.Mobile = row[1].ToString();
                                entity.IdCard = row[2].ToString();
                                var schoolList = SchoolBLL.Instance.GetList(new SchoolEntity() { Name = row[3].ToString() });
                                if (schoolList != null && schoolList.Count() > 0)
                                {
                                    var school = schoolList.FirstOrDefault();
                                    entity.SchoolId = school.SchoolId;
                                    entity.SchoolName = school.Name;
                                }

                                entity.IsWithDriving = row[4].ToString() == "是" ? 1 : 0;
                                entity.IsTakeCar = row[5].ToString() == "是" ? 1 : 0;
                                entity.MasterAccount = entity.Mobile;
                                entity.Pwd = "888888";
                                entity.CreateTime = DateTime.Now;
                                entity.CreateId = LoginUser.UserId;
                                entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                                entity.CarNumber = row[6].ToString();
                                TeacherBLL.Instance.Add(entity);
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
