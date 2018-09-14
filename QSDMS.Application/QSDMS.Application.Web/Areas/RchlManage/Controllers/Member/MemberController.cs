using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.WebControl;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Model;
using QX360.Business;
using QSDMS.Business;
using QSDMS.Util.Excel;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class MemberController : BaseController
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
            MemberEntity para = new MemberEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.Mobile = queryParam["keyword"].ToString();
                            break;
                        case "schoolname":
                            para.SchoolName = queryParam["keyword"].ToString();
                            break;
                    }
                }
                //等级
                if (!queryParam["lev"].IsEmpty())
                {
                    para.LevId = queryParam["lev"].ToString();
                }
                if (!queryParam["simplespelling"].IsEmpty())
                {
                    para.SimpleSpelling = queryParam["simplespelling"].ToString();
                }
            }
            var pageList = MemberBLL.Instance.GetPageList(para, ref pagination);
            pageList.ForEach((o) =>
            {
                if (o.StudyHour1 != null)
                {
                    o.HaveStudyHours = string.Format("正班数:{0},加班数:{1}", o.StudyHour1, o.StudyHour2);
                }

                o.StudyOrderCount = StudyOrderBLL.Instance.GetList(new StudyOrderEntity() { MemberId = o.MemberId }).Count();
                o.WithDrivingOrderCount = WithDrivingOrderBLL.Instance.GetList(new WithDrivingOrderEntity() { MemberId = o.MemberId }).Count();
                o.AuditOrderCount = AuditOrderBLL.Instance.GetList(new AuditOrderEntity() { MemberId = o.MemberId }).Count();
                o.TakeAuditOrderCount = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity() { MemberId = o.MemberId }).Count();
                o.SeeCarOrderCount = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity() { MemberId = o.MemberId }).Count();
            });
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
        public ActionResult GetListJson()
        {
            var list = MemberBLL.Instance.GetList(null);
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
            var data = MemberBLL.Instance.GetEntity(keyValue);
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
                MemberBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, MemberEntity entity)
        {
            try
            {
                if (keyValue != "")
                {
                    entity.MemberId = keyValue;
                    MemberBLL.Instance.Update(entity);
                }
                else
                {
                    if (!new System.Text.RegularExpressions.Regex("^1[0-9]{10}$").IsMatch(entity.Mobile))
                    {

                        return Error("请输入正确的手机号");
                    }
                    int count = MemberBLL.Instance.GetList(new MemberEntity() { Mobile = entity.Mobile }).Count();
                    if (count > 0)
                    {
                        return Error("该号码已经注册了");
                    }
                    entity.MemberId = Util.Util.NewUpperGuid();
                    entity.Point = 0;
                    entity.Status = (int)Enums.UseStatus.启用;
                    entity.CreateTime = DateTime.Now;
                    MemberBLL.Instance.Add(entity);

                }
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "AccountController>>Register";
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
                var entity = MemberBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    MemberBLL.Instance.Update(entity);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>UnLockAccount";
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
                var entity = MemberBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    MemberBLL.Instance.Update(entity);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>UnLockAccount";
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
                //这里要url解码
                var queryParam = Server.UrlDecode(queryJson).ToJObject();
                MemberEntity para = new MemberEntity();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "mobile":
                            para.Mobile = queryParam["keyword"].ToString();
                            break;
                        case "schoolname":
                            para.SchoolName = queryParam["keyword"].ToString();
                            break;
                    }
                }
                //等级
                if (!queryParam["lev"].IsEmpty())
                {
                    para.LevId = queryParam["lev"].ToString();
                }
                var list = MemberBLL.Instance.GetList(para);
                if (list != null)
                {
                    list.ForEach((o) =>
                    {
                        if (o.StudyHour1 != null)
                        {
                            o.HaveStudyHours = string.Format("正班数:{0},加班数:{1}", o.StudyHour1, o.StudyHour2);
                        }
                    });
                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "会员信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "会员信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "用户姓名", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "NikeName", ExcelColumn = "用户昵称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "注册手机号", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SchoolName", ExcelColumn = "所属驾校", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "HaveStudyHours", ExcelColumn = "剩余学时", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateTime", ExcelColumn = "注册时间", Width = 50 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Point", ExcelColumn = "账户积分", Width = 50 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "LevName", ExcelColumn = "会员等级", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<MemberEntity>.ExcelDownload(list, excelconfig);
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
                                MemberEntity entity = new MemberEntity();
                                entity.MemberId = Util.Util.NewUpperGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.MemberName = row[0].ToString();
                                entity.NikeName = row[1].ToString();
                                entity.Mobile = row[2].ToString();
                                entity.StudyHour1 = int.Parse(row[3].ToString());
                                entity.StudyHour2 = int.Parse(row[4].ToString());
                                entity.Point = 0;
                                entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                                entity.LevId = ((int)QX360.Model.Enums.UserType.预约记时会员).ToString();
                                entity.LevName = QX360.Model.Enums.UserType.预约记时会员.ToString();
                                entity.Pwd = "123456";
                                MemberBLL.Instance.Add(entity);
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

    }

}
