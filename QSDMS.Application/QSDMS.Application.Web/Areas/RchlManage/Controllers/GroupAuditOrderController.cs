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
using QSDMS.Application.Web.Controllers;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class GroupAuditOrderController : BaseController
    {
        //
        // GET: /QX360Manage/GroupAuditOrder/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
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
            GroupAuditOrderEntity para = new GroupAuditOrderEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "groupauditorderno":
                            para.GroupAuditOrderNo = queryParam["keyword"].ToString();
                            break;
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.Mobile = queryParam["keyword"].ToString();
                            break;
                        case "organizationname":
                            para.OrganizationName = queryParam["keyword"].ToString();
                            break;
                        case "groupname":
                            para.GroupName = queryParam["keyword"].ToString();
                            break;
                    }
                }
                if (!queryParam["status"].IsEmpty())
                {
                    para.Status = int.Parse(queryParam["status"].ToString());
                }
            }

            var pageList = GroupAuditOrderBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {
                    if (o.Status != null)
                    {
                        o.StatusName = ((QX360.Model.Enums.PaySatus)o.Status).ToString();
                    }
                    if (o.CashType != null)
                    {
                        o.CashTypeName = ((QX360.Model.Enums.CashType)o.CashType).ToString();
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
            var data = GroupAuditOrderBLL.Instance.GetEntity(keyValue);

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
                        var entity = GroupAuditOrderBLL.Instance.GetEntity(key);
                        if (entity != null && (entity.Status != (int)QX360.Model.Enums.PaySatus.待支付 && entity.Status != (int)QX360.Model.Enums.PaySatus.已取消))
                        {
                            flag = false;
                            return Error("非待支付/已取消状态的订单不能删除操作");
                        }
                    }
                    if (flag)
                    {
                        foreach (var key in keys)
                        {
                            GroupAuditOrderBLL.Instance.Delete(key);
                        }
                    }
                }
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "GroupAuditOrderController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "GroupAuditOrderController>>Register";
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
        public ActionResult Pay(string keyValue,string json)
        {
            try
            {
                var order = Serializer.DeserializeJson<GroupAuditOrderEntity>(json, true);
                if (order == null)
                {
                    return Error("无效对象");
                }
                var entity = GroupAuditOrderBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    if (entity.Status != (int)QX360.Model.Enums.PaySatus.待支付)
                    {
                        return Error("非待支付状态的订单不能支付操作");
                    }
                    if (entity.Status == (int)QX360.Model.Enums.PaySatus.待支付)
                    {
                        entity.Price = order.Price;
                        entity.CashType = order.CashType;
                        entity.Status = (int)QX360.Model.Enums.PaySatus.已支付;
                        GroupAuditOrderBLL.Instance.Update(entity);

                        //送积分
                        GivePointBLL.GivePoint(QX360.Model.Enums.OperationType.预约年检缴费, entity.MemberId, double.Parse(entity.Price.ToString()), entity.GroupAuditOrderNo);
                        //插入财务表
                        FinaceBLL.Instance.Add(new FinaceEntity()
                        {
                            FinaceId = QSDMS.Util.Util.NewUpperGuid(),
                            SourceType = (int)QX360.Model.Enums.FinaceSourceType.年检预约,
                            ObjectId=order.OrganizationId,
                            CreateTime = DateTime.Now,
                            CosMoney = entity.Price,
                            Status = (int)QX360.Model.Enums.PaySatus.已支付,
                            MemberId = entity.MemberId,
                            MemberName = entity.MemberName,
                            PayType = (int)QX360.Model.Enums.PayType.线下支付,
                            Operate = (int)QX360.Model.Enums.FinaceOperateType.增加,
                            Remark = string.Format("年检预约|{0}|{1}", entity.MemberName, entity.GroupAuditOrderNo)

                        });
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "GroupAuditOrderController>>Pay";
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

                GroupAuditOrderEntity para = new GroupAuditOrderEntity();
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
                            case "groupauditorderno":
                                para.GroupAuditOrderNo = queryParam["keyword"].ToString();
                                break;
                            case "membername":
                                para.MemberName = queryParam["keyword"].ToString();
                                break;
                            case "mobile":
                                para.Mobile = queryParam["keyword"].ToString();
                                break;
                            case "organizationname":
                                para.OrganizationName = queryParam["keyword"].ToString();
                                break;
                            case "groupname":
                                para.GroupName = queryParam["keyword"].ToString();
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

                var list = GroupAuditOrderBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.ForEach((o) =>
                    {
                        if (o.ServiceDate != null)
                        {
                            o.ServiceTime = Converter.ParseDateTime(o.ServiceDate).ToString("yyyy-MM-dd") + " " + o.ServiceTime;
                        }
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.StudySubscribeStatus)o.Status).ToString();
                        }
                        if (o.CashType != null)
                        {
                            o.CashTypeName = ((QX360.Model.Enums.CashType)o.CashType).ToString();
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "集团年检订单";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "集团年检订单.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GroupAuditOrderNo", ExcelColumn = "订单号", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "用户名", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "联系方式", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "GroupName", ExcelColumn = "集团名称", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarCount", ExcelColumn = "数量", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarType", ExcelColumn = "汽车类型", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceTime", ExcelColumn = "预约时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Price", ExcelColumn = "费用", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CashTypeName", ExcelColumn = "支付方式", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrganizationName", ExcelColumn = "年检机构", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<GroupAuditOrderEntity>.ExcelDownload(list, excelconfig);
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
