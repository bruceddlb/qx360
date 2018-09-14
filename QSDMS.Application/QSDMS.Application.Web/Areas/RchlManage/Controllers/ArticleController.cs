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
using QSDMS.Business;


namespace QSDMS.Application.Web.Areas.QX360Manage.Controllers
{
    public class ArticleController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Form()
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
            ArticleEntity para = new ArticleEntity();

            if (!string.IsNullOrWhiteSpace(queryJson))
            {
                var queryParam = queryJson.ToJObject();

                //类型
                if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
                {
                    var condition = queryParam["condition"].ToString().ToLower();
                    switch (condition)
                    {
                        case "title":
                            para.Title = queryParam["keyword"].ToString();
                            break;
                    }
                }
                if (!queryParam["status"].IsEmpty())
                {
                    para.Status = int.Parse(queryParam["status"].ToString());
                }
            }

            var pageList = ArticleBLL.Instance.GetPageList(para, ref pagination);
            if (pageList != null)
            {
                pageList.ForEach((o) =>
                {

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
        public ActionResult GetDataListJson()
        {
            var list = ArticleBLL.Instance.GetList(null);
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
            var data = ArticleBLL.Instance.GetEntity(keyValue);

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
                ArticleBLL.Instance.Delete(keyValue);
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>Register";
                new ExceptionHelper().LogException(ex);
                return Error("删除失败");
            }

        }

        /// <summary>
        /// 适用人群
        /// </summary>
        /// <param name="articleid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ToGroupTreeJson(string articleid)
        {
            var article = ArticleBLL.Instance.GetEntity(articleid);
            List<string> existgroup = new List<string>();
            if (article != null)
            {
                if (article.ToGroup != null)
                {
                    string[] ids = article.ToGroup.Split(',');
                    for (int i = 0; i < ids.Length; i++)
                    {
                        existgroup.Add(ids[i]);
                    }
                }
            }

            string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.ToGroupType));
            int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.ToGroupType));
            List<KeyValueEntity> list = new List<KeyValueEntity>();
            for (int i = 0; i < names.Length; i++)
            {
                list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = names[i] });
            }
            var treeList = new List<TreeEntity>();
            foreach (KeyValueEntity item in list)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                tree.id = item.ItemId;
                tree.text = item.ItemName;
                tree.value = item.ItemId;
                tree.title = "";
                tree.checkstate = existgroup.Count(t => t == item.ItemId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = "0";
                tree.img = "";
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, string json)
        {
            try
            {
                //反序列化
                var entity = Serializer.DeserializeJson<ArticleEntity>(json, true);
                if (entity != null)
                {
                    if (keyValue == "")
                    {
                        entity.ArticleId = Util.Util.NewUpperGuid();
                        entity.CreateDate = DateTime.Now;
                        entity.CreateId = LoginUser.UserId;
                        entity.CreateName = LoginUser.UserName;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        var flag = ArticleBLL.Instance.Add(entity);
                        if (flag && entity.Status == (int)QX360.Model.Enums.ArticleStatus.已发送 && entity.ToGroup != null)
                        {
                            SendArticle(entity);
                        }
                    }
                    else
                    {
                        //修改
                        entity.ArticleId = keyValue;
                        entity.Content = entity.Content == null ? "" : entity.Content.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
                        var flag = ArticleBLL.Instance.Update(entity);
                        //if (flag && entity.Status == (int)QX360.Model.Enums.ArticleStatus.已发送 && entity.ToGroup != null)
                        //{
                        //    SendArticle(entity);
                        //}
                    }
                }
                return Success("保存成功");
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ArticleController>>SaveForm";
                new ExceptionHelper().LogException(ex);
                return Error("保存失败");
            }

        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Send(string keyValue)
        {
            try
            {
                string[] keys = keyValue.Split(',');
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        //var receiver = tbl_Receiver.SingleOrDefault("where ReceiverId=@0", key);
                        var entity = ArticleBLL.Instance.GetEntity(keyValue);
                        if (entity != null)
                        {
                            if (entity.Status == (int)QX360.Model.Enums.ArticleStatus.已发送)
                            {
                                return Error("已发送,不用重复此操作");
                            }
                            if (entity.ToGroup != null)
                            {
                                SendArticle(entity);
                            }
                            entity.Status = (int)QX360.Model.Enums.ArticleStatus.已发送;
                            ArticleBLL.Instance.Update(entity);
                        }

                    }
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
        /// 发送文章信息
        /// </summary>
        /// <param name="article"></param>
        void SendArticle(ArticleEntity article)
        {
            string[] groups = article.ToGroup.Split(',');
            for (int i = 0; i < groups.Length; i++)
            {
                int type = int.Parse(groups[i]);
                switch (type)
                {
                    case (int)QX360.Model.Enums.ToGroupType.普通会员:
                        var list = MemberBLL.Instance.GetList(new MemberEntity() { LevId = ((int)QX360.Model.Enums.UserType.预约记时会员).ToString() });
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                InsertNotice(item.MemberId, item.MemberName, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.普通会员);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.VIP会员:
                        list = MemberBLL.Instance.GetList(new MemberEntity() { LevId = ((int)QX360.Model.Enums.UserType.VIP会员).ToString() });
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                InsertNotice(item.MemberId, item.MemberName, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.VIP会员);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.保险机构:
                        var userlist = UserBLL.Instance.GetList().Where((o) => { return o.OrganizeType == (int)QX360.Model.Enums.ToGroupType.保险机构; }).ToList();
                        if (userlist != null)
                        {
                            foreach (var item in userlist)
                            {
                                InsertNotice(item.UserId, item.Account, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.保险机构);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.店铺机构:
                        userlist = UserBLL.Instance.GetList().Where((o) => { return o.OrganizeType == (int)QX360.Model.Enums.ToGroupType.店铺机构; }).ToList();
                        if (userlist != null)
                        {
                            foreach (var item in userlist)
                            {
                                InsertNotice(item.UserId, item.Account, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.店铺机构);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.管理机构:
                        userlist = UserBLL.Instance.GetList().Where((o) => { return o.OrganizeType == (int)QX360.Model.Enums.ToGroupType.管理机构; }).ToList();
                        if (userlist != null)
                        {
                            foreach (var item in userlist)
                            {
                                InsertNotice(item.UserId, item.Account, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.管理机构);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.驾校机构:
                        userlist = UserBLL.Instance.GetList().Where((o) => { return o.OrganizeType == (int)QX360.Model.Enums.ToGroupType.驾校机构; }).ToList();
                        if (userlist != null)
                        {
                            foreach (var item in userlist)
                            {
                                InsertNotice(item.UserId, item.Account, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.驾校机构);
                            }
                        }
                        break;
                    case (int)QX360.Model.Enums.ToGroupType.年检机构:
                        userlist = UserBLL.Instance.GetList().Where((o) => { return o.OrganizeType == (int)QX360.Model.Enums.ToGroupType.年检机构; }).ToList();
                        if (userlist != null)
                        {
                            foreach (var item in userlist)
                            {
                                InsertNotice(item.UserId, item.Account, article.ArticleId, article.Title, (int)QX360.Model.Enums.ToGroupType.年检机构);
                            }
                        }
                        break;

                }
            }
        }

        /// <summary>
        /// 插入消息
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="customername"></param>
        /// <param name="articleId"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        void InsertNotice(string customerid, string customername, string articleId, string title, int? type)
        {

            NoticeEntity noticeentity = new NoticeEntity();
            noticeentity.NoticeId = Util.Util.NewUpperGuid();
            noticeentity.CustermerId = customerid;
            noticeentity.CustermerName = customername;
            noticeentity.CustermerType = type;
            noticeentity.ArticleId = articleId;
            noticeentity.ArticleName = title;
            noticeentity.Createtime = DateTime.Now;
            noticeentity.CreateId = LoginUser.UserId;
            NoticeBLL.Instance.Add(noticeentity);

        }
    }
}
