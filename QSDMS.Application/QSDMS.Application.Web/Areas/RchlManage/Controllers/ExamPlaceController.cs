using iFramework.Framework;
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
using QSDMS.Application.Web.Controllers;
using QSDMS.Model;
using QSDMS.Business;
using QSDMS.Util.Excel;
namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class ExamPlaceController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult MonthWorkDay()
        {
            return View();
        }

        public ActionResult WorkTimeTable()
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
            SchoolEntity para = new SchoolEntity();
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
                        case "conectname":
                            para.ConectName = queryParam["keyword"].ToString();
                            break;
                        case "conecttel":
                            para.ConectTel = queryParam["keyword"].ToString();
                            break;
                        case "countyname":
                            para.CountyName = queryParam["keyword"].ToString();
                            break;
                    }
                }
            }

            para.IsTraining = 1;
            var currentlogin = OperatorProvider.Provider.Current();
            if (currentlogin.Account != Util.Config.GetValue("SysAccount"))
            {
                para.CreateId = currentlogin.UserId;
            }
            var pageList = SchoolBLL.Instance.GetPageList(para, ref pagination);
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
                    var subjectlist = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = o.SchoolId });
                    var subjectname = "";
                    foreach (var item in subjectlist)
                    {
                        subjectname += string.Format("{0}：【{1}元】,", item.SubjectName, item.Price);
                    }
                    if (subjectname != "")
                    {
                        o.SubjectName = subjectname.Substring(0, subjectname.Length - 1);
                    }
                    var tagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = o.SchoolId });
                    var tagname = "";
                    foreach (var item in tagList)
                    {
                        tagname += string.Format("{0},", item.Value);
                    }
                    if (tagname != "")
                    {
                        o.TagName = tagname.Substring(0, tagname.Length - 1);
                    }
                    o.TrainingCarCount = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { SchoolId = o.SchoolId }).Count();
                    o.TrainingOrderCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { SchoolId = o.SchoolId }).Count();
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDataListJson(string queryJson)
        {
            SchoolEntity para = new SchoolEntity();
            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();
                if (!queryParam["istraining"].IsEmpty())
                {
                    para.IsTraining = int.Parse(queryParam["istraining"].ToString());
                }
            }
            para.IsTraining = 1;
            var list = SchoolBLL.Instance.GetList(para);
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
            var data = SchoolBLL.Instance.GetEntity(keyValue);
            if (data != null)
            {
                var imageList = AttachmentPicBLL.Instance.GetList(new AttachmentPicEntity() { ObjectId = data.SchoolId });
                if (imageList != null)
                {
                    data.AttachmentPicList = imageList.OrderBy(i => i.SortNum).ThenBy(i => i.SortNum).ToList();
                }
                var subjectlist = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = data.SchoolId });
                data.SubjectList = subjectlist;
                var tagList = TagBLL.Instance.GetList(new TagEntity() { ObjectId = data.SchoolId });
                data.TagList = tagList;
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
                AttachmentPicBLL.Instance.DeleteByObjectId(keyValue);
                SubjectBLL.Instance.DeleteByschoolId(keyValue);
                MonthWorkDayBLL.Instance.DeleteByObjectId(keyValue);
                WorkTimeTableBLL.Instance.DeleteByObjectId(keyValue);
                TrainingTimeTableBLL.Instance.DeleteByObjectId(keyValue);
                SchoolBLL.Instance.Delete(keyValue);

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
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                //反序列化
                var entity = Serializer.DeserializeJson<SchoolEntity>(json, true);
                if (entity != null)
                {
                    if (keyValue == "")
                    {
                        //int count = SchoolBLL.Instance.GetList(new SchoolEntity() { MasterAccount = entity.MasterAccount }).Count();
                        //if (count > 0)
                        //{
                        //    return Error("该管理账户已存在");
                        //}

                        entity.SchoolId = Util.Util.NewUpperGuid();
                        entity.CreateTime = DateTime.Now;
                        entity.CreateId = LoginUser.UserId;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        entity.IsTraining = 1;
                        if (SchoolBLL.Instance.Add(entity))
                        {
                            if (entity.SubjectList != null)
                            {
                                SubjectBLL.Instance.DeleteByschoolId(entity.SchoolId);
                                foreach (var subjectItem in entity.SubjectList)
                                {
                                    SubjectEntity subject = new SubjectEntity();
                                    subject.SubjectId = Util.Util.NewUpperGuid();
                                    subject.ItemId = subjectItem.ItemId;
                                    subject.SubjectName = subjectItem.SubjectName;
                                    subject.Price = subjectItem.Price;
                                    subject.SchoolId = entity.SchoolId;
                                    subject.SchoolName = entity.Name;
                                    subject.Remark = subjectItem.Remark;
                                    subject.CreateId = DateTime.Now.ToString();
                                    SubjectBLL.Instance.Add(subject);
                                }

                            }
                            if (entity.AttachmentPicList != null)
                            {
                                int index = 0;
                                AttachmentPicBLL.Instance.DeleteByObjectId(entity.SchoolId);
                                foreach (var picitem in entity.AttachmentPicList)
                                {
                                    if (picitem != null)
                                    {
                                        AttachmentPicEntity pic = new AttachmentPicEntity();
                                        pic.PicId = Util.Util.NewUpperGuid();
                                        pic.PicName = picitem.PicName;
                                        pic.SortNum = index;
                                        pic.ObjectId = entity.SchoolId;
                                        pic.Type = (int)QX360.Model.Enums.AttachmentPicType.考场;
                                        AttachmentPicBLL.Instance.Add(pic);
                                    }
                                    index++;
                                }
                            }
                            if (entity.TagList != null)
                            {
                                TagBLL.Instance.DeleteByObjectId(entity.SchoolId);
                                foreach (var tagitem in entity.TagList)
                                {
                                    if (tagitem != null)
                                    {
                                        TagEntity tag = new TagEntity();
                                        tag.TagId = Util.Util.NewUpperGuid();
                                        tag.ObjectId = entity.SchoolId;
                                        tag.ObjectName = entity.Name;
                                        tag.Value = tagitem.Value;
                                        tag.CreateId = LoginUser.UserId;
                                        tag.CreateTime = DateTime.Now;
                                        TagBLL.Instance.Add(tag);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //修改
                        entity.SchoolId = keyValue;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        if (SchoolBLL.Instance.Update(entity))
                        {
                            if (entity.SubjectList != null)
                            {
                                //SubjectBLL.Instance.DeleteByschoolId(entity.SchoolId);
                                foreach (var subjectItem in entity.SubjectList)
                                {
                                    var subject = SubjectBLL.Instance.GetEntity(subjectItem.SubjectId);
                                    if (subject != null)
                                    {
                                        subject.ItemId = subjectItem.ItemId;
                                        subject.SubjectName = subjectItem.SubjectName;
                                        subject.Price = subjectItem.Price;
                                        subject.SchoolId = entity.SchoolId;
                                        subject.SchoolName = entity.Name;
                                        subject.Remark = subjectItem.Remark;
                                        SubjectBLL.Instance.Update(subject);
                                    }
                                    else
                                    {
                                        subject = new SubjectEntity();
                                        subject.SubjectId = Util.Util.NewUpperGuid();
                                        subject.ItemId = subjectItem.ItemId;
                                        subject.SubjectName = subjectItem.SubjectName;
                                        subject.Price = subjectItem.Price;
                                        subject.SchoolId = entity.SchoolId;
                                        subject.SchoolName = entity.Name;
                                        subject.Remark = subjectItem.Remark;
                                        subject.CreateId = DateTime.Now.ToString();
                                        SubjectBLL.Instance.Add(subject);
                                    }
                                }

                            }
                            if (entity.AttachmentPicList != null)
                            {
                                int index = 0;
                                AttachmentPicBLL.Instance.DeleteByObjectId(entity.SchoolId);
                                foreach (var picitem in entity.AttachmentPicList)
                                {

                                    if (picitem != null)
                                    {
                                        AttachmentPicEntity pic = new AttachmentPicEntity();
                                        pic.PicId = Util.Util.NewUpperGuid();
                                        pic.PicName = picitem.PicName;
                                        pic.SortNum = index;
                                        pic.ObjectId = entity.SchoolId;
                                        pic.Type = (int)QX360.Model.Enums.AttachmentPicType.考场;
                                        AttachmentPicBLL.Instance.Add(pic);
                                    }
                                    index++;
                                }
                            }
                            if (entity.TagList != null)
                            {
                                TagBLL.Instance.DeleteByObjectId(entity.SchoolId);
                                foreach (var tagitem in entity.TagList)
                                {
                                    if (tagitem != null)
                                    {
                                        TagEntity tag = new TagEntity();
                                        tag.TagId = Util.Util.NewUpperGuid();
                                        tag.ObjectId = entity.SchoolId;
                                        tag.ObjectName = entity.Name;
                                        tag.Value = tagitem.Value;
                                        tag.CreateId = LoginUser.UserId;
                                        tag.CreateTime = DateTime.Now;
                                        TagBLL.Instance.Add(tag);
                                    }
                                }
                            }
                        }
                    }


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
                var entity = SchoolBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.启用;
                    SchoolBLL.Instance.Update(entity);
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
                var entity = SchoolBLL.Instance.GetEntity(keyValue);
                if (entity != null)
                {
                    entity.Status = (int)QX360.Model.Enums.UseStatus.禁用;
                    SchoolBLL.Instance.Update(entity);
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
                SchoolEntity para = new SchoolEntity();
                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "name":
                            para.Name = queryParam["keyword"].ToString();
                            break;
                        case "conectname":
                            para.ConectName = queryParam["keyword"].ToString();
                            break;
                        case "conecttel":
                            para.ConectTel = queryParam["keyword"].ToString();
                            break;
                        case "countyname":
                            para.CountyName = queryParam["keyword"].ToString();
                            break;
                    }
                }
                para.IsTraining = 1;//实训
                var list = SchoolBLL.Instance.GetList(para);
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
                        var subjectlist = SubjectBLL.Instance.GetList(new SubjectEntity() { SchoolId = o.SchoolId });
                        var subjectname = "";
                        foreach (var item in subjectlist)
                        {
                            subjectname += string.Format("{0}：【{1}元】,", item.SubjectName, item.Price);
                        }
                        if (subjectname != "")
                        {
                            o.SubjectName = subjectname.Substring(0, subjectname.Length - 1);
                        }
                        o.TrainingCarCount = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { SchoolId = o.SchoolId }).Count();
                        o.TrainingOrderCount = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity() { SchoolId = o.SchoolId }).Count();
                        if (o.Status != null)
                        {
                            o.StatusName = ((QX360.Model.Enums.UseStatus)o.Status).ToString();
                        }
                    });

                    //设置导出格式
                    ExcelConfig excelconfig = new ExcelConfig();
                    excelconfig.Title = "考场信息";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 10;
                    excelconfig.FileName = "考场信息.xls";
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;
                    ColumnEntity columnentity = new ColumnEntity();
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Name", ExcelColumn = "考场名称", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddressInfo", ExcelColumn = "地址", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectName", ExcelColumn = "联系人", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConectTel", ExcelColumn = "联系电话", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingPrice", ExcelColumn = "实训费用", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SubjectName", ExcelColumn = "实训科目", Width = 15 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingCarCount", ExcelColumn = "实训车辆数", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TrainingOrderCount", ExcelColumn = "实训订单", Width = 20 });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "StatusName", ExcelColumn = "状态", Width = 20 });
                    //需合并索引
                    //excelconfig.MergeRangeIndexArr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

                    //调用导出方法
                    ExcelHelper<SchoolEntity>.ExcelDownload(list, excelconfig);
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
                    if (data.Columns.Count != 8)
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
                                SchoolEntity entity = new SchoolEntity();
                                entity.SchoolId = Util.Util.NewUpperGuid();
                                entity.Name = row[0].ToString();
                                var provincelist = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[1].ToString(); });
                                if (provincelist != null && provincelist.Count() > 0)
                                {
                                    var province = provincelist.FirstOrDefault();
                                    entity.ProvinceId = province.AreaId;
                                    entity.ProvinceName = province.AreaName;
                                }

                                var citylist = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[2].ToString(); });
                                if (citylist != null && citylist.Count() > 0)
                                {
                                    var city = citylist.FirstOrDefault();
                                    entity.CityId = city.AreaId;
                                    entity.CityName = city.AreaName;
                                }

                                var countyList = AreaBLL.Instance.GetList().Where((o) => { return o.AreaName == row[3].ToString(); });
                                if (countyList != null && countyList.Count() > 0)
                                {
                                    var county = countyList.FirstOrDefault();
                                    entity.CountyId = county.AreaId;
                                    entity.CountyName = county.AreaName;
                                }
                                entity.AddressInfo = row[4].ToString();
                                entity.ConectName = row[5].ToString();
                                entity.ConectTel = row[6].ToString();

                                if (row[7].ToString() != "")
                                {
                                    entity.TrainingPrice = decimal.Parse(row[7].ToString());
                                }
                                entity.CreateTime = DateTime.Now;
                                entity.CreateId = LoginUser.UserId;
                                entity.IsTraining = 1;//考场
                                SchoolBLL.Instance.Add(entity);
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
