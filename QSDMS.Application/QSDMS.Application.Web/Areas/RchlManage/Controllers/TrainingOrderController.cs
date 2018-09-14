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
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class TrainingOrderController : BaseController
    {
        //
        // GET: /QX360Manage/TrainingOrder/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult ChangeTime()
        {

            return View();
        }
        public ActionResult SelectTime()
        {
            return View();
        }
        public ActionResult Pay()
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
            TrainingOrderEntity para = new TrainingOrderEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "trainingorderno":
                            para.TrainingOrderNo = queryParam["keyword"].ToString();
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
                        case "TrainingCarNumber":
                            para.TrainingCarNumber = queryParam["keyword"].ToString();
                            break;
                    }
                }
                if (!queryParam["status"].IsEmpty())
                {
                    para.Status = int.Parse(queryParam["status"].ToString());
                }
            }

            var pageList = TrainingOrderBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.TrainingStatus)o.Status).ToString();
                    }
                    if (o.CashType != null)
                    {
                        o.CashTypeName = ((QX360.Model.Enums.CashType)o.CashType).ToString();
                    }
                    if (o.UserType != null)
                    {
                        o.UserTypeName = ((QX360.Model.Enums.TrainingUserType)o.UserType).ToString();
                    }
                    var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = o.TrainingOrderId });
                    string datetime = "";
                    int subCount = 0;
                    detail.ForEach((d) =>
                    {
                        datetime += d.ServiceTime + ",";
                        subCount = subCount + 1;

                    });
                    o.SubrictCount = subCount;
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
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = TrainingOrderBLL.Instance.GetEntity(keyValue);

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
                        var entity = TrainingOrderBLL.Instance.GetEntity(key);
                        if (entity != null && (entity.Status != (int)QX360.Model.Enums.TrainingStatus.待支付 && entity.Status != (int)QX360.Model.Enums.TrainingStatus.待审核 && entity.Status != (int)QX360.Model.Enums.TrainingStatus.已取消))
                        {
                            flag = false;
                            return Error("非待支付/待审核/已取消状态的订单不能删除操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            TrainingOrderBLL.Instance.Delete(key);
                        }
                    }
                }
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>RemoveForm";
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
                    var order = Serializer.DeserializeJson<TrainingOrderEntity>(json, true);
                    if (order == null)
                    {
                        return Error("无效对象");
                    }
                    if (order.DetailList == null || order.DetailList.Count == 0)
                    {

                        return Error("请选择预约时间");
                    }
                    var hasflag = false;
                    foreach (var item in order.DetailList)
                    {

                        var freetime = TrainingFreeTimeBLL.Instance.GetEntity(item.TrainingFreeTimeId);
                        if (freetime != null)
                        {
                            if (freetime.FreeStatus != (int)QX360.Model.Enums.FreeTimeStatus.空闲)
                            {
                                hasflag = true;
                                break;
                            }
                        }
                    }
                    if (hasflag)
                    {
                        return Error("您下手晚了,请重新选择预约时间");
                    }
                    order.TrainingOrderId = Util.Util.NewUpperGuid();
                    order.TrainingOrderNo = TrainingOrderBLL.Instance.GetOrderNo();
                    order.CreateTime = DateTime.Now;
                    order.Status = (int)QX360.Model.Enums.TrainingStatus.待支付;
                    //order.Price = order.Price * order.DetailList.Count;
                    string _ServiceTime = "";
                    if (TrainingOrderBLL.Instance.Add(order))
                    {
                        if (order.DetailList != null)
                        {
                            foreach (var item in order.DetailList)
                            {
                                TrainingOrderDetailEntity detail = new TrainingOrderDetailEntity();
                                detail.TrainingOrderDetailId = Util.Util.NewUpperGuid();
                                detail.ServiceTime = item.ServiceTime;
                                detail.ServiceDate = item.ServiceDate;
                                detail.TrainingOrderId = order.TrainingOrderId;

                                if (item.SubritTimeType == (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义)
                                {
                                    detail.SubritTimeType = (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义;
                                }
                                detail.TrainingFreeTimeId = item.TrainingFreeTimeId == "-1" ? Util.Util.NewUpperGuid() : item.TrainingFreeTimeId;
                                if (TrainingOrderDetailBLL.Instance.Add(detail))
                                {
                                    _ServiceTime += detail.ServiceTime + ",";
                                    //修改预约时间状态
                                    TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                                    freetime.TrainingFreeTimeId = detail.TrainingFreeTimeId;
                                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                                    TrainingFreeTimeBLL.Instance.Update(freetime);

                                    //插入自定义时间段
                                    if (item.SubritTimeType == (int)QX360.Model.Enums.SubritFreeTimeStatus.自定义)
                                    {
                                        TrainingCustomFreeTimeBLL.Instance.Add(new TrainingCustomFreeTimeEntity()
                                        {
                                            TrainingCustomFreeTimeId = detail.TrainingFreeTimeId,//
                                            TrainingFreeDateId = item.FreedateId,
                                            TimeSection = item.ServiceTime
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
                        string txt = string.Format("考场实训,预约考场：{0}，预约车辆：{1}请您提前30分钟到达办理相关手续", order.SchoolName, order.TrainingCarName + order.TrainingCarNumber);
                        SendSmsMessageBLL.SendSubricNotice(order.MemberId, order.MemberMobile, order.MemberName, servicetime, txt, order.TrainingOrderNo);

                    }
                }
                return Success("创建成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Audit(string keyValue)
        {
            try
            {
                string[] keys = keyValue.Split(',');
                if (keys != null)
                {
                    bool flag = true;
                    foreach (var key in keys)
                    {
                        var entity = TrainingOrderBLL.Instance.GetEntity(key);
                        if (entity != null && entity.Status != (int)QX360.Model.Enums.TrainingStatus.待审核)
                        {
                            flag = false;
                            return Error("非待审核状态的订单不能审核操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            var order = TrainingOrderBLL.Instance.GetEntity(key);
                            if (order != null)
                            {
                                if (order.Status == (int)QX360.Model.Enums.TrainingStatus.待审核)
                                {
                                    order.Status = (int)QX360.Model.Enums.TrainingStatus.待支付;
                                    TrainingOrderBLL.Instance.Update(order);
                                    var _ServiceTime = "";
                                    var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = order.TrainingOrderId });
                                    detail.ForEach((o) =>
                                    {
                                        _ServiceTime += o.ServiceTime + ",";

                                        //修改预约状态为已预约
                                        TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                                        freetime.TrainingFreeTimeId = o.TrainingFreeTimeId;
                                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                                        TrainingFreeTimeBLL.Instance.Update(freetime);
                                    });
                                    string servicetime = string.Format("{0} {1}", DateTime.Parse(order.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                                    if (order.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                                    {
                                        SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.预约提醒, order.MemberId, order.MemberName, servicetime, "预约实训,单据已审核。请前往机构缴费完成实训", order.TrainingOrderNo);
                                    }
                                    else if (order.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                                    {
                                        SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, order.MemberId, servicetime, "预约实训,单据已审核。请前往机构缴费完成实训", order.TrainingOrderNo);

                                    }
                                }
                            }
                        }
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>Audit";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Pay(string keyValue, string json)
        {
            try
            {
                var order = Serializer.DeserializeJson<TrainingOrderEntity>(json, true);
                if (order == null)
                {
                    return Error("无效对象");
                }
                var entity = TrainingOrderBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    if (entity.Status != (int)QX360.Model.Enums.TrainingStatus.待支付)
                    {
                        return Error("非待支付状态的订单不能支付操作");
                    }
                    if (entity.Status == (int)QX360.Model.Enums.TrainingStatus.待支付)
                    {
                        entity.Status = (int)QX360.Model.Enums.TrainingStatus.已支付;
                        entity.Price = order.Price;
                        entity.CashType = order.CashType;
                        TrainingOrderBLL.Instance.Update(entity);
                        //插入财务表
                        FinaceBLL.Instance.Add(new FinaceEntity()
                        {
                            FinaceId = QSDMS.Util.Util.NewUpperGuid(),
                            SourceType = (int)QX360.Model.Enums.FinaceSourceType.实训报名,
                            ObjectId = order.SchoolId,
                            CreateTime = DateTime.Now,
                            CosMoney = entity.Price,
                            Status = (int)QX360.Model.Enums.TrainingStatus.已支付,
                            MemberId = entity.MemberId,
                            MemberName = entity.MemberName,
                            PayType = (int)QX360.Model.Enums.PayType.线下支付,
                            Operate = (int)QX360.Model.Enums.FinaceOperateType.增加,
                            Remark = string.Format("实训报名|{0}|{1}", entity.MemberName, entity.TrainingOrderNo)

                        });
                    }
                    var _ServiceTime = "";
                    var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = entity.TrainingOrderId });
                    detail.ForEach((o) =>
                    {
                        _ServiceTime += o.ServiceTime + ",";
                    });
                    string servicetime = string.Format("{0} {1}", DateTime.Parse(entity.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                    if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                    {
                        SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.预约提醒, entity.MemberId, entity.MemberName, servicetime, "预约实训,单据已付款。请按预约时间完成实训", entity.TrainingOrderNo);
                    }
                    else if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                    {
                        SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, entity.MemberId, servicetime, "预约实训,单据已付款。请按预约时间完成实训", entity.TrainingOrderNo);
                    }

                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>Pay";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
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
                        var entity = TrainingOrderBLL.Instance.GetEntity(key);
                        if (entity != null && entity.Status != (int)QX360.Model.Enums.TrainingStatus.待审核)
                        {
                            flag = false;
                            return Error("非待审核状态的订单不能取消操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            TrainingOrderBLL.Instance.Cancel(key);

                            var entity = TrainingOrderBLL.Instance.GetEntity(key);
                            //发送取消微信通知
                            var _ServiceTime = "";
                            var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = entity.TrainingOrderId });
                            detail.ForEach((o) =>
                            {
                                _ServiceTime += o.ServiceTime + ",";
                            });
                            string servicetime = string.Format("{0} {1}", DateTime.Parse(entity.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                            if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                            {
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.取消提醒, entity.MemberId, entity.MemberName, servicetime, "预约实训,该实训订单已取消，请重新预约", entity.TrainingOrderNo);
                            }
                            else if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                            {
                                SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.取消提醒, entity.MemberId, servicetime, "预约实训,该实训订单已取消，请重新预约", entity.TrainingOrderNo);

                            }
                        }
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>Cancel";
                new ExceptionHelper().LogException(ex);
                return Error("操作失败");
            }

        }

        /// <summary>
        /// 时间调整
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeTime(string keyValue, string json)
        {
            try
            {
                var detaillist = Serializer.DeserializeJson<List<TrainingOrderDetailEntity>>(json, true);
                if (detaillist != null && detaillist.Count == 0)
                {
                    return Error("请选择预约时间");
                }
                var entity = TrainingOrderBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    var _OldServiceTime = "";
                    var _OldServiceDate = "";
                    //修改之前状态
                    var olddetail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = entity.TrainingOrderId });
                    olddetail.ForEach((o) =>
                    {
                        //更改之前预约时间状态
                        TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                        freetime.TrainingFreeTimeId = o.TrainingFreeTimeId;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                        TrainingFreeTimeBLL.Instance.Update(freetime);
                        //删除明细
                        TrainingOrderDetailBLL.Instance.Delete(o.TrainingOrderDetailId);
                        _OldServiceTime += o.ServiceTime + ",";
                        _OldServiceDate = DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd");

                    });

                    //插入新的预约
                    var _ServiceTime = "";
                    foreach (var item in detaillist)
                    {
                        TrainingOrderDetailEntity detail = new TrainingOrderDetailEntity();
                        detail.TrainingOrderDetailId = Util.Util.NewUpperGuid();
                        detail.ServiceTime = item.ServiceTime;
                        detail.ServiceDate = item.ServiceDate;
                        detail.TrainingOrderId = entity.TrainingOrderId;
                        detail.TrainingFreeTimeId = item.TrainingFreeTimeId;
                        if (TrainingOrderDetailBLL.Instance.Add(detail))
                        {
                            _ServiceTime += detail.ServiceTime + ",";
                            //修改预约时间状态
                            TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                            freetime.TrainingFreeTimeId = detail.TrainingFreeTimeId;
                            freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.已预约;
                            TrainingFreeTimeBLL.Instance.Update(freetime);
                        }
                    }
                    //发送取消微信通知

                    string servicetime = string.Format("{0} {1}", DateTime.Parse(entity.ServiceDate.ToString()).ToString("yyyy-MM-dd"), _ServiceTime.TrimEnd(','));
                    string text = "预约实训,预约时间调整，原预约时间：" + string.Format("{0} {1}", _OldServiceDate.ToString(), _OldServiceTime.TrimEnd(','));
                    if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                    {
                        SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.更改提醒, entity.MemberId, entity.MemberName, servicetime, text, entity.TrainingOrderNo);
                    }
                    else if (entity.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                    {
                        SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, entity.MemberId, servicetime, text, entity.TrainingOrderNo);
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "TrainingOrderController>>ChangeTime";
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

                TrainingOrderEntity para = new TrainingOrderEntity();
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
                            case "trainingorderno":
                                para.TrainingOrderNo = queryParam["keyword"].ToString();
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
                            case "trainingcarnumber":
                                para.TrainingCarNumber = queryParam["keyword"].ToString();
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
                para.sidx = "CreateTime";
                para.sord = "desc";
                var list = TrainingOrderBLL.Instance.GetList(para);
                if (list != null)
                {
                    int subcount = 0;
                    list.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            o.ServiceTime = Converter.ParseDateTime(o.ServiceDate).ToString("yyyy-MM-dd") + " " + o.ServiceTime;
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.TrainingStatus)o.Status).ToString();
                        }
                        if (o.CashType != null)
                        {
                            o.CashTypeName = ((QX360.Model.Enums.CashType)o.CashType).ToString();
                        }
                        if (o.UserType != null)
                        {
                            o.UserTypeName = ((QX360.Model.Enums.TrainingUserType)o.UserType).ToString();
                        }
                        var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = o.TrainingOrderId });

                        int subCount = 0;
                        detail.ForEach((d) =>
                        {

                            subCount = subCount + 1;

                        });
                        o.SubrictCount = subCount;
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "实训预约订单";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "实训预约订单.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingOrderNo", ExcelColumn = "订单号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UserTypeName", ExcelColumn = "预约类型", Width = 15 }); 
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "学员/教练用户名", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberMobile", ExcelColumn = "联系方式", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "预约时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubrictCount", ExcelColumn = "合计预约数", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "预约驾校", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Price", ExcelColumn = "费用", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CashTypeName", ExcelColumn = "支付方式", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingTypeName", ExcelColumn = "实训类型", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingCarName", ExcelColumn = "实训车名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingCarNumber", ExcelColumn = "实训车车牌", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<TrainingOrderEntity>.ExcelDownload(list, excelconfig);
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
