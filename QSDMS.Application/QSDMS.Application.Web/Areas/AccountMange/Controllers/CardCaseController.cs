using QSDMS.Application.Web.Controllers;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util.Extension;
using iFramework.Framework;
using Hydrosphere.Data;
using QSDMS.Util;
using QSDMS.Util.Excel;
using QSDMS.Business;
namespace QSDMS.Application.Web.Areas.AccountMange.Controllers
{
    public class CardCaseController : BaseController
    {
        //
        // GET: /AccountMange/CardCase/

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Detail()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            try
            {
                var sql = PetaPoco.Sql.Builder.Append(@"select * from tbl_CardCase where 1=1");

                var queryParam = queryJson.ToJObject();
               
                if (!queryParam["key"].IsEmpty())
                {
                    string contact = queryParam["contact"].ToString();
                    sql.Append(" and (charindex(@0,Name)>0 or charindex(@0,Mobile)>0 or charindex(@0,email)>0)", contact);
                }
                if (!string.IsNullOrWhiteSpace(pagination.sidx))
                {
                    sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
                }
                var currentpage = tbl_CardCase.Page(pagination.page, pagination.rows, sql);
                //数据对象
                var pageList = currentpage.Items;
                //分页对象           
                pagination.records = Converter.ParseInt32(currentpage.TotalItems);
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
            catch (Exception ex)
            {
                ex.Data["Method"] = "EngineeringController>>GetPageListJson";
                new ExceptionHelper().LogException(ex);
            }
            return Content(new
            {
                rows = "[]",
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            }.ToJson());
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var model = tbl_CardCase.SingleOrDefault("where Id=@0", keyValue);
            if (model != null)
            {
                model.TagList = tbl_CardTag.Fetch("where CardId=@0", model.Id);              
                model.ProductList = tbl_CardAttachment.Fetch("where cardid=@0 and Type=@1", model.Id, (int)Hydrosphere.Data.Enums.CardAttachmentType.产品);
                model.ProjectList = tbl_CardAttachment.Fetch("where cardid=@0 and Type=@1", model.Id, (int)Hydrosphere.Data.Enums.CardAttachmentType.案例);
                model.FileList = tbl_CardAttachment.Fetch("where cardid=@0 and Type=@1", model.Id, (int)Hydrosphere.Data.Enums.CardAttachmentType.文件);
            }
            return Content(model.ToJson());
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
                var sql = PetaPoco.Sql.Builder.Append(@"select * from tbl_CardCase where 1=1");

                var queryParam = Server.UrlDecode(queryJson).ToJObject();
                if (!queryParam["key"].IsEmpty())
                {
                    string contact = queryParam["contact"].ToString();
                    sql.Append(" and (charindex(@0,Name)>0 or charindex(@0,Mobile)>0 or charindex(@0,email)>0)", contact);
                }
                //设置导出格式
                ExcelConfig excelconfig = new ExcelConfig();
                excelconfig.Title = "名片信息";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 10;
                excelconfig.FileName = "名片信息.xls";
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                ColumnEntity columnentity = new ColumnEntity();
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "公司名称", Width = 20 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "联系人", Width = 15 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Email", ExcelColumn = "联系电话", Width = 20 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Job", ExcelColumn = "职务", Width = 50 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ComName", ExcelColumn = "公司名称", Width = 50 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PriviceName", ExcelColumn = "省", Width = 20 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CityName", ExcelColumn = "市", Width = 20 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CoverName", ExcelColumn = "区", Width = 20 });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "BussAreas", ExcelColumn = "业务领域", Width = 50 });
                //需合并索引
                //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
                var list = tbl_CardCase.Query(sql);
                List<ExcelCardCase> newList = new List<ExcelCardCase>();
                if (list != null)
                {
                    foreach (tbl_CardCase item in list)
                    {
                        ExcelCardCase model = new ExcelCardCase();
                        model = EntityConvertTools.CopyToModel<tbl_CardCase, ExcelCardCase>(item, model);
                        if (!string.IsNullOrWhiteSpace(model.ProvinceId))
                        {
                            model.PriviceName = AreaBLL.Instance.GetEntity(model.ProvinceId).AreaName;
                        }
                        if (!string.IsNullOrWhiteSpace(model.CityId))
                        {
                            model.CityName = AreaBLL.Instance.GetEntity(model.CityId).AreaName;
                        }
                        if (!string.IsNullOrWhiteSpace(model.CountyId))
                        {
                            model.CoverName = AreaBLL.Instance.GetEntity(model.CountyId).AreaName;
                        }
                        newList.Add(model);
                    }
                    //调用导出方法
                    ExcelHelper<ExcelCardCase>.ExcelDownload(newList.ToList(), excelconfig);
                    HttpRuntime.Cache[cacheKey + "-state"] = "done";
                }
            }
            catch (Exception ex)
            {
                HttpRuntime.Cache[cacheKey + "-state"] = "error";
            }

        }
    }
    public class ExcelCardCase : tbl_CardCase
    {
        public string PriviceName { get; set; }
        public string CityName { get; set; }
        public string CoverName { get; set; }
    }
}
