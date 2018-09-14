using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using QSDMS.Util.Excel;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class StudyOrderController : BaseController
    {
        //
        // GET: /QX360Manage/StudyOrder/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }
        public ActionResult SelectTime()
        {
            return View();
        }
        public ActionResult AddTime()
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
            StudyOrderEntity para = new StudyOrderEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "studyorderno":
                            para.StudyOrderNo = queryParam["keyword"].ToString();
                            break;
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "membermobile":
                            para.MemberMobile = queryParam["keyword"].ToString();
                            break;
                        case "schoolname":
                            para.SchoolName = queryParam["keyword"].ToString();
                            break;
                        case "teachername":
                            para.TeacherName = queryParam["keyword"].ToString();
                            break;
                    }
                }
                if (!queryParam["status"].IsEmpty())
                {
                    para.Status = int.Parse(queryParam["status"].ToString());
                }
            }

            var pageList = StudyOrderBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.StudySubscribeStatus)o.Status).ToString();
                    }
                    if (o.StudyType != null)
                    {
                        o.StudyTypeName = ((QX360.Model.Enums.StudyType)o.StudyType).ToString();
                    }
                    var detail = StudyOrderDetailBLL.Instance.GetList(new StudyOrderDetailEntity() { StudyOrderId = o.StudyOrderId });
                    string datetime = "";
                    detail.ForEach((d) =>
                   {
                       if (d.TimeType != null)
                       {
                           d.TimeTypeName = ((QX360.Model.Enums.WorkTimeType)d.TimeType).ToString();
                       }
                       datetime += d.ServiceTime + "(" + d.TimeTypeName + "),";

                   });
                    //o.DetailList = detail;
                    o.ServiceTime = string.Format("{0} {1}", Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd"), datetime.TrimEnd(','));
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
        /// 获取服务预约时间 可能多个
        /// </summary>
        /// <param name="studyOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDetailListJson(string studyOrderId)
        {
            var list = StudyOrderDetailBLL.Instance.GetList(new StudyOrderDetailEntity() { StudyOrderId = studyOrderId });
            list.ForEach((o) =>
            {
                if (o.TimeType != null)
                {
                    o.TimeTypeName = ((QX360.Model.Enums.WorkTimeType)o.TimeType).ToString();
                }
            });
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
            var data = StudyOrderBLL.Instance.GetEntity(keyValue);
            //if (data != null)
            //{
            //    if (data.ServiceDate != null)
            //    {
            //        data.ServiceTime = Converter.ParseDateTime(data.ServiceDate).ToString("yyyy-MM-dd") + " " + data.ServiceTime;
            //    }
            //}
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

                string[] keys = keyValue.Split(',');
                if (keys != null)
                {
                    bool flag = true;
                    foreach (var key in keys)
                    {
                        var entity = StudyOrderBLL.Instance.GetEntity(key);
                        if (entity != null && (entity.Status != (int)QX360.Model.Enums.StudySubscribeStatus.预约成功 && entity.Status != (int)QX360.Model.Enums.StudySubscribeStatus.取消))
                        {
                            flag = false;
                            return Error("非预约成功/取消状态的订单不能删除操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            StudyOrderBLL.Instance.Delete(key);
                        }
                    }
                }
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyOrderController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string issend, string json)
        {
            try
            {
                if (keyValue != "")
                {

                }
                else
                {
                    var order = Serializer.DeserializeJson<StudyOrderEntity>(json, true);
                    if (order == null)
                    {
                        return Error("无效对象");
                    }
                    if (order.DetailList == null || order.DetailList.Count == 0)
                    {

                        return Error("请选择预约时间");
                    }
                    var member = MemberBLL.Instance.GetEntity(order.MemberId);
                    if (member == null)
                    {
                        return Error("会员对象无效");
                    }

                    //判断课时是否还有
                    if (order.DetailList != null)
                    {

                        int hour1 = 0;
                        int hour2 = 0;
                        foreach (var item in order.DetailList)
                        {
                            if (item.TimeType == (int)QX360.Model.Enums.WorkTimeType.白班)
                            {
                                hour1++;
                            }
                            else if (item.TimeType == (int)QX360.Model.Enums.WorkTimeType.夜班)
                            {
                                hour2++;
                            }
                        }
                        if ((member.StudyHour1 ?? 0) < hour1)
                        {
                            return Error("白班课时不足,预约此时段失败");
                        }
                        if ((member.StudyHour2 ?? 0) < hour2)
                        {
                            return Error("夜班课时不足,预约此时段失败");
                        }
                    }

                    order.StudyOrderId = Util.Util.NewUpperGuid();
                    order.StudyOrderNo = StudyOrderBLL.Instance.GetOrderNo();
                    order.Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约成功;
                    order.MemberId = order.MemberId;
                    order.MemberName = order.MemberName;
                    order.MemberMobile = order.MemberMobile;
                    order.CreateId = LoginUser.UserId;
                    order.CreateTime = DateTime.Now;
                    string _ServiceTime = "";
                    if (StudyOrderBLL.Instance.Add(order))
                    {
                        if (order.DetailList != null)
                        {
                            foreach (var item in order.DetailList)
                            {
                                StudyOrderDetailEntity detail = new StudyOrderDetailEntity();
                                detail.StudyOrderDetailId = Util.Util.NewUpperGuid();
                                detail.TimeType = item.TimeType;
                                detail.ServiceTime = item.ServiceTime;
                                detail.ServiceDate = item.ServiceDate;
                                detail.StudyOrderId = order.StudyOrderId;
                                detail.StudyFreeTimeId = item.StudyFreeTimeId == "-1" ? Util.Util.NewUpperGuid() : item.StudyFreeTimeId;
                                if (item.SubritTimeType == (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义)
                                {
                                    detail.SubritTimeType = (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义;
                                }
                                if (StudyOrderDetailBLL.Instance.Add(detail))
                                {
                                    _ServiceTime += detail.ServiceTime + ",";
                                    //修改预约时间状态
                                    StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                                    freetime.StudyFreeTimeId = detail.StudyFreeTimeId;
                                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                                    StudyFreeTimeBLL.Instance.Update(freetime);
                                    //修改对应课时
                                    if (detail.TimeType == (int)QX360.Model.Enums.WorkTimeType.白班)
                                    {
                                        member.MemberId = order.MemberId;
                                        member.StudyHour1 = member.StudyHour1 - 1;
                                        MemberBLL.Instance.Update(member);
                                    }
                                    else if (detail.TimeType == (int)QX360.Model.Enums.WorkTimeType.夜班)
                                    {
                                        member.MemberId = order.MemberId;
                                        member.StudyHour2 = member.StudyHour2 - 1;
                                        MemberBLL.Instance.Update(member);
                                    }

                                    //插入自定义时间段
                                    if (item.SubritTimeType == (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义)
                                    {
                                        StudyCustomFreeTimeBLL.Instance.Add(new StudyCustomFreeTimeEntity()
                                        {
                                            StudyCustomFreeTimeId = detail.StudyFreeTimeId,
                                            StudyFreeDateId = item.FreedateId,
                                            TimeSection = item.ServiceTime,
                                            TimeType = item.TimeType,
                                        });
                                    }
                                }

                            }
                        }
                    }

                    if (issend == "1")
                    {
                        //发送短信提醒
                        string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                        SendSmsMessageBLL.SendSubricNotice(order.MemberId, order.MemberMobile, order.MemberName, servicetime, "预约学车成功", order.StudyOrderNo);

                    }
                }

                return Success("创建成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyOrderController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(string keyValue)
        {
            try
            {
                string[] keys = keyValue.Split(',');
                if (keys != null)
                {
                    bool flag = true;
                    foreach (var key in keys)
                    {
                        var entity = StudyOrderBLL.Instance.GetEntity(key);
                        if (entity != null && entity.Status != (int)QX360.Model.Enums.StudySubscribeStatus.预约成功)
                        {
                            flag = false;
                            return Error("非预约成功状态的订单不能取消操作");
                        }
                        if (!flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            StudyOrderBLL.Instance.Cancel(key);
                        }
                    }

                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "StudyOrderController>>Cancel";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="productOutId"></param>
        /// <param name="conditions"></param>
        public void ExportExcel(string queryJson)
        {
            string cacheKey = Request["cacheid"] as string;
            HttpRuntime.Cache[cacheKey + "-state"] = "processing";
            HttpRuntime.Cache[cacheKey + "-row"] = "0";
            try
            {

                StudyOrderEntity para = new StudyOrderEntity();
                if (!string.IsNullOrWhiteSpace(queryJson))
                {
                    //这里要url解码
                    var queryParam = Server.UrlDecode(queryJson).ToJObject();
                    //类型
                    if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                    {
                        var condition = queryParam["condition"].ToString().ToLower();
                        switch (condition)
                        {
                            case "studyorderno":
                                para.StudyOrderNo = queryParam["keyword"].ToString();
                                break;
                            case "membername":
                                para.MemberName = queryParam["keyword"].ToString();
                                break;
                            case "membermobile":
                                para.MemberMobile = queryParam["keyword"].ToString();
                                break;
                            case "schoolname":
                                para.SchoolName = queryParam["keyword"].ToString();
                                break;
                            case "teachername":
                                para.TeacherName = queryParam["keyword"].ToString();
                                break;
                        }
                    }
                    if (!queryParam["status"].IsEmpty())
                    {
                        para.Status = int.Parse(queryParam["status"].ToString());
                    }
                    if (!queryParam["checkedids"].IsEmpty())
                    {
                        List<string> liststr = new List<string>();
                        string[] ids = queryParam["checkedids"].ToString().Split(',');
                        foreach (var item in ids)
                        {
                            if (item != "")
                            {
                                liststr.Add(item);
                            }
                        }
                        if (liststr.Count > 0)
                        {
                            para.CheckIds = liststr;
                        }
                    }
                }

                var list = StudyOrderBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.ForEach((o) =>
                    {
                        if (o.StudyType != null)
                        {
                            o.StudyTypeName = ((QX360.Model.Enums.StudyType)o.StudyType).ToString();
                        }
                        if (o.ServiceDate != null)
                        {
                            o.ServiceTime = Converter.ParseDateTime(o.ServiceDate).ToString("yyyy-MM-dd") + " " + o.ServiceTime;
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.StudySubscribeStatus)o.Status).ToString();
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "学车预约订单";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "学车预约订单.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StudyOrderNo", ExcelColumn = "订单号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StudyTypeName", ExcelColumn = "学车类型", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "学员用户名", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberMobile", ExcelColumn = "联系方式", Width = 15 });
                    //excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "预约时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "预约驾校", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TeacherName", ExcelColumn = "教练", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<StudyOrderEntity>.ExcelDownload(list, excelconfig);
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
