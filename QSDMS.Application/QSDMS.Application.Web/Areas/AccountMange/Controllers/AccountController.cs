using Hydrosphere.Data;
using iFramework.Framework;
using QSDMS.Application.Web.Controllers;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSDMS.Util.Excel;
using QSDMS.Business;

namespace QSDMS.Application.Web.Areas.AccountMange.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /AccountMange/Account/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            ViewBag.AccountId = Request.Params["keyValue"];
            return View();
        }

        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var sql = PetaPoco.Sql.Builder.Append(@"select * from tbl_account where 1=1");

            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                sql.Append(" and (charindex(@0,Mobile)>0)", keyword);
            }

            //类型
            if (!queryParam["type"].IsEmpty())
            {
                string type = queryParam["type"].ToString();
                sql.Append(" and MemberType=@0", type);
            }
            //等级
            if (!queryParam["lev"].IsEmpty())
            {
                string lev = queryParam["lev"].ToString();
                sql.Append(" and MemberLevel=@0", lev);
            }

            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = tbl_Account.Page(pagination.page, pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            if (pageList != null)
            {
                foreach (var item in pageList)
                {
                    if (item.MemberLevel != null)
                    {
                        var lev = DataItemDetailBLL.Instance.GetEntity(item.MemberLevel);
                        if (lev != null)
                        {
                            item.MemberLevelName = lev.ItemName;
                        }
                    }
                    if (item.MemberType != null)
                    {
                        var type = DataItemDetailBLL.Instance.GetEntity(item.MemberType);
                        if (type != null)
                        {
                            item.MemberTypeName = type.ItemName;
                        }
                    }
                }
            }
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

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = tbl_Account.SingleOrDefault("where accountId=@0", keyValue);
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
                tbl_Account.Delete("where accountId=@0", keyValue);
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
        public ActionResult SaveForm(string keyValue, tbl_Account account)
        {
            try
            {

                if (keyValue != "")
                {
                    //account.AccountId = keyValue;
                    var model = tbl_Account.SingleOrDefault("where accountId=@0", keyValue);
                    if (model != null)
                    {
                        model = EntityConvertTools.CopyToModel<tbl_Account, tbl_Account>(account, model);
                        model.Update();
                    }

                }
                else
                {
                    //新增
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
        /// 导出EXCEL
        /// </summary>
        /// <param name="productOutId"></param>
        /// <param name="conditions"></param>
        public void ExportExcel(string queryJson)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from tbl_Account where 1=1");

            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                sql.Append(" and (charindex(@0,Mobile)>0)", keyword);
            }

            //类型
            if (!queryParam["type"].IsEmpty())
            {
                string type = queryParam["type"].ToString();
                sql.Append(" and MemberType=@0", type);
            }
            //等级
            if (!queryParam["lev"].IsEmpty())
            {
                string lev = queryParam["lev"].ToString();
                sql.Append(" and MemberLevel=@0", lev);
            }
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.Title = "会员账号信息";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 10;
            excelconfig.FileName = "会员信息.xls";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Mobile", ExcelColumn = "会员账号", Width = 20 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UserName", ExcelColumn = "会员名称", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberLevelName", ExcelColumn = "会员等级", Width = 20 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "MemberTypeName", ExcelColumn = "会员类型", Width = 50 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TypeName", ExcelColumn = "账号类型", Width = 50 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WXName", ExcelColumn = "微信昵称", Width = 20 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateTime", ExcelColumn = "注册时间", Width = 20 });
            //需合并索引
            //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var list = tbl_Account.Query(sql);
            List<ExcelAccount> newList = new List<ExcelAccount>();
            if (list != null)
            {
                foreach (tbl_Account item in list)
                {
                    ExcelAccount model = new ExcelAccount();
                    model = EntityConvertTools.CopyToModel<tbl_Account, ExcelAccount>(item, model);

                    if (model.Type != null)
                    {
                        model.TypeName = model.Type == 1 ? "工程公司" : "供应商";
                    }
                    if (model.MemberLevel != null)
                    {
                        var lev = DataItemDetailBLL.Instance.GetEntity(model.MemberLevel);
                        if (lev != null)
                        {
                            model.MemberLevelName = lev.ItemName;
                        }
                    }
                    if (model.MemberType != null)
                    {
                        var type = DataItemDetailBLL.Instance.GetEntity(model.MemberType);
                        if (type != null)
                        {
                            model.MemberTypeName = type.ItemName;
                        }
                    }
                    newList.Add(model);
                }
                //调用导出方法
                ExcelHelper<ExcelAccount>.ExcelDownload(newList.ToList(), excelconfig);
            }
        }
    }

    public class ExcelAccount : tbl_Account
    {
        public string MemberLevelName { get; set; }
        public string MemberTypeName { get; set; }
        public string TypeName { get; set; }
    }
}
