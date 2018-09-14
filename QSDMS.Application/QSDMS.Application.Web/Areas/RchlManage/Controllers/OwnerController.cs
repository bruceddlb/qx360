using iFramework.Framework;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Business;
using QSDMS.Util;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util.Excel;
using QSDMS.Util.Attributes;

namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class OwnerController : BaseController
    {
        //
        // GET: /QX360Manage/Owner/

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
            OwnerEntity para = new OwnerEntity();
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
                        case "membermobile":
                            para.MemberMobile = queryParam["keyword"].ToString();
                            break;
                        case "carframenum":
                            para.CarFrameNum = queryParam["keyword"].ToString();
                            break;
                        case "carnumber":
                            para.CarNumber = queryParam["keyword"].ToString();
                            break;

                    }
                }

                if (!queryParam["usetype"].IsEmpty())
                {
                    para.UseType = int.Parse(queryParam["usetype"].ToString());
                }
                if (!queryParam["cartype"].IsEmpty())
                {
                    para.CarType = int.Parse(queryParam["cartype"].ToString());
                }
            }

            var pageList = OwnerBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
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
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = OwnerBLL.Instance.GetEntity(keyValue);
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
                OwnerBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "OwnerController>>RemoveForm";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, OwnerEntity entity)
        {
            try
            {
                if (keyValue != "")
                {
                    entity.OwnerId = keyValue;
                    OwnerBLL.Instance.Update(entity);
                }
                else
                {
                    entity.OwnerId = Util.Util.NewUpperGuid();
                    entity.CreateTime = DateTime.Now;
                    OwnerBLL.Instance.Add(entity);

                }
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "OwnerController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
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
                OwnerEntity para = new OwnerEntity();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "membername":
                            para.MemberName = queryParam["keyword"].ToString();
                            break;
                        case "membermobile":
                            para.MemberMobile = queryParam["keyword"].ToString();
                            break;
                        case "carframenum":
                            para.CarFrameNum = queryParam["keyword"].ToString();
                            break;
                        case "carnumber":
                            para.CarNumber = queryParam["keyword"].ToString();
                            break;

                    }
                }

                if (!queryParam["usetype"].IsEmpty())
                {
                    para.UseType = int.Parse(queryParam["usetype"].ToString());
                }
                if (!queryParam["cartype"].IsEmpty())
                {
                    para.CarType = int.Parse(queryParam["cartype"].ToString());
                }
                var list = OwnerBLL.Instance.GetList(para);
                if (list != null)
                {

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "车辆信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "车辆信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberName", ExcelColumn = "车主姓名", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberMobile", ExcelColumn = "车主电话", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarNumber", ExcelColumn = "车牌号码", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarFrameNum", ExcelColumn = "车架号后四位", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "RegisterTime", ExcelColumn = "注册时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "LastCheckTime", ExcelColumn = "最后检测时间", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UseTypeName", ExcelColumn = "使用性质", Width = 50 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CarTypeName", ExcelColumn = "车辆类型", Width = 50 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<OwnerEntity>.ExcelDownload(list, excelconfig);
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
                var data = ExcelHelper.ExcelImport(file.InputStream, 2);//第2行为头部
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
                                int hascount = OwnerBLL.Instance.GetList(new OwnerEntity() { CarNumber = row[0].ToString() }).Count();
                                if (hascount > 0)
                                {
                                    continue;
                                }
                                OwnerEntity entity = new OwnerEntity();
                                entity.OwnerId = Util.Util.NewUpperGuid();
                                entity.CreateTime = DateTime.Now;
                                entity.CarNumber = row[0].ToString();
                                entity.MemberMobile = row[1].ToString();
                                //entity.CarNumber = row[2].ToString();
                                //entity.CarFrameNum = row[3].ToString();
                                if (row[2].ToString() != "")
                                {
                                    int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.UseType));
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        var discript = EnumAttribute.GetDescription((QX360.Model.Enums.UseType)values[i]);
                                        if (discript == row[2].ToString())
                                        {
                                            entity.UseType = values[i];
                                            entity.UseTypeName = row[2].ToString();
                                            break;
                                        }
                                    }
                                }
                                if (row[3].ToString() != "")
                                {
                                    int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.CarType));
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        var discript = EnumAttribute.GetDescription((QX360.Model.Enums.CarType)values[i]);
                                        if (discript == row[3].ToString())
                                        {
                                            entity.CarType = values[i];
                                            entity.CarTypeName = row[3].ToString();
                                            break;
                                        }
                                    }
                                }
                                string registerTime = "";
                                if (row[4].ToString() != "")
                                {
                                    registerTime = row[4].ToString();
                                }
                                if (row[5].ToString() != "")
                                {
                                    registerTime = registerTime + "-" + row[5].ToString();
                                }
                                if (registerTime != "")
                                {
                                    entity.RegisterTime = Convert.ToDateTime(registerTime);
                                }
                                if (row[6].ToString() != "")
                                {
                                    entity.PeopleCount = int.Parse(row[6].ToString());
                                }
                                OwnerBLL.Instance.Add(entity);
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
                ex.Data["Method"] = "OwnerController>>Import";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
