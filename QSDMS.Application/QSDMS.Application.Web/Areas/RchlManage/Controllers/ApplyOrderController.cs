using QSDMS.Application.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using iFramework.Framework;
using QX360.Model;
using QX360.Business;
using QSDMS.Util.Excel;
using QSDMS.Business;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class ApplyOrderController : BaseController
    {
        //
        // GET: /QX360Manage/ApplyOrder/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult AllotTeacher()
        {
            return View();
        }
        public ActionResult Pay()
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
            ApplyOrderEntity para = new ApplyOrderEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "applyorderno":
                            para.ApplyOrderNo = queryParam["keyword"].ToString();
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

            var pageList = ApplyOrderBLL.Instance.GetPageList(para, ref pagination);
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

                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.ApplyStatus)o.Status).ToString();
                    }
                    if (o.ServiceDate != null)
                    {
                        o.ServiceTime = Converter.ParseDateTime(o.ServiceDate).ToString("yyyy-MM-dd") + " " + o.ServiceTime;
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
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = ApplyOrderBLL.Instance.GetEntity(keyValue);
            if (data != null)
            {
                if (data.ServiceDate != null)
                {
                    data.ServiceTime = Converter.ParseDateTime(data.ServiceDate).ToString("yyyy-MM-dd") + " " + data.ServiceTime;
                }
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
                string[] keys = keyValue.Split(',');
                if (keys != null)
                {
                    bool flag = true;
                    foreach (var key in keys)
                    {
                        var entity = ApplyOrderBLL.Instance.GetEntity(key);
                        if (entity != null && (entity.Status != (int)QX360.Model.Enums.ApplyStatus.待支付 && entity.Status != (int)QX360.Model.Enums.ApplyStatus.已取消))
                        {
                            flag = false;
                            return Error("非待支付/已取消状态的订单不能删除操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            ApplyOrderBLL.Instance.Delete(key);
                        }
                    }
                }
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyOrderController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, ApplyOrderEntity entity)
        {
            try
            {
                if (keyValue != "")
                {
                    entity.ApplyOrderId = keyValue;
                    entity.Status = (int)QX360.Model.Enums.ApplyStatus.已分配;
                    if (entity.ServiceTime != null)
                    {
                        //entity.ServiceDate = Converter.ParseDateTime(Converter.ParseDateTime(entity.ServiceTime).ToString("yyyy-MM-dd"));
                        //entity.ServiceTime = Converter.ParseDateTime(entity.ServiceTime).ToString("hh:mm");
                    }
                    ApplyOrderBLL.Instance.Update(entity);
                }
                else
                {
                    var num = ApplyOrderBLL.Instance.CheckHasApplay(entity.MemberId);
                    if (num == 0)
                    {
                        return Error("该学员已报名");
                    }
                    entity.ApplyOrderNo = ApplyOrderBLL.Instance.GetOrderNo();
                    entity.ApplyOrderId = Util.Util.NewUpperGuid();
                    entity.ServiceDate = Converter.ParseDateTime(Converter.ParseDateTime(entity.ServiceTime).ToString("yyyy-MM-dd"));
                    entity.ServiceTime = Converter.ParseDateTime(entity.ServiceTime).ToString("hh:mm");
                    entity.CreateTime = DateTime.Now;
                    entity.Status = (int)QX360.Model.Enums.ApplyStatus.已分配;
                    entity.CreateId = LoginUser.UserId;
                    ApplyOrderBLL.Instance.Add(entity);
                    //修改会员表中报名驾校
                    MemberBLL.Instance.Update(new MemberEntity()
                    {
                        MemberId = entity.MemberId,
                        SchoolId = entity.SchoolId,
                        SchoolName = entity.SchoolName
                    });

                    //插入财务信息
                    FinaceBLL.Instance.Add(new FinaceEntity()
                    {
                        FinaceId = Util.Util.NewUpperGuid(),
                        SourceType = (int)QX360.Model.Enums.FinaceSourceType.驾校报名,
                        ObjectId = entity.SchoolId,
                        CreateTime = DateTime.Now,
                        CosMoney = entity.PayMoney,
                        Status = (int)QX360.Model.Enums.PaySatus.已支付,
                        MemberId = entity.MemberId,
                        MemberName = entity.MemberName,
                        PayType = (int)QX360.Model.Enums.PayType.线下支付,
                        Operate = (int)QX360.Model.Enums.FinaceOperateType.增加,
                        Remark = string.Format("报名学车|{0}|{1}", entity.MemberName, entity.ApplyOrderNo)

                    });
                }
                return Success("创建成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyOrderController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 已支付
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Pay(string keyValue, string json)
        {
            try
            {
                var order = Serializer.DeserializeJson<ApplyOrderEntity>(json, true);
                if (order == null)
                {
                    return Error("无效对象");
                }
                var entity = ApplyOrderBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    if (entity.Status != (int)QX360.Model.Enums.PaySatus.待支付)
                    {
                        return Error("非待支付状态的订单不能支付操作");
                    }
                    if (entity.Status == (int)QX360.Model.Enums.ApplyStatus.待支付)
                    {
                        entity.Status = (int)QX360.Model.Enums.ApplyStatus.已支付;
                        entity.TotalMoney = order.TotalMoney;
                        entity.CashType = order.CashType;
                        ApplyOrderBLL.Instance.Update(entity);

                        //送积分
                        GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.预约驾校缴费, entity.MemberId, double.Parse(entity.TotalMoney.ToString()), entity.ApplyOrderNo);

                        //插入财务表
                        FinaceBLL.Instance.Add(new FinaceEntity()
                        {
                            FinaceId = Util.Util.NewUpperGuid(),
                            SourceType = (int)QX360.Model.Enums.FinaceSourceType.驾校报名,
                            ObjectId = order.SchoolId,
                            CreateTime = DateTime.Now,
                            CosMoney = entity.TotalMoney - entity.PayMoney,
                            Status = (int)QX360.Model.Enums.PaySatus.已支付,
                            MemberId = entity.MemberId,
                            MemberName = entity.MemberName,
                            PayType = (int)QX360.Model.Enums.PayType.线下支付,
                            Operate = (int)QX360.Model.Enums.FinaceOperateType.增加,
                            Remark = string.Format("报名学车|{0}|{1}", entity.MemberName, entity.ApplyOrderNo)

                        });
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ApplyOrderController>>Pay";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="queryJson"></param>
        public void ExportExcel(string queryJson)
        {
            string cacheKey = Request["cacheid"] as string;
            HttpRuntime.Cache[cacheKey + "-state"] = "processing";
            HttpRuntime.Cache[cacheKey + "-row"] = "0";
            try
            {

                ApplyOrderEntity para = new ApplyOrderEntity();
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
                            case "applyorderno":
                                para.ApplyOrderNo = queryParam["keyword"].ToString();
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

                var list = ApplyOrderBLL.Instance.GetList(para);
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
                    excelconfig.Title = "报名订单";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "报名订单.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ApplyOrderNo", ExcelColumn = "订单号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "用户名", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberMobile", ExcelColumn = "联系方式", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "地址", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "上门时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayMoney", ExcelColumn = "已付金额", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalMoney", ExcelColumn = "总费用", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "报名驾校", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TeacherName", ExcelColumn = "负责教练", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubjectName", ExcelColumn = "课程名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<ApplyOrderEntity>.ExcelDownload(list, excelconfig);
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
