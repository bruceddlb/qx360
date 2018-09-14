using iFramework.Framework;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class MessageController : BaseController
    {
       
        /// <summary>
        /// 消息提示设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMessageAlterType()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //查询用户设置表
                var setuplist = MessageSetUpBLL.Instance.GetList(new MessageSetUpEntity() { AccountId = LoginUser.UserId });
                string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.MessageAlterType));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.MessageAlterType));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    int Status = 0;
                    if (setuplist != null)
                    {
                        var setup = setuplist.Where((o) => o.AlterType == int.Parse(values[i].ToString())).FirstOrDefault();
                        if (setup != null)
                        {
                            Status = setup.Status ?? 0;
                        }
                        else
                        {
                            Status = 1;
                        }
                    }
                    else
                    {
                        Status = 1;
                    }

                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = names[i].ToString(), Status = Status.ToString() });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MessageController>>GetMessageAlterType";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetUp(string status, string altertype)
        {
            var result = new ReturnMessage(false) { Message = "操作失败!" };
            try
            {
                MessageSetUpEntity entity = new MessageSetUpEntity();
                var setlist = MessageSetUpBLL.Instance.GetList(new MessageSetUpEntity() { AccountId = LoginUser.UserId, AlterType = int.Parse(altertype) });
                if (setlist != null && setlist.Count() > 0)
                {
                    //修改
                    entity = setlist.FirstOrDefault() as MessageSetUpEntity;
                    entity.Status = int.Parse(status);
                    MessageSetUpBLL.Instance.Update(entity);
                }
                else
                {
                    //新增
                    entity.MessageSetUpId = Util.NewUpperGuid();
                    entity.AlterType = int.Parse(altertype);
                    entity.AccountId = LoginUser.UserId;
                    entity.Status = int.Parse(status);
                    MessageSetUpBLL.Instance.Add(entity);
                }
                result.IsSuccess = true;
                result.Message = "设置成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "MessageController>>SetUp";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
